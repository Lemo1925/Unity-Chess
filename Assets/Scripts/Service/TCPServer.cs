using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

public class TCPServer : NetworkConfig
{
    private TcpListener listener;
    private Thread clientThread;

    private void Start()
    {
        listener = new TcpListener(IPAddress.Parse(SERVER_IP), PORT);
        listener.Start();

        clientThread = new Thread(ListenForClients);
        clientThread.Start();
    }
    
    private void ListenForClients()
    {
        while (true)
        {
            TcpClient client = listener.AcceptTcpClient();
            Thread ClientThread = new Thread(HandleClientConnection);
            ClientThread.Start(client);
        }
    }
    
    private void HandleClientConnection(object client)
    {
        TcpClient tcpClient = (TcpClient)client;
        NetworkStream stream = tcpClient.GetStream();

        while (true)
        {
            byte[] data = new byte[1024];
            int bytesRead = stream.Read(data, 0, data.Length);

            if (bytesRead > 0)
            {
                string request = Encoding.ASCII.GetString(data, 0, bytesRead);
                print(request);
                
                // 进行对等模型的状态同步处理
                // ...

                byte[] response = Encoding.ASCII.GetBytes("Hello from server");
                stream.Write(response, 0, response.Length);
                stream.Flush();
            }
        }
    }
    
    private void OnDestroy()
    {
        clientThread.Abort();
        listener.Stop();
    }
}
