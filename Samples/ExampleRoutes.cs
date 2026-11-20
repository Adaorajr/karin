using System.Text.Json;
using Karin.Http;

namespace Karin.Samples;

public static class ExampleRoutes
{
    public static HttpResponse Handle(HttpRequest request)
    {
        if (request.Method == "GET" && request.Path == "/")
        {
            return new HttpResponse("HTTP/1.1 200 OK", "Olá!");
        }

        if (request.Method == "GET" && request.Path == "/hello")
        {
            string? name = null;

            foreach (string parameter in request.QueryString.Split('&'))
            {
                int equalsIndex = parameter.IndexOf('=');
                if (equalsIndex > 0 && parameter[..equalsIndex] == "name")
                {
                    name = parameter[(equalsIndex + 1)..];
                    break;
                }
            }

            return string.IsNullOrEmpty(name)
                ? new HttpResponse("HTTP/1.1 400 Bad Request", "The name parameter is required.")
                : new HttpResponse("HTTP/1.1 200 OK", $"Olá, {name}!");
        }

        if (request.Method == "POST" && request.Path == "/echo")
        {
            return new HttpResponse("HTTP/1.1 200 OK", request.Body);
        }

        if (request.Method == "POST" && request.Path == "/sum")
        {
            const string jsonContentType = "application/json; charset=utf-8";

            try
            {
                var jsonOptions = new JsonSerializerOptions(JsonSerializerDefaults.Web);
                SumRequest numbers = JsonSerializer.Deserialize<SumRequest>(request.Body, jsonOptions)
                    ?? throw new JsonException("The JSON body is empty.");

                string body = JsonSerializer.Serialize(new { result = numbers.A + numbers.B });
                return new HttpResponse("HTTP/1.1 200 OK", body, jsonContentType);
            }
            catch (JsonException)
            {
                string body = JsonSerializer.Serialize(new { error = "Invalid JSON." });
                return new HttpResponse("HTTP/1.1 400 Bad Request", body, jsonContentType);
            }
        }

        return new HttpResponse("HTTP/1.1 404 Not Found", "Not found.");
    }
}
