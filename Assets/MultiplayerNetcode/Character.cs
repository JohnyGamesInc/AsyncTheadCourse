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

        // [SyncVar]
        protected Vector3 serverPosition;

        
        protected virtual void Initiate()
        {
            OnUpdateAction += Movement;
        }


        private void Update()
        {
            OnUpdateAction?.Invoke();
        }

        
        // [Command]
        protected void CmdUpdatePosition(Vector3 position)
        {
            serverPosition = position;
        }


        public abstract void Movement();


    }
}