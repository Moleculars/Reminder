# Reminder [![Build status](https://ci.appveyor.com/api/projects/status/ihs9q79w4oreeg6p?svg=true)](https://ci.appveyor.com/project/gaelgael5/reminder)

Implement a reminder

Installation by nuget package <a href="https://github.com/Moleculars/Reminder">Here</a>
``` 
    Install-Package Black.Beard.Reminder
```

Installation of implementation of broker on rabbitMQ <a href="https://github.com/Black-Beard-Sdk/Brokers">here</a>
``` 
    Install-Package Black.Beard.RabbitMq
```


A Script of creation of the table in Sqlserver. <a href="https://github.com/Moleculars/Reminder/blob/master/Src/Black.Beard.ReminderStore.Sgbd/Table_Reminder.Sqlserver.sql">here</a>

```CSharp

    // Initialisation of DbFactory (it is specfic for type 'ReminderStoreSqlServer')
    string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Reminder;Integrated Security=True";
    DbProviderFactories.RegisterFactory(_providerInvariantName, System.Data.SqlClient.SqlClientFactory.Instance);
    // creation of store that working on Sqlserver.
    var store = new ReminderStoreSqlServer(connectionString, _providerInvariantName, 5);
    

    IBroker broker = null; // use an instance of broker    
    // create a response that push in a broker
    Bb.ReminderResponse.Broker serviceActionBroker = new Bb.ReminderResponse.Broker(broker, "nameOfThePublisher")


    var reminder = new ReminderService(store, serviceActionBroker);


    var model = new WakeUpRequestModel()
    {
        Uuid = Guid.NewGuid(),
        Binding = "broker",
        Address = "publisherName",
        CurrentDateCaller = DateTimeOffset.Now,
        DelayInMinute = 20,
        Message = Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes("Test")),
    };

    reminder.Watch(model);

```


