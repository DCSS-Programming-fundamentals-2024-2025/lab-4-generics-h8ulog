using System.Collections.Generic;
using System.Linq;
using generics.Interfaces;
using generics.Models;

namespace generics.InMemoryRepository
{
    public class InMemoryRepository<TEntity, TKey> : IRepository<TEntity, TKey>,
                                                         IReadOnlyRepository<TEntity, TKey>,
                                                         IWriteRepository<TEntity, TKey>
        where TEntity : class, new()
        where TKey : struct
    {
        private readonly Dictionary<TKey, TEntity> _storage = new Dictionary<TKey, TEntity>();

        public void Add(TKey id, TEntity entity)
        {
            _storage[id] = entity;
        }

  
        void IWriteRepository<TEntity, TKey>.Add(TEntity entity)
        {

            var idProperty = typeof(TEntity).GetProperty("Id");
            if (idProperty != null && idProperty.PropertyType == typeof(TKey))
            {
                TKey id = (TKey)idProperty.GetValue(entity);
                Add(id, entity);
            }
            else
            {

                System.Console.WriteLine($"Warning: Could not add entity. {typeof(TEntity).Name} does not have a public 'Id' property of type {typeof(TKey).Name}.");
            }
        }

        public TEntity Get(TKey id)
        {
            _storage.TryGetValue(id, out var entity);
            return entity;
        }

        public IEnumerable<TEntity> GetAll()
        {
            return _storage.Values;
        }

        public void Remove(TKey id)
        {
            _storage.Remove(id);
        }
    }
}
