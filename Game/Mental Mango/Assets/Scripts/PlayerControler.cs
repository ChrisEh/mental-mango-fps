using UnityEngine;
[RequireComponent(typeof(Animator))]
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

    [SerializeField]
    private float thrusterFuelBurnSpeed = 1f;

    [SerializeField]
    private float thrusterFuelRegenSpeed = 0.3f;

    private float thrusterFuelAmount = 1;

    public float GetThrusterFuelAmount()
    {
        return thrusterFuelAmount;
    }

    [SerializeField]
    private LayerMask environmentMask;

    [Header("Spring settings:")]
    [SerializeField]
    private float jointSpring = 20f;
    [SerializeField]
    private float jointMaxForce = 30f;

    // Component caching
    private PlayerMotor motor;
    private ConfigurableJoint joint;
    private Animator animator;

    void Start()
    {
        motor = GetComponent<PlayerMotor>();
        joint = GetComponent<ConfigurableJoint>();
        animator = GetComponent<Animator>();

        SetJointSetting(jointSpring);
    }

    void Update()
    {
        // Set the target pos. for spring that makes thge physics act right when flying.
        RaycastHit hit;
        if (Physics.Raycast(transform.position, Vector3.down, out hit, 100f, environmentMask))
        {
            joint.targetPosition = new Vector3(0, -hit.point.y, 0);
        }
        else
        {
            joint.targetPosition = new Vector3(0, 0, 0);
        }

        // Calculate movement velocity as a 3D vector.
        float xMovement = Input.GetAxis("Horizontal");
        float zMovement = Input.GetAxis("Vertical");

        Vector3 moveHorizontal = transform.right * xMovement; // (1, 0 ,0)
        Vector3 moveVertical = transform.forward * zMovement; // (0, 0, 1)

        // Final movement vector.
        Vector3 velocity = (moveHorizontal + moveVertical) * speed;

        // Animate movement
        animator.SetFloat("ForwardVelocity", zMovement);

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
        if (Input.GetButton("Jump") && thrusterFuelAmount > 0)
        {
            thrusterFuelAmount -= thrusterFuelBurnSpeed * Time.deltaTime;

            if (thrusterFuelAmount >= 0.01)
            {
                thrusterForce = Vector3.up * this.thrusterForce;
                SetJointSetting(0f);
            }
        }
        else
        {
            thrusterFuelAmount += thrusterFuelRegenSpeed * Time.deltaTime;
            SetJointSetting(jointSpring);
        }

        thrusterFuelAmount = Mathf.Clamp(thrusterFuelAmount, 0, 1);

        // Apply the thruster force.
        motor.ApplyThruster(thrusterForce);
    }

    private void SetJointSetting(float jointSpring)
    {
        joint.yDrive = new JointDrive
        {
            positionSpring = jointSpring,
            maximumForce = jointMaxForce
        };
    }
}
