# COMP1019 - LẬP TRÌNH TRÊN WINDOWS
## BUỔI 4 - LAB 04: Exception, Delegate/Event, Func/Action và Generic trong C#

---

### 1. Giới thiệu tổng quan
Dự án được xây dựng theo yêu cầu của bài thực hành **Lab 04 - COMP1019 (Lập trình trên Windows)**, tập trung vào các kỹ thuật nâng cao trong C# và .NET:
- **Generic Repository**: Áp dụng Generic class `Repository<T>` với ràng buộc kiểu `where T : IEntity`.
- **Custom Exceptions**: Xây dựng ít nhất 2 ngoại lệ tự tạo (`DuplicateProductException`, `ProductNotFoundException`) và bắt lỗi nhập liệu toàn diện.
- **Delegate & Event**: Sử dụng custom delegate `ProductActionHandler` và event `OnProductAdded`, `OnProductRemoved` để thông báo khi dữ liệu thay đổi.
- **Func<T, bool>**: Ứng dụng lambda expression và `Func<Product, bool>` cho các tác vụ tìm kiếm và lọc dữ liệu linh hoạt.
- **Kiến trúc phân tầng rõ ràng**: Tách biệt giữa Thực thể dữ liệu (`Models`), Kho lưu trữ (`Repositories`), Tầng nghiệp vụ (`Services`), Ngoại lệ (`Exceptions`) và Giao diện tương tác (`Program`).

---

### 2. Cấu trúc thư mục dự án

```text
Lab-04/
├── Lab04.sln                               # Solution Visual Studio
├── ProductManagement/                      # Project chính Console App C# (.NET 10)
│   ├── Models/
│   │   ├── IEntity.cs                      # Interface ràng buộc generic Id { get; }
│   │   └── Product.cs                      # Model Sản phẩm (MaSP, TenSP, Price, Quantity)
│   ├── Exceptions/
│   │   ├── DuplicateProductException.cs    # Ngoại lệ trùng mã sản phẩm
│   │   └── ProductNotFoundException.cs     # Ngoại lệ không tìm thấy sản phẩm
│   ├── Repositories/
│   │   └── Repository.cs                   # Generic Repository<T> where T : IEntity
│   ├── Services/
│   │   └── ProductService.cs               # Nghiệp vụ kho, phát sinh Event, dùng Func lọc
│   ├── Program.cs                          # Menu Console, đăng ký Event, bắt Try-Catch
│   └── ProductManagement.csproj
├── ProductManagement.Tests/                # Project kiểm thử tự động xUnit (13 Unit Tests)
│   ├── ProductServiceTests.cs              # Bộ test kiểm tra toàn bộ ngoại lệ, event, func
│   └── ProductManagement.Tests.csproj
├── .gitignore                              # File cấu hình bỏ qua thư mục build .NET
└── README.md                               # Tài liệu hướng dẫn và báo cáo kiểm thử
```

---

### 3. Chi tiết triển khai kỹ thuật

#### 3.1. Ràng buộc Generic (`IEntity` & `Repository<T>`)
- Giao diện `IEntity` có thuộc tính `string Id { get; }`.
- Lớp `Product` kế thừa `IEntity` với `public string Id => MaSP;`.
- Lớp `Repository<T> where T : IEntity` quản lý danh sách `List<T>`, cung cấp các thao tác CRUD cơ bản:
  - `Add(T entity)`
  - `Remove(string id)`
  - `FindById(string id)`
  - `Find(Func<T, bool> predicate)`
  - `GetAll()`

#### 3.2. Ngoại lệ tự tạo (`Exceptions`)
- `DuplicateProductException`: Kế thừa `Exception`, được ném ra từ `ProductService.AddProduct` khi mã sản phẩm đã tồn tại trong kho hàng.
- `ProductNotFoundException`: Kế thừa `Exception`, được ném ra từ `ProductService.RemoveProduct` khi người dùng yêu cầu xóa mã sản phẩm không có trong kho.
- Các thuộc tính `Price` và `Quantity` trong `Product` kiểm tra giá trị âm và ném `ArgumentOutOfRangeException`. Thuộc tính `MaSP`, `TenSP` kiểm tra chuỗi rỗng và ném `ArgumentException`.

#### 3.3. Delegate và Event
- Định nghĩa custom delegate:
  ```csharp
  public delegate void ProductActionHandler(object? sender, Product product);
  ```
- Định nghĩa event trong `ProductService`:
  ```csharp
  public event ProductActionHandler? OnProductAdded;
  public event ProductActionHandler? OnProductRemoved;
  ```
- `Program.cs` đăng ký lắng nghe (subscribe) các sự kiện trên và hiển thị thông báo thời gian thực lên màn hình mỗi khi thêm hoặc xóa sản phẩm thành công.

#### 3.4. Ứng dụng `Func<Product, bool>`
- Tìm kiếm theo tên sản phẩm:
  ```csharp
  Func<Product, bool> searchFilter = p => 
      p.TenSP.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0;
  return _repository.Find(searchFilter);
  ```
- Lọc theo khoảng giá `[minPrice, maxPrice]`:
  ```csharp
  Func<Product, bool> priceRangeFilter = p => p.Price >= minPrice && p.Price <= maxPrice;
  return _repository.Find(priceRangeFilter);
  ```

---

### 4. Menu và các chức năng thực hiện

Menu điều khiển của chương trình:
```text
========================================
        ===== PRODUCT MANAGER =====     
========================================
  1. Thêm sản phẩm
  2. Xuất danh sách
  3. Tìm theo mã
  4. Tìm theo tên
  5. Lọc theo khoảng giá
  6. Xóa sản phẩm
  7. Tính tổng giá trị kho
  0. Thoát
========================================
Chọn: 
```

---

### 5. Báo cáo kiểm thử & Kết quả chạy thực tế (Console Output)

#### 5.1. Chức năng 2: Xuất toàn bộ danh sách sản phẩm (Bảng định dạng chuẩn)
```text
--- CHỨC NĂNG 2: XUẤT DANH SÁCH SẢN PHẨM ---
+------------+--------------------------------+-----------------+----------+------------------+
| Mã SP      | Tên Sản Phẩm                   |   Đơn Giá (VNĐ) |   SL Kho | Thành Tiền (VNĐ) |
+------------+--------------------------------+-----------------+----------+------------------+
| SP01       | Laptop Dell XPS 15             |      35,000,000 |        5 |      175,000,000 |
| SP02       | Chuột Logitech MX Master 3S    |       2,200,000 |       20 |       44,000,000 |
| SP03       | Bàn phím cơ Keychron K2        |       1,850,000 |       15 |       27,750,000 |
| SP04       | Màn hình LG UltraGear 27"      |       7,500,000 |        8 |       60,000,000 |
| SP05       | Tai nghe Sony WH-1000XM5       |       6,990,000 |       12 |       83,880,000 |
+------------+--------------------------------+-----------------+----------+------------------+
| Tổng cộng: 5 mặt hàng                       |       Tổng SL: 60 |      390,630,000 |
+---------------------------------------------+----------------------------+------------------+
```

#### 5.2. Chức năng 1: Thêm sản phẩm & Bắt lỗi trùng mã (`DuplicateProductException`)
- **Trường hợp hợp lệ**:
  ```text
  --- CHỨC NĂNG 1: THÊM SẢN PHẨM MỚI ---
  Nhập mã sản phẩm (MaSP): SP06
  Nhập tên sản phẩm (TenSP): Ổ cứng SSD Samsung 1TB
  Nhập đơn giá (Price >= 0): 2500000
  Nhập số lượng (Quantity >= 0): 10

  >>> [SỰ KIỆN: ON_PRODUCT_ADDED] Đã thêm thành công sản phẩm: [SP06] Ổ cứng SSD Samsung 1TB - Giá: 2,500,000 đ - SL: 10
  [THÀNH CÔNG] Sản phẩm 'Ổ cứng SSD Samsung 1TB' đã được lưu trữ vào bộ nhớ.
  ```

- **Trường hợp kiểm thử trùng mã (Duplicate)**:
  ```text
  --- CHỨC NĂNG 1: THÊM SẢN PHẨM MỚI ---
  Nhập mã sản phẩm (MaSP): SP01
  Nhập tên sản phẩm (TenSP): Laptop Dell Mới
  Nhập đơn giá (Price >= 0): 30000000
  Nhập số lượng (Quantity >= 0): 2
  [THẤT BẠI] [DuplicateProductException] Lỗi nghiệp vụ: Mã sản phẩm 'SP01' đã tồn tại trong hệ thống!
  ```

- **Trường hợp nhập đơn giá âm / số lượng âm**:
  ```text
  --- CHỨC NĂNG 1: THÊM SẢN PHẨM MỚI ---
  Nhập mã sản phẩm (MaSP): SP07
  Nhập tên sản phẩm (TenSP): Cáp sạc Type-C
  Nhập đơn giá (Price >= 0): -50000
  Nhập số lượng (Quantity >= 0): 10
  [THẤT BẠI] [Dữ liệu ngoài phạm vi] Đơn giá không được nhận giá trị âm. (Parameter 'Price')
  ```

- **Trường hợp nhập chuỗi không phải số cho đơn giá**:
  ```text
  --- CHỨC NĂNG 1: THÊM SẢN PHẨM MỚI ---
  Nhập mã sản phẩm (MaSP): SP08
  Nhập tên sản phẩm (TenSP): Chuột không dây
  Nhập đơn giá (Price >= 0): abcxyz
  [THẤT BẠI] [Lỗi định dạng] Đơn giá phải là số thực hợp lệ (ví dụ: 1500000)!
  ```

#### 5.3. Chức năng 3: Tìm theo mã
```text
--- CHỨC NĂNG 3: TÌM SẢN PHẨM THEO MÃ ---
Nhập mã sản phẩm cần tìm: SP02
[THÀNH CÔNG] Tìm thấy sản phẩm có mã 'SP02':
+------------+--------------------------------+-----------------+----------+------------------+
| Mã SP      | Tên Sản Phẩm                   |   Đơn Giá (VNĐ) |   SL Kho | Thành Tiền (VNĐ) |
+------------+--------------------------------+-----------------+----------+------------------+
| SP02       | Chuột Logitech MX Master 3S    |       2,200,000 |       20 |       44,000,000 |
+------------+--------------------------------+-----------------+----------+------------------+
```

#### 5.4. Chức năng 4: Tìm theo tên (sử dụng `Func<Product, bool>`)
```text
--- CHỨC NĂNG 4: TÌM SẢN PHẨM THEO TÊN ---
Nhập từ khóa tên sản phẩm cần tìm: logitech
[THÀNH CÔNG] Tìm thấy 1 sản phẩm có tên chứa 'logitech':
+------------+--------------------------------+-----------------+----------+------------------+
| Mã SP      | Tên Sản Phẩm                   |   Đơn Giá (VNĐ) |   SL Kho | Thành Tiền (VNĐ) |
+------------+--------------------------------+-----------------+----------+------------------+
| SP02       | Chuột Logitech MX Master 3S    |       2,200,000 |       20 |       44,000,000 |
+------------+--------------------------------+-----------------+----------+------------------+
```

#### 5.5. Chức năng 5: Lọc theo khoảng giá (sử dụng `Func<Product, bool>`)
```text
--- CHỨC NĂNG 5: LỌC SẢN PHẨM THEO KHOẢNG GIÁ ---
Nhập giá thấp nhất (Min Price): 1000000
Nhập giá cao nhất (Max Price): 5000000
[THÀNH CÔNG] Tìm thấy 2 sản phẩm trong khoảng giá từ 1,000,000 đ đến 5,000,000 đ:
+------------+--------------------------------+-----------------+----------+------------------+
| Mã SP      | Tên Sản Phẩm                   |   Đơn Giá (VNĐ) |   SL Kho | Thành Tiền (VNĐ) |
+------------+--------------------------------+-----------------+----------+------------------+
| SP02       | Chuột Logitech MX Master 3S    |       2,200,000 |       20 |       44,000,000 |
| SP03       | Bàn phím cơ Keychron K2        |       1,850,000 |       15 |       27,750,000 |
+------------+--------------------------------+-----------------+----------+------------------+
```

#### 5.6. Chức năng 6: Xóa sản phẩm & Bắt lỗi không tìm thấy (`ProductNotFoundException`)
- **Trường hợp xóa thành công (phát event)**:
  ```text
  --- CHỨC NĂNG 6: XÓA SẢN PHẨM ---
  Nhập mã sản phẩm cần xóa: SP03

  >>> [SỰ KIỆN: ON_PRODUCT_REMOVED] Đã xóa thành công sản phẩm: [SP03] Bàn phím cơ Keychron K2 khỏi hệ thống kho!
  [THÀNH CÔNG] Xóa sản phẩm mã 'SP03' hoàn tất.
  ```

- **Trường hợp xóa mã không tồn tại**:
  ```text
  --- CHỨC NĂNG 6: XÓA SẢN PHẨM ---
  Nhập mã sản phẩm cần xóa: SP999
  [THẤT BẠI] [ProductNotFoundException] Lỗi nghiệp vụ: Không tìm thấy sản phẩm có mã 'SP999' để xóa!
  ```

#### 5.7. Chức năng 7: Tính tổng giá trị kho hàng
```text
--- CHỨC NĂNG 7: TÍNH TỔNG GIÁ TRỊ KHO HÀNG ---
  * Số loại mặt hàng hiện có: 5 loại
  * Tổng số lượng cá thể trong kho: 60 chiếc/cái
  * TỔNG GIÁ TRỊ TOÀN BỘ KHO HÀNG: 390,630,000 VNĐ
```

---

### 6. Tổng hợp các trường hợp kiểm thử lỗi (Test Cases)

| STT | Kịch bản kiểm thử | Dữ liệu đầu vào | Kết quả mong đợi | Kết quả thực tế | Trạng thái |
|:---:|:---|:---|:---|:---|:---:|
| 1 | Thêm sản phẩm có mã trùng | `MaSP: SP01` | Ném `DuplicateProductException`, hiển thị lỗi màu đỏ, chương trình không crash | Bắt đúng ngoại lệ, thông báo rõ ràng | **PASSED** |
| 2 | Nhập đơn giá âm | `Price: -1000` | Ném `ArgumentOutOfRangeException`, hiển thị cảnh báo không được âm | Bắt đúng ngoại lệ | **PASSED** |
| 3 | Nhập số lượng âm | `Quantity: -5` | Ném `ArgumentOutOfRangeException`, hiển thị cảnh báo | Bắt đúng ngoại lệ | **PASSED** |
| 4 | Nhập sai định dạng số | `Price: abc` | Bắt `FormatException`, hướng dẫn người dùng nhập đúng số | Bắt đúng ngoại lệ | **PASSED** |
| 5 | Để trống mã hoặc tên SP | `MaSP: ""` | Bắt `ArgumentException`, cảnh báo không được rỗng | Thông báo nhắc nhở | **PASSED** |
| 6 | Xóa mã SP không tồn tại | `MaSP: SP999` | Ném `ProductNotFoundException`, thông báo không tìm thấy | Bắt đúng ngoại lệ | **PASSED** |
| 7 | Nhập Min Price > Max Price | `Min: 5tr, Max: 1tr` | Bắt `ArgumentException`, báo lỗi logic khoảng giá | Bắt đúng ngoại lệ | **PASSED** |
| 8 | Lựa chọn menu ngoài phạm vi | `Chọn: 99` | Thông báo lựa chọn không hợp lệ, yêu cầu chọn lại | Xử lý đúng | **PASSED** |

---

### 7. Hướng dẫn biên dịch và thực thi

Yêu cầu môi trường: [.NET SDK 8.0, 9.0 hoặc 10.0+](https://dotnet.microsoft.com/download)

1. **Biên dịch toàn bộ Solution**:
   ```bash
   dotnet build
   ```

2. **Chạy ứng dụng Console Quản lý sản phẩm**:
   ```bash
   dotnet run --project ProductManagement
   ```

3. **Chạy bộ kiểm thử tự động (xUnit)**:
   ```bash
   dotnet test
   ```
   Kết quả kiểm thử: `13/13 tests passed (100%)`.
