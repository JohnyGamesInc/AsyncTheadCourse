using System;
using Unity.Netcode;
using UnityEngine;


namespace MultiplayerNetcode
{
    
    [RequireComponent(typeof(CharacterController))]
    public abstract class Character : NetworkBehaviour
    {

        public Action OnUpdateAction { get; protected set; }
        
        public abstract FireAction FireAction { get; protected set; }

        protected NetworkVariable<Vector3> serverPosition = new();

        
        protected virtual void Initiate()
        {
            OnUpdateAction += Movement;
        }


        private void Update()
        {
            OnUpdateAction?.Invoke();
        }

        
        [ServerRpc]
        protected void UpdatePositionServerRpc(Vector3 position)
        {
            serverPosition.Value = position;
        }


        public abstract void Movement();


    }
}