using System.Net;
using System.Text.Json;
using Karin.Http;

namespace Karin;

public sealed class KarinApp
{
    private static readonly JsonSerializerOptions JsonOptions = new(JsonSerializerDefaults.Web);
    private readonly Dictionary<(string Method, string Path), Func<HttpRequest, HttpResponse>> _routes = [];

    public void MapGet(string path, Func<HttpRequest, HttpResponse> handler)
    {
        Map("GET", path, handler);
    }

    public void MapPost(string path, Func<HttpRequest, HttpResponse> handler)
    {
        Map("POST", path, handler);
    }

    public void MapPost<TRequest>(string path, Func<TRequest, HttpResponse> handler)
    {
        MapJson("POST", path, handler);
    }

    public void MapPut(string path, Func<HttpRequest, HttpResponse> handler)
    {
        Map("PUT", path, handler);
    }

    public void MapPut<TRequest>(string path, Func<TRequest, HttpResponse> handler)
    {
        MapJson("PUT", path, handler);
    }

    public void MapDelete(string path, Func<HttpRequest, HttpResponse> handler)
    {
        Map("DELETE", path, handler);
    }

    public Task RunAsync(IPAddress address, int port)
    {
        var server = new HttpServer(HandleRequest);
        return server.ListenAsync(address, port);
    }

    private void Map(string method, string path, Func<HttpRequest, HttpResponse> handler)
    {
        _routes[(method, path)] = handler;
    }

    private void MapJson<TRequest>(string method, string path, Func<TRequest, HttpResponse> handler)
    {
        Map(method, path, request =>
        {
            TRequest? body;
            try
            {
                body = JsonSerializer.Deserialize<TRequest>(request.Body, JsonOptions);
            }
            catch (JsonException)
            {
                return new HttpResponse("HTTP/1.1 400 Bad Request", "Invalid JSON.");
            }

            return body is null
                ? new HttpResponse("HTTP/1.1 400 Bad Request", "Invalid JSON.")
                : handler(body);
        });
    }

    private HttpResponse HandleRequest(HttpRequest request)
    {
        return _routes.TryGetValue((request.Method, request.Path), out var handler)
            ? handler(request)
            : new HttpResponse("HTTP/1.1 404 Not Found", "Not found.");
    }
}
