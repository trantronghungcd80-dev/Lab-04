namespace ProductManagement.Models
{
    /// <summary>
    /// Giao diện định nghĩa thực thể có định danh Id (chuỗi).
    /// Dùng làm ràng buộc generic (type constraint) cho Repository<T>.
    /// </summary>
    public interface IEntity
    {
        string Id { get; }
    }
}
