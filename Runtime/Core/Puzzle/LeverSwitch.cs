using UnityEngine;

#if HAS_MIRROR
using Mirror;
#endif

namespace UMK.Core.Puzzle
{
    /// <summary>
    /// A simple lever switch that can be toggled on/off by interacting. The state is replicated across the network when Mirror is present.
    /// Use this as a building block for puzzles or interactive objects.
    /// </summary>
    public class LeverSwitch : NetBehaviourBase, IInteractable
    {
        [Tooltip("Whether the lever is currently on.")]
        #if HAS_MIRROR
        [SyncVar(hook = nameof(OnStateChanged))]
        #endif
        public bool isOn;

        [Tooltip("Transform to rotate when toggled.")]
        public Transform handle;

        [Tooltip("Rotation when off.")]
        public Vector3 rotationOff = new Vector3(0f, 0f, -30f);

        [Tooltip("Rotation when on.")]
        public Vector3 rotationOn = new Vector3(0f, 0f, 30f);

        public void Interact(GameObject interactor)
        {
            // Only host/offline can toggle state
            if (IsServerOrOffline())
            {
                isOn = !isOn;
                OnStateChanged(!isOn, isOn);
            }
        }

        private void OnStateChanged(bool oldState, bool newState)
        {
            if (handle)
            {
                handle.localEulerAngles = newState ? rotationOn : rotationOff;
            }
        }
    }
}