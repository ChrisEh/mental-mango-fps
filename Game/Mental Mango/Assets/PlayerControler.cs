using UnityEngine;

[RequireComponent(typeof(PlayerMotor))]
public class PlayerControler : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float lookSensitivity = 3f;

    private PlayerMotor motor;

    void Start()
    {
        motor = GetComponent<PlayerMotor>();
    }

    void Update()
    {
        // Calculate movement velocity as a 3D vector.
        float xMovement = Input.GetAxisRaw("Horizontal");
        float zMovement = Input.GetAxisRaw("Vertical");

        Vector3 moveHorizontal = transform.right * xMovement; // (1, 0 ,0)
        Vector3 moveVertical = transform.forward * zMovement; // (0, 0, 1)

        // Final movement vector.
        Vector3 velocity = (moveHorizontal + moveVertical).normalized * speed;

        // Apply movement.
        motor.Move(velocity);

        // Cal. player rotation as a 3D vector (turning around).
        float yRot = Input.GetAxisRaw("Mouse X");
        Vector3 rotation = new Vector3(0, yRot, 0f) * lookSensitivity;

        // Apply rotation.
        motor.Rotate(rotation);

        // Cal. camera rotation as a 3D vector (turning around).
        float xRot = Input.GetAxisRaw("Mouse Y");
        Vector3 cameraRotation = new Vector3(xRot, 0, 0) * lookSensitivity;

        // Apply camera rotation.
        motor.RotateCamera(cameraRotation);

    }
}
