# Reminder [![Build status](https://ci.appveyor.com/api/projects/status/ihs9q79w4oreeg6p?svg=true)](https://ci.appveyor.com/project/gaelgael5/reminder)

Implementation of a reminder.

Principle is very simple and can be resume as such. "Call me at time and said me something."

``` CSharp
    Service.Watch(message);
```

## Model properties

1. Uuid : Is a unique key given by caller and can identify the request
2. DelayInMinute : Specify the delay before you want the reminder callback your service 
3. Binding : Is the way you want the reminder callback you, the key is the name of serviceResponse ("broker" for use callback by a broker, "http.post" for a callback e web service)
4. Address : Address of the service callback
5. Message : the message storaged in base 64 encoding

``` CSharp
    var model = new WakeUpRequestModel()
    {
        Uuid = Guid.NewGuid(),
        Binding = "broker",
        Address = "publisherName",
        DelayInMinute = 20,   
    }.SetMessage("test");   // remarque the method extension for encoding in base 64

```



Installation of 'reminder' is by nuget package <a href="https://github.com/Moleculars/Reminder">Here</a>
``` 
    Install-Package Black.Beard.Reminder
```

The reminder service need two arguments to be created
1. A store for storage requests pending the restitution of the response
2. A list of Response service for implement a restitution way ex : (by a broker, http request, ...)


At time that I write this documentation, I have two implémentation of the storage

1. An implementation in Sqlserver
``` 
    Install-Package Black.Beard.ReminderStore.Sgbd
```

2. An implementation in mongodb
``` 
    Install-Package Black.Beard.ReminderStore.MongoDb
```
A Script of creation of the table in Sqlserver. <a href="https://github.com/Moleculars/Reminder/blob/master/Src/Black.Beard.ReminderStore.Sgbd/Table_Reminder.Sqlserver.sql">here</a>

And an implementation of Response service for calling a broker
```
    Install-Package Black.Beard.ReminderResponse.Broker
```

Note the implementation doen't contains implementation of middleware message and use only interface contract of IFactoryBroker that you can find in Black.Beard.Brokers.

For test you can use an implementation of the broker on rabbitMQ <a href="https://github.com/Black-Beard-Sdk/Brokers">here</a>
``` 
    Install-Package Black.Beard.RabbitMq
```
Please report to documentation of the implementation of the broker for configuration


##  Sample
```CSharp

    // Alternatively, you can use a store on Sql server on on mongodb

    // Initialisation of DbFactory (it is specfic for type 'ReminderStoreSqlServer')
    // Install-Package Black.Beard.Reminder
    string connectionString = @"Data Source=.\SQLEXPRESS;Initial Catalog=Reminder;Integrated Security=True";
    DbProviderFactories.RegisterFactory(_providerInvariantName, System.Data.SqlClient.SqlClientFactory.Instance);
    // creation of store that working on Sqlserver.
    var store = new ReminderStoreSqlServer(connectionString, _providerInvariantName, 5);
    
    // create a store on mongo : Install-Package Black.Beard.Reminder
    // var store = new ReminderStoreMongo(connectionString, "databaseName", "collectionName", 5);

    IBroker broker = null; // use an instance of broker    
    // create a response that push in a broker
    Bb.ReminderResponse.Broker serviceActionBroker = new Bb.ReminderResponse.Broker(broker, "nameOfThePublisher")


    var reminder = new ReminderService(store, serviceActionBroker);

    // And small test
    var model = new WakeUpRequestModel()
    {
        Uuid = Guid.NewGuid(),
        Binding = "broker",
        Address = "publisherName",
        DelayInMinute = 20,   
    }.SetMessage("test");

    reminder.Watch(model);

```

## Sample implementation of Response service
```CSharp

using Bb.Brokers;
using Bb.Reminder;
using System;
using System.Collections.Generic;

namespace Bb.ReminderResponse.Broker
{

    public class ReminderResponseMyImplementation : IReminderResponseService, IDisposable
    {

        public ReminderResponseMyImplementation()
        {

        }

        public void Push(Guid uuid, string address, string message, Dictionary<string, object> headers)
        {
            // Put here implementation of your specific implementation response
        }

        public string MethodName => "name that you want specify in the binding property model request";


        #region IDisposable Support

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {

                }
                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(true);
        }

        #endregion

        private bool disposedValue = false;

    }

}

```

```CSahrp

    var myService = new ReminderResponseMyImplementation();
    var reminder = new ReminderService(store, myService);

```

