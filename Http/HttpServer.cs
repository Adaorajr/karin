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

                TcpClient client = listener.AcceptTcpClient();
                Console.WriteLine("Connected!");

                _ = Task.Run(() => HandleClient(client));
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

    private void HandleClient(TcpClient client)
    {
        using (client)
        {
            try
            {
                using var stream = client.GetStream();

                try
                {
                    HttpRequest? request = HttpRequestReader.Read(stream);
                    if (request is null)
                    {
                        return;
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
            catch (Exception exception)
            {
                Console.WriteLine("Client error: {0}", exception);
            }
        }
    }
}
