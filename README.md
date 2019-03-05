# Reminder [![Build status](https://ci.appveyor.com/api/projects/status/ihs9q79w4oreeg6p?svg=true)](https://ci.appveyor.com/project/gaelgael5/reminder)

Implement a reminder


Install a implementation of broker on rabbitMQ
https://github.com/Black-Beard-Sdk/Brokers
``` 
    Install-Package Black.Beard.RabbitMq -Version 1.0.2
```

Script of creation of the table in Sqlserver.
https://github.com/Moleculars/Reminder/blob/master/Src/Black.Beard.ReminderStore.Sgbd/Table_Reminder.Sqlserver.sql

```CSharp

    // Initialisation of DbFactory
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


