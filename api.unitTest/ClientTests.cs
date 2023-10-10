using System;
using System.Net;
using api.Models;
using api.Repositories;
using api.Services;
using Moq;

namespace api.unitTest
{
    [TestFixture]
	public class ClientTests
	{
        private IClientService _clientService;
        private Client[] _sampleClients;

        [SetUp]
        public void Setup()
        {
        //    // Initialize the mock repository and sample data
        //    var mockRepository = new Mock<IClientService>();
            _sampleClients = new Client[]
        {
            new Client { Id = "345345sdfu", FirstName = "Bucky", LastName = "Barnes", Email = "winter.soldier@email.com" },
            new Client { Id = "345345sdfs", FirstName = "Robert", LastName = "McCall", Email = "the.equalizer@email.com" },
            new Client { Id = "345345sdf4", FirstName = "John", LastName = "Wick", Email = "john.wick@email.com" },
            new Client { Id = "345345sdf4", FirstName = "Jason", LastName = "Bourne", Email = "jason.bourne@email.com" }
        };

            //    // Configure methods on the mock repository
            //    mockRepository.Setup(repo => repo.Create(It.IsAny<Client>()))
            //        .Returns((Task.CompletedTask));

            //    mockRepository.Setup(repo => repo.Update(It.IsAny<Client>()))
            //        .Returns((Task.CompletedTask));

            //    mockRepository.Setup(repo => repo.Search(It.IsAny<string>()))
            //        .ReturnsAsync((string keyword) =>
            //        {
            //            return _sampleClients
            //                .Where(c => c.FirstName.Contains(keyword, StringComparison.OrdinalIgnoreCase) ||
            //                        c.LastName.Contains(keyword, StringComparison.OrdinalIgnoreCase))
            //                .ToArray();
            //        });

            //    mockRepository.Setup(repo => repo.Get())
            //       .ReturnsAsync(() => _sampleClients);

            //    _clientService = mockRepository.Object;
        }

        [Test]
        public async Task CreateClient_ShouldAddClientToRepository()
        {
            // Arrange
            var mockRepository = new Mock<IClientRepository>();
            var _clientService = new ClientService(mockRepository.Object);
            var newClient = new Client {Id = "345345sdf1", FirstName = "Alice", LastName = "Wonderland", Email = "alice.wonderland@email.com", PhoneNumber = "324234" };

            // Act
            await _clientService.Create(newClient);

            // Assert
            mockRepository.Verify(repo => repo.Create(It.IsAny<Client>()), Times.Once);
        }

        [Test]
        public async Task UpdateClient_ShouldUpdateClientInRepository()
        {
            // Arrange
            var existingClient = _sampleClients[0];
            var updatedClient = new Client { Id = existingClient.Id, FirstName = "UpdatedBucky", LastName = "UpdatedBarnes", Email = "updatedbucky@email.com", PhoneNumber = "9238y28934" };


            var mockRepository = new Mock<IClientRepository>();

            var _clientService = new ClientService(mockRepository.Object);

            // Act
            await _clientService.Update(updatedClient);

            // Assert
            var updated = _sampleClients.FirstOrDefault(c => c.Id == existingClient.Id);


            mockRepository.Verify(repo => repo.Update(updatedClient), Times.Once);

            Assert.IsNotNull(updated);
            Assert.AreNotEqual(existingClient.FirstName, updatedClient.FirstName);
            Assert.AreNotEqual(existingClient.LastName, updatedClient.LastName);
            Assert.AreNotEqual(existingClient.Email, updatedClient.Email);
        }

        [Test]
        public async Task SearchClients_ShouldReturnMatchingClients()
        {
            // Arrange
            var keyword = "John";
            _sampleClients = new Client[]
            {
                new Client { Id = "345345sdf4", FirstName = "John", LastName = "Wick", Email = "john.wick@email.com" }
            };

            var mockRepository = new Mock<IClientRepository>();
                mockRepository.Setup(repo => repo.Get())
                    .ReturnsAsync(_sampleClients);

            var _clientService = new ClientService(mockRepository.Object);

            // Act
            var searchResults = await _clientService.Search(keyword);

            // Assert
            Assert.IsNotNull(searchResults);
            Assert.AreEqual(1, searchResults.Length);
            Assert.AreEqual("John", searchResults[0].FirstName);
        }

        [Test]
        public async Task GetAllClients_ShouldReturnAllClients()
        {
            //Arrange
            var mockRepository = new Mock<IClientRepository>();
            mockRepository.Setup(repo => repo.Get())
                .ReturnsAsync(_sampleClients);

            var _clientService = new ClientService(mockRepository.Object);

            // Act
            var allClients = await _clientService.Get();

            // Assert
            CollectionAssert.AreEqual(_sampleClients, allClients);
        }
        }
}

