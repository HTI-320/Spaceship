using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public ArduinoController controller;

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
        if (controller == null)
        {
            Debug.LogError("Controller puuttuu!");
            return;
        }

        Vector2 move = controller.MoveInput;

        yawInput = move.x;          // tatti vasen/oikea
        pitchInput = -move.y;       // tatti ylös/alas (käännä merkki, jos tuntuu väärin päin)

        transform.Rotate(
            pitchInput * pitchSpeed * Time.deltaTime,
            yawInput * yawSpeed * Time.deltaTime,
            0f,
            Space.Self
        );

        float targetBank = yawInput * maxBankAngle;
        currentBank = Mathf.Lerp(currentBank, targetBank, bankSmoothness * Time.deltaTime);

        transform.position += transform.forward * moveSpeed * Time.deltaTime;
    }

    void LateUpdate()
    {
        transform.localRotation *= Quaternion.Euler(0f, 0f, currentBank);
    }
}