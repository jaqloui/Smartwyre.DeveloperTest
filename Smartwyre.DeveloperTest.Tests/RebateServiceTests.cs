using Smartwyre.DeveloperTest.Data.Interfaces;
using Smartwyre.DeveloperTest.Services;
using Smartwyre.DeveloperTest.Types;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Xunit;
using Moq;

namespace Smartwyre.DeveloperTest.Tests
{
    public class RebateServiceTests
    {
        [Fact]
        public void Calculate_RebateIsNull_Failed()
        {
            // Arrange
            var mockRebateDataStore = new Mock<IRebateDataStore>();
            var mockProductDataStore = new Mock<IProductDataStore>();
            mockProductDataStore.Setup(x => x.GetProduct(It.IsAny<string>())).Returns(new Product { Identifier = "1", Id = 1, Uom = "123" });
            mockRebateDataStore.Setup(x => x.GetRebate(It.IsAny<string>())).Returns((Rebate)null);

            var service = new RebateService(mockRebateDataStore.Object, mockProductDataStore.Object);

            // Act
            var result = service.Calculate(new CalculateRebateRequest { RebateIdentifier = "1", ProductIdentifier = "1" });

            // Assert
            Assert.False(result.Success);
        }

        [Fact]
        public void Calculate_ProductIsNull_Failed()
        {
            // Arrange
            var mockRebateDataStore = new Mock<IRebateDataStore>();
            var mockProductDataStore = new Mock<IProductDataStore>();
            mockProductDataStore.Setup(x => x.GetProduct(It.IsAny<string>())).Returns((Product)null);
            mockRebateDataStore.Setup(x => x.GetRebate(It.IsAny<string>())).Returns(new Rebate { Identifier = "1" });

            var service = new RebateService(mockRebateDataStore.Object, mockProductDataStore.Object);

            // Act
            var result = service.Calculate(new CalculateRebateRequest { RebateIdentifier = "1", ProductIdentifier = "1" });

            // Assert
            Assert.False(result.Success);
        }

        [Fact]
        public void CalculateFixedCashAmount_Success()
        {
            // Arrange
            var mockRebateDataStore = new Mock<IRebateDataStore>();
            var mockProductDataStore = new Mock<IProductDataStore>();
            mockProductDataStore.Setup(x => x.GetProduct(It.IsAny<string>())).Returns(new Product { Identifier = "1", Id = 1, Uom = "123", SupportedIncentives = SupportedIncentiveType.FixedCashAmount });
            mockRebateDataStore.Setup(x => x.GetRebate(It.IsAny<string>())).Returns(new Rebate { Identifier = "1", Incentive = IncentiveType.FixedCashAmount, Amount = 10 });

            var service = new RebateService(mockRebateDataStore.Object, mockProductDataStore.Object);

            // Act
            var result = service.Calculate(new CalculateRebateRequest { RebateIdentifier = "1", ProductIdentifier = "1" });

            // Assert
            Assert.True(result.Success);
            Assert.Equal(10, result.RebateAmount);
        }

        [Fact]
        public void CalculateFixedCashAmount_Failed()
        {
            // Arrange
            var mockRebateDataStore = new Mock<IRebateDataStore>();
            var mockProductDataStore = new Mock<IProductDataStore>();
            mockProductDataStore.Setup(x => x.GetProduct(It.IsAny<string>())).Returns(new Product { Identifier = "1", Id = 1, Uom = "123" });
            mockRebateDataStore.Setup(x => x.GetRebate(It.IsAny<string>())).Returns(new Rebate { Identifier = "1", Incentive = IncentiveType.FixedCashAmount, Amount = 10 });

            var service = new RebateService(mockRebateDataStore.Object, mockProductDataStore.Object);

            // Act
            var result = service.Calculate(new CalculateRebateRequest { RebateIdentifier = "1", ProductIdentifier = "1" });

            // Assert
            Assert.False(result.Success);

        }

        [Fact]
        public void CalculateFixedRateRebate_Success()
        {
            // Arrange
            var mockRebateDataStore = new Mock<IRebateDataStore>();
            var mockProductDataStore = new Mock<IProductDataStore>();
            
            mockProductDataStore.Setup(x => x.GetProduct(It.IsAny<string>())).Returns(new Product { Identifier = "1", Id = 1, Uom = "123", SupportedIncentives = SupportedIncentiveType.FixedRateRebate, Price = 10 });
            mockRebateDataStore.Setup(x => x.GetRebate(It.IsAny<string>())).Returns(new Rebate { Identifier = "1", Incentive = IncentiveType.FixedRateRebate, Amount = 10, Percentage = 10 });

            var service = new RebateService(mockRebateDataStore.Object, mockProductDataStore.Object);

            // Act
            var result = service.Calculate(new CalculateRebateRequest { RebateIdentifier = "1", ProductIdentifier = "1", Volume = 10 });

            // Assert
            Assert.True(result.Success);
            Assert.Equal(1000, result.RebateAmount);
        }

        [Fact]
        public void CalculateFixedRateRebate_Failed()
        {
            // Arrange
            var mockRebateDataStore = new Mock<IRebateDataStore>();
            var mockProductDataStore = new Mock<IProductDataStore>();
            mockProductDataStore.Setup(x => x.GetProduct(It.IsAny<string>())).Returns(new Product { Identifier = "1", Id = 1, Uom = "123" });
            mockRebateDataStore.Setup(x => x.GetRebate(It.IsAny<string>())).Returns(new Rebate { Identifier = "1", Incentive = IncentiveType.FixedRateRebate, Amount = 10 });

            var service = new RebateService(mockRebateDataStore.Object, mockProductDataStore.Object);

            // Act
            var result = service.Calculate(new CalculateRebateRequest { RebateIdentifier = "1", ProductIdentifier = "1" });

            // Assert
            Assert.False(result.Success);
        }

        [Fact]
        public void CalculateAmountPerUom_Success()
        {
            // Arrange
            var mockRebateDataStore = new Mock<IRebateDataStore>();
            var mockProductDataStore = new Mock<IProductDataStore>();

            mockProductDataStore.Setup(x => x.GetProduct(It.IsAny<string>())).Returns(new Product { Identifier = "1", Id = 1, Uom = "123", SupportedIncentives = SupportedIncentiveType.AmountPerUom, Price = 10 });
            mockRebateDataStore.Setup(x => x.GetRebate(It.IsAny<string>())).Returns(new Rebate { Identifier = "1", Incentive = IncentiveType.AmountPerUom, Amount = 10, Percentage = 10 });

            var service = new RebateService(mockRebateDataStore.Object, mockProductDataStore.Object);

            // Act
            var result = service.Calculate(new CalculateRebateRequest { RebateIdentifier = "1", ProductIdentifier = "1", Volume = 10 });

            // Assert
            Assert.True(result.Success);
            Assert.Equal(100, result.RebateAmount);
        }

        [Fact]
        public void CalculateAmountPerUom_Failed()
        {
            // Arrange
            var mockRebateDataStore = new Mock<IRebateDataStore>();
            var mockProductDataStore = new Mock<IProductDataStore>();
            mockProductDataStore.Setup(x => x.GetProduct(It.IsAny<string>())).Returns(new Product { Identifier = "1", Id = 1, Uom = "123" });
            mockRebateDataStore.Setup(x => x.GetRebate(It.IsAny<string>())).Returns(new Rebate { Identifier = "1", Incentive = IncentiveType.AmountPerUom, Amount = 10 });

            var service = new RebateService(mockRebateDataStore.Object, mockProductDataStore.Object);

            // Act
            var result = service.Calculate(new CalculateRebateRequest { RebateIdentifier = "1", ProductIdentifier = "1" });

            // Assert
            Assert.False(result.Success);

        }

        [Fact]
        public void CalculateNullRequest_Failed()
        {
            // Arrange
            var mockRebateDataStore = new Mock<IRebateDataStore>();
            var mockProductDataStore = new Mock<IProductDataStore>();
            mockProductDataStore.Setup(x => x.GetProduct(It.IsAny<string>())).Returns(new Product { Identifier = "1", Id = 1, Uom = "123" });
            mockRebateDataStore.Setup(x => x.GetRebate(It.IsAny<string>())).Returns(new Rebate { Identifier = "1", Incentive = IncentiveType.AmountPerUom, Amount = 10 });

            var service = new RebateService(mockRebateDataStore.Object, mockProductDataStore.Object);

            // Assert
            Assert.Throws<ArgumentNullException>(() => service.Calculate(null));
        }

        [Fact]
        public void ArgumentNullException_RebateDataStore_Exception()
        {
            // Arrange
            var mockProductDataStore = new Mock<IProductDataStore>();
          
            // Assert
            Assert.Throws<ArgumentNullException>(() => new RebateService(null, mockProductDataStore.Object));

        }

        [Fact]
        public void ArgumentNullException_ProductDataStore_Exception()
        {
            // Arrange
            var mockRebateDataStore = new Mock<IRebateDataStore>();

            // Assert
            Assert.Throws<ArgumentNullException>(() => new RebateService(mockRebateDataStore.Object, null));

        }

        [Fact]
        public void CalculateFixedRateRebate_Exception()
        {
            // Arrange
            var mockRebateDataStore = new Mock<IRebateDataStore>();
            var mockProductDataStore = new Mock<IProductDataStore>();

            mockProductDataStore.Setup(x => x.GetProduct(It.IsAny<string>())).Returns(new Product { Identifier = "1", Id = 1, Uom = "123", SupportedIncentives = SupportedIncentiveType.FixedRateRebate, Price = 10 });
                            
            mockRebateDataStore.Setup(x => x.GetRebate(It.IsAny<string>())).Throws(new Exception());

            var service = new RebateService(mockRebateDataStore.Object, mockProductDataStore.Object);

            // Assert
            Assert.Throws<Exception>(() => service.Calculate(new CalculateRebateRequest { RebateIdentifier = "1", ProductIdentifier = "1", Volume = 10 }));
        }


    }
}
