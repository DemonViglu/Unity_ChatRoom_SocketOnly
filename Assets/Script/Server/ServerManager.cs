using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
public class ServerManager : MonoBehaviour
{
    [ReadOnly(true)]
    public string IP = string.Empty;
    [ReadOnly(true)]
    public int Port;

    [SerializeField] private GameObject ServerButton;
    [SerializeField] private Text IPinputText;
    [SerializeField] private Text PortInputText;


    public static ServerManager Instance;
    private void Awake() {
        if (Instance != null) {
            Destroy(Instance);
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    private void Start() {
        InitialButton();
        IP = string.Empty;
        Port = 0;
    }

    private void InitialButton() {
        ServerButton.GetComponent<Button>().onClick.AddListener(() =>
        {
            IP=IPinputText.text;
            if(IP==string.Empty) { IP = "127.0.0.1"; }
            Port=int.Parse(PortInputText.text);
            if (Port == 0) { Port = 7777; }
            SceneManager.LoadSceneAsync("ServerWorkScene");
        });
    }
}
