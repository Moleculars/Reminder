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
}
