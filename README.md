# Universal Multiplayer Kit (UMK)

The **Universal Multiplayer Kit (UMK)** is a comprehensive networking and gameplay framework for **Unity 6** (Unity 2023.x/6.x). It provides a unified API over multiple transports (LAN, Steam, Unity Relay) and optional anti‑cheat modules, while remaining completely agnostic to your game logic. UMK goes far beyond a simple network layer: it includes modular systems for characters, environment simulation, physics replication, puzzles, AI, audio, object actions, terrain modification, UI, diagnostics and developer utilities.  

**Highlights**

* 🧠 **One API for Many Transports:** UMK supports Mirror (Auto, Telepathy, KCP), Steam SDR and Unity Relay. Switch transports with a config asset—no code changes.  
* 🎮 **Fully Modular Gameplay Systems:** Plug‑and‑play modules for first‑person characters, inventory, environment, puzzles, AI, audio, destructible objects and more. Use only what you need.  
* 🔒 **Optional Anti‑Cheat:** Integrate server‑authority validation or third‑party solutions (Unity Game Shield, GUARD, VAC, EAC, BattlEye) with a single toggle.  
* 📦 **URP/HDRP/Default Ready:** UMK is render‑pipeline‑agnostic. All components work in the Built‑in, URP and HDRP pipelines without modification.  
* 🛠️ **Developer Tools:** Setup Wizard, Diagnostics overlay, Debug console and Performance monitor streamline development and testing.  
* 🧩 **Extensible:** A simple EventBus decouples systems. Extend or replace any module without touching core code.  

UMK is designed to scale from tiny prototypes to full‑fledged co‑op or competitive games. It runs fully offline (no Mirror) and in local LAN, listen‑host or dedicated server modes.  

## Installation & Setup

1. **Import the Package** – Copy the `UMK` folder into your project’s `Assets/` directory.  
2. **Run the Setup Wizard** – In Unity, open **Tools → UMK → Setup Wizard**. Click **Create Config** to generate a `UMK_NetworkConfig` asset in your project.  
3. **Choose a Transport & Anti‑Cheat** – In the config asset, select your transport (Mirror Telepathy/KCP/Steam SDR/Unity Relay) and optional anti‑cheat module.  
4. **Add the Network Service** – In your bootstrap scene, add an empty GameObject and attach **UMK_NetworkService**. Assign the config asset if not using the default.  
5. **Build & Run** – Use the service to start host/client/server flows from code (see examples below).  

## Quick Start

Set up a listen‑server (host) or join a game in just a few lines:

```csharp
using UMK.Core;

// Start a host (server + client)
UMK_NetworkService.Instance.StartHost();

// Join a host at an IP/hostname or Unity Relay join code
UMK_NetworkService.Instance.StartClient("127.0.0.1");

// Start a dedicated server
UMK_NetworkService.Instance.StartServer();

// Toggle diagnostics overlay (FPS + ping)
UMK_NetworkService.Instance.Diagnostics.Toggle();
```

## Core Features

### Networking & Transport

UMK wraps multiple transports behind a single interface:

| Transport          | Description | Requirements |
|--------------------|-------------|-------------|
| **Mirror Auto**    | Automatically selects the best Mirror transport based on your platform. | Requires Mirror. |
| **Telepathy**      | Simple TCP transport for LAN and testing. | Requires Mirror. |
| **KCP**            | High‑performance UDP transport (recommended for Internet play). | Requires Mirror. |
| **Steam SDR**      | Uses Steam’s relay network (via FizzySteamworks or SteamSocketsTransport). | Requires Mirror and Steamworks. |
| **Unity Relay**    | Uses Unity’s first‑party relay service via UTP/NGO. | Requires `com.unity.services.relay`, `com.unity.transport`, `com.unity.netcode.gameobjects`. |

Switch transports by changing a dropdown in your `UMK_NetworkConfig`—no code changes needed.  

### Anti‑Cheat (Pick One)

UMK can integrate a single anti‑cheat provider at a time. Choose between:

* **Default Validation** – Lightweight server‑side checks (speed limits, cooldowns, teleport detection).  
* **Unity Game Shield** – Client‑side tamper detection (requires package).  
* **GUARD** – Source‑level anti‑cheat for IL2CPP builds.  
* **VAC** – Steam’s Valve Anti‑Cheat.  
* **EAC / BattlEye** – Commercial anti‑cheats for larger releases.  

Each adapter auto‑detects if its SDK is present and logs a clear warning if missing.

### Event Bus

A simple publish/subscribe system decouples your game systems. Raise events (like `DoorToggleEvent`, `FootstepEvent`) from any code and subscribe from another without references.

**Need a full list of events?**  See the [EVENTS.md](EVENTS.md) reference for every built‑in event type and its fields.

### Character Framework

UMK includes modular building blocks for first‑person characters:

* **CharacterAgent** – The orchestrator; holds references to motor, look and equipment.
* **CharacterMotor** – Handles walking, sprinting, crouching and jumping. Emits footstep events.
* **CharacterLook** – Handles mouse/controller look with pitch/yaw and optional smoothing.
* **CharacterEquipment** – Manages `IEquipable` items (e.g. flashlights, weapons) and toggling.
* **CharacterInput_Default** – Implements `IInputSource` using Unity’s Input System.
* **CharacterInventory**, **CharacterHealth**, **CharacterAI** – Optional modules for inventory, health/damage and simple waypoint AI.

Use the components you need; all derive from `NetBehaviourBase` and automatically replicate when Mirror is present. See the [Character Setup Example](#character-setup-example) below.

### Environment Simulation

UMK ships with a lightweight environment system:

* **DayNightCycle** – Syncs a normalized time across clients and updates a directional light for realistic sunrise/noon/sunset lighting.  
* **WeatherSystem** – Replicates weather states (clear, cloudy, rain, fog, storm) and toggles particle systems/audio on all clients.  
* **FirePropagator** – Allows objects to catch fire and optionally spread it to nearby objects, replicating the burn state.  

These components work offline and replicate automatically when Mirror is available.

### Physics & Damage

* **NetworkPhysicsObject** – Wraps a Rigidbody and adds a `NetworkTransform` when Mirror is installed. Disables client‑side physics when not authoritative to prevent divergence.
* **DamageSystem** – Centralizes damage application. Implement `IDamageReceiver` on any component to react to damage; call `DamageSystem.ApplyDamage()` to apply on the server.

### Puzzle & Interaction

* **LeverSwitch** – A simple on/off lever that replicates its state.  
* **KeypadPuzzle** – Replicates keypad input; solves when the code matches.  
* **PuzzleManager** – Aggregates multiple puzzles and raises an event when all are solved.  
* **IInteractable** – Implement this to make any object react to interactions. The built‑in `DoorInteractable`, `PickupItem` and `LockableInteractable` are ready‑to‑use examples.

### AI Pathfinding

* **PathfindingAgent** – Wraps a `NavMeshAgent` and replicates destination changes. Works with Mirror or offline.  
* **AIController** – Uses a `PathfindingAgent` to patrol waypoints on the server.

### Audio & Object Actions

* **AudioEmitter** – Plays one‑shot or looping clips and raises audio events for AI or remote clients.  
* **ObjectAction** – A generic action event publisher; implement `IObjectAction` for custom behaviours.  
* **DestructibleObject** – Example of a damageable object that broadcasts destruction.  

### Terrain & Modifiers

* **TerrainModifier** – Interface to deform or modify terrain (e.g. footprints, voxel edits).  
* **TerrainReplicator** – Sample implementation replicating modifications across Mirror clients.

### UI & Diagnostics

* **AdvancedScoreboard** – Displays player names, kills, deaths and ping. Detects a `PlayerStats` component or falls back to connection IDs.  
* **NetworkChat** – Basic chat system that broadcasts messages over the network or offline.  
* **DiagnosticsOverlay** – Top‑left HUD showing FPS and ping (Mirror RTT). Toggle with `F9`.  
* **PerformanceMonitor** – Toggle an FPS display with `F10`.  

### Utilities

* **NetworkDebugConsole** – In‑game console for executing commands locally or (optionally) over the network. Great for testing.  
* **ServerMoveValidator** – Example server authority check limiting movement speed, acceleration spikes and vertical travel.  

## Examples Directory

The `Examples/` folder contains small C# scripts demonstrating how to use UMK. Each script is a self‑contained example you can drop into your own project:

| Example | Description |
|--------|-------------|
| **BasicNetworkSetup.cs** | Shows how to start a host, client and server using `UMK_NetworkService`. |
| **CharacterSetupExample.cs** | Demonstrates creating a Player prefab with `CharacterAgent`, movement, look and equipment. |
| **EnvironmentExample.cs** | Shows how to set up `DayNightCycle` and `WeatherSystem` and change weather at runtime. |
| **PuzzleExample.cs** | Demonstrates a simple puzzle with a `LeverSwitch`, `KeypadPuzzle` and `PuzzleManager`. |
| **ScoreboardExample.cs** | Illustrates using `AdvancedScoreboard` to display player stats. |

Feel free to browse and modify these scripts. They’re designed to be illustrative rather than production‑ready.

## Character Setup Example

This example illustrates how to set up a basic first‑person character with movement, look and equipment:

```csharp
using UnityEngine;
using UMK.Core;

public class MyPlayerSpawner : MonoBehaviour
{
    public GameObject playerPrefab;

    void Start()
    {
        // Spawn a player on the server or offline
        if (UMK_NetworkService.Instance.IsServerOrOffline)
        {
            SpawnPlayer(Vector3.zero);
        }
    }

    void SpawnPlayer(Vector3 position)
    {
        var go = Instantiate(playerPrefab, position, Quaternion.identity);
        var agent = go.GetComponent<CharacterAgent>();
        // Optionally assign a custom input source (default is CharacterInput_Default)
        agent.inputSource = new CharacterInput_Default();
        // Equipment and look setup is handled via inspector
    }
}

// In your prefab:
//  - Add CharacterAgent, CharacterMotor, CharacterLook, CharacterEquipment, CharacterInput_Default
//  - Create a CameraRoot child with a Camera, assign it in CharacterAgent and CharacterLook
//  - Add equipable children (e.g. FlashlightEquipable with a Light) and list them in CharacterEquipment
```

## Environment Example

Set up a day/night cycle and toggle weather on key presses:

```csharp
using UnityEngine;
using UMK.Core.Environment;

public class EnvironmentController : MonoBehaviour
{
    public DayNightCycle dayNight;
    public WeatherSystem weather;

    void Update()
    {
        // Press 1–5 to switch weather states
        if (Input.GetKeyDown(KeyCode.Alpha1)) weather.SetWeather(WeatherSystem.WeatherType.Clear);
        if (Input.GetKeyDown(KeyCode.Alpha2)) weather.SetWeather(WeatherSystem.WeatherType.Cloudy);
        if (Input.GetKeyDown(KeyCode.Alpha3)) weather.SetWeather(WeatherSystem.WeatherType.Rain);
        if (Input.GetKeyDown(KeyCode.Alpha4)) weather.SetWeather(WeatherSystem.WeatherType.Fog);
        if (Input.GetKeyDown(KeyCode.Alpha5)) weather.SetWeather(WeatherSystem.WeatherType.Storm);
    }
}
```

## Puzzle Example

Create a simple puzzle that requires flipping a lever and entering a code:

```csharp
using UnityEngine;
using UMK.Core.Puzzle;

public class PuzzleSetup : MonoBehaviour
{
    public LeverSwitch lever;
    public KeypadPuzzle keypad;
    public PuzzleManager manager;

    void Start()
    {
        // Register puzzles with the manager
        manager.puzzles.Add(lever);
        manager.puzzles.Add(keypad);
    }
}

// In your scene:
//  - Place a LeverSwitch and assign its handle and rotations.
//  - Place a KeypadPuzzle and set the unlock code.
//  - Add a PuzzleManager and list the two puzzles in its inspector.
// When both are solved, the PuzzleManager logs "All puzzles solved!".
```

## Scoreboard Example

Use the advanced scoreboard to display player stats:

```csharp
using UnityEngine;
using UMK.Core.UI;

public class ScoreboardSetup : MonoBehaviour
{
    public AdvancedScoreboard scoreboard;
    public Transform rowsContainer;
    public GameObject rowPrefab;

    void Start()
    {
        scoreboard.rowsContainer = rowsContainer;
        scoreboard.rowPrefab = rowPrefab;
        scoreboard.refreshInterval = 0.5f;
    }
}

// Create a UI panel with a VerticalLayoutGroup for rowsContainer and a prefab containing two Text components. Assign them in the inspector.
```

## Troubleshooting & Tips

* **Mirror Not Detected?** – Make sure you’ve imported Mirror (Asset Store or Git URL). The UMK Setup Wizard logs a warning if Mirror is missing.  
* **URP/HDRP:** – No special settings are required. Ensure your lights and cameras are configured for your pipeline.  
* **Anti‑Cheat Errors:** – If a selected anti‑cheat module is not installed, UMK will fall back gracefully but log a clear message in the console.  
* **Running Offline:** – All systems work without Mirror. `NetBehaviourBase` automatically uses `MonoBehaviour`.  

## Contributing & License

UMK is provided under the [MIT License](LICENSE). Contributions, bug reports and feature suggestions are welcome via pull requests or issues on the repository. Please follow Unity’s coding guidelines and document your code.
