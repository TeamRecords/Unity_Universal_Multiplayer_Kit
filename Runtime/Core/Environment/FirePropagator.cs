using UnityEngine;

#if HAS_MIRROR
using Mirror;
#endif

namespace UMK.Core.Environment
{
    /// <summary>
    /// Enables objects to catch fire and optionally propagate fire to nearby objects. Fire state is replicated across the network when Mirror is present.
    /// Attach this script to any object that can burn. Assign a particle system to visualize fire.
    /// </summary>
    public class FirePropagator : NetBehaviourBase
    {
        [Tooltip("Particle system used to visualize fire.")]
        public ParticleSystem fireParticle;

        [Tooltip("Whether the fire can spread to nearby FirePropagators.")]
        public bool allowPropagation = true;

        [Tooltip("Radius within which the fire can spread to other objects.")]
        public float propagationRadius = 2f;

        #if HAS_MIRROR
        [SyncVar(hook = nameof(OnFireStateChanged))]
        #endif
        private bool isBurning;

        private float burnDuration = 0f;
        private float maxBurnDuration = 30f;

        private void Update()
        {
            if (IsServerOrOffline())
            {
                if (isBurning)
                {
                    burnDuration += Time.deltaTime;
                    // stop burning after max duration
                    if (burnDuration >= maxBurnDuration)
                    {
                        SetBurning(false);
                    }
                    else if (allowPropagation)
                    {
                        // propagate fire once per second on average
                        if (Random.value < Time.deltaTime)
                        {
                            PropagateFire();
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Ignite this object. Only allowed on server/offline.
        /// </summary>
        public void Ignite(float duration = 30f)
        {
            if (IsServerOrOffline())
            {
                maxBurnDuration = duration;
                SetBurning(true);
            }
        }

        /// <summary>
        /// Extinguish this object. Only allowed on server/offline.
        /// </summary>
        public void Extinguish()
        {
            if (IsServerOrOffline())
            {
                SetBurning(false);
            }
        }

        private void SetBurning(bool burning)
        {
            isBurning = burning;
            burnDuration = 0f;
            // Mirror hook will call OnFireStateChanged
            OnFireStateChanged(!burning, burning);
        }

        // Mirror SyncVar hook signature (oldState unused)
        private void OnFireStateChanged(bool oldState, bool newState)
        {
            if (fireParticle)
            {
                var emission = fireParticle.emission;
                emission.enabled = newState;
                if (newState && !fireParticle.isPlaying) fireParticle.Play();
                if (!newState && fireParticle.isPlaying) fireParticle.Stop();
            }
        }

        private void PropagateFire()
        {
            // find nearby FirePropagators and ignite them
            Collider[] hits = UnityEngine.Physics.OverlapSphere(transform.position, propagationRadius);
            foreach (var hit in hits)
            {
                var other = hit.GetComponent<FirePropagator>();
                if (other && other != this && !other.isBurning)
                {
                    other.Ignite(maxBurnDuration);
                }
            }
        }
    }
}