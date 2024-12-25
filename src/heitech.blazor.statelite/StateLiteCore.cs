using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using LiteDB;

namespace heitech.blazor.statelite
{
    /// <summary>
    /// <inheritdoc cref="IStateLite"/>
    /// </summary>
    internal sealed class StateLiteCore : IStateLite
    {
        private MemoryStream _dbStream;
        private LiteDatabase _database;

        private HashSet<Type> _collections = new HashSet<Type>();
        
        public StateLiteCore()
        {
            _dbStream = new MemoryStream();
            _database = new LiteDatabase(_dbStream);
        }

        private string CollectionName<T>() => typeof(T).Name;
        public MemoryStream DatabaseStream => _dbStream;

        public IEnumerable<T> GetAll<T>()
            where T : IHasId
        {
            var collection = _database.GetCollection<T>(CollectionName<T>());
            return collection.FindAll();
        }

        public void Dump(Action<string> writerCallback)
        {
            foreach (var type in _collections)
            {
                var allFromType = _database.GetCollection(type.Name).FindAll();
                var asOneString = string.Join(Environment.NewLine, allFromType);
                writerCallback(asOneString);
            }
        }

        public void Insert<T>(T record)
            where T : IHasId
        {
            _collections.Add(typeof(T));
            var collection = _database.GetCollection<T>(CollectionName<T>());
            collection.Insert(record);
        }

        public void Delete<T>(T record) where T : IHasId
        {
            var collection = _database.GetCollection<T>(CollectionName<T>());
            collection.Delete(record.Id);
        }

        public void Update<T>(T record)
            where T : IHasId
        {
            var collection = _database.GetCollection<T>(CollectionName<T>());
            var id = collection.FindOne(x => x.Id == record.Id);
            if (id == null)
            {
                return;
            }
            collection.Delete(id.Id);
            collection.Insert(record);
        }

        public T GetById<T>(Guid id)
            where T : IHasId
        {
            var result = Query<T>(x => x.Id == id);
            return result.Any() ? result.First() : default;
        }

        public IEnumerable<T> Query<T>(Func<T, bool> filter)
            where T : IHasId
        {
            var c = _database.GetCollection<T>(CollectionName<T>());
            return c.FindAll().Where(filter);
        }

        public void Purge()
        {
            _dbStream = new MemoryStream();
            _database = new LiteDatabase(_dbStream);
        }

        public void Dispose()
        {
            _dbStream.Dispose();
            _database?.Dispose();
        }
    }
}