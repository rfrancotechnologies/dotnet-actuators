# dotnet-actuators

Dotnet actuators includes a number of additional features to help you monitor and manage your application when it’s pushed to production using HTTP endpoints.
Auditing, health, configuration and metrics gathering can be automatically applied to your application.

## Enabling actuators

The simplest way to enable the features is to add the next snippet of code in your application:

```csharp
    //  Create the Http Server defining its port (require elevated permissions in Windows platform. See below)
    IHttpActuatorServer actuatorServer = new HttpActuatorServer(port: 8080);

    //  Define actuators
    ...
    //  Run  the server
    actuatorServer.Start();
```

Note: In windows platform, remember to open the ports with elevated permissions. For more information check the next link https://docs.microsoft.com/en-us/dotnet/framework/wcf/feature-details/configuring-http-and-https?redirectedfrom=MSDN

### Actuator Configuration

Provide a way to retrieve the whole configuration of the component.

```csharp
    //  Add "/config" endpoint to retrieve the configuration of the component
    actuatorServer.Add(new ConfigActuator(configuration));

```

### Actuator Health / Readiness / Liveness probes

Provide a way to retrieve the status of the component.

```csharp
    //  Add "/health" endpoint to retrieve the health status of the component. 
    // In this example, monitoring the exceptions exceptions that have happened
    HealthActuator health = new HealthActuator();
    health.AddChecker(new ApplicationHealthChecker());
    actuatorServer.Add(health);

    //To detect the exceptions that have happened, it requieres to publish the exceptions
    try{

    }catch(Exception e)
    {
        ApplicationExceptionMetrics.Add(e);
    }

```

For readiness / liveness probes, to check the status of external components as Databases, streaming platforms, etc.

```csharp
    //  Add multiple health checker to retrieve the readiness of the component. In this example, Apache Kafka and SQL Server
    HealthActuator readiness = new HealthActuator(null, "readiness"); //    readiness is the endpoint http(s)://path/readiness
    readiness.AddChecker(new KafkaHealthChecker(new AdminClientConfig { BootstrapServers = "<<bootstraperServer>>" }));
    readiness.AddChecker(new SqlServerHealthCheck("<<connectionString>>"));
    actuatorServer.Add(readiness);

```

## Features

TODO
