using System;
using Unity.Netcode;
using UnityEngine;


namespace MultiplayerNetcode
{
    
    public class Player : NetworkBehaviour
    {

        [SerializeField] private GameObject playerPrefab;
        
        private GameObject playerCharacter;

        
        
        private void Start()
        {
            SpawnCharacter();
        }


        private void SpawnCharacter()
        {
            if (!IsServer) return;

            playerCharacter = Instantiate(playerPrefab);
            
            // NetworkServer.SpawnWithClientAuthority(playerCharacter, connectionToClient);
        }
        
        
    }
}