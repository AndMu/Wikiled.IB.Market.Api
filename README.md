# Interactive Brockers API C#

9.74

Ported from official IB library and updated to support RX TPL and AutoFac


Nuget library

![nuget](https://img.shields.io/nuget/v/Wikiled.IB.Market.Api.svg)

```
Install-Package Wikiled.IB.Market.Api
```

## Configure AutoFac

## Receive Historical Data

```
using (var client = new IBClientWrapper(factory))
{
	client.Connect("127.0.0.1", 7496, 1);
	IObservable<HistoricalDataMessage> amd = client.GetManager<HistoricalDataManager>()
					.Request(
						new MarketDataRequest(
							GetMDContract("VXX"),
							new DateTime(2016, 01, 01).ToUtc(client.TimeZone),
							new Duration(5, DurationType.Years),
							BarSize.Day,
							WhatToShow.ASK));
	HistoricalDataMessage[] data = await amd.ToArray();                
}
```
