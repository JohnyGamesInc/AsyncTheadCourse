using Unity.Netcode;
using UnityEngine;


namespace MultiplayerCodeMonkey
{
    
    public class PlayerNetwork : NetworkBehaviour
    {

        [SerializeField] private Transform spawnedObjectPrefab;
        private Transform spawnedObj;
        
        
        private NetworkVariable<int> randomNumber = new NetworkVariable<int>(1, NetworkVariableReadPermission.Everyone, NetworkVariableWritePermission.Owner);

        private NetworkVariable<Vector3> serverPosition = new NetworkVariable<Vector3>();


        public override void OnNetworkSpawn()
        {
            
            randomNumber.OnValueChanged += ((prevValue, newValue) =>
            {
                Debug.Log(OwnerClientId + " :Random: " + randomNumber.Value);
            });
        }

        
        private void Update()
        {
            if (!IsOwner) return;

            if (Input.GetKeyDown(KeyCode.R))
            {
                // TestServerRpc(new ServerRpcParams());
                // TestClientRpc(new ClientRpcParams());
                // randomNumber.Value = Random.Range(0, 100);
                spawnedObj = Instantiate(spawnedObjectPrefab);
                spawnedObj.GetComponent<NetworkObject>().Spawn(true);
            }

            if (Input.GetKeyDown(KeyCode.Y))
            {
                Destroy(spawnedObj.gameObject);
            }
            
            
            Vector3 moveDirection = new Vector3();

            if (Input.GetKey(KeyCode.W)) moveDirection.z += 1f;
            if (Input.GetKey(KeyCode.S)) moveDirection.z -= 1f;
            if (Input.GetKey(KeyCode.A)) moveDirection.x -= 1f;
            if (Input.GetKey(KeyCode.D)) moveDirection.x += 1f;

            float moveSpeed = 3.0f;

            if (NetworkManager.Singleton.IsServer)
            {
                var newPos = transform.position +  moveDirection  * moveSpeed * Time.deltaTime;
                transform.position = newPos;
                serverPosition.Value = newPos;
            }
            else
            {
                if (IsLocalPlayer)
                {
                    
                    SendPositionServerRpc(moveDirection);
                    // Debug.Log($"READ SERVER POSITION: [{serverPosition.Value}]");
                    // transform.position = serverPosition.Value;
                }
                
            }
            
            
            
        }


        [ServerRpc]
        private void TestServerRpc(ServerRpcParams serverParams)
        {
            Debug.Log("TestServerRpc " + OwnerClientId + "|" + serverParams.Receive.SenderClientId);
        }
        
        
        [ClientRpc]
        private void TestClientRpc(ClientRpcParams clientParams)
        {
            Debug.Log("TestClientRPC " + OwnerClientId + "|" + clientParams.Receive);
        }
        
        
        // private void Move

        
        [ServerRpc]
        private void SendPositionServerRpc(Vector3 moveDirection)
        {
            // Debug.Log($"NEW POSITION {newPosition}");
            
            transform.position += moveDirection  * 3.0f * Time.deltaTime;
            // serverPosition.Value = transform.position;
        }
        
        
    }
}