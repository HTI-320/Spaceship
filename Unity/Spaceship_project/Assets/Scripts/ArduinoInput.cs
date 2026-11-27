using System;
using System.Collections;
using System.IO.Ports;
using System.Threading;
using UnityEngine;

public class ArduinoInput : MonoBehaviour
{
    public static ArduinoInput Instance { get; private set; }

    [Header("Serial")]
    public string portName = "COM3";   // check in Arduino IDE -> Tools -> Port
    public int baudRate = 115200;

    [Header("Joystick")]
    [Range(0f, 0.5f)] public float deadzone = 0.12f;
    public bool invertX1 = false;
    public bool invertY1 = false;
    public bool invertX2 = false;
    public bool invertY2 = false;

    // ---- Values to read from other scripts (-1..1) ----
    public Vector2 Move1 { get; private set; }   // left-hand joystick
    public Vector2 Move2 { get; private set; }   // right-hand joystick
    public bool Fire1 { get; private set; }      // held
    public bool Fire2 { get; private set; }
    public bool Fire1Down { get; private set; }  // true only on the frame it was pressed
    public bool Fire2Down { get; private set; }
    public bool IsConnected => port != null && port.IsOpen;

    SerialPort port;
    Thread readThread;
    volatile bool running;

    readonly object lockObj = new object();
    int[] raw = new int[6] { 512, 512, 0, 512, 512, 0 };

    bool prevFire1, prevFire2;
    int lastHealth = 3, lastScore = 0;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (Instance != this) return;

        try
        {
            port = new SerialPort(portName, baudRate);
            port.ReadTimeout = 100;
            port.NewLine = "\n";
            port.DtrEnable = true;
            port.Open();

            running = true;
            readThread = new Thread(ReadLoop) { IsBackground = true };
            readThread.Start();

            StartCoroutine(SendInitialValues());
        }
        catch (Exception e)
        {
            Debug.LogError("Could not open " + portName + ": " + e.Message);
        }
    }

    // The Arduino resets when the port opens, so wait before sending
    IEnumerator SendInitialValues()
    {
        yield return new WaitForSeconds(2f);
        SendHealth(lastHealth, true);
        SendScore(lastScore, true);
    }

    void ReadLoop()
    {
        while (running)
        {
            try
            {
                string line = port.ReadLine().Trim();
                string[] parts = line.Split(',');
                if (parts.Length != 6) continue;

                int[] values = new int[6];
                bool ok = true;
                for (int i = 0; i < 6; i++)
                    if (!int.TryParse(parts[i], out values[i])) { ok = false; break; }

                if (ok)
                    lock (lockObj) raw = values;
            }
            catch (TimeoutException) { }
            catch (Exception) { if (!running) break; }
        }
    }

    void Update()
    {
        if (Instance != this) return;

        int[] r;
        lock (lockObj) r = (int[])raw.Clone();

        Move1 = new Vector2(
            Normalize(r[0]) * (invertX1 ? -1 : 1),
            Normalize(r[1]) * (invertY1 ? -1 : 1));
        Move2 = new Vector2(
            Normalize(r[3]) * (invertX2 ? -1 : 1),
            Normalize(r[4]) * (invertY2 ? -1 : 1));

        Fire1 = r[2] == 1;
        Fire2 = r[5] == 1;
        Fire1Down = Fire1 && !prevFire1;
        Fire2Down = Fire2 && !prevFire2;
        prevFire1 = Fire1;
        prevFire2 = Fire2;
    }

    float Normalize(int value)
    {
        float v = (value - 512) / 512f;
        if (Mathf.Abs(v) < deadzone) return 0f;
        return Mathf.Clamp(v, -1f, 1f);
    }

    // ---- Call these from your game scripts ----
    public void SendHealth(int health, bool force = false)
    {
        if (!force && health == lastHealth) return;
        lastHealth = health;
        Write("H" + health);
    }

    public void SendScore(int score, bool force = false)
    {
        if (!force && score == lastScore) return;
        lastScore = score;
        Write("S" + score);
    }

    void Write(string text)
    {
        if (!IsConnected) return;
        try { port.WriteLine(text); }
        catch (Exception e) { Debug.LogWarning("Serial write failed: " + e.Message); }
    }

    void OnDestroy()
    {
        if (Instance == this) Close();
    }

    void OnApplicationQuit()
    {
        if (Instance == this) Close();
    }

    void Close()
    {
        running = false;
        if (readThread != null && readThread.IsAlive) readThread.Join(300);
        if (port != null && port.IsOpen) port.Close();
    }
}