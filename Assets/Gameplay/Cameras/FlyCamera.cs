using UnityEngine;

public class FlyCamera : MonoBehaviour
{
    [Header("Movement")]
    [Range(1000.0f, 5000.0f)]
    public float mouseSensitivity = 2000.0f;
    [Min(0.1f)]
    public float climbSpeedFactor = 1.0f;
    [Min(0.1f)]
    public float normalMoveSpeed = 10.0f;
    [Min(0.1f)]
    public float slowMoveFactor = 0.25f;
    [Min(0.1f)]
    public float fastMoveFactor = 3.0f;

    [Header("Key bindings")]
    public KeyCode fastMovementKey = KeyCode.LeftShift;
    public KeyCode slowMovementKey = KeyCode.LeftControl;
    public KeyCode lockMouseKey = KeyCode.End;

    private float rotationX = 0.0f;
    private float rotationY = 0.0f;

    void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        rotationX += Input.GetAxis("Mouse X") * mouseSensitivity * Time.deltaTime;
        rotationY += Input.GetAxis("Mouse Y") * mouseSensitivity * Time.deltaTime;
        rotationY = Mathf.Clamp(rotationY, -90, 90);

        transform.localRotation = Quaternion.AngleAxis(rotationX, Vector3.up);
        transform.localRotation *= Quaternion.AngleAxis(rotationY, Vector3.left);

        var moveSpeed = normalMoveSpeed;
        if (Input.GetKey(fastMovementKey))
        {
            moveSpeed *= fastMoveFactor;
        }
        else if (Input.GetKey(slowMovementKey))
        {
            moveSpeed *= slowMoveFactor;
        }

        var climbInput = Input.GetKey(KeyCode.Q) ? 1.0f : Input.GetKey(KeyCode.E) ? -1.0f : 0.0f;

        transform.position += transform.forward * moveSpeed * Input.GetAxis("Vertical") * Time.deltaTime;
        transform.position += transform.right * moveSpeed * Input.GetAxis("Horizontal") * Time.deltaTime;
        transform.position += transform.up * (moveSpeed * climbSpeedFactor) * climbInput * Time.deltaTime;

        if (Input.GetKeyDown(lockMouseKey))
        {
            Cursor.lockState = (Cursor.lockState == CursorLockMode.None) ? CursorLockMode.Locked : CursorLockMode.None;
        }
    }
}