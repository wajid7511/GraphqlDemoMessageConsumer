using GraphQl.Mongo.Database;
using GraphQl.Mongo.Database.DALs;
using GraphQl.Mongo.Database.Models;
using GraphQl.Mongo.Database.Options;
using GraphQlDemo.Shared.Database;
using GraphQlDemo.Shared.Enums;
using GraphQlDemo.Shared.Messaging;
using MessageConsumer.Processors;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using Moq;

namespace MessageConsumer.Tests.Processors
{
    [TestClass]
    public class EmailProcessorTests
    {
        private Mock<ILogger<EmailProcessor>> _loggerMock = new();
        private Mock<CustomerOrderDAL> _customerOrderDalMock = null!;
        private EmailProcessor _emailProcessor = null!;

        [TestInitialize]
        public void Setup()
        {
            Mock<IMongoDatabase> mongoDatabase = new();
            Mock<IDbBaseModelFactory> dbBaseModelFactory = new();
            Mock<ILogger<CustomerOrderDAL>> _dalLoggerMock = new();
            Mock<IOptions<MongoDbOptions>> _options = new();
            _options.Setup(s => s.Value).Returns(new MongoDbOptions()
            {
                OrdersCollectionName = "tesitng"
            });
            _customerOrderDalMock = new Mock<CustomerOrderDAL>(mongoDatabase.Object, dbBaseModelFactory.Object, _options.Object, _dalLoggerMock.Object);
            _loggerMock = new Mock<ILogger<EmailProcessor>>();
            _emailProcessor = new EmailProcessor(_customerOrderDalMock.Object, _loggerMock.Object);
        }

        [TestMethod]
        public async Task ProcessAsync_OrderFoundAndUpdatedSuccessfully_ReturnsTrue()
        {
            // Arrange
            var referenceId = Guid.NewGuid();
            var messageDto = new MessageDto { ReferenceId = referenceId.ToString(), MessageType = MessageType.Order };
            var customerOrder = new CustomerOrder { Id = Guid.Parse(messageDto.ReferenceId), OrderStatusId = OrderStatusEnum.Created };
            var dbGetResult = new DbGetResult<CustomerOrder>(true, customerOrder);
            var dbUpdateResult = new DbUpdateResult(true);

            _customerOrderDalMock.Setup(dal => dal.GetCustomerOrderByIdAsync(It.Is<Guid>(id => id == referenceId)))
            .ReturnsAsync(dbGetResult)
            .Verifiable(Times.Once);
            _customerOrderDalMock.Setup(dal => dal.UpdateCustomerOrder(It.Is<Guid>(id => id == referenceId), It.Is<Dictionary<string, object>>(d => d.Count() == 1 && (OrderStatusEnum)d[nameof(CustomerOrder.OrderStatusId)] == OrderStatusEnum.Processing)))
            .ReturnsAsync(dbUpdateResult)
            .Verifiable(Times.Once);

            // Act
            var result = await _emailProcessor.ProcessAsync(messageDto);

            // Assert
            Assert.IsTrue(result);
            _customerOrderDalMock.Verify();
            _customerOrderDalMock.VerifyNoOtherCalls();
            _loggerMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task ProcessAsync_OrderNotFound_ReturnsFalse()
        {
            // Arrange
            var messageDto = new MessageDto { ReferenceId = Guid.NewGuid().ToString(), MessageType = MessageType.Order };
            var dbGetResult = new DbGetResult<CustomerOrder>(false)
            {
                Exception = new Exception("Order not found")
            };

            _customerOrderDalMock.Setup(dal => dal.GetCustomerOrderByIdAsync(It.IsAny<Guid>()))
            .ReturnsAsync(dbGetResult)
            .Verifiable(Times.Once);

            // Act
            var result = await _emailProcessor.ProcessAsync(messageDto);

            // Assert
            Assert.IsFalse(result);
            _customerOrderDalMock.Verify();
            _loggerMock.Verify(
                      l => l.Log(
                          LogLevel.Error,
                          It.IsAny<EventId>(),
                          It.Is<It.IsAnyType>((v, t) => v!.ToString()!.StartsWith("No order found with id")),
                          It.Is<Exception>(ex => ex.Message == "Order not found"),
                          It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                      Times.Once);
            _customerOrderDalMock.VerifyNoOtherCalls();
            _loggerMock.VerifyNoOtherCalls();
        }

        [TestMethod]
        public async Task ProcessAsync_UpdateOrderFails_ReturnsFalse()
        {
            // Arrange
            var orderId = Guid.NewGuid();
            var messageDto = new MessageDto { ReferenceId = orderId.ToString(), MessageType = MessageType.Order };
            var customerOrder = new CustomerOrder { Id = Guid.Parse(messageDto.ReferenceId), OrderStatusId = OrderStatusEnum.Created };
            var dbGetResult = new DbGetResult<CustomerOrder>(true, customerOrder);
            var dbUpdateResult = new DbUpdateResult(false)
            {
                Exception = new Exception("Failed to update order")
            };

            _customerOrderDalMock.Setup(dal => dal.GetCustomerOrderByIdAsync(orderId))
            .ReturnsAsync(dbGetResult)
            .Verifiable(Times.Once);
            _customerOrderDalMock.Setup(dal => dal.UpdateCustomerOrder(It.Is<Guid>(id => id == orderId), It.Is<Dictionary<string, object>>(d => d.Count() == 1 && (OrderStatusEnum)d[nameof(CustomerOrder.OrderStatusId)] == OrderStatusEnum.Processing)))
              .ReturnsAsync(dbUpdateResult)
              .Verifiable(Times.Once);

            // Act
            var result = await _emailProcessor.ProcessAsync(messageDto);

            // Assert
            Assert.IsFalse(result);
            _customerOrderDalMock.Verify();
            _loggerMock.Verify(
                   l => l.Log(
                       LogLevel.Error,
                       It.IsAny<EventId>(),
                       It.Is<It.IsAnyType>((v, t) => v!.ToString()!.StartsWith("Unable to update customer order")),
                       It.Is<Exception>(ex => ex.Message == "Failed to update order"),
                       It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                   Times.Once);
            _customerOrderDalMock.VerifyNoOtherCalls();
            _loggerMock.VerifyNoOtherCalls();
        }
    }
}
