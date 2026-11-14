using System.Net;
using System.Net.Sockets;

namespace Karin;

public static class CustomTcpListener
{
    public static void Listen()
    {
        TcpListener? server = null;

        try
        {
            int port = 13000;
            var localAddress = IPAddress.Parse("127.0.0.1");

            server = new TcpListener(localAddress, port);
            server.Start();

            var bytes = new byte[256];
            string? data = null;

            while (true)
            {
                Console.WriteLine("Waiting for connection..");

                using var client = server.AcceptTcpClient();
                Console.WriteLine("Connected!");

                data = null;

                var stream = client.GetStream();

                while (true)
                {
                    int bytesRead = stream.Read(bytes, 0, bytes.Length);

                    if (bytesRead == 0)
                    {
                        break;
                    }
                    data = System.Text.Encoding.ASCII.GetString(bytes, 0, bytesRead);
                    Console.WriteLine("Received: {0}", data);

                    data = data.ToUpper();

                    byte[] msg = System.Text.Encoding.ASCII.GetBytes(data);

                    stream.Write(msg, 0, msg.Length);
                    Console.WriteLine("Sent: {0}", data);
                }
            }
        }
        catch (SocketException e)
        {
            Console.WriteLine("SocketException: {0}", e);
        }
        finally
        {
            server?.Stop();
        }

        Console.WriteLine("\n Hit enter to continue..");
        Console.Read();
    }
}
