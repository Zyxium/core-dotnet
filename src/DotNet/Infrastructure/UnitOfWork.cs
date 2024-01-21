using Microsoft.EntityFrameworkCore;

namespace Core.DotNet.Infrastructure;

public class UnitOfWork : IUnitOfWork
{
    private readonly DbContext _dbContext;

    public UnitOfWork(DbContext dataContext)
    {
        _dbContext = dataContext;
    }

    public int SaveChanges()
    {
        return _dbContext.SaveChanges();
    }
}