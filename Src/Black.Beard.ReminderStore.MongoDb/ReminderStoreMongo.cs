﻿using Bb.Reminder;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;


namespace Bb.ReminderStore.MongoDb
{


    public class ReminderStoreMongo : IReminderStore, IInitializer, IDisposable
    {

        public ReminderStoreMongo(string connectionString, string databaseName, string collectionName, int wakeUpIntervalSeconds = 20)
        {
            _connectionString = connectionString;
            _databaseName = databaseName;
            _collectionName = collectionName;

            _timer = new Timer(WakeUpAsync, null, wakeUpIntervalSeconds * 1000, wakeUpIntervalSeconds * 1000);
        }

        public void Initialize(object o)
        {

            _client = new MongoClient(_connectionString);
            _database = _client.GetDatabase(_databaseName);
            _wathItemCollection = _database.GetCollection<WatchItem>(_collectionName);

            var indexKeys = Builders<WatchItem>.IndexKeys.Ascending(_ => _.Expire);
            var indexOptions = new CreateIndexOptions();

            _wathItemCollection.Indexes.CreateOne(indexKeys, indexOptions);

            _filterLeft = Builders<WatchItem>.Filter
                .Eq("resolved", false)
                ;

        }

        public void Watch(WakeUpRequestModel model)
        {

            var watchItem = new WatchItem()
            {
                Uuid = model.Uuid,
                Address = model.Address,
                Binding = model.Binding,
                Message = model.Binding,
                Resolved = false,
                Expire = DateTimeOffset.Now.AddSeconds(model.DelayInMinute),
            };

            _wathItemCollection.InsertOne(watchItem);

        }

        public void Cancel(Guid uuid)
        {

        }

        private void WakeUpAsync(object state)
        {
            if (!_execute)
                lock (_lock)
                    if (!_execute)
                        try
                        {
                            _execute = true;
                            WakeUp_Impl();
                        }
                        finally
                        {
                            _execute = false;
                        }
        }

        private void WakeUp_Impl()
        {

            List<WatchItem> _ids = CollectIds();

            foreach (var item in _ids.AsParallel())
            {

                var filterRight = Builders<WatchItem>.Filter.Eq("_id", item.Id);
                var filter = Builders<WatchItem>.Filter.And(_filterLeft, filterRight);
                var update = Builders<WatchItem>.Update.Set("resolved", true);

                var result = _wathItemCollection.UpdateOne(filter, update);

                if (result.ModifiedCount != 0)
                {
                    try
                    {

                        WakeUp(new WakeUpRequestModel()
                        {
                            Address = item.Address,
                            Binding = item.Binding,
                            Uuid = item.Uuid,
                            Message = item.Message,
                            Headers = item.Headers,
                        });

                    }
                    catch (Exception)
                    {
                        update = Builders<WatchItem>.Update.Set("resolved", false);
                        result = _wathItemCollection.UpdateOne(filterRight, update);
                        throw;
                    }

                }




            }

        }

        public Action<WakeUpRequestModel> WakeUp { get; set; }

        private List<WatchItem> CollectIds()
        {

            var filter = Builders<WatchItem>.Filter.And
            (
                _filterLeft,
                Builders<WatchItem>.Filter.AnyGte("expire", DateTimeOffset.Now)
            );

            var result = _wathItemCollection.Find(filter).ToList();

            return result;


        }

        private readonly string _connectionString;
        private readonly string _databaseName;
        private readonly string _collectionName;
        private readonly Timer _timer;
        private readonly object _lock = new object();
        private bool _execute;

        #region IDisposable Support

        private bool disposedValue = false; // Pour détecter les appels redondants
        private MongoClient _client;
        private IMongoDatabase _database;
        private IMongoCollection<WatchItem> _wathItemCollection;
        private FilterDefinition<WatchItem> _filterLeft;

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: supprimer l'état managé (objets managés).
                }

                // TODO: libérer les ressources non managées (objets non managés) et remplacer un finaliseur ci-dessous.
                // TODO: définir les champs de grande taille avec la valeur Null.

                disposedValue = true;
            }
        }

        // TODO: remplacer un finaliseur seulement si la fonction Dispose(bool disposing) ci-dessus a du code pour libérer les ressources non managées.
        // ~ReminderStoreSql() {
        //   // Ne modifiez pas ce code. Placez le code de nettoyage dans Dispose(bool disposing) ci-dessus.
        //   Dispose(false);
        // }

        // Ce code est ajouté pour implémenter correctement le modèle supprimable.
        public void Dispose()
        {
            // Ne modifiez pas ce code. Placez le code de nettoyage dans Dispose(bool disposing) ci-dessus.
            Dispose(true);
            // TODO: supprimer les marques de commentaire pour la ligne suivante si le finaliseur est remplacé ci-dessus.
            // GC.SuppressFinalize(this);
        }

        #endregion

    }


    /// <summary>
    /// Class used in business logic to represent user.
    /// </summary>
    public class WatchItem
    {

        [BsonId]
        public ObjectId Id { get; set; }

        [BsonElement("uuid")]
        public Guid Uuid { get; set; }

        [BsonElement("binding")]
        public string Binding { get; set; }

        [BsonElement("address")]
        public string Address { get; set; }

        [BsonElement("message")]
        public string Message { get; set; }

        [BsonElement("resolved")]
        public bool Resolved { get; set; }

        [BsonElement("expire")]
        public DateTimeOffset Expire { get; set; }

        [BsonElement("headers")]
        public string Headers { get; set; }

    }

}
