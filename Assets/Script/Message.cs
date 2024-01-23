
using CustomInspector;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public enum SendObject { Server,Client,Setting}
[Serializable]
public class Message
{
    public SendObject sendObject;
    public List<int> clientId;
    public string message;

    public int sendClientId;

    public Message(SendObject sendObject, List<int> clientId, string message,int sendCliendId=0) {
        this.clientId = new List<int>();
        this.sendObject = sendObject;
        this.clientId = clientId;
        this.message = message;
        this.sendClientId = sendCliendId;
    }

    public Message(SendObject sendObject, int clientId, string message,int sendCliendId=0) {
        this.clientId = new List<int> { clientId };
        this.sendObject = sendObject;
        this.message = message;
        this.sendClientId = sendCliendId;
    }
}
