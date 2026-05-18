using MCQSystem.Application.Interfaces;
using MCQSystem.Domain.Common;
using Microsoft.Extensions.DependencyInjection;

namespace MCQSystem.Persistence.Repositories;

public sealed class MongoUnitOfWork(IServiceProvider serviceProvider) : IUnitOfWork
{
    public IRepository<T> Repository<T>() where T : BaseEntity
        => serviceProvider.GetRequiredService<IRepository<T>>();

    public Task<int> SaveChangesAsync(CancellationToken cancellationToken = default)
        => Task.FromResult(0);
}
