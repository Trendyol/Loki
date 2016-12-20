#   **Loki**
------------------------------

![alt tag](https://raw.githubusercontent.com/GokGokalp/Loki/master/misc/logo.png)  

Loki provides an easy way to handle locking scenarios on distributed systems.

####Features:
- Currently support **Redis** locking handler
- Multiple locking handlers can be added such as **Redis**, **MongoDB**, **MsSQL**
- Secondary locking handler can be set for against connection failure problems

####Usage:
Firstly you have to easily initialize the Loki with **LokiConfigurationBuilder**.

```csharp
List<EndPoint> redisEndPoints = new List<EndPoint>
{
	new DnsEndPoint("redisUri", redisPort)
};

LokiConfigurationBuilder.Instance.SetTenantType("SimpleTestClient")
						.SetPrimaryLockHandler(new RedisLokiLockHandler(redisEndPoints.ToArray()))
						.Build();
```

Then just use **Loki.ExecuteWithinLock()** method where you want to provide concurrency.

```csharp
Loki.ExecuteWithinLock(() =>
{
	//do somethings..
},  expiryFromSeconds: 2);
```

Also you can easily implement custom locking handlers.

```csharp
public class FooLockHandler : LokiLockHandler
{
    public override bool Lock(string tenantType, int expiryFromSeconds)
    {
        //Lock operations
    }

    public override void Release(string tenantType)
    {
        //Release operations
    }
}
```