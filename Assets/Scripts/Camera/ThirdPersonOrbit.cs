using UnityEngine;

/// <summary>
/// This is a fairly simple camera script that makes the camera orbit around a Target
/// </summary>
public class ThirdPersonOrbit : MonoBehaviour
{
    [Tooltip("The Target the camera will orbit around")]
    public Transform Target;
    
    [Tooltip("Camera speed horizontally")]
    public float HorizontalSensitivity = 40.0f;
    [Tooltip("Camera speed vertically")]
    public float VerticalSensitivity = 40.0f;
    [Tooltip("Camera speed horizontally (for gamepad)")]
    public float HorizontalSensitivityGamepad = 40.0f;
    [Tooltip("Camera speed vertically (for gamepad)")]
    public float VerticalSensitivityGamepad = 40.0f;

    [Tooltip("The minimum angle the camera can move vertically, measured in degrees")]
    public float ClampCameraMin = -15f;
    [Tooltip("The maximum angle the camera can move vertically, measured in degrees")]
    public float ClampCameraMax = 40f;
    
    [Tooltip("The preferred distance of the camera, relative to the Target, in Unity units")]
    public float PreferredDistance = 10.0f;
    
    [Tooltip("The speed which the Camera zooms in if an object is obstructing it")]
    public float CameraAccelerationIn = 5.0f;
    [Tooltip("The speed which the Camera zooms out if there is a clear path to do so")]
    public float CameraAccelerationOut = 2.0f;
    
    [Tooltip("What should the Camera collision detecting check against")]
    public LayerMask LayerMask;
    
    public bool InvertX;
    public bool InvertY;

    private bool isUsingController;
    private float x;
    private float y;
    private float actualDistance;

    [HideInInspector]
    public float Margin = 0.3f;
    [HideInInspector]
    public float OffsetY = 0.1f;
    
    [Tooltip("Time in seconds before the camera starts auto following, after movement")]
    public float TimeBeforeFollow = 1.5f;

    private bool isMoving;
    private Vector3 prevPosition;
    private Quaternion prevRotation;
    private float followTimer;
    private float angleError = 0.01f;

    void Start()
    {
        // Getting the angles to start from
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
        actualDistance = PreferredDistance;

        // this should probably be moved to a seperate helper class that allows us to determine what kind of controller we are using, if any
        // right now we're only checking if there is ANY controller plugged in and assuming it is an Xbox 360 controller
        // it would be better to split this into possibly three categories: PS4, Xbox 360, No Controller
        isUsingController = Input.GetJoystickNames().Length != 0;

        // might chang this to transform instead of Target
        prevRotation = Target.rotation;
        prevPosition = Target.position;
        isMoving = false;
        followTimer = TimeBeforeFollow;
    }

    void Update()
    {
        /**
         * What we want to do in this block is check if the player has moved or rotated, if that's the case then we do nothing
         * If the player is standing still we want to start a timer and then lerp the camera behind the player again
         * One way of doing this could be to check for controller input
         */
        if (Input.GetAxisRaw("Right Horizontal") >= 0.1f)
        {
            followTimer = TimeBeforeFollow;
            isMoving = true;
        }
        else if (Input.GetAxis("Right Vertical") >= 0.1f)
        {
            followTimer = TimeBeforeFollow;
            isMoving = true;
        }
        else if (Input.GetAxisRaw("Mouse X") >= 0.1f)
        {
            followTimer = TimeBeforeFollow;
            isMoving = true;
        }
        else if (Input.GetAxisRaw("Mouse Y") >= 0.1f)
        {
            followTimer = TimeBeforeFollow;
            isMoving = true;
        }
        else if (Input.GetAxisRaw("Horizontal") >= 0.1f)
        {
            followTimer = TimeBeforeFollow;
            isMoving = true;
        }
        else if (Input.GetAxisRaw("Vertical") >= 0.1f)
        {
            followTimer = TimeBeforeFollow;
            isMoving = true;
        }
        else
        {
            followTimer -= Time.deltaTime;
            if (followTimer <= Mathf.Epsilon)
                isMoving = false;
        }
    }

    void LateUpdate()
    {
        if (!Target) return;

        if (isUsingController)
        {
            x += Input.GetAxis("Right Horizontal") * HorizontalSensitivityGamepad * Time.deltaTime * (InvertX ? 1 : -1);
            y -= Input.GetAxis("Right Vertical") * VerticalSensitivityGamepad * Time.deltaTime * (InvertY ? 1 : -1);
        }
        else
        {
            x += Input.GetAxisRaw("Mouse X") * HorizontalSensitivity * Time.deltaTime * (InvertX ? 1 : -1);
            y -= Input.GetAxisRaw("Mouse Y") * VerticalSensitivity * Time.deltaTime * (InvertY ? 1 : -1);
        }

        // we need to use eulerAngler to get the values in degrees
        // we're gonna do this after X amount of time with no input, so we need to check how much time has passed since no input
        if (!isMoving)
        {
            var rot = Mathf.Lerp(x, Target.eulerAngles.y, Time.deltaTime * 2);
            x = rot;
        }

        y = ClampAngle(y, ClampCameraMin, ClampCameraMax);
        Quaternion rotation = Quaternion.Euler(y, x, 0); // this might seem confusing but that's because yaw and pitch, X plane corresponds to moving UP/DOWN and Y lane corresponds to LEFT/RIGHT

        var rayLength = (Target.position - transform.position).magnitude;

        RaycastHit rayHit;
        var hit = Physics.Raycast(Target.position, -transform.forward, out rayHit, rayLength, LayerMask);
        
        if (hit)
        {
            Debug.DrawLine(Target.position, transform.position, Color.magenta);
            
            float steps = rayHit.distance * CameraAccelerationIn * Time.deltaTime;
            actualDistance -= steps;

            Vector3 negDistance = new Vector3(0.0f, OffsetY, -actualDistance);
            Vector3 position = rotation * negDistance + Target.position;

            transform.rotation = rotation;
            transform.position = position;
        }
        else
        {
            Debug.DrawLine(Target.position, transform.position, Color.green);

            // when zooming out we're gonna check a little further ahead than the total distance, to see if we can actually zoom out or not
            var cantZoomOut = Physics.Raycast(Target.position, -transform.forward, out rayHit, rayLength + Margin, LayerMask);
            
            if (actualDistance < PreferredDistance && !cantZoomOut)
            {
                actualDistance += actualDistance * CameraAccelerationOut * Time.deltaTime;
                // here we should use a lerp or something
            }

            Vector3 negDistance = new Vector3(0.0f, OffsetY, -actualDistance);
            Vector3 position = rotation * negDistance + Target.position;
            
            transform.rotation = rotation;
            transform.position = position;
        }
        prevRotation = Target.rotation;
        prevPosition = Target.position;
    }


    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }


    /// <summary>
    /// http://answers.unity3d.com/questions/131624/vector3-comparison.html
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private bool CompareVector3(Vector3 a, Vector3 b)
    {
        //if they aren't the same length, don't bother checking the rest.
        if (!Mathf.Approximately(a.magnitude, b.magnitude))
            return false;

        var cosAngleError = Mathf.Cos(angleError * Mathf.Deg2Rad);
        //A value between -1 and 1 corresponding to the angle.
        var cosAngle = Vector3.Dot(a.normalized, b.normalized);
        //The dot product of normalized Vectors is equal to the cosine of the angle between them.
        //So the closer they are, the closer the value will be to 1.  Opposite Vectors will be -1
        //and orthogonal Vectors will be 0.

        if (cosAngle >= cosAngleError)
        {
            //If angle is greater, that means that the angle between the two vectors is less than the error allowed.
            return true;
        }

        return false;
    }

    /// <summary>
    /// http://answers.unity3d.com/questions/288338/how-do-i-compare-quaternions.html
    /// </summary>
    /// <param name="a"></param>
    /// <param name="b"></param>
    /// <returns></returns>
    private bool CompareQuaternion(Quaternion a, Quaternion b)
    {
        // if angle between the two rotations are less or equal to epsilon value then it's true (might need to reverse, unsure)
        return Quaternion.Angle(a, b) <= Quaternion.kEpsilon;
    }
}