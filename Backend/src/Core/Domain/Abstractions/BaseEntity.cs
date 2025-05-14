namespace Domain.Abstractions;

public abstract class BaseEntity<TPrimaryKey> where TPrimaryKey : struct
{
    public TPrimaryKey Id { get; set; }
}