using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Threading;
using System.Text;
using System;
using Unity.VisualScripting;

public class ClientSocket : MonoBehaviour
{
    Socket m_clientSocket;
    byte[] m_data=new byte[1024];

    private bool justReceiveMessage = false;
    private Message m_message;

    public event Action<Message> receiveMessage;
    private void Start() {
        try {
            OpenClient();
        }
        catch { }
    }

    private void Update() {
        if (justReceiveMessage) {
            justReceiveMessage = false;
            receiveMessage?.Invoke(m_message);
        }
    }
    private void OpenClient() {
        m_clientSocket=new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);

        m_clientSocket.Connect(ClientManager.Instance.IP, ClientManager.Instance.Port);

        Thread thread = new Thread(StartReceiveFormServer);
        thread.Start();
    }


    private void StartReceiveFormServer() {
        while (true) {
            int length = m_clientSocket.Receive(m_data);
            string str= Encoding.UTF8.GetString(m_data,0, length);
            Debug.Log(str);
            m_message= JsonUtility.FromJson<Message>(str);
            justReceiveMessage = true;
            
        }
    }
    byte[] send_m_data=new byte[1024];
    public void SendMessageToServer(Message message) {

        string str=JsonUtility.ToJson(message);
        send_m_data=Encoding.UTF8.GetBytes(str);
        m_clientSocket.Send(send_m_data);
    }
}
