using System.Net.Sockets;
using System.Text;

namespace Karin.Http;

public static class HttpResponseWriter
{
    public static void Write(NetworkStream stream, HttpResponse response)
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
        stream.Write(headerBytes, 0, headerBytes.Length);
        stream.Write(bodyBytes, 0, bodyBytes.Length);
    }
}
