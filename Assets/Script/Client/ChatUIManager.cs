using CustomInspector;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.ComponentModel;
using UnityEngine.UI;
public class ChatUIManager : MonoBehaviour
{

    [SerializeField] private ClientSocket clientSocket;

    [SerializeField] private Text idText;

    [SerializeField] private int m_clientID=-1;

    [HorizontalLine("MessageBox")]
    [SerializeField] private Transform iconContentTransform;
    [SerializeField] private Transform messageContentTransform;
    [SerializeField] private GameObject IconPrefab;
    [SerializeField] private GameObject MessagePrefab;
    [SerializeField] private Text inputText;
    [SerializeField] private Button sendButton;
    Dictionary<int,Icon> icons = new Dictionary<int,Icon>();

    private Icon currentIcon;
    private void Start() {
        clientSocket.receiveMessage += ReceiveMessage;
        sendButton.onClick.AddListener(SendMessageToClient);
    }
    public void ReceiveMessage(Message message) {
        switch (message.sendObject) {
            case SendObject.Server:
                break;
            case SendObject.Client:
                icons[message.sendClientId].chatHistory.Add("Other:"+message.message);
                break;
            case SendObject.Setting:
                if (m_clientID == -1) {
                    m_clientID = message.clientId[0];
                    idText.text += message.clientId[0].ToString();
                }
                else {
                    Debug.Log(message.clientId[0].ToString() + " 上线了！");
                }
                //如果上线者不是自己
                if (message.clientId[0] != m_clientID) {
                    //如果已经有该对话对象
                    if (icons.ContainsKey(message.clientId[0])) {

                    }
                    else {
                        GameObject tmpIconGameObject=Instantiate(IconPrefab,iconContentTransform);
                        Icon tmpIcon=tmpIconGameObject.GetComponent<Icon>();
                        tmpIconGameObject.GetComponentInChildren<Text>().text = message.clientId[0].ToString();
                        if(currentIcon == null) {
                            currentIcon = tmpIcon;
                        }
                        tmpIcon.cliendId = message.clientId[0];
                        tmpIcon.GetIcon += IconCallBack;
                        icons.Add(message.clientId[0],tmpIcon);
                    }
                }

                break;
        }
    }

    [Button(nameof(SendMessageToServer),true)]
    public string str;
    public void SendMessageToServer(string str) {
        clientSocket.SendMessageToServer(new Message(SendObject.Server,0,str));
    }


    public void SendMessageToClient() {
        clientSocket.SendMessageToServer(new Message(SendObject.Client, currentIcon.cliendId, inputText.text, this.m_clientID));
        currentIcon.chatHistory.Add("Your:"+inputText.text);
        inputText.text = "";
        ReFreshMessageBox();
    }

    public void IconCallBack(Icon icon) {
        currentIcon = icon;
        for(int i=0;i<messageContentTransform.childCount;i++) {
            Destroy(messageContentTransform.GetChild(0).gameObject);
        }
        for(int i=0;i<icon.chatHistory.Count;i++) {
            GameObject gameObject= Instantiate(MessagePrefab, messageContentTransform);
            gameObject.GetComponentInChildren<Text>().text = icon.chatHistory[i];
        }
    }

    private void ReFreshMessageBox() {
        for (int i = 0; i < messageContentTransform.childCount; i++) {
            Destroy(messageContentTransform.GetChild(0).gameObject);
        }
        for (int i = 0; i < currentIcon.chatHistory.Count; i++) {
            GameObject gameObject = Instantiate(MessagePrefab, messageContentTransform);
            gameObject.GetComponentInChildren<Text>().text = currentIcon.chatHistory[i];
        }
    }
}
