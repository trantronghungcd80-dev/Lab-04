using System.Text;
using ProductManagement.Exceptions;
using ProductManagement.Models;
using ProductManagement.Services;

namespace ProductManagement
{
    internal class Program
    {
        private static readonly ProductService _productService = new();

        private static void Main(string[] args)
        {
            // Cấu hình hiển thị tiếng Việt UTF-8 trên Windows Console
            Console.OutputEncoding = Encoding.UTF8;
            Console.InputEncoding = Encoding.UTF8;

            // Đăng ký nhận sự kiện (Event Listener) từ ProductService
            RegisterServiceEvents();

            // Nạp dữ liệu mẫu ban đầu để thuận tiện kiểm thử
            SeedInitialData();

            bool isRunning = true;
            while (isRunning)
            {
                DisplayMenu();
                Console.Write("Chọn: ");
                string? choice = Console.ReadLine()?.Trim();
                Console.WriteLine();

                switch (choice)
                {
                    case "1":
                        HandleAddProduct();
                        break;
                    case "2":
                        HandleDisplayAllProducts();
                        break;
                    case "3":
                        HandleFindById();
                        break;
                    case "4":
                        HandleSearchByName();
                        break;
                    case "5":
                        HandleFilterByPriceRange();
                        break;
                    case "6":
                        HandleRemoveProduct();
                        break;
                    case "7":
                        HandleCalculateTotalInventory();
                        break;
                    case "0":
                        isRunning = false;
                        Console.ForegroundColor = ConsoleColor.Cyan;
                        Console.WriteLine("Cảm ơn bạn đã sử dụng chương trình Quản lý sản phẩm. Tạm biệt!");
                        Console.ResetColor();
                        break;
                    default:
                        PrintWarning("Lựa chọn không hợp lệ! Vui lòng chọn các số từ 0 đến 7.");
                        break;
                }

                if (isRunning)
                {
                    Console.WriteLine();
                    Console.Write("Nhấn phím bất kỳ để tiếp tục...");
                    Console.ReadKey(true);
                    Console.WriteLine("\n");
                }
            }
        }

        #region Event Registration

        /// <summary>
        /// Đăng ký phương thức xử lý (Event Handlers) cho các sự kiện của ProductService.
        /// Minh họa việc sử dụng delegate/event để thông báo khi dữ liệu thay đổi.
        /// </summary>
        private static void RegisterServiceEvents()
        {
            _productService.OnProductAdded += (sender, product) =>
            {
                Console.ForegroundColor = ConsoleColor.Green;
                Console.WriteLine($"\n>>> [SỰ KIỆN: ON_PRODUCT_ADDED] Đã thêm thành công sản phẩm: [{product.MaSP}] {product.TenSP} - Giá: {product.Price:N0} đ - SL: {product.Quantity}");
                Console.ResetColor();
            };

            _productService.OnProductRemoved += (sender, product) =>
            {
                Console.ForegroundColor = ConsoleColor.Yellow;
                Console.WriteLine($"\n>>> [SỰ KIỆN: ON_PRODUCT_REMOVED] Đã xóa thành công sản phẩm: [{product.MaSP}] {product.TenSP} khỏi hệ thống kho!");
                Console.ResetColor();
            };
        }

        #endregion

        #region Initial Data Seeding

        /// <summary>
        /// Nạp sẵn một số sản phẩm khởi tạo cho kho hàng.
        /// </summary>
        private static void SeedInitialData()
        {
            try
            {
                _productService.AddProduct(new Product("SP01", "Laptop Dell XPS 15", 35000000m, 5));
                _productService.AddProduct(new Product("SP02", "Chuột Logitech MX Master 3S", 2200000m, 20));
                _productService.AddProduct(new Product("SP03", "Bàn phím cơ Keychron K2", 1850000m, 15));
                _productService.AddProduct(new Product("SP04", "Màn hình LG UltraGear 27\"", 7500000m, 8));
                _productService.AddProduct(new Product("SP05", "Tai nghe Sony WH-1000XM5", 6990000m, 12));
            }
            catch (Exception ex)
            {
                PrintError($"Lỗi khởi tạo dữ liệu mẫu: {ex.Message}");
            }
        }

        #endregion

        #region Menu UI

        private static void DisplayMenu()
        {
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("========================================");
            Console.WriteLine("        ===== PRODUCT MANAGER =====     ");
            Console.WriteLine("========================================");
            Console.ResetColor();
            Console.WriteLine("  1. Thêm sản phẩm");
            Console.WriteLine("  2. Xuất danh sách");
            Console.WriteLine("  3. Tìm theo mã");
            Console.WriteLine("  4. Tìm theo tên");
            Console.WriteLine("  5. Lọc theo khoảng giá");
            Console.WriteLine("  6. Xóa sản phẩm");
            Console.WriteLine("  7. Tính tổng giá trị kho");
            Console.WriteLine("  0. Thoát");
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("========================================");
            Console.ResetColor();
        }

        #endregion

        #region Function Handlers

        /// <summary>
        /// Chức năng 1: Thêm sản phẩm mới vào kho hàng.
        /// Bắt lỗi người dùng nhập sai kiểu, nhập mã rỗng hoặc mã trùng (DuplicateProductException).
        /// </summary>
        private static void HandleAddProduct()
        {
            PrintHeader("--- CHỨC NĂNG 1: THÊM SẢN PHẨM MỚI ---");

            try
            {
                Console.Write("Nhập mã sản phẩm (MaSP): ");
                string? maSP = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(maSP))
                {
                    PrintWarning("Mã sản phẩm không được rỗng!");
                    return;
                }

                Console.Write("Nhập tên sản phẩm (TenSP): ");
                string? tenSP = Console.ReadLine()?.Trim();
                if (string.IsNullOrWhiteSpace(tenSP))
                {
                    PrintWarning("Tên sản phẩm không được rỗng!");
                    return;
                }

                Console.Write("Nhập đơn giá (Price >= 0): ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal price))
                {
                    throw new FormatException("Đơn giá phải là số thực hợp lệ (ví dụ: 1500000)!");
                }

                Console.Write("Nhập số lượng (Quantity >= 0): ");
                if (!int.TryParse(Console.ReadLine(), out int quantity))
                {
                    throw new FormatException("Số lượng phải là số nguyên hợp lệ (ví dụ: 10)!");
                }

                // Khởi tạo đối tượng Product (đã có kiểm tra validation trong property setter)
                var newProduct = new Product(maSP, tenSP, price, quantity);

                // Thêm vào service (sẽ ném DuplicateProductException nếu trùng mã)
                _productService.AddProduct(newProduct);
                PrintSuccess($"Sản phẩm '{newProduct.TenSP}' đã được lưu trữ vào bộ nhớ.");
            }
            catch (DuplicateProductException dupEx)
            {
                PrintError($"[DuplicateProductException] {dupEx.Message}");
            }
            catch (ArgumentOutOfRangeException outEx)
            {
                PrintError($"[Dữ liệu ngoài phạm vi] {outEx.Message}");
            }
            catch (ArgumentException argEx)
            {
                PrintError($"[Dữ liệu không hợp lệ] {argEx.Message}");
            }
            catch (FormatException fmtEx)
            {
                PrintError($"[Lỗi định dạng] {fmtEx.Message}");
            }
            catch (Exception ex)
            {
                PrintError($"[Lỗi phát sinh ngoài dự kiến] {ex.Message}");
            }
        }

        /// <summary>
        /// Chức năng 2: Xuất toàn bộ danh sách sản phẩm dưới dạng bảng chuẩn.
        /// </summary>
        private static void HandleDisplayAllProducts()
        {
            PrintHeader("--- CHỨC NĂNG 2: XUẤT DANH SÁCH SẢN PHẨM ---");

            var products = _productService.GetAllProducts().ToList();
            if (!products.Any())
            {
                PrintWarning("Kho hàng hiện tại đang trống. Hãy thêm sản phẩm mới!");
                return;
            }

            RenderProductTable(products);
        }

        /// <summary>
        /// Chức năng 3: Tìm sản phẩm theo mã định danh (Id / MaSP).
        /// </summary>
        private static void HandleFindById()
        {
            PrintHeader("--- CHỨC NĂNG 3: TÌM SẢN PHẨM THEO MÃ ---");

            Console.Write("Nhập mã sản phẩm cần tìm: ");
            string? maSP = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(maSP))
            {
                PrintWarning("Mã sản phẩm nhập vào không được để trống!");
                return;
            }

            var product = _productService.GetProductById(maSP);
            if (product != null)
            {
                PrintSuccess($"Tìm thấy sản phẩm có mã '{product.MaSP}':");
                RenderProductTable(new[] { product });
            }
            else
            {
                PrintWarning($"Không tìm thấy sản phẩm nào có mã '{maSP}' trong hệ thống!");
            }
        }

        /// <summary>
        /// Chức năng 4: Tìm sản phẩm theo tên sử dụng Func<Product, bool>.
        /// </summary>
        private static void HandleSearchByName()
        {
            PrintHeader("--- CHỨC NĂNG 4: TÌM SẢN PHẨM THEO TÊN ---");

            Console.Write("Nhập từ khóa tên sản phẩm cần tìm: ");
            string? keyword = Console.ReadLine()?.Trim();

            if (string.IsNullOrWhiteSpace(keyword))
            {
                PrintWarning("Từ khóa tìm kiếm không được rỗng!");
                return;
            }

            var results = _productService.Search(keyword).ToList();
            if (results.Any())
            {
                PrintSuccess($"Tìm thấy {results.Count} sản phẩm có tên chứa '{keyword}':");
                RenderProductTable(results);
            }
            else
            {
                PrintWarning($"Không có sản phẩm nào có tên chứa từ khóa '{keyword}'!");
            }
        }

        /// <summary>
        /// Chức năng 5: Lọc sản phẩm theo khoảng giá [minPrice, maxPrice] bằng Func<Product, bool>.
        /// </summary>
        private static void HandleFilterByPriceRange()
        {
            PrintHeader("--- CHỨC NĂNG 5: LỌC SẢN PHẨM THEO KHOẢNG GIÁ ---");

            try
            {
                Console.Write("Nhập giá thấp nhất (Min Price): ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal minPrice))
                {
                    throw new FormatException("Giá thấp nhất phải là số hợp lệ!");
                }

                Console.Write("Nhập giá cao nhất (Max Price): ");
                if (!decimal.TryParse(Console.ReadLine(), out decimal maxPrice))
                {
                    throw new FormatException("Giá cao nhất phải là số hợp lệ!");
                }

                var results = _productService.FilterByPriceRange(minPrice, maxPrice).ToList();
                if (results.Any())
                {
                    PrintSuccess($"Tìm thấy {results.Count} sản phẩm trong khoảng giá từ {minPrice:N0} đ đến {maxPrice:N0} đ:");
                    RenderProductTable(results);
                }
                else
                {
                    PrintWarning($"Không có sản phẩm nào nằm trong khoảng giá từ {minPrice:N0} đ đến {maxPrice:N0} đ.");
                }
            }
            catch (FormatException fmtEx)
            {
                PrintError($"[Lỗi định dạng] {fmtEx.Message}");
            }
            catch (ArgumentException argEx)
            {
                PrintError($"[Dữ liệu không hợp lệ] {argEx.Message}");
            }
            catch (Exception ex)
            {
                PrintError($"[Lỗi] {ex.Message}");
            }
        }

        /// <summary>
        /// Chức năng 6: Xóa sản phẩm khỏi kho theo mã.
        /// Bắt ProductNotFoundException nếu sản phẩm không tồn tại.
        /// </summary>
        private static void HandleRemoveProduct()
        {
            PrintHeader("--- CHỨC NĂNG 6: XÓA SẢN PHẨM ---");

            try
            {
                Console.Write("Nhập mã sản phẩm cần xóa: ");
                string? maSP = Console.ReadLine()?.Trim();

                if (string.IsNullOrWhiteSpace(maSP))
                {
                    PrintWarning("Mã sản phẩm cần xóa không được rỗng!");
                    return;
                }

                // Gọi service xóa (sẽ ném ProductNotFoundException nếu không tìm thấy)
                _productService.RemoveProduct(maSP);
                PrintSuccess($"Xóa sản phẩm mã '{maSP}' hoàn tất.");
            }
            catch (ProductNotFoundException notFoundEx)
            {
                PrintError($"[ProductNotFoundException] {notFoundEx.Message}");
            }
            catch (ArgumentException argEx)
            {
                PrintError($"[Tham số không hợp lệ] {argEx.Message}");
            }
            catch (Exception ex)
            {
                PrintError($"[Lỗi hệ thống] {ex.Message}");
            }
        }

        /// <summary>
        /// Chức năng 7: Tính và hiển thị tổng giá trị hàng tồn kho (Price * Quantity).
        /// </summary>
        private static void HandleCalculateTotalInventory()
        {
            PrintHeader("--- CHỨC NĂNG 7: TÍNH TỔNG GIÁ TRỊ KHO HÀNG ---");

            var all = _productService.GetAllProducts().ToList();
            decimal totalValue = _productService.CalculateTotalInventoryValue();
            int totalQuantity = all.Sum(p => p.Quantity);

            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine($"  * Số loại mặt hàng hiện có: {all.Count:N0} loại");
            Console.WriteLine($"  * Tổng số lượng cá thể trong kho: {totalQuantity:N0} chiếc/cái");
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"  * TỔNG GIÁ TRỊ TOÀN BỘ KHO HÀNG: {totalValue:N0} VNĐ");
            Console.ResetColor();
        }

        #endregion

        #region Table Rendering & Console Helpers

        /// <summary>
        /// In bảng dữ liệu sản phẩm với định dạng viền kẻ và canh lề chuẩn.
        /// </summary>
        private static void RenderProductTable(IEnumerable<Product> products)
        {
            var list = products.ToList();
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("+------------+--------------------------------+-----------------+----------+------------------+");
            Console.ResetColor();
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("| {0,-10} | {1,-30} | {2,15} | {3,8} | {4,16} |", "Mã SP", "Tên Sản Phẩm", "Đơn Giá (VNĐ)", "SL Kho", "Thành Tiền (VNĐ)");
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("+------------+--------------------------------+-----------------+----------+------------------+");
            Console.ResetColor();

            foreach (var p in list)
            {
                Console.WriteLine("| {0,-10} | {1,-30} | {2,15:N0} | {3,8} | {4,16:N0} |",
                    p.MaSP,
                    p.TenSP.Length > 30 ? p.TenSP.Substring(0, 27) + "..." : p.TenSP,
                    p.Price,
                    p.Quantity,
                    p.TotalValue);
            }

            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("+------------+--------------------------------+-----------------+----------+------------------+");
            Console.ResetColor();

            decimal total = list.Sum(p => p.TotalValue);
            int totalQty = list.Sum(p => p.Quantity);
            Console.ForegroundColor = ConsoleColor.Cyan;
            Console.WriteLine("| {0,-43} | {1,26} | {2,16:N0} |",
                $"Tổng cộng: {list.Count} mặt hàng",
                $"Tổng SL: {totalQty}",
                total);
            Console.ForegroundColor = ConsoleColor.DarkGray;
            Console.WriteLine("+---------------------------------------------+----------------------------+------------------+");
            Console.ResetColor();
        }

        private static void PrintHeader(string title)
        {
            Console.ForegroundColor = ConsoleColor.Magenta;
            Console.WriteLine(title);
            Console.ResetColor();
        }

        private static void PrintSuccess(string message)
        {
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine($"[THÀNH CÔNG] {message}");
            Console.ResetColor();
        }

        private static void PrintWarning(string message)
        {
            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine($"[CẢNH BÁO] {message}");
            Console.ResetColor();
        }

        private static void PrintError(string message)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"[THẤT BẠI] {message}");
            Console.ResetColor();
        }

        #endregion
    }
}
