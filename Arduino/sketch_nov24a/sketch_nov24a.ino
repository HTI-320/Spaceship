// ---------- Joystick ----------
const int moveX = A0;
const int moveY = A1;
const int fireSW = 3;

// ---------- 7-segment ----------
// Jos näyttö on common anode, vaihda arvoksi true
const bool COMMON_ANODE = false;

const int Aseg  = 51;
const int Bseg  = 47;
const int Cseg  = 44;
const int Dseg  = 42;
const int Eseg  = 41;
const int Fseg  = 50;
const int Gseg  = 45;
const int DPseg = 43;

const int DIG1 = 52;
const int DIG2 = 49;
const int DIG3 = 48;
const int DIG4 = 46;

const int segPins[8] = {Aseg, Bseg, Cseg, Dseg, Eseg, Fseg, Gseg, DPseg};
const int digPins[4] = {DIG1, DIG2, DIG3, DIG4};

// Bitit: A=0, B=1, C=2, D=3, E=4, F=5, G=6, DP=7
const byte digitPatterns[10] = {
    0x3F, // 0
    0x06, // 1
    0x5B, // 2
    0x4F, // 3
    0x66, // 4
    0x6D, // 5
    0x7D, // 6
    0x07, // 7
    0x7F, // 8
    0x6F  // 9
};

int health = 3;                 // näytettävä luku
byte displayBuffer[4] = {0, 0, 0, 0};
int currentDigit = 0;
unsigned long lastDisplayMicros = 0;
unsigned long lastSendMillis = 0;

String inputLine = "";

void setup()
{
    Serial.begin(115200);
    pinMode(fireSW, INPUT_PULLUP);

    for (int i = 0; i < 8; i++)
        pinMode(segPins[i], OUTPUT);

    for (int i = 0; i < 4; i++)
        pinMode(digPins[i], OUTPUT);

    setNumber(health);
}

void loop()
{
    readIncoming();
    refreshDisplay();

    if (millis() - lastSendMillis >= 10)
    {
        lastSendMillis = millis();
        sendJoystick();
    }
}

// ---------- Serial ----------
void sendJoystick()
{
    int mx = analogRead(moveX);
    int my = analogRead(moveY);
    int fire = digitalRead(fireSW) == LOW ? 1 : 0;

    Serial.print(mx);
    Serial.print(",");
    Serial.print(my);
    Serial.print(",");
    Serial.println(fire);
}

void readIncoming()
{
    while (Serial.available() > 0)
    {
        char c = Serial.read();

        if (c == '\n')
        {
            inputLine.trim();

            if (inputLine.length() > 1 && inputLine.charAt(0) == 'H')
            {
                health = inputLine.substring(1).toInt();
                setNumber(health);
            }

            inputLine = "";
        }
        else if (inputLine.length() < 16)
        {
            inputLine += c;
        }
    }
}

// ---------- Display ----------
// Näyttää luvun oikealle tasattuna (0-9999)
void setNumber(int value)
{
    value = constrain(value, 0, 9999);

    for (int i = 0; i < 4; i++)
        displayBuffer[i] = 0;

    int pos = 3; // oikeanpuoleisin numero
    do
    {
        displayBuffer[pos] = digitPatterns[value % 10];
        value /= 10;
        pos--;
    } while (value > 0 && pos >= 0);
}

void refreshDisplay()
{
    if (micros() - lastDisplayMicros < 2000)
        return;

    lastDisplayMicros = micros();

    // Sammuta kaikki numerot, ettei synny haamukuvaa
    for (int i = 0; i < 4; i++)
        digitalWrite(digPins[i], COMMON_ANODE ? LOW : HIGH);

    // Aseta segmentit
    byte pattern = displayBuffer[currentDigit];
    for (int s = 0; s < 8; s++)
    {
        bool on = pattern & (1 << s);
        digitalWrite(segPins[s], COMMON_ANODE ? !on : on);
    }

    // Kytke nykyinen numero päälle
    digitalWrite(digPins[currentDigit], COMMON_ANODE ? HIGH : LOW);

    currentDigit = (currentDigit + 1) % 4;
}