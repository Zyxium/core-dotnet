using Core.DotNet.Domain.SeedWork;
using Core.DotNet.Extensions.Utilities;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System.Linq.Expressions;

namespace Core.DotNet.Infrastructure;

public abstract class BaseRepository<TDbContext, TEntity> : IRepository<TEntity>
    where TDbContext : DbContext
    where TEntity : BaseEntity
{
    protected readonly TDbContext Context;

    private readonly IHttpContextAccessor _httpContextAccessor;

    public BaseRepository(TDbContext dbContext)
    {
        Context = dbContext;
    }

    public BaseRepository(TDbContext dbContext, IHttpContextAccessor httpContextAccessor)
    {
        Context = dbContext;

        _httpContextAccessor = httpContextAccessor;
    }

    public TEntity Get(Guid id)
    {
        return Context.Set<TEntity>().Find(id);
    }

    public IList<TEntity> List()
    {
        return Context.Set<TEntity>().ToList();
    }

    public IList<TEntity> List(Expression<Func<TEntity, bool>> expression)
    {
        return Context.Set<TEntity>().Where(expression).ToList();
    }

    public int Insert(TEntity entity)
    {
        SetEntityInfo(entity);

        Context.Set<TEntity>().Add(entity);

        return Context.SaveChanges();
    }

    public int InsertRange(List<TEntity> entities)
    {
        entities.ForEach(entity =>
        {
            SetEntityInfo(entity);
        });

        Context.Set<TEntity>().AddRange(entities);

        return Context.SaveChanges();
    }

    public int Update(TEntity entity)
    {
        SetEntityInfo(entity);

        Context.Entry<TEntity>(entity).State = EntityState.Modified;

        return Context.SaveChanges();
    }

    public int UpdateRange(List<TEntity> entities)
    {
        entities.ForEach(entity =>
        {
            SetEntityInfo(entity);
            Context.Entry<TEntity>(entity).State = EntityState.Modified;
        });

        return Context.SaveChanges();
    }

    public int Delete(TEntity entity)
    {
        // Is soft delete
        if (entity is ISoftDelete softDelete)
        {
            var currentTime = DateTime.UtcNow;

            softDelete.DeletedAt = currentTime;
            softDelete.DeletedBy = GetUserId();
            entity.UpdatedAt = currentTime;
            entity.UpdatedBy = GetUserId();
        }
        else
        {
            Context.Set<TEntity>().Remove(entity);
        }

        return Context.SaveChanges();
    }
    
    public int DeleteRange(List<TEntity> entities)
    {
        entities.ForEach(entity =>
        {
            // Is soft delete
            if (entity is ISoftDelete softDelete)
            {
                var currentTime = DateTime.UtcNow;

                softDelete.DeletedAt = currentTime;
                softDelete.DeletedBy = GetUserId();
                entity.UpdatedAt = currentTime;
                entity.UpdatedBy = GetUserId();
            }
            else
            {
                Context.Set<TEntity>().Remove(entity);
            }
        });
        
        return Context.SaveChanges();
    }

    private string GetUserId()
    {
        return (_httpContextAccessor != null && _httpContextAccessor.HasAuthorization()) ? _httpContextAccessor.GetUserId() : null;
    }

    public void SetEntityInfo(BaseEntity entity)
    {
        var repeatedEntities = new List<object>();
        var currentDateTime = DateTime.UtcNow;
        var userId = GetUserId();

        SetEntityInfo(entity, repeatedEntities, currentDateTime, userId);
    }

    public void SetEntityInfo(BaseEntity entity, List<object> repeatedEntities, DateTime currentDateTime, string userId)
    {
        if (repeatedEntities.Exists(m => m.Equals(entity)))
            return;

        var propertiesInfo = entity.GetType().GetProperties();

        for (int i = 0; i < propertiesInfo.Length; i++)
        {
            var propValue = propertiesInfo[i].GetValue(entity);

            if (propValue is null)
                continue;

            if (propValue.GetType().IsICollection() &&
                propValue is IEnumerable<BaseEntity> collectionValues)
            {
                for (int j = 0; j < collectionValues.Count(); j++)
                {
                    if (collectionValues.ElementAt(j) is BaseEntity childEntity)
                    {
                        repeatedEntities.Add(entity);
                        SetEntityInfo(childEntity, repeatedEntities, currentDateTime, userId);
                    }
                }
            }
            else if (propValue is BaseEntity childEntity)
            {
                repeatedEntities.Add(entity);
                SetEntityInfo(childEntity, repeatedEntities, currentDateTime, userId);
            }
        }

        if (entity.CreatedAt == DateTime.MinValue)
            entity.CreatedAt = currentDateTime;

        if (string.IsNullOrEmpty(entity.CreatedBy))
            entity.CreatedBy = string.IsNullOrEmpty(userId) ? "Anonymous" : userId;

        entity.UpdatedAt = currentDateTime;
        entity.UpdatedBy = string.IsNullOrEmpty(userId) ? "Anonymous" : userId;
    }
}