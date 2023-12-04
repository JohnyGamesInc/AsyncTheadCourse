using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Serialization;
using UnityEngine.UI;


namespace NetworkingChat
{
    
    public class UIController : MonoBehaviour
    {

        [SerializeField] private Button startServerButton;
        [SerializeField] private Button shutDownServerButton;
        [SerializeField] private Button connectClientButton;
        [SerializeField] private Button disconnectClientButton;
        [SerializeField] private Button sendMessageButton;

        [SerializeField] private TMP_InputField inputField;
        
        [SerializeField] private TextArea textArea;
        
        [SerializeField] private Server server;
        [SerializeField] private Client client;
        
        
        private void Start()
        {
            startServerButton.onClick.AddListener(() => StartServer());
            shutDownServerButton.onClick.AddListener(() => ShutDownServer());
            connectClientButton.onClick.AddListener(() => Connect());
            disconnectClientButton.onClick.AddListener(() => Disconnect());
            sendMessageButton.onClick.AddListener(() => SendMessage());
            client.OnMessageReceive += ReceiveMessage;
        }
        
        
        private void StartServer()
        {
            server.StartServer();
        }
        
        
        private void ShutDownServer()
        {
            server.ShutDownServer();
        }
        
        
        private void Connect()
        {
            client.Connect();
        }
        
        
        private void Disconnect()
        {
            client.Disconnect();
        }
        
        
        private void SendMessage()
        {
            client.SendMessage(inputField.text);
            inputField.text = "";
        }
        
        
        public void ReceiveMessage(object message)
        {
            textArea.ReceiveMessage(message);
        }


        private void OnDestroy()
        {
            startServerButton.onClick.RemoveAllListeners();
            shutDownServerButton.onClick.RemoveAllListeners();
            connectClientButton.onClick.RemoveAllListeners();
            disconnectClientButton.onClick.RemoveAllListeners();
            sendMessageButton.onClick.RemoveAllListeners();
            client.OnMessageReceive -= ReceiveMessage;
        }
        
        
    }
}