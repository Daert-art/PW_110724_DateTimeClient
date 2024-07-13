using System.Net.Sockets;
using System.Net;
using System.Text;

namespace PW_110724_AsyncDateTimeClient
{
    class AsyncClient
    {
        private IPEndPoint endP;
        private Socket socket;

        public AsyncClient(string strAddr, int port)
        {
            endP = new IPEndPoint(IPAddress.Parse(strAddr), port);
        }

        public void StartClient(string request)
        {
            socket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            socket.BeginConnect(endP, new AsyncCallback(MyConnectCallbackFunction), new object[] { socket, request });
        }

        private void MyConnectCallbackFunction(IAsyncResult ia)
        {
            object[] state = (object[])ia.AsyncState;
            Socket clientSocket = (Socket)state[0];
            string request = (string)state[1];

            clientSocket.EndConnect(ia);
            if (clientSocket.Connected)
            {
                byte[] requestBytes = Encoding.ASCII.GetBytes(request);
                clientSocket.BeginSend(requestBytes, 0, requestBytes.Length, SocketFlags.None, new AsyncCallback(MySendCallbackFunction), clientSocket);
            }
        }

        private void MySendCallbackFunction(IAsyncResult ia)
        {
            Socket clientSocket = (Socket)ia.AsyncState;
            int bytesSent = clientSocket.EndSend(ia);

            byte[] buffer = new byte[1024];
            clientSocket.BeginReceive(buffer, 0, buffer.Length, SocketFlags.None, new AsyncCallback(MyReceiveCallbackFunction), buffer);
        }

        private void MyReceiveCallbackFunction(IAsyncResult ia)
        {
            byte[] buffer = (byte[])ia.AsyncState;
            int bytesRead = socket.EndReceive(ia);
            string serverResponse = Encoding.ASCII.GetString(buffer, 0, bytesRead);
            Console.WriteLine($"Received from server: {serverResponse}");
            socket.Shutdown(SocketShutdown.Both);
            socket.Close();
        }
    }

    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Enter 'time' to request current time or 'date' to request current date:");
            string request = Console.ReadLine();

            AsyncClient client = new AsyncClient("127.0.0.1", 11000);
            client.StartClient(request);
            Console.ReadLine();
        }
    }
}
