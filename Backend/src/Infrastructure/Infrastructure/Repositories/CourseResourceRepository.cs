using Contract.Repositories;
using Domain.Entities;
using Infrastructure.Persistence.Data;
using Infrastructure.Repositories.Base;

namespace Infrastructure.Repositories;

public class CourseResourceRepository(ApplicationDbContext context)
    : BaseRepository<CourseResource, Guid>(context), ICourseResourceRepository;