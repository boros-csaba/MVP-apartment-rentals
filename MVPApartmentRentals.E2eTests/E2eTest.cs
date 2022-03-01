using NUnit.Framework;
using OpenQA.Selenium;
using OpenQA.Selenium.Chrome;
using OpenQA.Selenium.Firefox;
using Protractor;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;

namespace MVPApartmentRentals.E2eTests
{
    [TestFixture]
    public class E2eTest
    {
        IWebDriver driver;
        NgWebDriver ngDriver;

        [SetUp]
        public void Setup()
        {
            driver = new ChromeDriver(@"C:\Users\boros\Desktop\MVPApartmentRentals\MVPApartmentRentals\MVPApartmentRentals.E2eTests\");
            ngDriver = new NgWebDriver(driver);
        }

        [TearDown]
        public void Teardown()
        {
            ngDriver.Quit();
        }

        /// <summary>
        /// Creates and apartment from the Realtor dashboard
        /// Edits the same apartment from the Admin dashboard
        /// Deletes the new apartment from the Realtor dashboard
        /// </summary>
        [Test]
        public void FullCycle_Realtor_CreateEditAndDeleteApartmentTest()
        {
            var app = new Po(ngDriver);
            app.Login()
                  .GoToRealtorDashboard()
                     .AddNewApartment("Realtor e2e apartment")
                  .GoToMainPage()
                     .FirstApartmentNameShouldBe("Realtor e2e apartment")
                  .GoToRealtorDashboard()
                     .EditFirstApartment("Realtor e2e apartment with new name")
                  .GoToMainPage()
                     .FirstApartmentNameShouldBe("Realtor e2e apartment with new name")
                  .GoToRealtorDashboard()
                     .DeleteApartment()
                .Logout();
            ngDriver.Close();
        }


        /// <summary>
        /// Creates and apartment from the Admin dashboard
        /// Edits the same apartment from the Admin dashboard
        /// Deletes the new apartment from the Realtor dashboard
        /// </summary>
        [Test]
        public void FullCycle_Admin_CreateEditAndDeleteApartmentTest()
        {
            var app = new Po(ngDriver);
            app.Login()
                  .GoToAdminDashboard()
                     .GoToAdminApartmentManagementPage()
                        .AddNewApartment("Admin e2e apartment")
                  .GoToMainPage()
                     .FirstApartmentNameShouldBe("Admin e2e apartment")
                  .GoToAdminDashboard()
                     .GoToAdminApartmentManagementPage()
                        .EditFirstApartment("Admin e2e apartment with new name")
                  .GoToMainPage()
                     .FirstApartmentNameShouldBe("Admin e2e apartment with new name")
                  .GoToAdminDashboard()
                     .GoToAdminApartmentManagementPage()
                        .DeleteApartment()
                .Logout();
            ngDriver.Close();
        }

        /// <summary>
        /// Registers a new user from the registration page
        /// Edits the usr from the Admin panel
        /// Deletes the user from the Admin panel
        /// </summary>
        [Test]
        public void UserRegistration_EditAndDeleteTest()
        {
            var app = new Po(ngDriver);
            app.RegisterUser("aaaa@test.com")
               .Login()
                  .GoToAdminDashboard()
                     .ChangeUserFirstName("newName")
                     .UserFirstNameShouldBe("newName")
                     .DeleteUser()
                .Logout();
            ngDriver.Close();
        }

        /// <summary>
        /// Creates a new user from the Admin panel
        /// Deletes the new user
        /// </summary>
        [Test]
        public void AdminPanel_UserCreation()
        {
            var app = new Po(ngDriver);
            app.Login()
                  .GoToAdminDashboard()
                     .AddNewUser("aaaa@test.com")
                     .DeleteUser()
                .Logout();
            ngDriver.Close();
        }

    }
}
