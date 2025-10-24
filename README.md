# Universal Multiplayer Kit (UMK)
Goal: a universal, modular foundation usable for Characters, Objects, Environment, Terrain, etc. Works offline and with networking.

## Folders:
- Core: interfaces (INetworkAdapter, IInputSource, IInteractable, IEquipable), EventBus, NetBehaviourBase
- Character: CharacterAgent (uses IInputSource + CharacterMotor + CharacterLook + CharacterEquipment), Flashlight equipable
- Interaction: DoorInteractable, PickupItem
- Adapters:
  - MirrorAdapter (enabled when Mirror present)  [symbol: HAS_MIRROR]
  - Pixel Anti-Cheat bridge (enabled when PAC present) [symbol: HAS_PAC]

## How to use for Character now:
1) Create an empty "Player" object with CharacterController; add:
   - CharacterAgent, CharacterMotor, CharacterLook, CharacterEquipment, CharacterInput_Default
   - Set yawRoot = the Player transform; create CameraPivot child, assign to CharacterAgent & CharacterLook.
2) Add a child "Flashlight" with a Light, add FlashlightEquipable and assign its flashlightObj.
3) (Networking) If using Mirror, Player becomes a network prefab; NetBehaviourBase will inherit NetworkBehaviour automatically.

## Extending later:
- Environment: create controllers that implement your own interfaces; publish events via EventBus (e.g., DayNightChanged, WeatherChanged).
- Objects: implement IInteractable for doors, pickups, levers; handle authority on server if multiplayer.
- Terrain: write a component that listens for movement events and triggers water ripples / foliage rustle, or exposes methods for voxel deformation.
