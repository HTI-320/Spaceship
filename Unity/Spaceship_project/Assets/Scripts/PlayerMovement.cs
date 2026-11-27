using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerMovement : MonoBehaviour
{
    public float YawInput => yawInput;
    public float PitchInput => pitchInput;

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

    void Update()
    {
        yawInput = 0f;
        pitchInput = 0f;

        // Left joystick
        if (ArduinoInput.Instance != null)
        {
            Vector2 stick = ArduinoInput.Instance.Move1;
            yawInput = stick.x;
            pitchInput = stick.y;   // stick up = -1 = nose up (same as W). Use invertY1 if reversed.
        }

        // Keyboard fallback (overrides the joystick while a key is held)
        if (Keyboard.current != null)
        {
            if (Keyboard.current.aKey.isPressed)
                yawInput = -1f;

            if (Keyboard.current.dKey.isPressed)
                yawInput = 1f;

            if (Keyboard.current.wKey.isPressed)
                pitchInput = -1f;

            if (Keyboard.current.sKey.isPressed)
                pitchInput = 1f;
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

        // Always move forward
        transform.position += transform.forward * moveSpeed * Time.deltaTime;
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