using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using MVPApartmentRentals.Models;
using MVPApartmentRentals.Services;

namespace MVPApartmentRentals.Tests
{
    public class ApartmentServiceTests
    {
        private DataContext context;

        [SetUp]
        public void Setup()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                        .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                        .Options;
            context = new DataContext(options);
        }

        [Test]
        public async Task ApartemntService_FilterMinSize()
        {
            //Arrange
            CreateApartment("Apartment 1", 30, 3, 250);
            CreateApartment("Apartment 2", 50, 2, 300);
            CreateApartment("Apartment 3", 150, 5, 1000);
            CreateApartment("Apartment 4", 200, 6, 1200);
            CreateApartment("Apartment 5", 250, 8, 2500);
            var paginationService = new PaginationService();
            var apartmentService = new ApartmentService(context, paginationService);
            var filter = new ApartmentFilter { MinArea = 220 };

            //Act
            var result = await apartmentService.GetApartmentsAsync(filter);

            //Assert
            Assert.AreEqual(1, result.Data.Count());
            Assert.AreEqual("Apartment 5", result.Data[0].Name);
        }
        [Test]
        public async Task ApartemntService_FilterMaxSize()
        {
            //Arrange
            CreateApartment("Apartment 1", 30, 3, 250);
            CreateApartment("Apartment 2", 50, 2, 300);
            CreateApartment("Apartment 3", 150, 5, 1000);
            CreateApartment("Apartment 4", 200, 6, 1200);
            CreateApartment("Apartment 5", 250, 8, 2500);
            var paginationService = new PaginationService();
            var apartmentService = new ApartmentService(context, paginationService);
            var filter = new ApartmentFilter { MaxArea = 35 };

            //Act
            var result = await apartmentService.GetApartmentsAsync(filter);

            //Assert
            Assert.AreEqual(1, result.Data.Count());
            Assert.AreEqual("Apartment 1", result.Data[0].Name);
        }

        [Test]
        public async Task ApartemntService_FilterMinRooms()
        {
            //Arrange
            CreateApartment("Apartment 1", 30, 1, 250);
            CreateApartment("Apartment 2", 50, 2, 300);
            CreateApartment("Apartment 3", 150, 5, 1000);
            CreateApartment("Apartment 4", 200, 6, 1200);
            CreateApartment("Apartment 5", 250, 8, 2500);
            var paginationService = new PaginationService();
            var apartmentService = new ApartmentService(context, paginationService);
            var filter = new ApartmentFilter { MinRooms = 7 };

            //Act
            var result = await apartmentService.GetApartmentsAsync(filter);

            //Assert
            Assert.AreEqual(1, result.Data.Count());
            Assert.AreEqual("Apartment 5", result.Data[0].Name);
        }

        [Test]
        public async Task ApartemntService_FilterMaxRooms()
        {
            //Arrange
            CreateApartment("Apartment 1", 30, 1, 250);
            CreateApartment("Apartment 2", 50, 2, 300);
            CreateApartment("Apartment 3", 150, 5, 1000);
            CreateApartment("Apartment 4", 200, 6, 1200);
            CreateApartment("Apartment 5", 250, 8, 2500);
            var paginationService = new PaginationService();
            var apartmentService = new ApartmentService(context, paginationService);
            var filter = new ApartmentFilter { MaxRooms = 1 };

            //Act
            var result = await apartmentService.GetApartmentsAsync(filter);

            //Assert
            Assert.AreEqual(1, result.Data.Count());
            Assert.AreEqual("Apartment 1", result.Data[0].Name);
        }

        [Test]
        public async Task ApartemntService_FilterMinPrice()
        {
            //Arrange
            CreateApartment("Apartment 1", 30, 3, 250);
            CreateApartment("Apartment 2", 50, 2, 300);
            CreateApartment("Apartment 3", 150, 5, 1000);
            CreateApartment("Apartment 4", 200, 6, 1200);
            CreateApartment("Apartment 5", 250, 8, 2500);
            var paginationService = new PaginationService();
            var apartmentService = new ApartmentService(context, paginationService);
            var filter = new ApartmentFilter { MinPrice = 2000 };

            //Act
            var result = await apartmentService.GetApartmentsAsync(filter);

            //Assert
            Assert.AreEqual(1, result.Data.Count());
            Assert.AreEqual("Apartment 5", result.Data[0].Name);
        }

        [Test]
        public async Task ApartemntService_FilterMaxPrice()
        {
            //Arrange
            CreateApartment("Apartment 1", 30, 3, 250);
            CreateApartment("Apartment 2", 50, 2, 300);
            CreateApartment("Apartment 3", 150, 5, 1000);
            CreateApartment("Apartment 4", 200, 6, 1200);
            CreateApartment("Apartment 5", 250, 8, 2500);
            var paginationService = new PaginationService();
            var apartmentService = new ApartmentService(context, paginationService);
            var filter = new ApartmentFilter { MaxPrice = 270 };

            //Act
            var result = await apartmentService.GetApartmentsAsync(filter);

            //Assert
            Assert.AreEqual(1, result.Data.Count());
            Assert.AreEqual("Apartment 1", result.Data[0].Name);
        }

        [Test]
        public async Task ApartemntService_ComplexFilter()
        {
            //Arrange
            CreateApartment("Apartment 1", 30, 3, 250);
            CreateApartment("Apartment 2", 50, 2, 300);
            CreateApartment("Apartment 3", 150, 5, 1000);
            CreateApartment("Apartment 4", 200, 6, 1200);
            CreateApartment("Apartment 5", 250, 8, 2500);
            var paginationService = new PaginationService();
            var apartmentService = new ApartmentService(context, paginationService);
            var filter = new ApartmentFilter {
                MinArea = 140,
                MaxArea = 160,
                MinRooms = 4,
                MaxRooms = 6,
                MinPrice = 900,
                MaxPrice = 1100
            };

            //Act
            var result = await apartmentService.GetApartmentsAsync(filter);

            //Assert
            Assert.AreEqual(1, result.Data.Count());
            Assert.AreEqual("Apartment 3", result.Data[0].Name);
        }


        private Apartment CreateApartment(string name, double size, int rooms, decimal price)
        {
            var apartment = new Apartment
            {
                Name = name,
                FloorAreaSize = size,
                NumberOfRooms = rooms,
                PricePerMonth = price
            };
            context.Apartments.Add(apartment);
            context.SaveChanges();
            return apartment;
        }
    }
}
