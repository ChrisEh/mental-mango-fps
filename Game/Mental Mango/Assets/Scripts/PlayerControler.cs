using UnityEngine;
[RequireComponent(typeof(ConfigurableJoint))]
[RequireComponent(typeof(PlayerMotor))]
public class PlayerControler : MonoBehaviour
{
    [SerializeField]
    private float speed = 5f;
    [SerializeField]
    private float lookSensitivity = 3f;

    [SerializeField]
    private float thrusterForce = 1000f;

    [Header("Spring settings:")]
    [SerializeField]
    private JointDriveMode jointMode = JointDriveMode.Position;
    [SerializeField]
    private float jointSpring = 20f;
    [SerializeField]
    private float jointMaxForce = 30f;

    private PlayerMotor motor;
    private ConfigurableJoint joint;

    void Start()
    {
        motor = GetComponent<PlayerMotor>();
        joint = GetComponent<ConfigurableJoint>();

        SetJointSetting(jointSpring);
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
        float cameraRotation = xRot * lookSensitivity;

        // Apply camera rotation.
        motor.RotateCamera(cameraRotation);

        Vector3 thrusterForce = Vector3.zero;

        // Cal. thruisterforce based on player input.
        if (Input.GetButton("Jump"))
        {
            thrusterForce = Vector3.up * this.thrusterForce;
            SetJointSetting(0f);
        }
        else
        {
            SetJointSetting(jointSpring);
        }            

        // Apply the thruster force.
        motor.ApplyThruster(thrusterForce);
    }

    private void SetJointSetting(float jointSpring)
    {
        joint.yDrive = new JointDrive
        {
            mode = jointMode,
            positionSpring = jointSpring,
            maximumForce = jointMaxForce
        };
    }
}
