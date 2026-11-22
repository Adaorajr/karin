using System.Net.Sockets;
using System.Text;

namespace Karin.Http;

public static class HttpRequestReader
{
    public static async Task<HttpRequest?> ReadAsync(NetworkStream stream)
    {
        byte[] bytes = new byte[255];
        using var requestBuffer = new MemoryStream();

        string requestText = "";
        int headersEnd = -1;

        while (headersEnd < 0)
        {
            int bytesRead = await stream.ReadAsync(bytes);
            if (bytesRead == 0)
            {
                return null;
            }

            requestBuffer.Write(bytes, 0, bytesRead);
            requestText = Encoding.ASCII.GetString(requestBuffer.ToArray());
            headersEnd = requestText.IndexOf("\r\n\r\n", StringComparison.Ordinal);
        }

        string headersText = requestText[..headersEnd];
        string[] headerLines = headersText.Split("\r\n");
        string[] firstLine = headerLines[0].Split(' ');

        if (firstLine.Length != 3)
        {
            throw new InvalidDataException("Invalid request line.");
        }

        int contentLength = 0;
        foreach (string headerLine in headerLines.Skip(1))
        {
            if (!headerLine.StartsWith("Content-Length:", StringComparison.OrdinalIgnoreCase))
            {
                continue;
            }

            string lengthText = headerLine["Content-Length:".Length..].Trim();
            if (!int.TryParse(lengthText, out contentLength) || contentLength < 0)
            {
                throw new InvalidDataException("Invalid Content-Length.");
            }
        }

        int bodyStart = headersEnd + 4;
        while (requestBuffer.Length - bodyStart < contentLength)
        {
            int bytesRead = await stream.ReadAsync(bytes);
            if (bytesRead == 0)
            {
                return null;
            }

            requestBuffer.Write(bytes, 0, bytesRead);
        }

        string body = "";
        if (contentLength > 0)
        {
            byte[] requestBytes = requestBuffer.ToArray();
            body = Encoding.UTF8.GetString(requestBytes, bodyStart, contentLength);
        }

        string method = firstLine[0];
        string requestTarget = firstLine[1];
        int queryStart = requestTarget.IndexOf('?');
        string path = queryStart >= 0 ? requestTarget[..queryStart] : requestTarget;
        string queryString = queryStart >= 0 ? requestTarget[(queryStart + 1)..] : "";
        string version = firstLine[2];

        return new HttpRequest(method, path, queryString, version, body);
    }
}
