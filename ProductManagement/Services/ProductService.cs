using ProductManagement.Exceptions;
using ProductManagement.Models;
using ProductManagement.Repositories;

namespace ProductManagement.Services
{
    /// <summary>
    /// Custom delegate cho các sự kiện thao tác trên sản phẩm.
    /// </summary>
    /// <param name="sender">Đối tượng phát sinh sự kiện</param>
    /// <param name="product">Sản phẩm chịu tác động</param>
    public delegate void ProductActionHandler(object? sender, Product product);

    /// <summary>
    /// Lớp xử lý nghiệp vụ quản lý sản phẩm.
    /// Tích hợp Generic Repository, Exception nghiệp vụ và Delegate/Event.
    /// </summary>
    public class ProductService
    {
        private readonly Repository<Product> _repository;

        /// <summary>
        /// Event thông báo khi thêm sản phẩm thành công.
        /// </summary>
        public event ProductActionHandler? OnProductAdded;

        /// <summary>
        /// Event thông báo khi xóa sản phẩm thành công.
        /// </summary>
        public event ProductActionHandler? OnProductRemoved;

        /// <summary>
        /// Khởi tạo ProductService với Repository tương ứng.
        /// </summary>
        /// <param name="repository">Repository quản lý Product (tùy chọn)</param>
        public ProductService(Repository<Product>? repository = null)
        {
            _repository = repository ?? new Repository<Product>();
        }

        /// <summary>
        /// Thêm mới một sản phẩm vào kho.
        /// Kiểm tra tính hợp lệ và ném DuplicateProductException nếu trùng mã.
        /// Phát sinh sự kiện OnProductAdded khi thêm thành công.
        /// </summary>
        /// <param name="product">Sản phẩm cần thêm</param>
        public void AddProduct(Product product)
        {
            if (product == null)
                throw new ArgumentNullException(nameof(product), "Dữ liệu sản phẩm không được null.");

            if (string.IsNullOrWhiteSpace(product.MaSP))
                throw new ArgumentException("Mã sản phẩm không được để trống.", nameof(product.MaSP));

            // Kiểm tra trùng mã trong Repository
            var existing = _repository.FindById(product.MaSP);
            if (existing != null)
            {
                throw new DuplicateProductException(
                    $"Lỗi nghiệp vụ: Mã sản phẩm '{product.MaSP}' đã tồn tại trong hệ thống!",
                    product.MaSP
                );
            }

            _repository.Add(product);

            // Kích hoạt sự kiện thông báo thêm thành công
            OnProductAdded?.Invoke(this, product);
        }

        /// <summary>
        /// Xóa sản phẩm theo mã.
        /// Ném ProductNotFoundException nếu không tìm thấy mã trong kho.
        /// Phát sinh sự kiện OnProductRemoved khi xóa thành công.
        /// </summary>
        /// <param name="maSP">Mã sản phẩm cần xóa</param>
        public bool RemoveProduct(string maSP)
        {
            if (string.IsNullOrWhiteSpace(maSP))
                throw new ArgumentException("Mã sản phẩm cần xóa không được để trống.", nameof(maSP));

            var product = _repository.FindById(maSP);
            if (product == null)
            {
                throw new ProductNotFoundException(
                    $"Lỗi nghiệp vụ: Không tìm thấy sản phẩm có mã '{maSP.Trim()}' để xóa!",
                    maSP.Trim()
                );
            }

            bool isRemoved = _repository.Remove(product.MaSP);
            if (isRemoved)
            {
                // Kích hoạt sự kiện thông báo xóa thành công
                OnProductRemoved?.Invoke(this, product);
            }

            return isRemoved;
        }

        /// <summary>
        /// Lấy toàn bộ danh sách sản phẩm.
        /// </summary>
        public IEnumerable<Product> GetAllProducts()
        {
            return _repository.GetAll();
        }

        /// <summary>
        /// Tìm sản phẩm theo mã định danh.
        /// </summary>
        /// <param name="maSP">Mã sản phẩm cần tìm</param>
        /// <returns>Product nếu có, ngược lại null</returns>
        public Product? GetProductById(string maSP)
        {
            if (string.IsNullOrWhiteSpace(maSP))
                return null;

            return _repository.FindById(maSP);
        }

        /// <summary>
        /// Tìm kiếm sản phẩm theo tên chứa từ khóa (không phân biệt chữ hoa/thường).
        /// Sử dụng Func<Product, bool> vị từ tìm kiếm.
        /// </summary>
        /// <param name="keyword">Từ khóa cần tìm</param>
        public IEnumerable<Product> Search(string keyword)
        {
            if (string.IsNullOrWhiteSpace(keyword))
                return _repository.GetAll();

            string cleanKw = keyword.Trim();
            // Sử dụng Func<Product, bool>
            Func<Product, bool> searchFilter = p => 
                p.TenSP.IndexOf(cleanKw, StringComparison.OrdinalIgnoreCase) >= 0;

            return _repository.Find(searchFilter);
        }

        /// <summary>
        /// Lọc danh sách sản phẩm dựa trên một điều kiện Func<Product, bool> tùy biến.
        /// </summary>
        /// <param name="predicate">Biểu thức lọc Func</param>
        public IEnumerable<Product> Filter(Func<Product, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate), "Biểu thức lọc không được null.");

            return _repository.Find(predicate);
        }

        /// <summary>
        /// Lọc danh sách sản phẩm theo khoảng giá [minPrice, maxPrice].
        /// Sử dụng Func<Product, bool> để lọc.
        /// </summary>
        /// <param name="minPrice">Giá nhỏ nhất</param>
        /// <param name="maxPrice">Giá lớn nhất</param>
        public IEnumerable<Product> FilterByPriceRange(decimal minPrice, decimal maxPrice)
        {
            if (minPrice < 0 || maxPrice < 0)
                throw new ArgumentException("Khoảng giá lọc không được nhận giá trị âm.");

            if (minPrice > maxPrice)
                throw new ArgumentException($"Giá nhỏ nhất ({minPrice:N0} đ) không được lớn hơn giá lớn nhất ({maxPrice:N0} đ).");

            // Ràng buộc Func<Product, bool>
            Func<Product, bool> priceRangeFilter = p => p.Price >= minPrice && p.Price <= maxPrice;

            return _repository.Find(priceRangeFilter);
        }

        /// <summary>
        /// Tính tổng giá trị toàn bộ kho hàng (Tổng = Đơn giá * Số lượng của tất cả sản phẩm).
        /// </summary>
        public decimal CalculateTotalInventoryValue()
        {
            return _repository.GetAll().Sum(p => p.TotalValue);
        }

        /// <summary>
        /// Tổng số sản phẩm trong kho.
        /// </summary>
        public int TotalCount => _repository.Count;
    }
}
