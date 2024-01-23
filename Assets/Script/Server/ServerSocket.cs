using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Net.Sockets;
using System.Net;
using System.Threading;
using System;

public class ServerSocket : MonoBehaviour
{

    [SerializeField] private int listenMax;

    private Socket m_ServerSocket;

    List<WithClientSocket> withClientSockets = new List<WithClientSocket>();
    Dictionary<WithClientSocket, int> withClientSocketDictionary=new Dictionary<WithClientSocket, int>();

    private bool justReceiveMessage = false;
    private Message message;
    public event Action<Message> ReceiveMessage;
    private void Start() {
        try {
            OpenServer();
        }
        catch {
        
        }
        message = new Message(SendObject.Server, 0, "");
    }

    private void Update() {
        CheckTheClient();
        MessageCallback(message);
    }

    /// <summary>
    /// Create Socket and bind and start listen
    /// </summary>
    private void OpenServer() {
        m_ServerSocket=new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
        m_ServerSocket.Bind(new IPEndPoint(IPAddress.Parse(ServerManager.Instance.IP),ServerManager.Instance.Port));
        m_ServerSocket.Listen(listenMax);
        Thread thread=new Thread(StartListenFromClient); thread.Start();
    }
    Thread broadTread;
    private void StartListenFromClient() {
        while(true) {
            Socket socket=m_ServerSocket.Accept();
            WithClientSocket withClientSocket = new WithClientSocket(socket,this);
            //ConsoleUIManager.Instance.AddConsoleLog("A Client has link to server");
            justReceiveMessage = true;
            message.message = "A Client has link to server";
            ChargeTheClient(withClientSocket);
            Debug.Log("Send the ID to client");
            SendMessageToClient(new Message(SendObject.Setting, withClientSocketDictionary[withClientSocket], "Your Id"),withClientSocket);

            broadTread = new Thread(MakeClientAll);
            broadTread.Start();
        }
    }
    /// <summary>
    /// Now the version only support p2p
    /// </summary>
    /// <param name="message"></param>
    public void SendMessageToClient(Message message,WithClientSocket fromSocket) {
        message.sendClientId = withClientSocketDictionary[fromSocket];
        foreach(WithClientSocket socket in withClientSockets) {
            if (withClientSocketDictionary[socket] == message.clientId[0]) {
                socket.SendMessageToClient(message);
                //ConsoleUIManager.Instance.AddConsoleLog("A message send to a client:" + withClientSocketDictionary[socket] + " ,contect:" + message.message + " from: " + withClientSocketDictionary[fromSocket]);
                this.message.message = "A message send to a client:" + withClientSocketDictionary[socket] +  " from: " + withClientSocketDictionary[fromSocket]+"that:"+message.message;
                justReceiveMessage = true;
            }
        }
    }

    public void SendMessageToServer(Message message,WithClientSocket socket) {
        message.sendClientId = withClientSocketDictionary[socket];
        //ConsoleUIManager.Instance.AddConsoleLog("A message send to server,content:" + message.message + " from: " + withClientSocketDictionary[socket]);
        this.message.message = "A message send to server: from: " + withClientSocketDictionary[socket]+" that:"+message.message ;
        justReceiveMessage = true;
        
    }


    int ClientNumber = 0;
    private void ChargeTheClient(WithClientSocket Tosocket) {
        withClientSockets.Add(Tosocket);
        ClientNumber++;
        withClientSocketDictionary[Tosocket] = ClientNumber;
       
    }

    private void CheckTheClient() {
        List<WithClientSocket> newWithClient = new List<WithClientSocket>();
        foreach (var client in withClientSockets) {
            if (client.m_withClientSocket.Connected) {

            }
            else {
                newWithClient.Add(client);
            }
        }
        foreach (var client in newWithClient) {
            withClientSockets.Remove(client);
            withClientSocketDictionary.Remove(client);
        }
    }

    private void MessageCallback(Message message) {
        if (justReceiveMessage) {
            justReceiveMessage = false;
            ReceiveMessage?.Invoke(message);
        }
    }

    private void MakeClientAll() {
        Thread.Sleep(1000);
        foreach (var client in withClientSockets) {
            BroadCastTheClient(new Message(SendObject.Setting, withClientSocketDictionary[client], ""));
        }
    }
    private void BroadCastTheClient(Message message) {
        foreach (var client in withClientSockets) {
            Thread.Sleep(1000);
            client.SendMessageToClient(message);
            Debug.Log(withClientSocketDictionary[client] + "知道了" + message.clientId[0] + "的存在");
            
        }
    }
}
