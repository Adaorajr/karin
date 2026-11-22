using System.Net;
using Karin.Http;
using Karin.Samples;

if (args.Length == 0)
{
    Console.WriteLine("Usage: dotnet run -- server");
    return;
}

if (args[0] == "server")
{
    var server = new HttpServer(ExampleRoutes.Handle);
    await server.ListenAsync(IPAddress.Loopback, 13000);
}
else
{
    Console.WriteLine("Usage: dotnet run -- server");
}
