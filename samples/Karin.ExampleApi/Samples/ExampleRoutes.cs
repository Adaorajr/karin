using Karin.Http;

namespace Karin.Samples;

public static class ExampleRoutes
{
    public static void Register(KarinApp app)
    {
        app.MapGet("/", _ => new HttpResponse("HTTP/1.1 200 OK", "Hello!"));

        app.MapPost<EchoRequest>("/echo", request => request.Message is null
            ? new HttpResponse("HTTP/1.1 400 Bad Request", "The message field is required.")
            : new HttpResponse("HTTP/1.1 200 OK", request.Message));
    }
}
