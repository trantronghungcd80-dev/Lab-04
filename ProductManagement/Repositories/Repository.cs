using ProductManagement.Models;

namespace ProductManagement.Repositories
{
    /// <summary>
    /// Generic repository quản lý danh sách các thực thể trong bộ nhớ.
    /// Áp dụng ràng buộc kiểu (Generic Constraint): T phải triển khai IEntity.
    /// </summary>
    /// <typeparam name="T">Kiểu dữ liệu thực thể kế thừa IEntity</typeparam>
    public class Repository<T> where T : IEntity
    {
        protected readonly List<T> _items = new();

        /// <summary>
        /// Thêm mới một thực thể vào kho lưu trữ.
        /// </summary>
        /// <param name="entity">Thực thể cần thêm</param>
        /// <exception cref="ArgumentNullException">Ném ra khi entity bị null</exception>
        public virtual void Add(T entity)
        {
            if (entity == null)
                throw new ArgumentNullException(nameof(entity), "Thực thể cần thêm không được null.");

            _items.Add(entity);
        }

        /// <summary>
        /// Xóa một thực thể dựa trên Id.
        /// </summary>
        /// <param name="id">Mã định danh của thực thể</param>
        /// <returns>True nếu xóa thành công, False nếu không tìm thấy</returns>
        public virtual bool Remove(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return false;

            var entity = FindById(id);
            if (entity == null)
                return false;

            return _items.Remove(entity);
        }

        /// <summary>
        /// Tìm thực thể đầu tiên có Id trùng khớp (không phân biệt chữ hoa/thường).
        /// </summary>
        /// <param name="id">Mã định danh cần tìm</param>
        /// <returns>Thực thể nếu tìm thấy, ngược lại trả về null/default</returns>
        public virtual T? FindById(string id)
        {
            if (string.IsNullOrWhiteSpace(id))
                return default;

            return _items.FirstOrDefault(item => string.Equals(item.Id, id.Trim(), StringComparison.OrdinalIgnoreCase));
        }

        /// <summary>
        /// Tìm kiếm và lọc danh sách thực thể theo một vị từ điều kiện (Func delegate).
        /// </summary>
        /// <param name="predicate">Hàm ủy thác Func nhận vào T và trả về bool</param>
        /// <returns>Tập hợp các thực thể thỏa mãn điều kiện</returns>
        /// <exception cref="ArgumentNullException">Ném ra khi predicate bị null</exception>
        public virtual IEnumerable<T> Find(Func<T, bool> predicate)
        {
            if (predicate == null)
                throw new ArgumentNullException(nameof(predicate), "Điều kiện lọc (predicate) không được null.");

            return _items.Where(predicate).ToList();
        }

        /// <summary>
        /// Lấy toàn bộ danh sách thực thể hiện có.
        /// </summary>
        /// <returns>Danh sách thực thể</returns>
        public virtual IEnumerable<T> GetAll()
        {
            return _items.ToList();
        }

        /// <summary>
        /// Số lượng thực thể trong repository.
        /// </summary>
        public virtual int Count => _items.Count;
    }
}
