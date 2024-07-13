using System.Net.Sockets;
using System.Net;
using System.Text;

namespace PW_110724_DateTimeClient
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("enter 'date' to request a date or 'time' to request a time:");
            string request = Console.ReadLine();

            IPAddress ip = IPAddress.Parse("127.0.0.1");
            IPEndPoint endPoint = new IPEndPoint(ip, 11000);
            Socket clientSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            try
            {
                clientSocket.Connect(endPoint);
                if (clientSocket.Connected)
                {
                    clientSocket.Send(Encoding.ASCII.GetBytes(request));

                    byte[] buffer = new byte[1024];
                    int receivedBytes = clientSocket.Receive(buffer);
                    string serverResponse = Encoding.ASCII.GetString(buffer, 0, receivedBytes);

                    Console.WriteLine($"Response from the server: {serverResponse}");
                }
                else
                {
                    Console.WriteLine("Failed to connect to server.");
                }
            }
            catch (SocketException ex)
            {
                Console.WriteLine(ex.ToString());
            }
            finally
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
        }
    }
}
