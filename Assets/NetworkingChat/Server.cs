using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Assertions;

using Unity.Collections;
using Unity.Networking.Transport;


namespace NetworkingChat
{
    
    public class Server : MonoBehaviour
    {

        private NetworkDriver driver;

        private NativeList<NetworkConnection> connections;

        // private const int MAX_CONNECTIONS = 10;
        //
        // private List<int> connectionIDs = new List<int>();
        //
        // private int port = 5805;
        //
        // private int hostID;
        // private int reliableChannel;
        //
        // private bool isStarted;
        // private byte error;


        private void Start() {}


        private void Update()
        {
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
                Debug.Log("Accepted a connection");
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
                            Debug.Log($"From Client: Got {inMessage} from a client");
                            // number += 2;

                            driver.BeginSend(NetworkPipeline.Null, connections[i], out var writer);
                            writer.WriteFixedString512($"Server: Message from client [{inMessage}] Received");
                            driver.EndSend(writer);
                            break;

                        case NetworkEvent.Type.Disconnect:
                            Debug.Log("Client Disconnected from Server");
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
        }


        public void ShutDownServer()
        {
            if (driver.IsCreated)
            {
                driver.Dispose();
                connections.Dispose();
            }
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