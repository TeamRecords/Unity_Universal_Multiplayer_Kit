using UnityEngine;

namespace UMK.Core.Interaction
{
    /// <summary>
    /// Simple pickup interactable. When interacted with, it sends a pickup event and optionally destroys itself.
    /// Extend to add inventory logic, UI notifications and network synchronisation.
    /// </summary>
    public class PickupItem : MonoBehaviour, IInteractable
    {
        [Tooltip("Name of the item that will be reported when picked up")]
        public string itemName = "Item";
        [Tooltip("If true, the GameObject will be destroyed on pickup")]
        public bool destroyOnPickup = true;
        public void Interact(NetBehaviourBase interactor)
        {
            // Publish event
            EventBus.Publish(new PickupEvent { itemName = itemName, interactor = interactor });
            if (destroyOnPickup) Destroy(gameObject);
        }
        public struct PickupEvent
        {
            public string itemName;
            public NetBehaviourBase interactor;
        }
    }
}