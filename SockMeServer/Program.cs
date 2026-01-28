
using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SockMeServer;

public class Program
{
    public static async Task Main(string[] args)
    {
        Console.WriteLine("Sock Me Server is starting up.");
        var me = Dns.GetHostName();
        var ipe = await Dns.GetHostEntryAsync(me);
        var ipa = ipe.AddressList[0];
        IPEndPoint ipEndPoint = new(ipa, 11_000);
        
        using Socket listener = new(
            ipEndPoint.AddressFamily,
            SocketType.Stream,
            ProtocolType.Tcp);

        listener.Bind(ipEndPoint);
        listener.Listen(100);
        
        Console.WriteLine("Server now listening on {0}:{1}", ipEndPoint.Address.ToString(), ipEndPoint.Port);
        
        var handler = await listener.AcceptAsync();
        
        if (handler.RemoteEndPoint is IPEndPoint endpoint)
        {
            Console.WriteLine("{0} connected", endpoint.Address.ToString());
        }

        while (true)
        {
            // Receive message.
            var buffer = new byte[1_024];
            var received = await handler.ReceiveAsync(buffer, SocketFlags.None);
            var response = Encoding.ASCII.GetString(buffer, 0, received);
    
            var eom = "<|EOM|>";
            if (response.IndexOf(eom) > -1 /* is end of message */)
            {
                Console.WriteLine($"RX: \"{response.Replace(eom, "")}\"");

                var ackMessage = "<|ACK|>";
                var echoBytes = Encoding.ASCII.GetBytes(ackMessage);
                await handler.SendAsync(echoBytes, 0);
                Console.WriteLine( $"TX: \"{ackMessage}\"");

                //break;
            }
        }

    }

}