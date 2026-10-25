namespace ProductManagement.Models
{
    /// <summary>
    /// Lớp biểu diễn đối tượng Sản phẩm trong kho hàng.
    /// Kế thừa giao diện IEntity với Id tương ứng là MaSP.
    /// </summary>
    public class Product : IEntity
    {
        private string _maSP = string.Empty;
        private string _tenSP = string.Empty;
        private decimal _price;
        private int _quantity;

        /// <summary>
        /// Mã sản phẩm (duy nhất, không được rỗng).
        /// </summary>
        public string MaSP
        {
            get => _maSP;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Mã sản phẩm không được để trống.", nameof(MaSP));
                _maSP = value.Trim().ToUpper();
            }
        }

        /// <summary>
        /// Tên sản phẩm (không được rỗng).
        /// </summary>
        public string TenSP
        {
            get => _tenSP;
            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    throw new ArgumentException("Tên sản phẩm không được để trống.", nameof(TenSP));
                _tenSP = value.Trim();
            }
        }

        /// <summary>
        /// Đơn giá sản phẩm (không được âm).
        /// </summary>
        public decimal Price
        {
            get => _price;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(Price), "Đơn giá không được nhận giá trị âm.");
                _price = value;
            }
        }

        /// <summary>
        /// Số lượng tồn kho (không được âm).
        /// </summary>
        public int Quantity
        {
            get => _quantity;
            set
            {
                if (value < 0)
                    throw new ArgumentOutOfRangeException(nameof(Quantity), "Số lượng sản phẩm không được nhận giá trị âm.");
                _quantity = value;
            }
        }

        /// <summary>
        /// Triển khai IEntity: Id ánh xạ tới MaSP.
        /// </summary>
        public string Id => MaSP;

        /// <summary>
        /// Tổng thành tiền = Đơn giá * Số lượng.
        /// </summary>
        public decimal TotalValue => Price * Quantity;

        public Product()
        {
        }

        public Product(string maSP, string tenSP, decimal price, int quantity)
        {
            MaSP = maSP;
            TenSP = tenSP;
            Price = price;
            Quantity = quantity;
        }

        public override string ToString()
        {
            return $"[Mã: {MaSP,-10} | Tên: {TenSP,-28} | Đơn giá: {Price,14:N0} đ | SL: {Quantity,5} | Thành tiền: {TotalValue,16:N0} đ]";
        }
    }
}
