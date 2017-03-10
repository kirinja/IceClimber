using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof(CustomCharacterController))]
    public class CharacterInput : MonoBehaviour
    {
        private CustomCharacterController character;
        private bool jump;         

        private void Start()
        {
            character = GetComponent<CustomCharacterController>();
        }


        private void Update()
        {
            if (!jump)
            {
                jump = Input.GetButtonDown("Jump");
            }
            if (Input.GetButtonDown("Attack")) character.Attack();
        }

        
        private void FixedUpdate()
        {
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = -CrossPlatformInputManager.GetAxis("Vertical"); // Should not be inverted, but model orientation is wrong atm
            
            character.Move(jump, h, v);
            jump = false;
        }
    }
}
