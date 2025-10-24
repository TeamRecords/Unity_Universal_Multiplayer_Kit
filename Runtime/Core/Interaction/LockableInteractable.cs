using UnityEngine;

namespace UMK.Core.Interaction
{
    /// <summary>
    /// Decorator that adds a lock state to any IInteractable. When locked, Interact will do nothing and
    /// optionally publish a LockedEvent. Unlock by calling Unlock() or listening for external events.
    /// </summary>
    public class LockableInteractable : MonoBehaviour, IInteractable
    {
        [Tooltip("The actual interactable to invoke when unlocked")]
        public MonoBehaviour interactable;
        public bool isLocked = true;
        public string lockedMessage = "Locked";
        public void Interact(NetBehaviourBase interactor)
        {
            if (isLocked)
            {
                // Publish locked event
                EventBus.Publish(new LockedEvent { locker = this, interactor = interactor });
                return;
            }
            if (interactable is IInteractable inner)
            {
                inner.Interact(interactor);
            }
        }
        public void Unlock()
        {
            if (!isLocked) return;
            isLocked = false;
            EventBus.Publish(new UnlockedEvent { locker = this });
        }
        public void Lock()
        {
            if (isLocked) return;
            isLocked = true;
            EventBus.Publish(new LockedStateChangedEvent { locker = this, locked = true });
        }
        // Event types
        public struct LockedEvent
        {
            public LockableInteractable locker;
            public NetBehaviourBase interactor;
        }
        public struct UnlockedEvent
        {
            public LockableInteractable locker;
        }
        public struct LockedStateChangedEvent
        {
            public LockableInteractable locker;
            public bool locked;
        }
    }
}