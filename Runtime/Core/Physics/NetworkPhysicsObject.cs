using UnityEngine;

#if HAS_MIRROR
using Mirror;
#endif

namespace UMK.Core.Physics
{
    /// <summary>
    /// Simple wrapper for a Rigidbody that replicates its movement and physics state across the network when Mirror is present.
    /// When Mirror is absent, the object behaves like a normal Rigidbody. If Mirror is present,
    /// a NetworkTransform or NetworkRigidbody2D may be required. Here we use NetworkTransform to sync position and rotation.
    /// </summary>
    [RequireComponent(typeof(Rigidbody))]
    public class NetworkPhysicsObject : NetBehaviourBase
    {
        private Rigidbody rb;

        #if HAS_MIRROR
        [Header("Networking")]
        [Tooltip("NetworkTransform used to sync physics state. Assign if present, otherwise this script adds one.")]
        public NetworkTransform networkTransform;
        #endif

        private void Awake()
        {
            rb = GetComponent<Rigidbody>();

            #if HAS_MIRROR
            // Ensure a NetworkTransform is attached when Mirror is present
            if (networkTransform == null)
            {
                networkTransform = GetComponent<NetworkTransform>();
                if (networkTransform == null)
                {
                    networkTransform = gameObject.AddComponent<NetworkTransform>();
                }
            }
            // Configure networkTransform for client authority
            networkTransform.clientAuthority = false;
            #endif
        }

        private void FixedUpdate()
        {
            #if HAS_MIRROR
            // On server, simulate physics normally. Clients are authoritative if clientAuthority was enabled.
            // Here we run physics on the server only to ensure consistency.
            if (!IsServerOrOffline())
            {
                // On clients, disable physics to prevent prediction/resimulation
                rb.isKinematic = true;
            }
            else
            {
                rb.isKinematic = false;
            }
            #endif
        }
    }
}