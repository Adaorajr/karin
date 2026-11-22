# Karin

A small HTTP/1.1 server written in C# to learn how web servers work. It supports basic routing and JSON request bodies. Not intended for production use.

## Requirements

.NET 10

## Usage

```csharp
using System.Net;
using Karin;
using Karin.Http;

var app = new KarinApp();

app.MapGet("/", _ => new HttpResponse("HTTP/1.1 200 OK", "Hello, world!"));

await app.RunAsync(IPAddress.Loopback, 13000);
```

Open `http://127.0.0.1:13000`.

## Example API

The included example also shows a POST route with a JSON request body:

```bash
dotnet run --project samples/Karin.ExampleApi
```
