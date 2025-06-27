using System.Collections.Generic;
using System.Linq;
using generics.Interfaces;
using generics.Models; // Потрібно для доступу до Person

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

        // Явна реалізація методу Add для IWriteRepository
        void IWriteRepository<TEntity, TKey>.Add(TEntity entity)
        {
            // Це припущення, що TEntity має властивість 'Id', яка є типом TKey.
            // В реальному проекті для цього використовувався б додатковий інтерфейс (наприклад, IHasId<TKey>)
            // або рефлексія, але для лабораторної роботи це є прийнятним спрощенням,
            // оскільки наші моделі (Person, Student, Group) мають public int Id, і TKey використовується як int.
            var idProperty = typeof(TEntity).GetProperty("Id");
            if (idProperty != null && idProperty.PropertyType == typeof(TKey))
            {
                TKey id = (TKey)idProperty.GetValue(entity);
                Add(id, entity);
            }
            else
            {
                // Якщо TEntity не має властивості Id або вона не відповідає TKey,
                // ми не можемо додати її таким чином.
                // Можна кинути виняток або просто вивести попередження.
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
