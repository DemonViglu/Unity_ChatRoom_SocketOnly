using CustomInspector;
using JetBrains.Annotations;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class ConsoleUIManager : MonoBehaviour
{
    [SerializeField] private Transform consolePanel;
    [SerializeField] private GameObject messagePrefab;

    [SerializeField] private ServerSocket m_serverSocket;

    private void Awake() {
        m_serverSocket.ReceiveMessage += AddConsoleLog;
    }

    [Button(nameof(AddConsoleLog),true)]
    public string message;
    public void AddConsoleLog(string message) {
        GameObject tmp=Instantiate(messagePrefab,consolePanel);
        tmp.GetComponent<Text>().text=message;
    }

    public void AddConsoleLog(Message message) {
        GameObject tmp = Instantiate(messagePrefab, consolePanel);
        string str = "A message that send to :"+message.sendObject;
        foreach (int a in message.clientId) {
            str += a.ToString() + " ";
        }
        str +='\n'+ " and content is :" + message.message;
        tmp.GetComponent<Text>().text = str;
    }
}
