using MCQSystem.Domain.Common;

namespace MCQSystem.Application.Interfaces;

public interface IUnitOfWork
{
    IRepository<T> Repository<T>() where T : BaseEntity;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
}
