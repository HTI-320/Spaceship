#include <LiquidCrystal.h>

// LCD: RS, E, D4, D5, D6, D7
LiquidCrystal lcd(22, 23, 24, 25, 26, 27);

// Joystick pins
const int joyX = A0;
const int joyY = A1;
const int joyButton = 2;

// Timing
unsigned long lastSend = 0;
const unsigned long sendInterval = 50;

// Incoming serial command
String incoming = "";

void setup() {
  Serial.begin(115200);

  pinMode(joyButton, INPUT_PULLUP);

  lcd.begin(16, 2);
  lcd.print("Waiting for");
  lcd.setCursor(0, 1);
  lcd.print("Unity...");
}

void loop() {
  // 1. Read joystick and send to Unity
  if (millis() - lastSend >= sendInterval) {
    lastSend = millis();

    int x = analogRead(joyX);
    int y = analogRead(joyY);
    int button =
        digitalRead(joyButton) == LOW ? 1 : 0;

    Serial.print("JOY,");
    Serial.print(x);
    Serial.print(",");
    Serial.print(y);
    Serial.print(",");
    Serial.println(button);
  }

  // 2. Receive messages from Unity
  while (Serial.available() > 0) {
    char c = Serial.read();

    if (c == '\n') {
      incoming.trim();
      handleCommand(incoming);
      incoming = "";
    }
    else if (c != '\r') {
      if (incoming.length() < 80) {
        incoming += c;
      }
    }
  }
}

void handleCommand(String command) {

  // Only accept LCD commands
  if (command.startsWith("LCD:")) {

    String message = command.substring(4);

    lcd.clear();

    // First row
    lcd.setCursor(0, 0);
    lcd.print(message.substring(0, 16));

    // Second row
    if (message.length() > 16) {
      lcd.setCursor(0, 1);
      lcd.print(message.substring(16, 32));
    }
  }
}