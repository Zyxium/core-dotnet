namespace Core.DotNet.Domain.SeedWork;

public abstract class BaseEntity
{
    private int? _requestedHashCode;

    public Guid Id { get; set; }
    public DateTime CreatedAt { get; set; }
    public string CreatedBy { get; set; }
    public DateTime UpdatedAt { get; set; }
    public string UpdatedBy { get; set; }
    
    public override bool Equals(object obj)
    {
        if (!(obj is BaseEntity))
            return false;

        if (ReferenceEquals(this, obj))
            return true;

        if (GetType() != obj.GetType())
            return false;

        var item = (BaseEntity)obj;

        if (item.IsTransient() || IsTransient())
            return false;

        return item.Id == Id;
    }

    public override int GetHashCode()
    {
        if (!IsTransient())
        {
            if (!_requestedHashCode.HasValue)
                _requestedHashCode = Id.GetHashCode() ^ 31;

            return _requestedHashCode.Value;
        }

        return base.GetHashCode();
    }

    public bool IsTransient()
    {
        return Id == default;
    }

    public static bool operator ==(BaseEntity left, BaseEntity right)
    {
        if (Equals(left, null))
            return Equals(right, null);

        return left.Equals(right);
    }

    public static bool operator !=(BaseEntity left, BaseEntity right)
    {
        return !(left == right);
    }
}

public abstract class BaseEntitySoftDelete : BaseEntity
{
    public DateTime? DeletedOn { get; set; }
    public string? DeletedBy { get; set; }
}