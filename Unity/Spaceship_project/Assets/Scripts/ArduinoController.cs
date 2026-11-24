using UnityEngine;
using System;
using System.IO.Ports;
using System.Threading;
using System.Globalization;

public class ArduinoController : MonoBehaviour
{
    [Header("Arduino connection")]
    public string portName = "COM3";
    public int baudRate = 115200;

    [Header("Joystick settings")]
    public float deadzone = 0.15f;

    private SerialPort serial;
    private Thread readThread;
    private volatile bool running = false;

    private readonly object dataLock = new object();

    private Vector2 moveInput = Vector2.zero;
    private bool fireInput = false;

    public Vector2 MoveInput
    {
        get { lock (dataLock) return moveInput; }
    }

    public bool FireInput
    {
        get { lock (dataLock) return fireInput; }
    }

    void Start()
    {
        try
        {
            serial = new SerialPort(portName, baudRate);
            serial.ReadTimeout = 100;
            serial.NewLine = "\n";
            serial.Open();

            running = true;

            readThread = new Thread(ReadSerial);
            readThread.IsBackground = true;
            readThread.Start();

            Debug.Log("Arduino connected: " + portName);
        }
        catch (Exception e)
        {
            Debug.LogError("Arduino connection error: " + e.Message);
        }
    }

    void ReadSerial()
    {
        while (running)
        {
            try
            {
                string line = serial.ReadLine().Trim();
                string[] values = line.Split(',');

                if (values.Length != 3)
                    continue;

                float mx = float.Parse(values[0], CultureInfo.InvariantCulture);
                float my = float.Parse(values[1], CultureInfo.InvariantCulture);
                bool fire = values[2] == "1";

                Vector2 newMove = new Vector2(Normalize(mx), -Normalize(my));

                lock (dataLock)
                {
                    moveInput = newMove;
                    fireInput = fire;
                }
            }
            catch (TimeoutException) { }
            catch (Exception e)
            {
                if (running)
                    Debug.LogWarning("Serial read error: " + e.Message);
            }
        }
    }
    public void SendHealth(int health)
    {
    if (serial == null || !serial.IsOpen)
        return;

    try
    {
        serial.Write("H" + health + "\n");
    }
    catch (Exception e)
    {
        Debug.LogWarning("Serial write error: " + e.Message);
    }
    }

    float Normalize(float value)
    {
        float result = (value - 512f) / 512f;
        result = Mathf.Clamp(result, -1f, 1f);

        if (Mathf.Abs(result) < deadzone)
            return 0f;

        return Mathf.Sign(result) * (Mathf.Abs(result) - deadzone) / (1f - deadzone);
    }

    void OnApplicationQuit() { CloseConnection(); }
    void OnDestroy() { CloseConnection(); }

    void CloseConnection()
    {
        running = false;

        if (serial != null)
        {
            try
            {
                if (serial.IsOpen)
                    serial.Close();
            }
            catch { }
        }

        if (readThread != null && readThread.IsAlive)
            readThread.Join(300);
    }
}