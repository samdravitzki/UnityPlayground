using System.Collections;
using System.Collections.Generic;
using FPController;
using UnityEngine;
using UnityEngine.Serialization;

namespace FPController
{

// Implementation of FPPlayerController that adds basic input
    public class SimpleFPController : FPPlayerController
    {

        [SerializeField] private float lookSpeed = 2;
        
        // Keycodes
        [SerializeField] private KeyCode jumpKey = KeyCode.Space;
        
        // Input: Keyboard Movement
        private float Horizontal => Input.GetAxisRaw("Horizontal");
        private float Vertical => Input.GetAxisRaw("Vertical");
        
        // Input: Mouse Movement
        private float MouseHorizontal => Input.GetAxis("Mouse X") * lookSpeed;
        private float MouseVertical => Input.GetAxis("Mouse Y") * lookSpeed;

        // Overrides
        protected new void Update() // new means I am overriding a function defined in the superclass
        {
            base.Update();
            MouseMove(MouseHorizontal, MouseVertical);
            UpdateInput();
        }

        public new void FixedUpdate()
        {
            base.FixedUpdate();
            Move(Horizontal, Vertical);
        }
        
        // Based of the defined input call operations such as crouch, sprint and more in the superclass
        private void UpdateInput()
        {
            if (Input.GetKeyDown(jumpKey))
            {
                Jump();
            }
        }
        
        
    }
}
