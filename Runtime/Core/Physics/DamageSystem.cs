using UnityEngine;

#if HAS_MIRROR
using Mirror;
#endif

namespace UMK.Core.Physics
{
    /// <summary>
    /// Central manager for applying damage and broadcasting damage events. Components that want to receive damage
    /// should implement IDamageReceiver and register with this system if desired. Damage events are replicated across
    /// the network when Mirror is present.
    /// </summary>
    public class DamageSystem : NetBehaviourBase
    {
        public interface IDamageReceiver
        {
            void TakeDamage(float amount, Vector3 hitPoint);
        }

        /// <summary>
        /// Apply damage to a receiver. On server, the damage is applied directly. On client/offline, this call
        /// simply invokes the receiver. Use Mirror or a custom RPC to propagate damage to the server in a real game.
        /// </summary>
        public void ApplyDamage(IDamageReceiver receiver, float amount, Vector3 hitPoint)
        {
            if (IsServerOrOffline())
            {
                receiver?.TakeDamage(amount, hitPoint);
            }
            else
            {
                // In a real game you would send a Command to the server to apply damage
                receiver?.TakeDamage(amount, hitPoint);
            }
        }
    }
}