using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
public class Icon : MonoBehaviour
{
    public int cliendId;

    public List<string> chatHistory;

    [SerializeField] Button iconButton;

    public Action<Icon> GetIcon;

    private void Start() {
        chatHistory = new List<string>();
        iconButton.onClick.AddListener(() =>
        {
            GetIcon?.Invoke(this);
        });
    }

    public List<string> GetChatHistory() {
        return chatHistory;
    }

    public void AddChatSentence(string str) {
        chatHistory.Add(str);
    }
}
