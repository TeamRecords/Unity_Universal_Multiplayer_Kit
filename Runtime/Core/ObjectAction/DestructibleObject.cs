using UnityEngine;

namespace UMK.Core.ObjectAction
{
    /// <summary>
    /// Example of a destructible object. It holds health and can be damaged via the TakeDamage method. When
    /// health reaches zero it triggers a Destroyed event and optionally destroys the GameObject. Network
    /// replication can be added by deriving from NetBehaviourBase and synchronising health values.
    /// </summary>
    public class DestructibleObject : MonoBehaviour
    {
        public float maxHealth = 50f;
        public bool destroyOnDeath = true;
        private float _health;
        void Awake()
        {
            _health = maxHealth;
        }
        public void TakeDamage(float amount)
        {
            if (amount <= 0f) return;
            _health -= amount;
            EventBus.Publish(new DamageEvent { target = this, damage = amount, remainingHealth = _health });
            if (_health <= 0f)
            {
                EventBus.Publish(new DestroyedEvent { target = this });
                if (destroyOnDeath) Destroy(gameObject);
            }
        }
        // Events
        public struct DamageEvent
        {
            public DestructibleObject target;
            public float damage;
            public float remainingHealth;
        }
        public struct DestroyedEvent
        {
            public DestructibleObject target;
        }
    }
}