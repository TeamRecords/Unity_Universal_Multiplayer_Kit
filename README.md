
# Universal Multiplayer Kit (UMK)

UMK is a standalone, drop‑in networking facade for **Unity 6** that enables Host/Client/Dedicated flows for **Mirror** transports and **Unity Relay**, with optional anti‑cheat integration and diagnostics. It’s system‑agnostic: Characters, environment, objects, audio, actions, terrain and more can all call UMK without depending on any specific netcode package. Beyond networking, UMK now includes a lightweight **EventBus** and a set of extensible gameplay frameworks to help you build characters, interactions, audio emitters, object actions and terrain modifiers.

## Quick Start
1. Import `Assets/UMK` into your project.
2. Tools → **UMK → Setup Wizard** → **Create Config** → pick **Transport** (Telepathy/KCP/**Steam (SDR)**/**Unity Relay**) and optional **Anti-Cheat**.
3. Add an empty GO in your startup scene → **UMK_NetworkService**.
4. Use anywhere:
```csharp
using UMK.Core;
UMK_NetworkService.Instance.StartHost();
UMK_NetworkService.Instance.StartClient("127.0.0.1"); // or Relay join code
UMK_NetworkService.Instance.StartServer();
UMK_NetworkService.Instance.Diagnostics.Toggle();
```

### Expanding your game with UMK

UMK is more than just a transport layer. It includes a set of optional helpers to accelerate common gameplay patterns and environmental simulation:

* **EventBus** – a simple publish/subscribe system that lets any part of your game send messages without tight coupling. You can subscribe to events like `FootstepEvent`, `DoorToggleEvent`, `PickupEvent`, `AudioEvent`, `ObjectActionEvent`, and `TerrainModifyEvent`.
* **NetBehaviourBase** – a base class that becomes a `NetworkBehaviour` when Mirror is present and falls back to `MonoBehaviour` otherwise. Use this for any component that needs network awareness.
* **Character framework** – modular components that implement first‑person movement (`CharacterMotor`), camera look (`CharacterLook`), equipment management (`CharacterEquipment`), a default input source (`CharacterInput_Default`), and a high‑level orchestrator (`CharacterAgent`). A sample `FlashlightEquipable` shows how to add equipable items.
* **Interaction system** – implement `IInteractable` on any object to handle interactions. Sample implementations include `DoorInteractable` (toggling a door and publishing events) and `PickupItem` (notifying a pickup and optionally destroying itself).
* **Audio system** – `IAudioEmitter` and the `AudioEmitter` component allow you to play one‑shots and loops and publish audio events for AI or remote clients.
* **Object actions** – implement `IObjectAction` to describe custom actions. The included `ObjectAction` component publishes an `ObjectActionEvent` whenever it’s invoked.
* **Terrain modifiers** – implement `ITerrainModifier` to deform terrain or spawn effects. The included `TerrainModifier` publishes a `TerrainModifyEvent` with position and intensity.

These systems are completely optional – you can use as much or as little as you need. They do not depend on Mirror unless you derive from `NetBehaviourBase` or add your own network replication. Everything still works in a pure offline/local context.

### Expanded Systems (UMK 2025 Update)

UMK now ships with many additional gameplay modules to support more advanced and varied projects:

* **Environment** – a `DayNightCycle` component replicates the time of day across the network and drives a sun light for realistic day/night transitions. A `WeatherSystem` replicates weather states (clear, cloudy, rain, fog, storm) and toggles particles and audio accordingly. `FirePropagator` allows objects to catch fire and optionally spread to nearby objects, replicating the burn state to all clients.

* **Physics** – `NetworkPhysicsObject` wraps a Rigidbody and automatically adds a `NetworkTransform` when Mirror is present. It disables client‑side physics when not authoritative and syncs position/rotation across the network. `DamageSystem` centralises applying damage; any component implementing `IDamageReceiver` can be damaged locally or via the server.

* **Puzzle** – building blocks for networked puzzles: `LeverSwitch` toggles on/off when interacted with and replicates state; `KeypadPuzzle` replicates input and solved status; `PuzzleManager` watches a set of puzzles and fires an event when all are solved.

* **AI** – `PathfindingAgent` replicates NavMeshAgent destinations across the network; `AIController` uses a `PathfindingAgent` to patrol a list of waypoints on the server.

* **UI Enhancements** – `AdvancedScoreboard` displays player names, kills, deaths and ping; it auto‑detects a `PlayerStats` component on player objects and falls back to connection IDs. A `NetworkChat` component (not pictured) provides basic text chat. A `PerformanceMonitor` shows FPS and toggle on/off via F10.

* **Utilities** – `NetworkDebugConsole` implements a simple in‑game console for running commands locally or via the server. `PerformanceMonitor` displays a lightweight FPS counter. These utilities help during development and can be disabled in shipping builds.

* **AI and Interaction Integration** – the existing `CharacterAI`, `CharacterInventory`, `CharacterHealth` and new `DamageSystem` integrate seamlessly, enabling AI agents to patrol, take damage and die, and enabling players to pick up items, equip them and use them in puzzles.

### Example: adding a networked character

1. Import `Assets/UMK` and create a `UMK_NetworkConfig` via the Setup Wizard.
2. Create a Player prefab with a `CharacterController` and the following components:
   - `CharacterAgent`
   - `CharacterMotor`
   - `CharacterLook`
   - `CharacterEquipment`
   - Optional: `AudioEmitter` for footstep sounds
3. Create a child `CameraRoot` and assign it in `CharacterAgent` and `CharacterLook`. Add a child `Camera` under `CameraRoot` and assign it to `CharacterAgent`.
4. Add equipable items as children (e.g., a `FlashlightEquipable` with a `Light` component) and list them in `CharacterEquipment.equipableMonoBehaviours`.
5. Spawn this Player via `UMK_NetworkService` in your bootstrap scene.
6. Subscribe to events using `EventBus.Subscribe<YourEvent>(OnEventMethod)` anywhere in your code.

You can extend these classes or write your own systems. See the source under `Runtime/Core` for examples.

## Transports
- **Mirror Auto**, **Telepathy**, **KCP** – LAN/Online client-server. (Requires Mirror.)
- **Steam (SDR)** – Mirror + Steam Sockets (FizzySteamworks/SteamSocketsTransport). Auto-detected via reflection.
- **Unity Relay** – **Actual implementation** (no scaffolding) when `com.unity.services.relay`, `com.unity.transport`, and **NGO** are present. Host creates an Allocation + Join Code; clients join by code.

## Anti-Cheat (one at a time)
- Default Validation (server-authority hooks)
- Unity Game Shield, GUARD, VAC, EAC, BattlEye (adapters auto-detect presence).

## Diagnostics
- Top-left overlay with FPS + ping (Mirror RTT). Toggle key default **F9**.

## Notes
- If a required package is missing, UMK shows a precise error but keeps your project compiling.
- Relay adapter uses Unity APIs behind `#if HAS_UGS_RELAY && HAS_UTP` and falls back to an inactive stub otherwise.
