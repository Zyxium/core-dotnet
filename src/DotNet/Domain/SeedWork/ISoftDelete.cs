namespace Core.DotNet.Domain.SeedWork;

public interface ISoftDelete
{
    DateTime? DeletedAt { get; set; }
    string? DeletedBy { get; set; }
}