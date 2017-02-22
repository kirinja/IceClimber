using UnityEngine;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Animator))]
    public class CustomCharacterController : MonoBehaviour
    {
        [SerializeField]
        public float m_MovingTurnSpeed = 360;
        [SerializeField]
        public float m_StationaryTurnSpeed = 180;
        [SerializeField]
        float m_MoveSpeedMultiplier = 1f;
        [SerializeField]
        float m_GroundCheckDistance = 0.3f;

        public float MaxJumpLength;
        public float MaxJumpHeight;
        public float MaxAirSpeed;
        

        new private Rigidbody rigidbody;
        private Animator animator;
        private bool isGrounded;
        float m_OrigGroundCheckDistance;
        float m_TurnAmount;
        float m_ForwardAmount;
        Vector3 m_GroundNormal;

        void Start()
        {
            animator = GetComponent<Animator>();
            rigidbody = GetComponent<Rigidbody>();

            rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            m_OrigGroundCheckDistance = m_GroundCheckDistance;
        }


        public void Move(bool jump, float h, float v)
        {
            Vector3 move = new Vector3(0, 0, v);
            m_ForwardAmount = v;
            m_TurnAmount = h;
            CheckGroundStatus();
            move = Vector3.ProjectOnPlane(move, m_GroundNormal);

            ApplyTurnRotation();
            
            if (isGrounded)
            {
                HandleGroundedMovement(jump, move);
            }
            else
            {
                HandleAirborneMovement();
            }
            
            UpdateAnimator(move);
        }


        void UpdateAnimator(Vector3 move)
        {
            animator.SetBool("OnGround", isGrounded);
            animator.SetBool("Walking", Mathf.Abs(m_ForwardAmount) > 0);
        }


        void HandleAirborneMovement()
        {
            Vector3 gravity = new Vector3(0f, (-2 * MaxJumpHeight * Mathf.Pow(MaxAirSpeed, 2)) / (Mathf.Pow(MaxJumpLength / 2, 2)), 0f);
            rigidbody.AddForce(gravity);

            m_GroundCheckDistance = rigidbody.velocity.y < 0 ? m_OrigGroundCheckDistance : 0.01f;
        }


        void HandleGroundedMovement(bool jump, Vector3 move)
        {
            if (jump)
            {
                Jump();
            }
            else
            {
                Vector3 v = new Vector3(0, move.y, move.z * m_MoveSpeedMultiplier);
                rigidbody.velocity = transform.TransformDirection(v);
            }
        }

        private void Jump()
        {
            float jumpVelocity = (2 * MaxJumpHeight * MaxAirSpeed) / (MaxJumpLength / 2);
            Vector3 horizontalVelocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
            horizontalVelocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z).normalized * Mathf.Min(horizontalVelocity.magnitude, MaxAirSpeed);
            rigidbody.velocity = new Vector3(horizontalVelocity.x, jumpVelocity, horizontalVelocity.z);
            isGrounded = false;
            m_GroundCheckDistance = 0.1f;
        }

        void ApplyTurnRotation()
        {
            float turnSpeed = Mathf.Lerp(m_StationaryTurnSpeed, m_MovingTurnSpeed, m_ForwardAmount);
            transform.Rotate(0, m_TurnAmount * turnSpeed * Time.deltaTime, 0);
        }


        void CheckGroundStatus()
        {
            RaycastHit hitInfo;
#if UNITY_EDITOR
            Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * m_GroundCheckDistance));
#endif

            if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, m_GroundCheckDistance))
            {
                isGrounded = true;
                m_GroundNormal = Vector3.up;
            }
            else
            {
                isGrounded = false;
                m_GroundNormal = hitInfo.normal;
            }

            
        }
    }
}
