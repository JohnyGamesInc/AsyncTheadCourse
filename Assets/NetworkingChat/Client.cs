using System;
using Unity.Collections;
using Unity.Networking.Transport;
using UnityEngine;


namespace NetworkingChat
{
    
    public class Client : MonoBehaviour
    {

        public event Action<object> OnMessageReceive = delegate(object o) {  };

        private NetworkDriver driver;
        private NetworkConnection connection;

        private bool isStarted;

        
        private void Update()
        {
            
            if(!connection.IsCreated) return;
            
            driver.ScheduleUpdate().Complete();

            DataStreamReader streamReader;
            NetworkEvent.Type cmd;
            while ((cmd = connection.PopEvent(driver, out streamReader)) != NetworkEvent.Type.Empty)
            {
                switch (cmd)
                {
                    case NetworkEvent.Type.Connect:
                        Debug.Log("You have been connected to the server");
                        OnMessageReceive("You have been connected to the server");
                        
                        driver.BeginSend(connection, out var writer);
                        writer.WriteFixedString512("Client is connected");
                        driver.EndSend(writer);
                        break;
                    
                    case NetworkEvent.Type.Data:
                        var inMessage = streamReader.ReadFixedString512();
                        Debug.Log($"From Server: [{inMessage}]");
                        OnMessageReceive(inMessage);
                        break;
                    
                    case NetworkEvent.Type.Disconnect:
                        Debug.Log("You have been disconnected from server");
                        OnMessageReceive("You have been disconnected from server");
                        connection = default;
                        break;
                }
            }

        }


        public void Connect()
        {
            driver = NetworkDriver.Create();

            var endpoint = NetworkEndpoint.LoopbackIpv4.WithPort(9000);
            connection = driver.Connect(endpoint);

            if (connection.IsCreated)
            {
                isStarted = true;
            }
        }


        public void Disconnect()
        {
            if (connection.IsCreated)
            {
                connection.Disconnect(driver);
                connection = default;
                isStarted = false;
            }
        }


        public void SendMessage(string message)
        {
            driver.BeginSend(connection, out var writer);
            writer.WriteFixedString512(message);
            driver.EndSend(writer);
        }


        private void OnDestroy()
        {
            driver.Dispose();
        }
        
        
    }
}