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

    private Vector3 prevPositon;
    private Quaternion prevRotation;
    [Tooltip("Time in seconds before the camera starts auto following, after movement")]
    public float TimeBeforeFollow = 1.5f;

    void Start()
    {
        // Getting the angles to start from
        Vector3 angles = transform.eulerAngles;
        x = angles.y;
        y = angles.x;
        actualDistance = PreferredDistance;

        prevPositon = Target.position;
        prevRotation = Target.rotation;

        // this should probably be moved to a seperate helper class that allows us to determine what kind of controller we are using, if any
        // right now we're only checking if there is ANY controller plugged in and assuming it is an Xbox 360 controller
        // it would be better to split this into possibly three categories: PS4, Xbox 360, No Controller
        isUsingController = Input.GetJoystickNames().Length != 0;
    }

    void LateUpdate()
    {
        if (!Target) return;

        if (isUsingController)
        {
            x += Input.GetAxis("Right Horizontal") * HorizontalSensitivity * Time.deltaTime * (InvertX ? 1 : -1);
            y -= Input.GetAxis("Right Vertical") * VerticalSensitivity * Time.deltaTime * (InvertY ? 1 : -1);
        }
        else
        {
            x += Input.GetAxisRaw("Mouse X") * HorizontalSensitivity * Time.deltaTime * (InvertX ? 1 : -1);
            y -= Input.GetAxisRaw("Mouse Y") * VerticalSensitivity * Time.deltaTime * (InvertY ? 1 : -1);
        }

        y = ClampAngle(y, ClampCameraMin, ClampCameraMax);
        Quaternion rotation = Quaternion.Euler(y, x, 0);

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
            Quaternion rotation2 = Target.rotation;
            Vector3 position2 = Target.position;
            bool follow = false;
            // these checks wont work, we need to use a lambda since it's basically float comparison
            if (prevPositon != Target.position && prevRotation != Target.rotation)
            {
                Debug.Log("Should follow");
                rotation2 = Quaternion.Slerp(prevRotation, Target.rotation, 0.5f);
                Vector3 negDistance2 = new Vector3(0.0f, OffsetY, -actualDistance);
                position2 = rotation2 * negDistance2 + Target.position;
                follow = true;
            }

            Debug.DrawLine(Target.position, transform.position, Color.green);

            // when zooming out we're gonna check a little further ahead than the total distance, to see if we can actually zoom out or not
            var cantZoomOut = Physics.Raycast(Target.position, -transform.forward, out rayHit, rayLength + Margin, LayerMask);
            
            if (actualDistance < PreferredDistance && !cantZoomOut)
            {
                actualDistance += actualDistance * CameraAccelerationOut * Time.deltaTime;
            }

            Vector3 negDistance = new Vector3(0.0f, OffsetY, -actualDistance);
            Vector3 position = rotation * negDistance + Target.position;
            
            //transform.rotation = follow ? rotation2 : rotation;
            //transform.position = follow ? position2 : position;

            transform.rotation = rotation;
            transform.position = position;
        }
    }

    public static float ClampAngle(float angle, float min, float max)
    {
        if (angle < -360F)
            angle += 360F;
        if (angle > 360F)
            angle -= 360F;
        return Mathf.Clamp(angle, min, max);
    }
}