# Spaceship

A game project combining **Unity** and **Arduino**. The Unity application communicates with Arduino-based hardware to create an interactive spaceship experience.

## Team

- Jani Suvela
- Lauri Vahosalmi
- Markus Aaltio
- Timo Pihlava

## Requirements

### Unity

Install the Unity version 6000.3.28f1

Open the project using Unity Hub.

### Arduino

Install:

- Arduino IDE 2.3.11
- Required board packages (to be documented)
- Required libraries (to be documented)

## Getting Started

### Clone the Repository

```bash
git clone https://github.com/HTI-320/Spaceship.git
cd Spaceship
```

### Open Unity Project

1. Open Unity Hub.
2. Click **Add → Add project from disk**.
3. Select the `Unity` folder.
4. Open the project.

### Upload Arduino Firmware

1. Open the `.ino` file from the `Arduino/firmware` folder.
2. Select the correct board and COM port.
3. Upload the firmware to the Arduino device.

## Development Workflow

### Branch Strategy

- `main` contains the final, stable release version of the project.
- `development` contains the latest tested and working development code.
- New features are developed in separate feature branches created from `development`.
- Feature branches are merged back into `development` after testing.
- At project completion, `development` will be merged into `main`.

### Start Working

Switch to the development branch and pull the latest changes:

```bash
git checkout development
git pull origin development
```

### Create a Feature Branch

Create a new branch from `development`:

```bash
git checkout development
git pull origin development
git checkout -b feature/spaceship-controls
```

Examples:

```bash
feature/player-movement
feature/arduino-communication
feature/game-ui
feature/sound-effects
```

### Commit Your Changes

```bash
git add .
git commit -m "Implement player movement"
```

### Push Your Branch

```bash
git push -u origin feature/player-movement
```

### Merge Process

1. Push your feature branch.
2. Open a Pull Request into `development`.
3. Test the changes.
4. Merge into `development` when approved.

### Updating Your Branch

Before continuing work, get the latest changes from `development`:

```bash
git checkout development
git pull origin development

git checkout feature/player-movement
git merge development
```

## Git Guidelines

Do not commit:

- Library/
- Temp/
- Logs/
- UserSettings/
- Build outputs

These are ignored by `.gitignore`.

## Future Features

- [ ] Spaceship movement
- [ ] Arduino controls
- [ ] Serial communication between Unity and Arduino
- [ ] Game UI
- [ ] Sound effects
- [ ] Score system

## License

This project is intended for educational use.
