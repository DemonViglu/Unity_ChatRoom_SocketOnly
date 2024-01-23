using System.Collections;
using System.Collections.Generic;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

public class WithClientSocket
{
    public Socket m_withClientSocket;
    ServerSocket m_serverSocket;
    byte[] m_data=new byte[1024];
    public WithClientSocket(Socket socket, ServerSocket serverSocket) {
        m_withClientSocket = socket;
        m_serverSocket = serverSocket;
        Thread thread=new Thread(StartReceiveFromClient); thread.Start();
    }

    private void StartReceiveFromClient() {
        while(true) {
            if (m_withClientSocket.Poll(10, SelectMode.SelectRead)) {
                m_withClientSocket.Close();
                return;
            }
            int length = m_withClientSocket.Receive(m_data);
            string str = Encoding.UTF8.GetString(m_data, 0, length);
            Message msg=JsonUtility.FromJson<Message>(str);

            switch (msg.sendObject) {
                case SendObject.Client:
                    m_serverSocket.SendMessageToClient(msg,this);
                    break;
                case SendObject.Server:
                    m_serverSocket.SendMessageToServer(msg,this);    
                    break;
            }
        }
    }

    public void SendMessageToClient(string str) {
        byte[] data = Encoding.UTF8.GetBytes(str);
        m_withClientSocket?.Send(data);
    }
    byte []sendBuffer=new byte[1024];
    public void SendMessageToClient(Message msg) {
        string tmp = JsonUtility.ToJson(msg);
        Debug.Log(tmp);
        sendBuffer = Encoding.UTF8.GetBytes(tmp);
        m_withClientSocket?.Send(sendBuffer);
    }
}
