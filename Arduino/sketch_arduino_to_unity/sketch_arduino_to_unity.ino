#include <LiquidCrystal.h>

// LCD: RS, E, D4, D5, D6, D7
LiquidCrystal lcd(22, 23, 24, 25, 26, 27);

// ---------- Joystick 1 (left-hand) ----------
const int moveX1 = A0;
const int moveY1 = A1;
const int fireSW1 = 2;

// ---------- Joystick 2 (right-hand) ----------
const int moveX2 = A3;
const int moveY2 = A2;
const int fireSW2 = 3;

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

int health = 3;                 // näytetään LCD:llä
int score = 0;                  // näytetään 7-segmentillä
byte displayBuffer[4] = {0, 0, 0, 0};
int currentDigit = 0;
unsigned long lastDisplayMicros = 0;
unsigned long lastSendMillis = 0;

String inputLine = "";

void setup()
{
    Serial.begin(115200);
    pinMode(fireSW1, INPUT_PULLUP);
    pinMode(fireSW2, INPUT_PULLUP);

    lcd.begin(16, 2);
    lcd.print("Waiting for");
    lcd.setCursor(0, 1);
    lcd.print("Unity...");

    for (int i = 0; i < 8; i++)
        pinMode(segPins[i], OUTPUT);

    for (int i = 0; i < 4; i++)
        pinMode(digPins[i], OUTPUT);

    setNumber(score);   // 7-segment näyttää scoren
}

void loop()
{
    readIncoming();
    refreshDisplay();

    if (millis() - lastSendMillis >= 10)
    {
        lastSendMillis = millis();
        sendJoysticks();
    }
}

// ---------- Serial ----------
// Lähettää: x1,y1,fire1,x2,y2,fire2
void sendJoysticks()
{
    int mx1 = analogRead(moveX1);
    int my1 = analogRead(moveY1);
    int fire1 = digitalRead(fireSW1) == LOW ? 1 : 0;

    int mx2 = analogRead(moveX2);
    int my2 = analogRead(moveY2);
    int fire2 = digitalRead(fireSW2) == LOW ? 1 : 0;

    Serial.print(mx1);
    Serial.print(",");
    Serial.print(my1);
    Serial.print(",");
    Serial.print(fire1);
    Serial.print(",");
    Serial.print(mx2);
    Serial.print(",");
    Serial.print(my2);
    Serial.print(",");
    Serial.println(fire2);
}

// Vastaanottaa rivit: H<luku> (health) ja S<luku> (score)
void readIncoming()
{
    while (Serial.available() > 0)
    {
        char c = Serial.read();

        if (c == '\n')
        {
            inputLine.trim();

            if (inputLine.length() > 1)
            {
                char cmd = inputLine.charAt(0);
                int value = inputLine.substring(1).toInt();

                if (cmd == 'H')
                {
                    health = value;
                    showHealth();      // LCD
                }
                else if (cmd == 'S')
                {
                    score = value;
                    setNumber(score);  // 7-segment
                }
            }

            inputLine = "";
        }
        else if (inputLine.length() < 16)
        {
            inputLine += c;
        }
    }
}

// ---------- LCD ----------
void showHealth()
{
    lcd.clear();
    lcd.print("Health: ");
    lcd.print(health);

    // Palkki toiselle riville (max 16 lohkoa)
    lcd.setCursor(0, 1);
    for (int i = 0; i < constrain(health, 0, 16); i++)
        lcd.write((byte)255);   // täysi lohko
}

// ---------- 7-segment ----------
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