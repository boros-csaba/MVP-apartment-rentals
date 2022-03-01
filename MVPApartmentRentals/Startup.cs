using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using AutoMapper;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.IdentityModel.Tokens;
using Swashbuckle.AspNetCore.Filters;
using Swashbuckle.AspNetCore.Swagger;
using MVPApartmentRentals.Models;
using MVPApartmentRentals.Security;
using MVPApartmentRentals.Services;

namespace MVPApartmentRentals
{
    public class Startup
    {
        private readonly IConfiguration configuration;

        public Startup(IConfiguration configuration)
        {
            this.configuration = configuration ?? throw new ArgumentNullException("configuration");
        }

        // This method gets called by the runtime. Use this method to add services to the container.
        // For more information on how to configure your application, visit https://go.microsoft.com/fwlink/?LinkID=398940
        public void ConfigureServices(IServiceCollection services)
        {
            services.AddAutoMapper(typeof(Startup));
            services.AddScoped<IAuthenticationService, AuthenticationService>();
            services.AddScoped<IKeyProvider, CertificateKeyProvider>();
            services.AddScoped<IApartmentService, ApartmentService>();
            services.AddScoped<IUserService, UserService>();
            services.AddScoped<IEncodingService, Base64EncodingService>();
            services.AddScoped<IPaginationService, PaginationService>();
            services.AddSingleton<IUriService>(provider =>
            {
                var request = provider.GetRequiredService<IHttpContextAccessor>().HttpContext.Request;
                var absoluteUri = string.Concat(request.Scheme, "://", request.Host.ToUriComponent(), "/");
                return new UriService(absoluteUri, 
                    configuration["EmailSender:EmailConfirmationLinkBase"], 
                    configuration["EmailSender:PasswordResetLinkBase"],
                    configuration["EmailSender:InvitationLinkBase"]);
            });

            SetupDataContext(services);
            SetupMvc(services);
            SetupAuthentication(services);
            AddSwagger(services);
            SetupEmailSender(services);
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        public void Configure(IApplicationBuilder app, IHostingEnvironment env, IServiceProvider services)
        {
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseCors();
            app.UseAuthentication();
            app.UseSwagger(options => { options.RouteTemplate = "swagger/{documentName}/swagger.json"; });
            app.UseSwaggerUI(options => { options.SwaggerEndpoint("api/swagger.json", "MVP Apartment Rentals"); });
            app.UseMvc();

            SetupUserRoles(services).Wait();
            SetupInitialAdminAsync(services).Wait();
        }

        private void SetupDataContext(IServiceCollection services)
        {
            services.AddDbContext<DataContext>(options =>
                options.UseSqlServer(configuration.GetConnectionString("ConnectionString"))
            );
            services.AddDefaultIdentity<User>()
                .AddRoles<IdentityRole>()
                .AddEntityFrameworkStores<DataContext>();
        }

        private void SetupMvc(IServiceCollection services)
        {
            services.AddMvc().SetCompatibilityVersion(CompatibilityVersion.Version_2_2);
            services.AddCors(options =>
                options.AddDefaultPolicy(policy =>
                    policy.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader().AllowCredentials().Build()
                ));
        }

        private void SetupAuthentication(IServiceCollection services)
        {
            var tokenValidationParameters = new TokenValidationParameters()
            {
                ValidateAudience = true,
                ValidateLifetime = true,
                RequireExpirationTime = false,
                ValidateIssuer = true,
                ValidateIssuerSigningKey = true,
                ValidAudience = "Audience",
                ValidIssuer = "Issuer",
                ClockSkew = TimeSpan.Zero,
                IssuerSigningKey = new CertificateKeyProvider().GetKey()
            };
            services.AddSingleton(tokenValidationParameters);
            services
                .AddAuthentication(options =>
                {
                    options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultScheme = JwtBearerDefaults.AuthenticationScheme;
                    options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
                })
                .AddJwtBearer(options =>
                {
                    options.TokenValidationParameters = tokenValidationParameters;
                });
            services.Configure<IdentityOptions>(options =>
            {
                options.Lockout.MaxFailedAccessAttempts = 3;
                options.Lockout.DefaultLockoutTimeSpan = TimeSpan.FromDays(100000);
                options.SignIn.RequireConfirmedEmail = true;
            });
        }

        private void AddSwagger(IServiceCollection services)
        {
            services.AddSwaggerGen(options =>
            {
                options.SwaggerDoc("api", new Info { Title = "api" });
                options.ExampleFilters();
                options.AddSecurityDefinition("Bearer", new ApiKeyScheme
                {
                    Name = "Authorization",
                    Description = "JWT Authentication",
                    In = "header",
                    Type = "apiKey"
                });
                options.AddSecurityRequirement(new Dictionary<string, IEnumerable<string>>
                {
                    { "Bearer", new string[0] }
                });

                var xmlFile = $"{Assembly.GetExecutingAssembly().GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                options.IncludeXmlComments(xmlPath);
            });
            services.AddSwaggerExamplesFromAssemblyOf<Startup>();
        }

        private void SetupEmailSender(IServiceCollection services)
        {
            services.AddSingleton<IEmailSenderService>(provider =>
            {
                var apiKey = configuration["EmailSender:ApiKey"];
                var fromEmail = configuration["EmailSender:FromEmail"];
                var fromName = configuration["EmailSender:FromName"];
                return new SendGridEmailSenderService(apiKey, fromEmail, fromName);
            });
        }

        private async Task SetupUserRoles(IServiceProvider services)
        {
            var roleManager = services.GetRequiredService<RoleManager<IdentityRole>>();
            await CreateRoleAsync(roleManager, "Admin");
            await CreateRoleAsync(roleManager, "Realtor");
            await CreateRoleAsync(roleManager, "Client");
        }

        private async Task CreateRoleAsync(RoleManager<IdentityRole> roleManager, string roleName)
        {
            var roleExists = await roleManager.RoleExistsAsync(roleName);
            if (!roleExists)
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }

        private async Task SetupInitialAdminAsync(IServiceProvider services)
        {
            var userManager = services.GetRequiredService<UserManager<User>>();
            var admin = await userManager.FindByEmailAsync("admin@admin.com");
            if (admin != null) return;
            admin = new User
            {
                Email = "admin@admin.com",
                UserName = "admin@admin.com",
                FirstName = "Admin",
                LastName = "User",
                EmailConfirmed = true
            };
            await userManager.CreateAsync(admin, "password1A!");
            await userManager.AddToRoleAsync(admin, "Admin");
        }
    }
}
