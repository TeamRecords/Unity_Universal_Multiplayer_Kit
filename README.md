# Universal Multiplayer Kit (UMK)

The **Universal Multiplayer Kit (UMK)** is a modular, drop-in system for Unity 6 that provides full multiplayer, peer-to-peer, and LAN support with Mirror, along with optional anti-cheat integration and diagnostics tools.

It is designed to be **standalone**, meaning it can be used with *any other gameplay system* — Characters, Environment, Terrain, Object Interactions, etc. — without modification to the UMK source itself.

---

## 🎯 Goals
- **Universal:** Works with any system via simple references.
- **Modular:** You choose your transport, anti-cheat, and security stack.
- **Lightweight:** Minimal overhead, efficient diagnostics overlay.
- **Extensible:** Add new transports (Steam, EOS, Unity Relay) or anti-cheat systems easily.
- **Error-Tolerant:** Graceful fallback when missing dependencies.

---

## 🧱 Core Architecture
UMK is divided into three runtime layers and one editor layer:

| Layer | Description |
|--------|--------------|
| **Core** | Core abstractions and service logic (`ITransport`, `IAntiCheatProvider`, `UMK_NetworkService`, `Diagnostics`). |
| **Adapters** | Optional Mirror, Relay, or custom transport implementations. |
| **AntiCheat** | Optional integration with GUARD, VAC, EAC, BattlEye, or Unity Game Shield. |
| **Editor** | Tools → UMK → Setup Wizard for easy setup, checks, and configuration. |

---

## 🚀 Quick Setup Guide

### 1️⃣ Import the Package
Copy the **UMK** folder into your Unity project's `Assets/` directory.

### 2️⃣ Create or Open a Network Config
Navigate to **Tools → UMK → Setup Wizard**.
- Click **Create Config Asset**.
- Choose a **Transport**: Auto, Telepathy, or KCP (more available later).
- Choose an **Anti-Cheat** (optional).
- Enable or disable the **Diagnostics Overlay**.
- Run **Environment Checks** to verify detected packages.

### 3️⃣ Add the Network Service
In your startup scene:
1. Create an empty GameObject named `UMK_NetworkManager`.
2. Add the `UMK_NetworkService` component.
3. Assign your `UMK_NetworkConfig` asset (optional — it will create one automatically).

### 4️⃣ Call It From Any System
UMK can be accessed from anywhere via a static singleton:

```csharp
using UMK.Core;

// Start a listen-host (peer2peer host)
UMK_NetworkService.Instance.StartHost();

// Connect to a host
UMK_NetworkService.Instance.StartClient("192.168.1.10");

// Start a dedicated server
UMK_NetworkService.Instance.StartServer();

// Diagnostics
int ping = UMK_NetworkService.Instance.Transport.GetPingMs();
UMK_NetworkService.Instance.Diagnostics.Toggle();
```

---

## ⚙️ Supported Transports

| Transport | Description |
|------------|--------------|
| **Offline** | Fallback mode when no Mirror package is present. No compile errors. |
| **Mirror Auto** | Automatically selects the best Mirror transport (default). |
| **Mirror Telepathy** | TCP-based LAN and online client-server mode. |
| **Mirror KCP** | High-performance UDP transport for latency-sensitive gameplay. |
| **(Planned)** Steam (SDR) | Uses Steam's P2P relay for NAT traversal. |
| **(Planned)** Unity Relay | Uses Unity Gaming Services for matchmaking and relay connections. |
| **(Planned)** EOS P2P | Uses Epic Online Services for cross-platform peer connections. |

All transports implement `ITransport`, so you can write your own.

---

## 🧩 Anti-Cheat Integrations

UMK ships with **optional** adapters. Each is auto-detected via reflection — if a dependency is missing, it logs a clear warning and safely disables itself.

| Provider | Type | Detection |
|-----------|------|------------|
| **Default Validation** | Free built-in system. Hooks for server authority + basic anti-cheat. | Always available. |
| **Unity Game Shield** | Commercial Unity-integrated anti-cheat. | `UnityGameShield.EntryPoint` |
| **GUARD** | Lightweight external AC for Unity. | `Guard.Core.Entry` |
| **VAC** | Steam VAC via Steamworks.NET or Facepunch.Steamworks. | Steamworks API presence. |
| **EAC** | Easy Anti-Cheat (Epic). | `EasyAntiCheat.Client.Hydra` |
| **BattlEye** | Trusted game anti-cheat for multiplayer titles. | `BattlEye.BEClient` |

Only **one anti-cheat** may be active at a time. You select it in the config file or via the Setup Wizard.

---

## 🔍 Diagnostics Overlay

- Shows **FPS** and **Ping** in real-time (Mirror RTT-based).
- Toggle visibility via key (default `F9`).
- Low-cost updates (twice per second).
- Renders in a top-left UI overlay via `UMK_DiagnosticsOverlay`.

---

## 🔐 Error Handling

UMK performs multiple runtime checks:
- If Mirror is missing, the shim activates automatically.
- If an anti-cheat module isn’t found, a user-facing error is displayed.
- Safe-to-call: No crashes even if no networking is installed.

---

## ⚡ Performance Notes
- Diagnostics overlay uses a half-second polling interval.
- Network RTT fetched every ~2s for minimal cost.
- Renderer-agnostic — **works with GPU Instancing, HDRP, URP**, or custom pipelines.
- Suitable for survival, horror, or sandbox multiplayer titles.

---

## 🧰 Editor Tools

- **Setup Wizard:** Tools → UMK → Setup Wizard
  - Create `UMK_NetworkConfig` assets.
  - Run dependency checks.
  - Edit transport and anti-cheat selections.
  - Error feedback and missing package detection.

---

## 🧪 Extending UMK

### Adding a Custom Transport
```csharp
using UMK.Core;

public class MyCustomTransport : ITransport
{
    public string Name => "MyTransport";
    public bool Available => true;
    public void Initialize(GameObject ctx, UMK_NetworkConfig cfg, System.Action<string> onError) {}
    public void StartHost() { }
    public void StartClient(string address) { }
    public void StartServer() { }
    public int GetPingMs() => 0;
}
```
Then register it in your own factory or hook it through `UMK_TransportFactory`.

### Adding a Custom Anti-Cheat
Implement `IAntiCheatProvider` and handle detection + violation reporting.

---

## 🧭 Roadmap
- [x] Mirror Telepathy/KCP transports.
- [x] Diagnostics Overlay.
- [x] Anti-Cheat adapters.
- [x] Editor setup wizard.
- [ ] Steam (SDR) transport.
- [ ] Unity Relay integration.
- [ ] EOS P2P integration.
- [ ] Basic web dashboard for monitoring.
- [ ] Session matchmaking (optional module).

---

## 🧠 Developer Notes

UMK was built for **ARChaotic Unity** and the **ARC Foundation** as a universal multiplayer foundation for survival, horror, and creative sandbox games. It is optimized for real-time performance and compatibility across systems.

To contribute, extend, or adapt UMK for your game, please document all transport or anti-cheat additions within the `/Adapters` or `/AntiCheat` folders to maintain clarity.

---

### 💬 Support
If you need setup help, create an issue or contact your development lead. Include your Unity version, Mirror version, and the full UMK log output (Unity Console).

---

© 2025 ARC Foundation / ARChaotic Unity — Universal Multiplayer Kit (UMK)
