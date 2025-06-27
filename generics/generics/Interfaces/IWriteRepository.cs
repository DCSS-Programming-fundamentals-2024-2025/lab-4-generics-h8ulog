namespace generics.Interfaces
{
    public interface IWriteRepository<in TEntity, in TKey>
    {
        void Add(TKey id, TEntity entity);
        void Remove(TKey id);
    }
}
