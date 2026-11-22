using System.Net.Sockets;
using System.Text;

namespace Karin.Http;

public static class HttpResponseWriter
{
    public static async Task WriteAsync(NetworkStream stream, HttpResponse response)
    {
        byte[] bodyBytes = Encoding.UTF8.GetBytes(response.Body);

        string headers = response.StatusLine + "\r\n";
        if (response.ContentType is not null)
        {
            headers += $"Content-Type: {response.ContentType}\r\n";
        }

        headers += $"Content-Length: {bodyBytes.Length}\r\n" +
                   "Connection: close\r\n" +
                   "\r\n";

        byte[] headerBytes = Encoding.ASCII.GetBytes(headers);
        await stream.WriteAsync(headerBytes);
        await stream.WriteAsync(bodyBytes);
    }
}
