using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace MVPApartmentRentals
{
    public static class ApiRoutes
    {
        private const string root = "api";

        public static class Apartments
        {
            public const string GetAll = root + "/apartments";
            public const string GetUserApartments = root + "/users/{userId}/apartments";
            public const string Get = root + "/apartments/{apartmentId}";
            public const string Create = root + "/apartments";
            public const string Update = root + "/apartments/{apartmentId}";
            public const string Delete = root + "/apartments/{apartmentId}";
        }

        public static class Authentication
        {
            public const string Login = root + "/authentication/login";
            public const string Register = root + "/authentication/registration";
            public const string RefreshToken = root + "/authentication/refreshToken";
            public const string ConfirmEmail = root + "/authentication/emailConfirmation";
            public const string ConfirmationEmailRequest = root + "/authentication/confirmationEmailRequest";
            public const string ExternalLogin = root + "/authentication/externalLogin";
            public const string Block = root + "/authentication/{userId}/block";
            public const string PasswordReset = root + "/authentication/passwordReset";
            public const string InviteUser = root + "/authentication/invitations";
        }

        public static class Users
        {
            public const string GetAll = root + "/users";
            public const string Get = root + "/users/{userId}";
            public const string Create = root + "/users";
            public const string Update = root + "/users/{userId}";
            public const string Delete = root + "/users/{userId}";
            public const string GetProfileImage = root + "/users/{userId}/profileImage";
            public const string UploadProfileImage = root + "/users/{userId}/profileImage";
        }
    }
}
