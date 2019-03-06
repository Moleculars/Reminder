using Bb.Reminder;
using Bb.ReminderStore.MongoDb;
using Bb.ReminderStore.Sgbd;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Threading;

namespace Black.Beard.Reminder.UnitTests
{
    [TestClass]
    public class UnitTest1
    {

        public UnitTest1()
        {
            UnitTest1._providerInvariantName = "sqlClient";
        }


        [TestMethod]
        public void WakeupUnitTest()
        {

            ReminderStoreSqlServer store = CreateSqlReminder();

            var model = new WakeUpRequestModel()
            {
                Uuid = Guid.NewGuid(),
                Binding = "http.post",
                Address = "http://localhost",
                DelayInMinute = 20,
                Message = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("Test")),
            };

            store.Watch(model);

            bool ok = false;
            store.WakeUp = m =>
            {
                if (m.Uuid == model.Uuid)
                    ok = true;
            };

            while (!ok)
                Thread.Yield();

        }

        [TestMethod]
        public void ReminderServiceUnitTest()
        {

            ReminderStoreSqlServer store = CreateSqlReminder();
            var r = new ReminderTest();

            var reminder = new ReminderService(store, r);

            var model = new WakeUpRequestModel()
            {
                Uuid = Guid.NewGuid(),
                Binding = "test",
                Address = "http://localhost",
                DelayInMinute = 20,
            }.SetMessage("test");

            reminder.Watch(model);

            while (!r.Evaluate(model))
                Thread.Yield();

            var t = DateTime.Now.AddSeconds(20);
            while (t < DateTime.Now)
                Thread.Yield();

        }

        private static ReminderStoreSqlServer CreateSqlReminder()
        {
            string connectionString = @"Data Source=L00280\SQLEXPRESS;Initial Catalog=Reminder;Integrated Security=True";
            DbProviderFactories.RegisterFactory(_providerInvariantName, System.Data.SqlClient.SqlClientFactory.Instance);
            var store = new ReminderStoreSqlServer(connectionString, _providerInvariantName, 5);
            return store;
        }

        private static ReminderStoreMongo CreateMongoDb()
        {
            string connectionString = @"Data Source=L00280\SQLEXPRESS;Initial Catalog=Reminder;Integrated Security=True";
            var store = new ReminderStoreMongo(connectionString, "databaseName", "collectionName", 5);
            return store;
        }

        private static string _providerInvariantName;

    }


    public class ReminderTest : IReminderResponseService
    {
        private Guid _uuid;

        public ReminderTest()
        {

        }

        public string MethodName => "test";

        public void Dispose()
        {
            throw new NotImplementedException();
        }

        public void Push(Guid uuid, string address, string message, Dictionary<string, object> headers)
        {
            _uuid = uuid;
        }

        internal bool Evaluate(WakeUpRequestModel model)
        {
            return _uuid == model.Uuid;
        }

    }
}
