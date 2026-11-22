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

    public async Task ListenAsync(IPAddress address, int port)
    {
        var listener = new TcpListener(address, port);

        try
        {
            listener.Start();

            while (true)
            {
                Console.WriteLine("Waiting for connection..");

                TcpClient client = await listener.AcceptTcpClientAsync();
                Console.WriteLine("Connected!");

                _ = HandleClientAsync(client);
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

    private async Task HandleClientAsync(TcpClient client)
    {
        using (client)
        {
            try
            {
                using var stream = client.GetStream();

                try
                {
                    HttpRequest? request;
                    using (var readTimeout = new CancellationTokenSource(TimeSpan.FromSeconds(5)))
                    {
                        try
                        {
                            request = await HttpRequestReader.ReadAsync(stream, readTimeout.Token);
                        }
                        catch (OperationCanceledException) when (readTimeout.IsCancellationRequested)
                        {
                            Console.WriteLine("Request timed out.");
                            return;
                        }
                    }

                    if (request is null)
                    {
                        return;
                    }

                    if (request.Body.Length > 0)
                    {
                        Console.WriteLine($"Body: {request.Body}");
                    }

                    HttpResponse response = _handleRequest(request);
                    await HttpResponseWriter.WriteAsync(stream, response);
                }
                catch (InvalidDataException)
                {
                    var response = new HttpResponse("HTTP/1.1 400 Bad Request", "", null);
                    await HttpResponseWriter.WriteAsync(stream, response);
                }
            }
            catch (Exception exception)
            {
                Console.WriteLine("Client error: {0}", exception);
            }
        }
    }
}
