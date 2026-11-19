
using UnityEngine;

public class SpaceshipBanking : MonoBehaviour
{
    public PlayerMovement movement;

    [Header("Banking")]
    public float maxBankAngle = 50f;
    public float maxPitchAngle = 20f;
    public float bankSmoothness = 4f;

    private float currentBank;
    private float currentPitch;

    void Update()
    {
        if (movement == null)
            return;

        // Sideways banking (A / D)
        float targetBank = -movement.YawInput * maxBankAngle;

        // Up and down banking (W / S)
        float targetPitch = movement.PitchInput * maxPitchAngle;

        // Smooth rotation
        currentBank = Mathf.Lerp(
            currentBank,
            targetBank,
            bankSmoothness * Time.deltaTime
        );

        currentPitch = Mathf.Lerp(
            currentPitch,
            targetPitch,
            bankSmoothness * Time.deltaTime
        );

        // Apply visual rotation
        transform.localRotation = Quaternion.Euler(
            currentPitch,
            0f,
            currentBank
        );
    }
}
