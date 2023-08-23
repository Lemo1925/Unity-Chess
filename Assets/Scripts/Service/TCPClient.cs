using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;


public class TCPClient : NetworkConfig
{
    private TcpClient client;
    private NetworkStream stream;
    private Thread receiveThread;

    private void Start()
    {
        client = new TcpClient();
        client.Connect(SERVER_IP, PORT);
        stream = client.GetStream();

        receiveThread = new Thread(new ThreadStart(ReceiveData));
        receiveThread.Start();
    }
    
    private void SendMessageToServer(string message)
    {
        byte[] data = Encoding.ASCII.GetBytes(message);
        stream.Write(data, 0, data.Length);
        stream.Flush();
    }
    
    private void ReceiveData()
    {
        byte[] data = new byte[1024];
        string response = string.Empty;
        while (true)
        {
            int bytesRead = stream.Read(data, 0, data.Length);
            response += Encoding.ASCII.GetString(data, 0, bytesRead);
            if (response.Length > 0)
            {
                response = string.Empty;
            }
        }
    }
    
    
    private void OnDestroy()
    {
        receiveThread.Abort();
        stream.Close();
        client.Close();
    }
}
