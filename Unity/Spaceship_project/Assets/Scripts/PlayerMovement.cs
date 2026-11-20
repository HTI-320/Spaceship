using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour

{
    public float YawInput => yawInput;
    public float PitchInput => pitchInput;

    [Header("Arduino")]
    [SerializeField] private ArduinoController arduinoController;

    [Header("Movement")]
    public float moveSpeed = 20f;

    [Header("Rotation")]
    public float yawSpeed = 80f;
    public float pitchSpeed = 60f;

    [Header("Banking")]
    public float maxBankAngle = 50f;
    public float bankSmoothness = 4f;

    private float currentBank = 0f;
    private float yawInput;
    private float pitchInput;

    void Awake()
    {
        if (arduinoController == null)
            arduinoController = FindFirstObjectByType<ArduinoController>();
    }

    void Update()
    {
        yawInput = 0f;
        pitchInput = 0f;

        if (arduinoController != null)
        {
            yawInput = arduinoController.x;
            pitchInput = arduinoController.y;
        }

        bool keyboardForward = false;
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed)
                yawInput = Mathf.Clamp(yawInput - 1f, -1f, 1f);

            if (Keyboard.current.dKey.isPressed)
                yawInput = Mathf.Clamp(yawInput + 1f, -1f, 1f);

            if (Keyboard.current.wKey.isPressed)
                pitchInput = Mathf.Clamp(pitchInput + 1f, -1f, 1f);

            if (Keyboard.current.sKey.isPressed)
                pitchInput = Mathf.Clamp(pitchInput - 1f, -1f, 1f);

            keyboardForward = Keyboard.current.spaceKey.isPressed;
        }

        // Rotate spaceship
        transform.Rotate(
            pitchInput * pitchSpeed * Time.deltaTime,
            yawInput * yawSpeed * Time.deltaTime,
            0f,
            Space.Self
        );

        // Calculate banking
        float targetBank = yawInput * maxBankAngle;

        currentBank = Mathf.Lerp(
            currentBank,
            targetBank,
            bankSmoothness * Time.deltaTime
        );

        // Move forward with the joystick button or Space
        bool moveForward = keyboardForward;
        if (arduinoController != null)
            moveForward |= arduinoController.buttonPressed;

        if (moveForward)
        {
            transform.position += transform.forward * moveSpeed * Time.deltaTime;
        }
    }

    void LateUpdate()
    {
        // Apply bank to local Z axis
        transform.localRotation *= Quaternion.Euler(
            0f,
            0f,
            currentBank
        );
    }
}