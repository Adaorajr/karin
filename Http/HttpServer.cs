using System.Net;
using System.Net.Sockets;

namespace Karin.Http;

public sealed class HttpServer
{
    private readonly Func<HttpRequest, HttpResponse> _handleRequest;

    public HttpServer(Func<HttpRequest, HttpResponse> handleRequest)
    {
        _handleRequest = handleRequest;
    }

    public void Listen(IPAddress address, int port)
    {
        var listener = new TcpListener(address, port);

        try
        {
            listener.Start();

            while (true)
            {
                Console.WriteLine("Waiting for connection..");

                using var client = listener.AcceptTcpClient();
                Console.WriteLine("Connected!");

                using var stream = client.GetStream();

                try
                {
                    HttpRequest? request = HttpRequestReader.Read(stream);
                    if (request is null)
                    {
                        continue;
                    }

                    if (request.Body.Length > 0)
                    {
                        Console.WriteLine($"Body: {request.Body}");
                    }

                    HttpResponse response = _handleRequest(request);
                    HttpResponseWriter.Write(stream, response);
                }
                catch (InvalidDataException)
                {
                    var response = new HttpResponse("HTTP/1.1 400 Bad Request", "", null);
                    HttpResponseWriter.Write(stream, response);
                }
            }
        }
        catch (SocketException exception)
        {
            Console.WriteLine("SocketException: {0}", exception);
        }
        finally
        {
            listener.Stop();
        }
    }
}
