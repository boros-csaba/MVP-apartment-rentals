using Microsoft.EntityFrameworkCore;
using NUnit.Framework;
using System;
using System.Linq;
using System.Threading.Tasks;
using MVPApartmentRentals;
using MVPApartmentRentals.Models;
using MVPApartmentRentals.Services;

namespace Tests
{
    public class PaginationServiceTests
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
        public async Task PagiantionService_WithLimit_WithOffset()
        {
            //Arrange
            CreateApartment("Apartment 1");
            CreateApartment("Apartment 2");
            CreateApartment("Apartment 3");
            CreateApartment("Apartment 4");
            CreateApartment("Apartment 5");
            var paginationService = new PaginationService();
            var filter = new PaginationFilter { Limit = 3, Offset = 1 };

            //Act
            var result = await paginationService.GetPaginatedListAsync(context.Apartments, filter);

            //Assert
            Assert.AreEqual(3, result.Data.Count());
            Assert.AreEqual("Apartment 2", result.Data[0].Name);
            Assert.AreEqual(5, result.TotalCount);
        }

        [Test]
        public async Task PagiantionService_WithLimit_WithHighOffset()
        {
            //Arrange
            CreateApartment("Apartment 1");
            CreateApartment("Apartment 2");
            CreateApartment("Apartment 3");
            CreateApartment("Apartment 4");
            CreateApartment("Apartment 5");
            var paginationService = new PaginationService();
            var filter = new PaginationFilter { Limit = 3, Offset = 4 };

            //Act
            var result = await paginationService.GetPaginatedListAsync(context.Apartments, filter);

            //Assert
            Assert.AreEqual(1, result.Data.Count());
            Assert.AreEqual("Apartment 5", result.Data[0].Name);
            Assert.AreEqual(5, result.TotalCount);
        }

        [Test]
        public async Task PagiantionService_WithoutLimit_WithOffset()
        {
            //Arrange
            CreateApartment("Apartment 1");
            CreateApartment("Apartment 2");
            CreateApartment("Apartment 3");
            CreateApartment("Apartment 4");
            CreateApartment("Apartment 5");
            var paginationService = new PaginationService();
            var filter = new PaginationFilter { Offset = 3 };

            //Act
            var result = await paginationService.GetPaginatedListAsync(context.Apartments, filter);

            //Assert
            Assert.AreEqual(2, result.Data.Count());
            Assert.AreEqual("Apartment 4", result.Data[0].Name);
            Assert.AreEqual(5, result.TotalCount);
        }

        [Test]
        public async Task PagiantionService_WithoutLimit_WithoutOffset()
        {
            //Arrange
            CreateApartment("Apartment 1");
            CreateApartment("Apartment 2");
            CreateApartment("Apartment 3");
            CreateApartment("Apartment 4");
            CreateApartment("Apartment 5");
            var paginationService = new PaginationService();
            var filter = new PaginationFilter();

            //Act
            var result = await paginationService.GetPaginatedListAsync(context.Apartments, filter);

            //Assert
            Assert.AreEqual(5, result.Data.Count());
            Assert.AreEqual("Apartment 1", result.Data[0].Name);
            Assert.AreEqual(5, result.TotalCount);
        }

        [Test]
        public async Task PagiantionService_WithNegativeLimit()
        {
            //Arrange
            var paginationService = new PaginationService();
            var filter = new PaginationFilter { Limit = -1 };

            //Act
            var result = await paginationService.GetPaginatedListAsync(context.Apartments, filter);

            //Assert
            Assert.False(result.Success);
        }

        [Test]
        public async Task PagiantionService_WithNegativeOffset()
        {
            //Arrange
            var paginationService = new PaginationService();
            var filter = new PaginationFilter { Offset = -1 };

            //Act
            var result = await paginationService.GetPaginatedListAsync(context.Apartments, filter);

            //Assert
            Assert.False(result.Success);
        }

        private Apartment CreateApartment(string name)
        {
            var apartment = new Apartment { Name = name };
            context.Apartments.Add(apartment);
            context.SaveChanges();
            return apartment;
        }
    }
}