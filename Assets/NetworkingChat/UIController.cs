using System;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;


namespace NetworkingChat
{
    
    public class UIController : MonoBehaviour
    {

        [SerializeField] private Button startServerButton;
        [SerializeField] private Button shutDownServerButton;
        [SerializeField] private Button connectClient;
        [SerializeField] private Button disconnectClient;
        [SerializeField] private Button sendMessageButton;

        [SerializeField] private TMP_InputField inputField;
        
        [SerializeField] private TextArea textArea;
        
        [SerializeField] private Server server;
        [SerializeField] private Client client;
        
        
        private void Start()
        {
            throw new NotImplementedException();
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
        
        



    }
}