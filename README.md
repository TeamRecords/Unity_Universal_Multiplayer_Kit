
# Universal Multiplayer Kit (UMK)

UMK is a standalone, drop-in networking facade for **Unity 6** that enables Host/Client/Dedicated flows for **Mirror** transports and **Unity Relay**, with optional anti-cheat integration and diagnostics. It’s system-agnostic: Characters, Environment, Objects, Terrain, etc. can all call UMK without depending on any specific netcode package.

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
