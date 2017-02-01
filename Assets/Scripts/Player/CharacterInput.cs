using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof(CustomCharacterController))]
    public class CharacterInput : MonoBehaviour
    {
        [Tooltip("The object that controls the character's direction")]
        public Transform Direction;

        private CustomCharacterController m_Character; // A reference to the ThirdPersonCharacter on the object
        private Vector3 m_CamForward;             // The current forward direction of the camera
        private Vector3 m_Move;
        private bool m_Jump;                      // the world-relative desired move direction, calculated from the camForward and user input.


        private void Start()
        {
            // get the third person character ( this should never be null due to require component )
            m_Character = GetComponent<CustomCharacterController>();
        }


        private void Update()
        {
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }
        }


        // Fixed update is called in sync with physics
        private void FixedUpdate()
        {
            // read inputs
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = CrossPlatformInputManager.GetAxis("Vertical");

            // calculate move direction to pass to character
            
                // calculate camera relative direction to move:
                m_CamForward = Vector3.Scale(Direction.forward, new Vector3(1, 0, 1)).normalized;
                m_Move = v * m_CamForward + h * Direction.right;
     

            // pass all parameters to the character control script
            m_Character.Move(m_Move, m_Jump);
            m_Jump = false;
        }
    }
}
