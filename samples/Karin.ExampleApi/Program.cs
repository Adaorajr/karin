using System.Net;
using Karin;
using Karin.Samples;

var app = new KarinApp();
ExampleRoutes.Register(app);
await app.RunAsync(IPAddress.Loopback, 13000);
