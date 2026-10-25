namespace ProductManagement.Exceptions
{
    /// <summary>
    /// Ngoại lệ tự tạo phát sinh khi cố gắng thêm một sản phẩm có mã đã tồn tại trong kho.
    /// </summary>
    public class DuplicateProductException : Exception
    {
        public string? ProductId { get; }

        public DuplicateProductException()
            : base("Mã sản phẩm đã tồn tại trong hệ thống.")
        {
        }

        public DuplicateProductException(string message)
            : base(message)
        {
        }

        public DuplicateProductException(string message, string productId)
            : base(message)
        {
            ProductId = productId;
        }

        public DuplicateProductException(string message, Exception innerException)
            : base(message, innerException)
        {
        }
    }
}
