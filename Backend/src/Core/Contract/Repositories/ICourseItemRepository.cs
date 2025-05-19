using Domain.Entities;

namespace Contract.Repositories;

public interface ICourseItemRepository : IBaseRepository<CourseItem, Guid>
{
    Task<CourseItem?> GetByIdAsync(Guid courseId, Guid resourceId);
    Task<List<CourseItem>> GetAllByCourseIdAsync(Guid courseId);
}
