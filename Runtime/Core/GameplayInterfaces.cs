using UnityEngine;

namespace UMK.Core
{
    /// <summary>
    /// Provides a source of user inputs for a character or actor. Abstracts away the concrete input system
    /// implementation (e.g., Unity Input System, custom VR controllers) so that characters can be driven
    /// consistently on host and remote clients. To implement your own input source, implement this
    /// interface and assign it to a CharacterAgent.
    /// </summary>
    public interface IInputSource
    {
        Vector2 GetMoveVector();
        Vector2 GetLookVector();
        bool GetJump();
        bool GetSprint();
        bool GetCrouchToggle();
        bool GetAction();
        bool GetEquipToggle();
    }

    /// <summary>
    /// Items that can be equipped by a character (e.g., weapons, flashlights). Equipables can hook
    /// into the CharacterEquipment system to become visible/active when equipped and provide behaviour
    /// (e.g., attack, light emission). Equipables may implement network logic to synchronise state.
    /// </summary>
    public interface IEquipable
    {
        string Name { get; }
        void OnEquip(NetBehaviourBase owner);
        void OnUnequip(NetBehaviourBase owner);
        /// <summary>
        /// Optional update when equipped. Implement to run per-frame behaviour.
        /// </summary>
        void Tick(NetBehaviourBase owner);
    }

    /// <summary>
    /// Represents an entity that can be interacted with (e.g., a door, item pickup). The Interact method
    /// will be called by a Character when the player presses their Interact input and the object is
    /// within range. Implement this interface on your objects and handle the interaction (e.g., open door).
    /// </summary>
    public interface IInteractable
    {
        void Interact(NetBehaviourBase interactor);
    }

    /// <summary>
    /// Audio emitters can play sounds in response to game events (e.g., footsteps, actions, ambient).
    /// Use this interface to trigger audio playback and replicate events across the network. Implement
    /// network replication in a derived component using NetBehaviourBase to broadcast audio events.
    /// </summary>
    public interface IAudioEmitter
    {
        void PlayOneShot(AudioClip clip, float volume = 1f);
        void PlayLoop(AudioClip clip, float volume = 1f);
        void StopLoop();
    }

    /// <summary>
    /// Represents an action that can be performed on an object (e.g., pressing a button, destroying an item).
    /// Invoke PerformAction from your code to trigger the behaviour. You can hook this into EventBus to notify
    /// other systems of the action. Network replication can be added via NetBehaviourBase if needed.
    /// </summary>
    public interface IObjectAction
    {
        void PerformAction(NetBehaviourBase caller);
    }

    /// <summary>
    /// Implements modifications to the terrain (e.g., voxel deformation, footprints). Call ModifyTerrain
    /// from your gameplay logic to alter the terrain. Network replication can be implemented in derived
    /// NetBehaviourBase components when Mirror is present.
    /// </summary>
    public interface ITerrainModifier
    {
        void ModifyTerrain(Vector3 worldPosition, float amount);
    }
}