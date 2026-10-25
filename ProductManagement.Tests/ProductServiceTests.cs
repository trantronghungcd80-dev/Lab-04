using ProductManagement.Exceptions;
using ProductManagement.Models;
using ProductManagement.Repositories;
using ProductManagement.Services;
using Xunit;

namespace ProductManagement.Tests
{
    public class ProductServiceTests
    {
        [Fact]
        public void Product_ValidCreation_PropertiesMatch()
        {
            var p = new Product("SP01", "Bàn phím cơ", 1500000m, 10);

            Assert.Equal("SP01", p.MaSP);
            Assert.Equal("SP01", p.Id);
            Assert.Equal("Bàn phím cơ", p.TenSP);
            Assert.Equal(1500000m, p.Price);
            Assert.Equal(10, p.Quantity);
            Assert.Equal(15000000m, p.TotalValue);
        }

        [Theory]
        [InlineData(-100, 5)]
        [InlineData(100, -5)]
        public void Product_NegativePriceOrQuantity_ThrowsArgumentOutOfRangeException(decimal price, int quantity)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new Product("SP01", "Chuột máy tính", price, quantity));
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public void Product_EmptyMaSP_ThrowsArgumentException(string invalidMaSP)
        {
            Assert.Throws<ArgumentException>(() => new Product(invalidMaSP, "Chuột máy tính", 200000m, 5));
        }

        [Fact]
        public void AddProduct_ValidProduct_AddsSuccessfullyAndFiresEvent()
        {
            var service = new ProductService();
            Product? eventProduct = null;
            service.OnProductAdded += (sender, product) => eventProduct = product;

            var newProduct = new Product("SP99", "Màn hình 4K", 9000000m, 3);
            service.AddProduct(newProduct);

            Assert.Equal(1, service.TotalCount);
            Assert.NotNull(eventProduct);
            Assert.Equal("SP99", eventProduct.MaSP);
        }

        [Fact]
        public void AddProduct_DuplicateMaSP_ThrowsDuplicateProductException()
        {
            var service = new ProductService();
            service.AddProduct(new Product("SP01", "Tai nghe", 500000m, 2));

            var duplicate = new Product("SP01", "Tai nghe Bluetooth", 750000m, 4);

            var ex = Assert.Throws<DuplicateProductException>(() => service.AddProduct(duplicate));
            Assert.Contains("SP01", ex.Message);
        }

        [Fact]
        public void RemoveProduct_ExistingProduct_RemovesSuccessfullyAndFiresEvent()
        {
            var service = new ProductService();
            service.AddProduct(new Product("SP01", "Loa Bluetooth", 1200000m, 5));

            Product? removedEventProduct = null;
            service.OnProductRemoved += (sender, product) => removedEventProduct = product;

            bool result = service.RemoveProduct("SP01");

            Assert.True(result);
            Assert.Equal(0, service.TotalCount);
            Assert.NotNull(removedEventProduct);
            Assert.Equal("SP01", removedEventProduct.MaSP);
        }

        [Fact]
        public void RemoveProduct_NonExistingProduct_ThrowsProductNotFoundException()
        {
            var service = new ProductService();
            service.AddProduct(new Product("SP01", "Loa Bluetooth", 1200000m, 5));

            var ex = Assert.Throws<ProductNotFoundException>(() => service.RemoveProduct("SP999"));
            Assert.Contains("SP999", ex.Message);
        }

        [Fact]
        public void Search_ByKeyword_UsingFunc_ReturnsMatchingProducts()
        {
            var service = new ProductService();
            service.AddProduct(new Product("SP01", "Laptop Dell Inspiron", 20000000m, 3));
            service.AddProduct(new Product("SP02", "Chuột Dell Optical", 300000m, 10));
            service.AddProduct(new Product("SP03", "Bàn phím cơ Asus", 1200000m, 4));

            var results = service.Search("Dell");

            Assert.Equal(2, results.Count());
            Assert.All(results, p => Assert.Contains("Dell", p.TenSP, StringComparison.OrdinalIgnoreCase));
        }

        [Fact]
        public void FilterByPriceRange_UsingFuncPredicate_ReturnsCorrectProducts()
        {
            var service = new ProductService();
            service.AddProduct(new Product("SP01", "Bút cảm ứng", 250000m, 10));
            service.AddProduct(new Product("SP02", "Webcam HD", 750000m, 5));
            service.AddProduct(new Product("SP03", "Ghế Gaming", 3500000m, 2));

            // Lọc từ 200,000 đ đến 1,000,000 đ
            var results = service.FilterByPriceRange(200000m, 1000000m).ToList();

            Assert.Equal(2, results.Count);
            Assert.Contains(results, p => p.MaSP == "SP01");
            Assert.Contains(results, p => p.MaSP == "SP02");
        }

        [Fact]
        public void CalculateTotalInventoryValue_ComputesCorrectTotal()
        {
            var service = new ProductService();
            service.AddProduct(new Product("SP01", "Sản phẩm A", 1000m, 5)); // 5,000
            service.AddProduct(new Product("SP02", "Sản phẩm B", 2000m, 3)); // 6,000

            decimal total = service.CalculateTotalInventoryValue();

            Assert.Equal(11000m, total);
        }

        [Fact]
        public void GenericRepository_WorksCorrectlyWithIEntity()
        {
            var repo = new Repository<Product>();
            var p1 = new Product("TEST01", "Test Product 1", 100m, 1);
            var p2 = new Product("TEST02", "Test Product 2", 200m, 2);

            repo.Add(p1);
            repo.Add(p2);

            Assert.Equal(2, repo.Count);
            Assert.Equal(p1, repo.FindById("TEST01"));

            // Sử dụng Func<Product, bool>
            var filtered = repo.Find(p => p.Price > 150m);
            Assert.Single(filtered);
            Assert.Equal("TEST02", filtered.First().MaSP);

            Assert.True(repo.Remove("TEST01"));
            Assert.Equal(1, repo.Count);
        }
    }
}
