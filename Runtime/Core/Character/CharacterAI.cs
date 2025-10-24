using UnityEngine;

namespace UMK.Core.Character
{
    /// <summary>
    /// Simple AI controller that moves a character between a series of waypoints using CharacterController.
    /// It does not perform any networking; networked NPCs should derive from NetBehaviourBase and handle
    /// state replication with Mirror if required. This component demonstrates how to drive movement
    /// outside of the player input system.
    /// </summary>
    [RequireComponent(typeof(CharacterController))]
    public class CharacterAI : NetBehaviourBase
    {
        public Transform[] waypoints;
        public float moveSpeed = 2f;
        public float arrivalThreshold = 0.5f;
        private int _currentIndex;
        private CharacterController _cc;
        private Vector3 _velocity;
        void Awake()
        {
            _cc = GetComponent<CharacterController>();
        }
        void Update()
        {
            if (waypoints == null || waypoints.Length == 0) return;
            if (waypoints[_currentIndex] == null) return;
            Vector3 target = waypoints[_currentIndex].position;
            Vector3 dir = (target - transform.position);
            dir.y = 0f;
            float dist = dir.magnitude;
            if (dist < arrivalThreshold)
            {
                _currentIndex = (_currentIndex + 1) % waypoints.Length;
                return;
            }
            Vector3 move = dir.normalized * moveSpeed;
            // Simple gravity
            if (_cc.isGrounded) _velocity.y = -1f;
            else _velocity.y += Physics.gravity.y * Time.deltaTime;
            Vector3 final = new Vector3(move.x, _velocity.y, move.z);
            _cc.Move(final * Time.deltaTime);
        }
    }
}