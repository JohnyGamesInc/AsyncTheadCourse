using UnityEngine;

using Unity.Collections;
using Unity.Networking.Transport;


namespace NetworkingChat
{
    
    public class Server : MonoBehaviour
    {

        private NetworkDriver driver;

        private NativeList<NetworkConnection> connections;
        
        
        private bool isStarted;


        private void Update()
        {
            if (!isStarted) return;
            
            driver.ScheduleUpdate().Complete();
            
            // Clean up connections
            for (int i = 0; i < connections.Length; i++)
            {
                if (!connections[i].IsCreated)
                {
                    connections.RemoveAtSwapBack(i);
                    i--;
                }
            }

            //Accept new connections
            NetworkConnection networkConnection;
            while ((networkConnection = driver.Accept()) != default)
            {
                connections.Add(networkConnection);
                Debug.Log($"Accepted a connection {networkConnection}");
                SendMessageToAll($"Player {networkConnection} has connected");
            }

            
            for (int i = 0; i < connections.Length; i++)
            {
                DataStreamReader streamReader;
                NetworkEvent.Type cmd;
                while ((cmd = driver.PopEventForConnection(connections[i], out streamReader)) !=
                       NetworkEvent.Type.Empty)
                {
                    switch (cmd)
                    {
                        case NetworkEvent.Type.Data:
                            var inMessage = streamReader.ReadFixedString512();
                            Debug.Log($"Client {connections[i]}. Message: {inMessage}");
                            SendMessageToAll($"PLayer {connections[i]}: {inMessage}");
                            break;

                        case NetworkEvent.Type.Disconnect:
                            Debug.Log("Client Disconnected from Server");
                            SendMessageToAll($"Player {connections[i]} has disconnected");
                            connections[i] = default;
                            break;
                    }
                }
            }
        }
        
        
        public void StartServer()
        {
            driver = NetworkDriver.Create();
            var endpoint = NetworkEndpoint.AnyIpv4;
            endpoint.Port = 9000;

            if (driver.Bind(endpoint) != 0)
                Debug.Log("Failed to bind to port 9000");
            else
                driver.Listen();

            connections = new NativeList<NetworkConnection>(16, Allocator.Persistent);

            isStarted = true;
            Debug.Log("Server Started");
        }


        public void ShutDownServer()
        {
            if (driver.IsCreated)
            {
                driver.Dispose();
                connections.Dispose();
                isStarted = false;
            }
            Debug.Log("Server Stopped");
        }


        public void SendMessageToAll(string message)
        {
            for (int i = 0; i < connections.Length; i++)
            {
                SendMessage(message, connections[i]);
            }
        }


        public void SendMessage(string message, NetworkConnection connection)
        {
            driver.BeginSend(NetworkPipeline.Null, connection, out var writer);
            writer.WriteFixedString512(message);
            driver.EndSend(writer);
        }
        
        
        private void OnDestroy()
        {
            ShutDownServer();
        }
        

    }
}