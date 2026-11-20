using System;
using System.Globalization;
using System.IO.Ports;
using UnityEngine;

public class ArduinoController : MonoBehaviour
{
    [Header("Serial Connection")]
    public string portName = "COM3";
    public int baudRate = 115200;

    [Header("Joystick Values")]
    [Range(-1f, 1f)] public float x;
    [Range(-1f, 1f)] public float y;
    public bool buttonPressed;

    private SerialPort serial;
    private string receiveBuffer = "";
    private float connectedAt;
    private bool initialMessageSent;

    void Start()
    {
        try
        {
            serial = new SerialPort(portName, baudRate);
            serial.ReadTimeout = 10;
            serial.WriteTimeout = 100;
            serial.Open();

            connectedAt = Time.realtimeSinceStartup;

            Debug.Log("Arduino connected!");
        }
        catch (Exception e)
        {
            Debug.LogError(
                "Arduino connection failed: " + e.Message
            );
        }
    }

    void Update()
    {
        if (serial == null || !serial.IsOpen)
            return;

        try
        {
            // Receive any available Arduino data
            string data = serial.ReadExisting();
            receiveBuffer += data;

            // Process complete lines
            int newline;
            while ((newline =
                    receiveBuffer.IndexOf('\n')) >= 0)
            {
                string line = receiveBuffer
                    .Substring(0, newline)
                    .Trim();

                receiveBuffer =
                    receiveBuffer.Substring(newline + 1);

                ProcessArduinoData(line);
            }

            // Prevent unlimited buffer growth
            if (receiveBuffer.Length > 1024)
                receiveBuffer = "";

            // Wait for Arduino to finish resetting
            if (!initialMessageSent &&
                Time.realtimeSinceStartup - connectedAt > 2f)
            {
                SendToLCD("Unity connected!");
                initialMessageSent = true;
            }
        }
        catch (Exception e)
        {
            Debug.LogWarning(
                "Serial communication error: " + e.Message
            );
        }
    }

    void ProcessArduinoData(string line)
    {
        string[] values = line.Split(',');

        if (values.Length != 4 ||
            values[0] != "JOY")
            return;

        if (!int.TryParse(values[1], out int rawX) ||
            !int.TryParse(values[2], out int rawY))
            return;

        // Convert 0-1023 to approximately -1 to +1
        x = Mathf.Clamp(
            (rawX - 512f) / 511f, -1f, 1f
        );

        y = Mathf.Clamp(
            (rawY - 512f) / 511f, -1f, 1f
        );

        // Small dead zone to prevent drifting
        if (Mathf.Abs(x) < 0.08f) x = 0;
        if (Mathf.Abs(y) < 0.08f) y = 0;

        buttonPressed = values[3] == "1";
    }

    public void SendToLCD(string message)
    {
        if (serial == null || !serial.IsOpen)
            return;

        try
        {
            // Keep a maximum of 32 characters
            message = message
                .Replace("\r", " ")
                .Replace("\n", " ");

            if (message.Length > 32)
                message = message.Substring(0, 32);

            serial.WriteLine("LCD:" + message);
        }
        catch (Exception e)
        {
            Debug.LogWarning(
                "LCD send failed: " + e.Message
            );
        }
    }

    void OnDestroy()
    {
        if (serial != null)
        {
            if (serial.IsOpen)
                serial.Close();

            serial.Dispose();
        }
    }
}