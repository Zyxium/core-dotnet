namespace Core.DotNet.Infrastructure;

public interface IUnitOfWork
{
    int SaveChanges();
}