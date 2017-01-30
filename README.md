#   **Loki**
------------------------------

![alt tag](https://raw.githubusercontent.com/GokGokalp/Loki/master/misc/logo.png)  

Loki provides an easy way to handle locking scenarios on distributed systems.

####Features:
- Support **Redis** locking handler for primary
- Support **MSSQL** locking handler for secondary
- Multiple locking handlers can be added such as **MongoDB** etc
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

Then just use **Locking.Instance.ExecuteWithinLock()** method where you want to provide concurrency.

```csharp
Locking.Instance.ExecuteWithinLock(() =>
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

If you want to use MSSQL locking handler for secondary, you need to create LokiLockings table as below:

```sql
CREATE TABLE [dbo].[LokiLockings](
	[TenantType] [varchar](50) NOT NULL,
	[CreationDate] [datetime] NOT NULL,
 CONSTRAINT [PK_LokiLockings] PRIMARY KEY CLUSTERED 
(
	[TenantType] ASC
))
```

then:

```csharp
LokiConfigurationBuilder.Instance.SetTenantType("SimpleTestClient")
						.SetPrimaryLockHandler(new RedisLokiLockHandler(redisEndPoints.ToArray()))
						.SetSecondaryLockHandler(new MSSQLLokiLockHandler("connectionString"))
						.Build();
```