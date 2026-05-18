using System.Linq.Expressions;
using MCQSystem.Application.Common;
using MCQSystem.Application.Interfaces;
using MCQSystem.Domain.Common;
using MongoDB.Driver;

namespace MCQSystem.Persistence.Repositories;

public sealed class MongoRepository<T>(MongoDbContext context) : IRepository<T> where T : BaseEntity
{
    private readonly IMongoCollection<T> _collection = context.Collection<T>();

    public async Task<T?> GetByIdAsync(string id, CancellationToken cancellationToken = default)
    {
        var filter = Builders<T>.Filter.And(
            Builders<T>.Filter.Eq(x => x.Id, id),
            Builders<T>.Filter.Eq(x => x.IsDeleted, false));

        return await _collection.Find(filter).FirstOrDefaultAsync(cancellationToken);
    }

    public async Task<IReadOnlyList<T>> ListAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        return await _collection
            .Find(BuildFilter(predicate))
            .SortByDescending(x => x.CreatedAtUtc)
            .ToListAsync(cancellationToken);
    }

    public async Task<PagedResult<T>> PageAsync(PaginationRequest pagination, Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
    {
        var filter = BuildFilter(predicate);
        var total = await _collection.CountDocumentsAsync(filter, cancellationToken: cancellationToken);
        var items = await _collection
            .Find(filter)
            .SortByDescending(x => x.CreatedAtUtc)
            .Skip((pagination.PageNumber - 1) * pagination.PageSize)
            .Limit(pagination.PageSize)
            .ToListAsync(cancellationToken);

        return new PagedResult<T>
        {
            Items = items,
            PageNumber = pagination.PageNumber,
            PageSize = pagination.PageSize,
            TotalCount = total
        };
    }

    public async Task<long> CountAsync(Expression<Func<T, bool>>? predicate = null, CancellationToken cancellationToken = default)
        => await _collection.CountDocumentsAsync(BuildFilter(predicate), cancellationToken: cancellationToken);

    public async Task<T> AddAsync(T entity, CancellationToken cancellationToken = default)
    {
        entity.CreatedAtUtc = entity.CreatedAtUtc == default ? DateTime.UtcNow : entity.CreatedAtUtc;
        await _collection.InsertOneAsync(entity, cancellationToken: cancellationToken);
        return entity;
    }

    public async Task UpdateAsync(T entity, CancellationToken cancellationToken = default)
    {
        entity.UpdatedAtUtc = DateTime.UtcNow;
        await _collection.ReplaceOneAsync(x => x.Id == entity.Id, entity, cancellationToken: cancellationToken);
    }

    public async Task DeleteAsync(string id, CancellationToken cancellationToken = default)
    {
        var update = Builders<T>.Update
            .Set(x => x.IsDeleted, true)
            .Set(x => x.UpdatedAtUtc, DateTime.UtcNow);
        await _collection.UpdateOneAsync(x => x.Id == id, update, cancellationToken: cancellationToken);
    }

    private static FilterDefinition<T> BuildFilter(Expression<Func<T, bool>>? predicate)
    {
        var filter = Builders<T>.Filter.Eq(x => x.IsDeleted, false);

        if (predicate is not null)
        {
            filter &= Builders<T>.Filter.Where(predicate);
        }

        return filter;
    }
}
