using UnityEngine;

namespace UnityStandardAssets.Characters.ThirdPerson
{
    [RequireComponent(typeof(Rigidbody))]
    [RequireComponent(typeof(CapsuleCollider))]
    [RequireComponent(typeof(Animator))]
    public class CustomCharacterController : MonoBehaviour
    {
        public float MaxGroundSpeed = 3f;
        public float GroundAcceleration = 3f;
        public float FrictionDeacceleration = 3;
        public float MaxAirSpeed = 2f;
        public float AirAcceleration = 2f;
        public float MaxJumpLength = 6.1f;
        public float MaxJumpHeight = 4.2f;
        public float MovingTurnSpeed = 360;
        public float StationaryTurnSpeed = 180;
        public float GroundCheckDistance = 0.3f;
        
        new private Rigidbody rigidbody;
        private Animator animator;
        private bool isGrounded;
        private float originalGroundCheckDistance;
        private float turnAmount;
        private float forwardAmount;
        private Vector3 groundNormal;

        void Start()
        {
            animator = GetComponent<Animator>();
            rigidbody = GetComponent<Rigidbody>();

            rigidbody.constraints = RigidbodyConstraints.FreezeRotationX | RigidbodyConstraints.FreezeRotationY | RigidbodyConstraints.FreezeRotationZ;
            originalGroundCheckDistance = GroundCheckDistance;
        }

        public void Move(bool jump, float h, float v)
        {
            Vector3 move = new Vector3(0, 0, v);
            forwardAmount = v;
            turnAmount = h;
            CheckGroundStatus();
            move = Vector3.ProjectOnPlane(move, groundNormal);

            if (isGrounded)
            {
                HandleGroundedMovement(jump, move);
            }
            else
            {
                HandleAirborneMovement(move);
            }

            UpdateAnimator(move);
        }

        void UpdateAnimator(Vector3 move)
        {
            animator.SetBool("OnGround", isGrounded);
            animator.SetBool("Walking", Mathf.Abs(forwardAmount) > 0);
        }

        void HandleGroundedMovement(bool jump, Vector3 move)
        {
            ApplyTurnRotation();

            if (jump)
            {
                Jump();
            }
            else
            {
                Vector3 localVelocity = transform.InverseTransformDirection(rigidbody.velocity);

                float desiredForwardVelocity = forwardAmount * MaxGroundSpeed;

                if (Mathf.Abs(forwardAmount) > 0)
                {
                    // Determine the acceleration amount
                    // Warning: ugly and possibly duplicate code
                    float accelerationAmount = 0f;
                    if (Mathf.Sign(forwardAmount) == 1f)
                    {
                        if (localVelocity.z < desiredForwardVelocity)
                        {
                            accelerationAmount = Time.deltaTime * GroundAcceleration;
                            if (localVelocity.z + accelerationAmount > desiredForwardVelocity)
                            {
                                accelerationAmount = Mathf.Max(0f, desiredForwardVelocity - localVelocity.z);
                            }
                        }
                    }
                    else
                    {
                        if (localVelocity.z > desiredForwardVelocity)
                        {
                            accelerationAmount = Time.deltaTime * -GroundAcceleration;
                            if (localVelocity.z + accelerationAmount < desiredForwardVelocity)
                            {
                                accelerationAmount = Mathf.Min(0f, desiredForwardVelocity - localVelocity.z);
                            }
                        }
                    }
                    localVelocity.z += accelerationAmount; // Apply acceleration
                }

                // Determine friction
                Vector3 invertedFrictionDirection = localVelocity - new Vector3(0, 0, desiredForwardVelocity);
                if (Mathf.Sign(localVelocity.z) != Mathf.Sign(invertedFrictionDirection.z))
                {
                    invertedFrictionDirection.z = 0f;
                }
                Vector3 friction = -invertedFrictionDirection.normalized * FrictionDeacceleration * Time.deltaTime;
                Vector3 newVelocity = localVelocity + friction;
                if ((desiredForwardVelocity > 0 && localVelocity.z > desiredForwardVelocity && desiredForwardVelocity > newVelocity.z) || (desiredForwardVelocity < 0 && localVelocity.z < desiredForwardVelocity && desiredForwardVelocity < newVelocity.z))
                {
                    newVelocity.z = desiredForwardVelocity;
                }
                if (Mathf.Sign(newVelocity.x) != Mathf.Sign(localVelocity.x)) newVelocity.x = 0f;
                if (Mathf.Sign(newVelocity.y) != Mathf.Sign(localVelocity.y)) newVelocity.y = 0f;
                if (Mathf.Sign(newVelocity.z) != Mathf.Sign(localVelocity.z)) newVelocity.z = 0f;
                localVelocity = newVelocity; // Apply friction

                // Apply changes to rigidbody's global transform
                rigidbody.velocity = transform.TransformDirection(localVelocity);
            }
        }

        void HandleAirborneMovement(Vector3 move)
        {
            Debug.Log(forwardAmount);
            if (Mathf.Abs(forwardAmount) > 0f)
            {
                Vector3 localVelocity = transform.InverseTransformDirection(rigidbody.velocity);
                localVelocity.z += Time.deltaTime * Mathf.Sign(forwardAmount) * AirAcceleration;
                if (Mathf.Abs(localVelocity.z) > MaxAirSpeed) localVelocity.z = MaxAirSpeed * Mathf.Sign(localVelocity.z);
                rigidbody.velocity = transform.TransformDirection(localVelocity);
            }
            if (Mathf.Abs(turnAmount) > 0f)
            {
                Vector3 localVelocity = transform.InverseTransformDirection(rigidbody.velocity);
                localVelocity.x += Time.deltaTime * Mathf.Sign(-turnAmount) * AirAcceleration;
                if (Mathf.Abs(localVelocity.x) > MaxAirSpeed) localVelocity.x = MaxAirSpeed * Mathf.Sign(localVelocity.x);
                rigidbody.velocity = transform.TransformDirection(localVelocity);
            }

            Vector3 gravity = new Vector3(0f, (-2 * MaxJumpHeight * Mathf.Pow(MaxAirSpeed, 2)) / (Mathf.Pow(MaxJumpLength / 2, 2)), 0f);
            rigidbody.AddForce(gravity);
            GroundCheckDistance = rigidbody.velocity.y < 0 ? originalGroundCheckDistance : 0.01f;
        }

        private void Jump()
        {
            float jumpVelocity = (2 * MaxJumpHeight * MaxAirSpeed) / (MaxJumpLength / 2);
            Vector3 horizontalVelocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z);
            horizontalVelocity = new Vector3(rigidbody.velocity.x, 0, rigidbody.velocity.z).normalized * Mathf.Min(horizontalVelocity.magnitude, MaxAirSpeed);
            rigidbody.velocity = new Vector3(horizontalVelocity.x, jumpVelocity, horizontalVelocity.z);
            isGrounded = false;
            GroundCheckDistance = 0.1f;
        }

        void ApplyTurnRotation()
        {
            float turnSpeed = Mathf.Lerp(StationaryTurnSpeed, MovingTurnSpeed, forwardAmount);
            transform.Rotate(0, turnAmount * turnSpeed * Time.deltaTime, 0);
        }

        void CheckGroundStatus()
        {
            RaycastHit hitInfo;
#if UNITY_EDITOR
            Debug.DrawLine(transform.position + (Vector3.up * 0.1f), transform.position + (Vector3.up * 0.1f) + (Vector3.down * GroundCheckDistance));
#endif

            if (Physics.Raycast(transform.position + (Vector3.up * 0.1f), Vector3.down, out hitInfo, GroundCheckDistance))
            {
                isGrounded = true;
                groundNormal = Vector3.up;
            }
            else
            {
                isGrounded = false;
                groundNormal = hitInfo.normal;
            }


        }
    }
}
