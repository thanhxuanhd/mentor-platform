using System;
using Domain.Entities;

namespace Contract.Repositories;

public interface IApplicationRepository : IBaseRepository<MentorApplication, Guid>
{

}
