using NUnit.Framework;
using OpenQA.Selenium;
using Protractor;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace MVPApartmentRentals.E2eTests
{
    public class Po
    {
        const int defaultDelaysForTesting = 0;
        private NgWebDriver ngDriver;

        public Po(NgWebDriver ngDriver)
        {
            this.ngDriver = ngDriver;
        }

        public Po Login()
        {
            ngDriver.Url = "https://localhost:4200/login";
            GetElementById("email").SendKeys("admin@admin.com");
            GetElementById("password").SendKeys("password1A!");
            GetElementById("loginButton").Click();
            return this;
        }

        public Po Logout()
        {
            GetElementById("logoutButton").Click();
            return this;
        }

        public Po GoToRealtorDashboard()
        {
            GetElementById("realtorDashboardButton").Click();
            return this;
        }

        public Po AddNewApartment(string name)
        {
            GetElementById("addNewApartmentButton").Click();
            GetElementById("name").SendKeys(name);
            GetElementById("floorAreaSize").SendKeys("150");
            GetElementById("numberOfRooms").SendKeys("5");
            GetElementById("pricePerMonth").SendKeys("300");
            GetElementById("isAvailable").Click();
            GetElementById("latitude").SendKeys("41.123");
            GetElementById("longitude").SendKeys("10.123");
            GetElementById("description").SendKeys("Description");
            GetElementById("apartmentSaveButton").Click();
            return this;
        }

        public Po GoToMainPage()
        {
            GetElementById("homeButton").Click();
            return this;
        }

        public Po FirstApartmentNameShouldBe(string name)
        {
            Assert.AreEqual(name, GetElementByClass("apartmentName").Text);
            return this;
        }

        public Po AddNewUser(string email)
        {
            GetElementById("addNewUserButton").Click();
            GetElementById("email").Clear();
            GetElementById("email").SendKeys(email);
            GetElementById("firstName").Clear();
            GetElementById("firstName").SendKeys("FName");
            GetElementById("lastName").Clear();
            GetElementById("lastName").SendKeys("LName");
            GetElementById("password").Clear();
            GetElementById("password").SendKeys("password1A!");
            GetElementById("passwordConfirmation").Clear();
            GetElementById("passwordConfirmation").SendKeys("password1A!");
            GetElementById("roles").Click();
            GetElementByClass("mat-option-text").Click();
            ngDriver.ExecuteScript("document.getElementsByClassName('cdk-overlay-backdrop')[1].style.display = 'none'");
            GetElementById("saveButton").Click();
            return this;
        }

        public Po GoToAdminDashboard()
        {
            GetElementById("adminPanelButton").Click();
            return this;
        }

        public Po GoToAdminApartmentManagementPage()
        {
            GetElementById("adminManageApartmensButton").Click();
            return this;
        }

        public Po EditFirstApartment(string name)
        {
            GetElementById("editApartmentButton").Click();
            GetElementById("name").Clear();
            GetElementById("name").SendKeys(name);
            GetElementById("apartmentSaveButton").Click();
            return this;
        }

        public Po DeleteApartment()
        {
            GetElementById("deleteApartmentButton").Click();
            ngDriver.SwitchTo().Alert().Accept();
            return this;
        }

        public Po ChangeUserFirstName(string newName) 
        {
            GetElementByClass("editButton").Click();
            GetElementById("firstName").Clear();
            GetElementById("firstName").SendKeys(newName);
            GetElementById("saveButton").Click();
            return this;
        }

        public Po DeleteUser()
        {
            GetElementByClass("deleteButton").Click();
            ngDriver.SwitchTo().Alert().Accept();
            return this;
        }

        public Po UserFirstNameShouldBe(string name)
        {
            Assert.AreEqual(name, GetElementByClass("userFirstName").Text);
            return this;
        }

        public Po RegisterUser(string email)
        {
            ngDriver.Url = "https://localhost:4200/login";
            GetElementById("registerButton").Click();
            GetElementById("email").Clear();
            GetElementById("email").SendKeys(email);
            GetElementById("firstName").Clear();
            GetElementById("firstName").SendKeys("FName");
            GetElementById("lastName").Clear();
            GetElementById("lastName").SendKeys("LName");
            GetElementById("password").Clear();
            GetElementById("password").SendKeys("password1A!");
            GetElementById("confirmedPassword").Clear();
            GetElementById("confirmedPassword").SendKeys("password1A!");
            GetElementById("registerButton").Click();
            return this;
        }

        public Po FirstUserEmailShouldBe(string email)
        {
            Assert.AreEqual(email, GetElementByClass("userEmail").Text);
            return this;
        }

        private NgWebElement GetElementById(string id)
        {
            Thread.Sleep(defaultDelaysForTesting);
            var limitCounter = 0;
            while (limitCounter < 100)
            {
                var elements = ngDriver.FindElements(By.Id(id));
                if (elements.Any())
                {
                    return elements.First();
                }
                limitCounter++;
                Thread.Sleep(100);
            }
            throw new Exception("Cannot find element with id: " + id);
        }

        private NgWebElement GetElementByClass(string className)
        {
            Thread.Sleep(defaultDelaysForTesting);
            var limitCounter = 0;
            while (limitCounter < 100)
            {
                var elements = ngDriver.FindElements(By.ClassName(className));
                if (elements.Any())
                {
                    return elements.First();
                }
                limitCounter++;
                Thread.Sleep(100);
            }
            throw new Exception("Cannot find element with class: " + className);
        }
    }
}
