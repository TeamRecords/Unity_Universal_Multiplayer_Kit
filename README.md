
# Universal Multiplayer Kit (UMK)

**UMK** is a standalone, drop‑in networking facade for Unity 6. It lets any of your systems (Characters, Environment, Objects, Terrain, etc.) opt‑in to multiplayer without depending on Mirror or any specific stack directly.

UMK detects what's in your project and adapts:
- **Mirror** transports: Auto / Telepathy / **KCP** / **Steam (SDR)**.
- **Unity Relay** (via Unity Transport + NGO), host/client with join codes.
- Optional anti‑cheat: **Default Validation**, **Unity Game Shield**, **GUARD**, **VAC**, **EAC**, **BattlEye**.
- Diagnostics overlay (FPS + Ping).

> UMK never owns your rendering or gameplay code; you just **call the service** from any script.

---

## ✨ Features
- Single API for **Host / Client / Dedicated Server**.
- Switch transports in one **Config asset** or via **Tools → UMK → Setup Wizard**.
- **Error‑tolerant**: If a package is missing, UMK logs a clear message and continues safely.
- **Renderer‑agnostic** (GPU/CPU instancing friendly).
- Minimal allocations; diagnostics updated twice/second.

---

## 🚀 Quick Start
1. Import the **UMK** folder into `Assets/`.
2. Open **Tools → UMK → Setup Wizard**, click **Create Config Asset**.
3. Choose **Transport** and **Anti‑Cheat** (optional), then **Run Environment Checks**.
4. In your startup scene, add an empty GO and attach **`UMK_NetworkService`**.
5. From any of your scripts:
   ```csharp
   using UMK.Core;

   // Host (listen server)
   UMK_NetworkService.Instance.StartHost();

   // Client (IP for Mirror transports; join code for Relay)
   UMK_NetworkService.Instance.StartClient("127.0.0.1");

   // Dedicated server
   UMK_NetworkService.Instance.StartServer();

   // Diagnostics
   int ping = UMK_NetworkService.Instance.Transport.GetPingMs();
   UMK_NetworkService.Instance.Diagnostics.Toggle();
   ```

---

## 🔌 Transports

| Transport | Stack | Notes |
|---|---|---|
| **Offline** | None | Fallback when Mirror/NGO missing. |
| **Mirror Auto** | Mirror | Creates NetworkManager if missing and attaches Telepathy. |
| **Mirror Telepathy** | Mirror/TCP | LAN/Internet. |
| **Mirror KCP** | Mirror/UDP (kcp2k) | Low‑latency, stable. |
| **Steam (SDR)** | Mirror + Steam transport | Requires a Steam Sockets transport (e.g., `SteamSocketsTransport` or `FizzySteamworks`). UMK finds it via reflection and uses Mirror flows. |
| **Unity Relay** | Unity Transport + NGO | Requires `com.unity.services.core`, `com.unity.services.relay`, `com.unity.transport`, and an NGO `NetworkManager`. Host shows a **join code**; client connects using the code. |

> Choose **Steam** for Steam‑centric releases; **Relay** for cross‑store join‑code flows; **KCP/Telepathy** for simple client‑server/LAN.

---

## 🛡 Anti‑Cheat (choose one)
- **Default Validation** (built‑in): server‑authority hooks you can drive from your gameplay code.
- **Unity Game Shield**, **GUARD**, **VAC**, **EAC**, **BattlEye**: UMK detects presence and logs actionable errors if not found.

> UMK ships adapters only; you own gameplay validation (speed/teleport checks, cooldowns) for best results.

---

## 🧰 Setup Wizard
**Tools → UMK → Setup Wizard** helps you:
- Create and edit the `UMK_NetworkConfig` asset.
- Select transport and anti‑cheat.
- **Run Environment Checks** for Steam/Relay/Mirror and anti‑cheat packages.
- See clear **error feedback** if something is missing.

---

## ⚙️ Unity Relay Notes
UMK’s Relay adapter uses reflection to configure `UnityTransport` and start NGO **host/client**. It expects:
- `UnityTransport` component in scene,
- `Unity.Netcode.NetworkManager` in scene,
- Unity Services Core and Relay packages installed and initialized.

> For production, replace the placeholder allocation/join logic with your preferred async flow and persist/display the **join code** in your UI.

---

## 🧪 Extending UMK
- **Custom Transport**: implement `ITransport` and plug it into `UMK_TransportFactory`.
- **Custom Anti‑Cheat**: implement `IAntiCheatProvider` and register in `UMK_AntiCheatFactory`.
- Keep adapters in `/Transports` and `/AntiCheat` to stay organized.

---

## 📦 File Layout
```
Assets/UMK/
  Runtime/
    Core/ (service, config, interfaces, overlay, factory)
    Core/Transports/ (Mirror Auto/Telepathy/KCP, Offline, SteamSDR, UnityRelay)
    Core/AntiCheat/ (Default/UGS/GUARD/VAC/EAC/BattleEye)
    Support/ (MirrorShim.cs)
  Editor/
    UMK_SetupWizard.cs
  README.md
```

---

## 🧭 Roadmap
- [x] Steam (SDR) transport (Mirror reflection hook)
- [x] Unity Relay adapter (UTP + NGO reflection hook)
- [ ] Async join‑code UI helpers
- [ ] EOS transport adapter
- [ ] Simple matchmaking module
- [ ] Metrics exporter (Prometheus/Grafana)

---

## 📎 License & Support
Use and modify in commercial and non‑commercial projects. No warranty.  
Report problems with: Unity version, transport selection, package list, and **UMK** console output.
