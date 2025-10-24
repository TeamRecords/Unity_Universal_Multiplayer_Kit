# UMK Event Reference

This document lists all event types published via the **UMK Event Bus**.  Each event is a simple data
structure whose sole purpose is to convey information between systems without creating hard
dependencies.  You can subscribe to any of these events using `EventBus.Subscribe<T>()` and
publish your own events with `EventBus.Publish(new MyEvent { … })`.

> **Note:** The event names below include the namespace and, where appropriate, the
> containing class.  For nested event structs, the class name is used as a prefix.

## Character Events

| Event | Description | Fields |
|------|-------------|--------|
| **`CharacterMotor.FootstepEvent`** | Fired by `CharacterMotor` whenever a footstep occurs while grounded. Useful for playing footstep sounds or notifying AI of movement. | `position` (`Vector3`): world position of the footstep.  `speed` (`float`): character’s current horizontal speed. |
| **`CharacterInventory.InventoryChangedEvent`** | Emitted by `CharacterInventory.AddItem()` and `RemoveItem()` when an item is added to or removed from an inventory. | `inventory` (`CharacterInventory`): the inventory instance.  `item` (`IInventoryItem`): the item added or removed.  `added` (`bool`): `true` if the item was added; `false` if removed. |
| **`CharacterInventory.ItemUsedEvent`** | Emitted by `CharacterInventory.UseItem()` after an item’s `OnUse()` method is invoked. | `inventory` (`CharacterInventory`): the inventory instance.  `item` (`IInventoryItem`): the item that was used. |
| **`CharacterHealth.HealthChangedEvent`** | Published whenever `CharacterHealth.CurrentHealth` changes, either due to damage or healing. | `entity` (`CharacterHealth`): the health component.  `oldHealth` (`float`): previous health value.  `newHealth` (`float`): new health value after the change.  `maxHealth` (`float`): the maximum health of the entity. |
| **`CharacterHealth.DeathEvent`** | Published by `CharacterHealth` when health drops to zero. | `entity` (`CharacterHealth`): the component that died. |

## Terrain & Environment Events

| Event | Description | Fields |
|------|-------------|--------|
| **`TerrainModifier.TerrainModifyEvent`** | Raised by `TerrainModifier.ModifyTerrain()` to signal a terrain deformation or effect (e.g. footprints, voxel edits). | `position` (`Vector3`): world coordinate of the modification.  `amount` (`float`): intensity or magnitude of the modification.  `modifier` (`TerrainModifier`): the component that originated the change. |

## Chat & UI Events

| Event | Description | Fields |
|------|-------------|--------|
| **`NetworkChat.ChatMessageEvent`** | Published by `NetworkChat` whenever a chat message is sent locally or received over the network. | `author` (`string`): sender’s display name.  `message` (`string`): the chat text. |

## Audio Events

| Event | Description | Fields |
|------|-------------|--------|
| **`AudioEmitter.AudioEvent`** | Fired by `AudioEmitter.PlayOneShot()` when a one‑shot sound is played.  Use this to drive AI perception or replicate audio in a networked game. | `clip` (`AudioClip`): audio clip played.  `volume` (`float`): volume at which the clip was played.  `position` (`Vector3`): world position of the emitter.  `emitter` (`AudioEmitter`): the source component. |
| **`AudioManager.MusicEvent`** | Raised by `AudioManager.PlayMusic()` when a new music track starts. | `clip` (`AudioClip`): music clip.  `manager` (`AudioManager`): the audio manager instance. |
| **`AudioManager.AmbientEvent`** | Raised by `AudioManager.PlayAmbient()` when a new ambient track starts. | `clip` (`AudioClip`): ambient clip.  `manager` (`AudioManager`): the audio manager instance. |
| **`AudioManager.SFXEvent`** | Raised by `AudioManager.PlaySFX()` when a sound effect is played. | `clip` (`AudioClip`): SFX clip.  `manager` (`AudioManager`): the audio manager instance. |

## Interaction Events

| Event | Description | Fields |
|------|-------------|--------|
| **`DoorInteractable.DoorToggleEvent`** | Published by `DoorInteractable.Interact()` when a door is toggled open or closed. | `door` (`DoorInteractable`): the door that changed state.  `isOpen` (`bool`): `true` if the door is now open; `false` if closed.  `interactor` (`NetBehaviourBase`): the entity that triggered the interaction (may be `null`). |
| **`PickupItem.PickupEvent`** | Emitted by `PickupItem.Interact()` when an item is picked up. | `itemName` (`string`): name of the item picked up.  `interactor` (`NetBehaviourBase`): the entity that picked up the item. |
| **`LockableInteractable.LockedEvent`** | Published by `LockableInteractable.Interact()` when an interaction is attempted on a locked object. | `locker` (`LockableInteractable`): the lockable component.  `interactor` (`NetBehaviourBase`): the interacting entity. |
| **`LockableInteractable.UnlockedEvent`** | Raised by `LockableInteractable.Unlock()` when a locked object becomes unlocked. | `locker` (`LockableInteractable`): the lockable component. |
| **`LockableInteractable.LockedStateChangedEvent`** | Raised by `LockableInteractable.Lock()` when the locked state changes programmatically. | `locker` (`LockableInteractable`): the lockable component.  `locked` (`bool`): the new lock state (`true` means locked). |

## Object Action & Damage Events

| Event | Description | Fields |
|------|-------------|--------|
| **`ObjectAction.ObjectActionEvent`** | Published by `ObjectAction.PerformAction()` to signal that an object action occurred. Use this to trigger remote effects or listen for interactions. | `actionName` (`string`): name or description of the action.  `caller` (`NetBehaviourBase`): the entity that invoked the action.  `target` (`ObjectAction`): the component on which the action occurred. |
| **`DestructibleObject.DamageEvent`** | Emitted by `DestructibleObject.TakeDamage()` when the object takes damage. | `target` (`DestructibleObject`): the object being damaged.  `damage` (`float`): amount of damage applied.  `remainingHealth` (`float`): health remaining after the damage. |
| **`DestructibleObject.DestroyedEvent`** | Emitted when a `DestructibleObject`’s health reaches zero. | `target` (`DestructibleObject`): the object that was destroyed. |

## Notes

* **Puzzle Events** – The built‑in puzzle components (`LeverSwitch`, `KeypadPuzzle`, `PuzzleManager`) do not currently publish events via the EventBus.  You can extend them to publish your own events if needed.
* **AI & Environment** – Systems like `AIController`, `PathfindingAgent`, `DayNightCycle` and `WeatherSystem` run autonomously and do not publish events.  They expose public methods and SyncVar hooks for network replication.
* **Extending the Bus** – To create your own event, simply define a lightweight struct or class and call `EventBus.Publish(new MyCustomEvent { … })`.  Consumers can subscribe with `EventBus.Subscribe<MyCustomEvent>(OnMyEvent)`.  Remember that messages are dispatched synchronously on the main thread.