using System;
using UnityEngine;
using UnityStandardAssets.CrossPlatformInput;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof(CustomCharacterController))]
    public class CharacterInput : MonoBehaviour
    {
        private CustomCharacterController m_Character;
        private bool m_Jump;         

        private void Start()
        {
            m_Character = GetComponent<CustomCharacterController>();
        }


        private void Update()
        {
            if (!m_Jump)
            {
                m_Jump = CrossPlatformInputManager.GetButtonDown("Jump");
            }
            if (CrossPlatformInputManager.GetButtonDown("Attack")) m_Character.Attack();
        }

        
        private void FixedUpdate()
        {
            float h = CrossPlatformInputManager.GetAxis("Horizontal");
            float v = -CrossPlatformInputManager.GetAxis("Vertical"); // Should not be inverted, but model orientation is wrong atm
            
            m_Character.Move(m_Jump, h, v);
            m_Jump = false;
        }
    }
}
