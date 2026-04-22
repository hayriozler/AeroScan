using Domain.Interfaces;

namespace Domain.Common;



public abstract class Entity
{
}
public abstract class Entity<T> : Entity, IKey<T>
{
    public T Id { get; private set; } = default!;
}
