namespace Domain.Abstractions;

public abstract class BaseEntity
{
    protected BaseEntity(uint id)
    {
        Id = id;
    }

    protected BaseEntity()
    {

    }

    public uint Id { get; set; }
}