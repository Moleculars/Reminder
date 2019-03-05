using Bb.Reminder;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Bb.ReminderStore.Sgbd
{


    public class ReminderStoreSqlServer : ReminderStoreSql
    {

        public ReminderStoreSqlServer(string connectionString, string providerInvariantName, int wakeUpIntervalSeconds = 20) 
            : base (connectionString, providerInvariantName, wakeUpIntervalSeconds)
        {
            SqlInsert = "INSERT INTO [dbo].[Reminders] ([Uuid], [Binding], [Address], [Message], [Expire], [Resolved]) VALUES (@uuid, @binding, @address, @message, @expire, 0)";
            SqlQuery = "SELECT [_id], [Uuid], [binding], [Address], [Message] FROM [dbo].Reminders r (UPDLOCK) WHERE r.Expire < CURRENT_TIMESTAMP AND r.Resolved = 0";
            SqlRemove = "UPDATE [dbo].Reminders SET Resolved = 1 WHERE Uuid = @uuid";
        }

    }


    public abstract class ReminderStoreSql : IReminderStore, IDisposable
    {

        protected ReminderStoreSql(string connectionString, string providerInvariantName, int wakeUpIntervalSeconds = 20)
        {

            if (!System.Data.Common.DbProviderFactories.TryGetFactory(providerInvariantName, out DbProviderFactory factory))
                throw new InvalidOperationException(providerInvariantName);

            _factory = factory;

            _connectionString = connectionString;
            _timer = new Timer(WakeUpAsync, null, wakeUpIntervalSeconds * 1000, wakeUpIntervalSeconds * 1000);

        }


        public void Watch(WakeUpRequestModel model)
        {

            using (var cnx = GetConnection())
            using (var cmd = GetCommand(cnx, SqlInsert))
            {

                cmd.Parameters.Add(GetParameter(nameof(model.Uuid), model.Uuid, System.Data.DbType.Guid));
                cmd.Parameters.Add(GetParameter(nameof(model.Binding), model.Binding, System.Data.DbType.String));
                cmd.Parameters.Add(GetParameter(nameof(model.Address), model.Address, System.Data.DbType.String));
                cmd.Parameters.Add(GetParameter(nameof(model.Message), model.Message, System.Data.DbType.String));
                cmd.Parameters.Add(GetParameter("Expire", DateTimeOffset.Now.AddSeconds(model.DelayInMinute), System.Data.DbType.DateTimeOffset));

                int reader = cmd.ExecuteNonQuery();

            }


        }

        public void Cancel(Guid uuid)
        {
            using (var cnx = GetConnection())
            {
                using (var cmd = GetCommand(cnx, SqlRemove))
                {
                    cmd.Parameters.Add(GetParameter(nameof(uuid), uuid, System.Data.DbType.Guid));
                    int reader = cmd.ExecuteNonQuery();
                }
            }

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
            using (var cnx = GetConnection())
            using (this._trans = cnx.BeginTransaction())
            {

                List<Model> _ids = CollectIds(cnx);

                foreach (var item in _ids.AsParallel())
                {

                    try
                    {

                        WakeUp(item);

                        using (var cmd = GetCommand(cnx, SqlRemove))
                        {
                            cmd.Parameters.Add(GetParameter("uuid", item.Uuid, System.Data.DbType.Guid));
                            int reader = cmd.ExecuteNonQuery();
                        }

                    }
                    catch (Exception e)
                    {
                        Trace.WriteLine(new { e.Message, Exception = e, item.Uuid }, TraceLevel.Error.ToString());
                    }

                }

                this._trans.Commit();

            }
        }

        private List<Model> CollectIds(DbConnection cnx)
        {

            List<Model> _ids = new List<Model>();

            using (var cmd = GetCommand(cnx, SqlQuery))
            using (var reader = cmd.ExecuteReader())
                while (reader.Read())
                    _ids.Add(new Model()
                    {
                        Id = reader.GetInt64(0),
                        Uuid = reader.GetGuid(1),
                        Binding = reader.GetString(2),
                        Address = reader.GetString(3),
                        Message = reader.GetString(4),
                    });

            return _ids;

        }

        private class Model : WakeUpRequestModel
        {

            public long Id { get; set; }

        }

        private DbConnection GetConnection()
        {
            var cnx = _factory.CreateConnection();
            cnx.ConnectionString = _connectionString;
            cnx.Open();
            return cnx;
        }

        private DbCommand GetCommand(DbConnection cnx, string sql)
        {


            var cmd = _factory.CreateCommand();
            cmd.Connection = cnx;
            cmd.CommandText = sql;

            if (this._trans != null)
                cmd.Transaction = this._trans;

            return cmd;
        }

        private DbParameter GetParameter(string name, object value, System.Data.DbType type)
        {
            DbParameter parameter = _factory.CreateParameter();
            parameter.ParameterName = "@" + name;
            parameter.Value = value;
            parameter.DbType = type;
            return parameter;
        }

        public Action<WakeUpRequestModel> WakeUp { get; set; }

        public string SqlQuery { get; protected set; }

        public string SqlInsert { get; protected set; }

        public string SqlRemove { get; protected set; }

        private readonly DbProviderFactory _factory;
        private readonly string _connectionString;
        private readonly Timer _timer;
        private readonly object _lock = new object();
        private bool _execute;
        private DbTransaction _trans;

        #region IDisposable Support
        private bool disposedValue = false; // Pour détecter les appels redondants

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
}
