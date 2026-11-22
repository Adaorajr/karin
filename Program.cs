using System.Net;
using Karin;
using Karin.Http;

var app = new KarinApp();

app.MapGet("/", _ => new HttpResponse("HTTP/1.1 200 OK", "Olá!"));

app.MapPost<EchoRequest>("/echo", request => request.Message is null
    ? new HttpResponse("HTTP/1.1 400 Bad Request", "The message field is required.")
    : new HttpResponse("HTTP/1.1 200 OK", request.Message));

await app.RunAsync(IPAddress.Loopback, 13000);
