using System;
using Unity.Netcode;
using UnityEngine;


namespace MultiplayerNetcode
{
    
    public class MouseLook : NetworkBehaviour
    {

        [Range(0.1f, 10.0f)]
        [SerializeField] private float sensativity = 2.0f;

        [Range(-90.0f, 0.0f)]
        [SerializeField] private float minVert = -45.0f;

        [Range(0.0f, 90.0f)]
        [SerializeField] private float maxVert = 45.0f;
        
        private Camera _camera;

        private float rotationX = 0.0f;
        private float rotationY = 0.0f;
        
        public Camera PlayerCamera => _camera;


        private NetworkVariable<Quaternion> serverRotation = new();


        private void Start()
        {
            _camera = GetComponentInChildren<Camera>();
            var rb = GetComponentInChildren<Rigidbody>();
            if (rb != null) rb.freezeRotation = true;
        }


        public void Rotation()
        {
            rotationX -= Input.GetAxis("Mouse Y") * sensativity;
            rotationY += Input.GetAxis("Mouse X") * sensativity;
            rotationX = Mathf.Clamp(rotationX, minVert, maxVert);
            transform.rotation = Quaternion.Euler(0, rotationY, 0);
            _camera.transform.localRotation = Quaternion.Euler(rotationX, 0, 0);
            
            UpdateRotationServerRpc(transform.rotation);
        }
        
        
        [ServerRpc]
        protected void UpdateRotationServerRpc(Quaternion rotation)
        {
            serverRotation.Value = rotation;
        }
        
        
    }
}