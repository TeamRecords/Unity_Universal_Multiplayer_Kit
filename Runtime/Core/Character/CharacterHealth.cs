using System;
using UnityEngine;

namespace UMK.Core.Character
{
    /// <summary>
    /// Simple health component for characters and objects. It supports healing, damage and death
    /// events. When Mirror is present, the health value is synchronised via a SyncVar. Other systems
    /// can listen for health change events via EventBus or the OnHealthChanged event.
    /// </summary>
    public class CharacterHealth : NetBehaviourBase
    {
        [Tooltip("Maximum health for this entity")]
        public float maxHealth = 100f;

        /// <summary>
        /// Current health value. Use methods Heal and TakeDamage to modify it. Do not set directly.
        /// </summary>
        public float CurrentHealth
        {
            get => _health;
            private set
            {
                if (Mathf.Approximately(_health, value)) return;
                float old = _health;
                _health = Mathf.Clamp(value, 0f, maxHealth);
                OnHealthChanged?.Invoke(this, _health, maxHealth);
                // Publish event to bus
                EventBus.Publish(new HealthChangedEvent { entity = this, oldHealth = old, newHealth = _health, maxHealth = maxHealth });
                if (_health <= 0f && old > 0f)
                {
                    // Publish death event
                    EventBus.Publish(new DeathEvent { entity = this });
                }
            }
        }

        /// <summary>
        /// Invoked whenever health changes (including death). Arguments: (component, current, max).
        /// </summary>
        public event Action<CharacterHealth, float, float> OnHealthChanged;

        // Backing field for current health. When Mirror is present, this becomes a SyncVar.
#if HAS_MIRROR
        [Mirror.SyncVar(hook = nameof(OnHealthSync))]
#endif
        private float _health;

        void Awake()
        {
            _health = maxHealth;
        }

        /// <summary>
        /// Damages the entity. Negative amounts will heal.
        /// </summary>
        public void TakeDamage(float amount)
        {
            if (amount < 0f) { Heal(-amount); return; }
            CurrentHealth -= amount;
        }

        /// <summary>
        /// Heals the entity. Negative amounts will damage.
        /// </summary>
        public void Heal(float amount)
        {
            if (amount < 0f) { TakeDamage(-amount); return; }
            CurrentHealth += amount;
        }

#if HAS_MIRROR
        // Mirror hook called on clients when health is changed on server
        void OnHealthSync(float oldValue, float newValue)
        {
            // Update local state and raise events
            float old = _health;
            _health = newValue;
            OnHealthChanged?.Invoke(this, _health, maxHealth);
            EventBus.Publish(new HealthChangedEvent { entity = this, oldHealth = old, newHealth = newValue, maxHealth = maxHealth });
            if (_health <= 0f && old > 0f)
                EventBus.Publish(new DeathEvent { entity = this });
        }
#endif

        // Event types for event bus
        public struct HealthChangedEvent
        {
            public CharacterHealth entity;
            public float oldHealth;
            public float newHealth;
            public float maxHealth;
        }
        public struct DeathEvent
        {
            public CharacterHealth entity;
        }
    }
}