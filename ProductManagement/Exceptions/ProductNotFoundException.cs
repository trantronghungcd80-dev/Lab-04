namespace ProductManagement.Exceptions
{
    /// <summary>
    /// Ngoại lệ tự tạo phát sinh khi thao tác (tìm kiếm, xóa, sửa) với sản phẩm không tồn tại trong hệ thống.
    /// </summary>
    public class ProductNotFoundException : Exception
    {
        public string? ProductId { get; }

        public ProductNotFoundException()
            : base("Không tìm thấy sản phẩm trong hệ thống.")
        {
        }

        public ProductNotFoundException(string message)
            : base(message)
        {
        }

        public ProductNotFoundException(string message, string productId)
            : base(message)
        {
            ProductId = productId;
        }

        public ProductNotFoundException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
