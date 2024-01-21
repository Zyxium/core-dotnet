using System.Linq.Expressions;
using Core.DotNet.Domain.SeedWork;

namespace Core.DotNet.Infrastructure;

public interface IRepository<TEntity>
    where TEntity : BaseEntity
{
    TEntity Get(Guid id);

    IList<TEntity> List();

    IList<TEntity> List(Expression<Func<TEntity, bool>> expression);

    int Insert(TEntity entity);

    int InsertRange(List<TEntity> entities);

    int Update(TEntity entity);

    int UpdateRange(List<TEntity> entities);

    int Delete(TEntity entity);
    
    int DeleteRange(List<TEntity> entities);
}