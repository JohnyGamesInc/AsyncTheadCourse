using System;
using UnityEngine;


namespace MultiplayerNetcode
{
    
    public class PlayerCharacter : Character
    {

        [Range(0, 100)] 
        [SerializeField] private int health = 100;
        
        [Range(0.5f, 10.0f)] 
        [SerializeField] private float movingSpeed = 8.0f;

        [SerializeField] private float acceleration = 3.0f;

        
        private CharacterController characterController;
        private MouseLook mouseLook;

        private Vector3 currentVelocity;
        
        private const float gravity = -9.8f;

        public override FireAction FireAction { get;  protected set; }


        private void Start()
        {
            Initiate();
        }


        private void OnGUI()
        {
            if (Camera.main == null) return;
            
            var info = $"Health: {health}\nClip: {FireAction.BulletCount}"; 
            var size = 12; 
            var bulletCountSize = 50; 
            var posX = Camera.main.pixelWidth / 2 - size / 4; 
            var posY = Camera.main.pixelHeight / 2 - size / 2; 
            var posXBul = Camera.main.pixelWidth - bulletCountSize * 2; 
            var posYBul = Camera.main.pixelHeight - bulletCountSize; 
            GUI.Label(new Rect(posX, posY, size, size), "+"); 
            GUI.Label(new Rect(posXBul, posYBul, bulletCountSize * 2, bulletCountSize * 2), info);
        }


        protected override void Initiate()
        {
            base.Initiate();
            FireAction = gameObject.AddComponent<RayShooter>();
            FireAction.Reloading();
            characterController = GetComponentInChildren<CharacterController>();
            characterController ??= gameObject.AddComponent<CharacterController>();
            mouseLook = GetComponentInChildren<MouseLook>();
            mouseLook ??= gameObject.AddComponent<MouseLook>();
        }


        public override void Movement()
        {
            if (mouseLook != null && mouseLook.PlayerCamera != null)
                mouseLook.PlayerCamera.enabled = IsOwner;

            if (IsOwner)
            {
                var moveX = Input.GetAxis("Horizontal") * movingSpeed * Time.deltaTime;
                var moveZ = Input.GetAxis("Vertical") * movingSpeed * Time.deltaTime;
                var movement = new Vector3(moveX, 0, moveZ);
                movement = Vector3.ClampMagnitude(movement, movingSpeed);

                if (Input.GetKey(KeyCode.LeftShift))
                {
                    movement *= acceleration;
                }

                movement.y = gravity;
                movement = transform.TransformDirection(movement);

                characterController.Move(movement);
                mouseLook.Rotation();
                
                UpdatePositionServerRpc(transform.position);
            }
            else
            {
                transform.position = Vector3.SmoothDamp(
                    transform.position, 
                    serverPosition.Value, 
                    ref currentVelocity,
                    movingSpeed * Time.deltaTime);
            }
        }
        
        
        
        

    }
}