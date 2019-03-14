using Bb.Reminder;
using MongoDB.Bson;
using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;


namespace Bb.ReminderStore.MongoDb
{

    /// <summary>
    /// store index on mongo
    /// </summary>
    /// <seealso cref="Bb.Reminder.IReminderStore" />
    /// <seealso cref="Bb.Reminder.IInitializer" />
    /// <seealso cref="System.IDisposable" />
    public class ReminderStoreMongo : IReminderStore, IInitializer, IDisposable
    {

        /// <summary>
        /// Initializes a new instance of the <see cref="ReminderStoreMongo"/> class.
        /// </summary>
        /// <param name="connectionString">The connection string.</param>
        /// <param name="databaseName">Name of the database.</param>
        /// <param name="collectionName">Name of the collection.</param>
        /// <param name="wakeUpIntervalSeconds">The wake up interval seconds.</param>
        public ReminderStoreMongo(string connectionString, string databaseName, string collectionName, int wakeUpIntervalSeconds = 20)
        {
            _connectionString = connectionString;
            _databaseName = databaseName;
            _collectionName = collectionName;

            _timer = new Timer(WakeUpAsync, null, wakeUpIntervalSeconds * 1000, wakeUpIntervalSeconds * 1000);
        }

        /// <summary>
        /// Initializes current object
        /// </summary>
        /// <param name="o">The o.</param>
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

        /// <summary>
        /// Add in index and watches the specified model.
        /// </summary>
        /// <param name="model">The model.</param>
        public void Watch(WakeUpRequestModel model)
        {

            var watchItem = new WatchItem()
            {
                Uuid = model.Uuid,
                Address = model.Address,
                Binding = model.Binding,
                Message = model.Binding,
                Resolved = false,
                Expire = DateTimeOffset.Now.AddMinutes(model.DelayInMinute),
            };

            _wathItemCollection.InsertOne(watchItem);

        }

        /// <summary>
        /// Cancels the specified UUID.
        /// </summary>
        /// <param name="uuid">The UUID.</param>
        public void Cancel(Guid uuid)
        {
            var filter = Builders<WatchItem>.Filter.Eq("uuid", uuid);
            var update = Builders<WatchItem>.Update.Set("resolved", true);
            var result = _wathItemCollection.UpdateOne(filter, update);
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

        /// <summary>
        /// Gets or sets the wake up method.
        /// </summary>
        /// <value>
        /// The wake up.
        /// </value>
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


        #region IDisposable Support

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// </summary>
        /// <param name="disposing"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
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
        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            // Ne modifiez pas ce code. Placez le code de nettoyage dans Dispose(bool disposing) ci-dessus.
            Dispose(true);
            // TODO: supprimer les marques de commentaire pour la ligne suivante si le finaliseur est remplacé ci-dessus.
            // GC.SuppressFinalize(this);
        }

        
        private bool disposedValue = false; // Pour détecter les appels redondants
        private MongoClient _client;
        private IMongoDatabase _database;
        private IMongoCollection<WatchItem> _wathItemCollection;
        private FilterDefinition<WatchItem> _filterLeft;

        #endregion

        private readonly string _connectionString;
        private readonly string _databaseName;
        private readonly string _collectionName;
        private readonly Timer _timer;
        private readonly object _lock = new object();
        private bool _execute;

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
