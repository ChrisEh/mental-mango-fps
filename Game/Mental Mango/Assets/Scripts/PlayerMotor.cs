using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class PlayerMotor : MonoBehaviour
{
    private Vector3 velocity = Vector3.zero;
    private Rigidbody rb;
    private Vector3 rotation = Vector3.zero;
    private float cameraRotationX = 0;
    private float currentCameraRotationX = 0f;
    private Vector3 thrusterForce = Vector3.zero;

    [SerializeField]
    private float cameraRotationLimit = 85f;

    [SerializeField]
    private Camera cam;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    // Gets a movement vector.
    public void Move(Vector3 velocity)
    {
        this.velocity = velocity;
    }

    // Gets a rotation vector.
    public void Rotate(Vector3 rotation)
    {
        this.rotation = rotation;
    }

    // Gets a camera rotation vector.
    public void RotateCamera(float cameraRotation)
    {
        this.cameraRotationX = cameraRotation;
    }

    // Get a force vec. for the thruster.
    public void ApplyThruster(Vector3 thrusterForce)
    {
        this.thrusterForce = thrusterForce;
    }

    // Run every physics iteration.
    void FixedUpdate()
    {
        PerformMovement();
        PerformRotation();
    }

    // Perform movement based on vel. variable
    void PerformMovement()
    {
        if (velocity != Vector3.zero)
            rb.MovePosition(rb.position + velocity * Time.fixedDeltaTime);

        if (thrusterForce != Vector3.zero)
            rb.AddForce(thrusterForce * Time.fixedDeltaTime, ForceMode.Acceleration);
    }

    // Perform rotation.
    void PerformRotation()
    {
        rb.MoveRotation(rb.rotation * Quaternion.Euler(rotation));
        if (cam != null)
        {
            // Set the rot. and clamp it.
            currentCameraRotationX -= cameraRotationX;
            currentCameraRotationX = Mathf.Clamp(currentCameraRotationX, -cameraRotationLimit, cameraRotationLimit);

            // Apply rot. to the transform of the cam.
            cam.transform.localEulerAngles = new Vector3(currentCameraRotationX, 0f, 0f);
        }
    }
}
