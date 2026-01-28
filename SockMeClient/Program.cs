// https://learn.microsoft.com/en-us/dotnet/fundamentals/networking/sockets/socket-services

using System.Net;
using System.Net.Sockets;
using System.Text;

namespace SockMeClient;

public class Program
{
    public static async Task Main(string[] args)
    {
        var addrs = await Dns.GetHostAddressesAsync("www.bbc.co.uk");
        
        Console.WriteLine("{1} IP Addresses: {0}", string.Join(',', addrs.Select(x => x.ToString())), addrs.Length);

        // this hostname - quite a workaround to get localhost
        var me = Dns.GetHostName();
        var ipe = await Dns.GetHostEntryAsync(me);
        var ipa = ipe.AddressList[0];
        IPEndPoint ipEndPoint = new(ipa, 11_000);
        
        using Socket client = new(
            ipEndPoint.AddressFamily, 
            SocketType.Stream, 
            ProtocolType.Tcp);

        Console.WriteLine("Attempting a connection to {0}:{1}", ipEndPoint.Address.ToString(), ipEndPoint.Port);
        await client.ConnectAsync(ipEndPoint);
        
        another:
        while (true)
        {
            // Send message.
            var message = "kys pls<|EOM|>";
            var messageBytes = Encoding.ASCII.GetBytes(message);
            _ = await client.SendAsync(messageBytes, SocketFlags.None);
            Console.WriteLine($"TX: \"{message}\"");

            // Receive ack.
            var buffer = new byte[1_024];
            var received = await client.ReceiveAsync(buffer, SocketFlags.None);
            var response = Encoding.ASCII.GetString(buffer, 0, received);
            if (response == "<|ACK|>")
            {
                Console.WriteLine($"RX: \"{response}\"");
            }

            Console.Write("Send another? Y/N");
            var userRequest = Console.ReadLine();

            if (!string.IsNullOrWhiteSpace(userRequest) && userRequest.ToUpper().StartsWith("Y"))
            {
                goto another; // fuck you goto is ok
            }
            
            break;
        }

        client.Shutdown(SocketShutdown.Both);
    }

}