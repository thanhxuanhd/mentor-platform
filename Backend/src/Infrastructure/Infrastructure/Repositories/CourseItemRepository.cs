using Contract.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories;

public class CourseItemRepository(ApplicationDbContext context)
    : BaseRepository<CourseItem, Guid>(context), ICourseItemRepository;