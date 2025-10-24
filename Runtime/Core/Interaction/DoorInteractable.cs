using UnityEngine;

namespace UMK.Core.Interaction
{
    /// <summary>
    /// Simple door interactable. Toggling the door rotates it open/closed and publishes an event via the EventBus.
    /// Extend this class or implement network replication to synchronise door state across clients.
    /// </summary>
    public class DoorInteractable : MonoBehaviour, IInteractable
    {
        public Transform doorTransform;
        public Vector3 closedRotation = Vector3.zero;
        public Vector3 openRotation = new Vector3(0, 90f, 0);
        public float openSpeed = 4f;
        private bool _isOpen;
        private Quaternion _targetRot;
        void Awake()
        {
            if (!doorTransform) doorTransform = transform;
            _targetRot = Quaternion.Euler(closedRotation);
        }
        void Update()
        {
            if (doorTransform) doorTransform.localRotation = Quaternion.Slerp(doorTransform.localRotation, _targetRot, Time.deltaTime * openSpeed);
        }
        public void Interact(NetBehaviourBase interactor)
        {
            _isOpen = !_isOpen;
            _targetRot = Quaternion.Euler(_isOpen ? openRotation : closedRotation);
            // Publish event for other systems (audio, AI, network)
            EventBus.Publish(new DoorToggleEvent { door = this, isOpen = _isOpen, interactor = interactor });
        }
        public struct DoorToggleEvent
        {
            public DoorInteractable door;
            public bool isOpen;
            public NetBehaviourBase interactor;
        }
    }
}