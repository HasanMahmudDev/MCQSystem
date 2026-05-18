using System.Linq.Expressions;
using MCQSystem.Application.Common;
using MCQSystem.Domain.Common;

namespace MCQSystem.Application.Interfaces;

public interface IRepository<T> where T : BaseEntity
{
    Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default);

    Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);

    Task<PagedResult<T>> PageAsync(
        PaginationRequest pagination,
        Expression<Func<T, bool>>? predicate = null,
        CancellationToken cancellationToken = default);

    Task<long> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default);

    Task<T> AddAsync(T entity, CancellationToken cancellationToken = default);

    Task UpdateAsync(T entity, CancellationToken cancellationToken = default);

    Task DeleteAsync(string id, CancellationToken cancellationToken = default);
}
