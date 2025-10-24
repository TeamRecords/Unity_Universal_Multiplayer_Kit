using System.Collections.Generic;
using UnityEngine;

namespace UMK.Core.Character
{
    /// <summary>
    /// Manages a collection of equipable items attached to a character. Allows toggling between
    /// equipables and calls OnEquip/OnUnequip on them. Also provides a Tick method for per-frame
    /// behaviour while equipped.
    /// </summary>
    public class CharacterEquipment : MonoBehaviour
    {
        [Tooltip("List of equipables attached to this character (e.g., weapons, tools, items)")]
        public List<MonoBehaviour> equipableMonoBehaviours = new List<MonoBehaviour>();

        private List<IEquipable> _equipables = new List<IEquipable>();
        private int _currentIndex = -1;

        void Awake()
        {
            // Populate list by extracting IEquipable implementations
            foreach (var mb in equipableMonoBehaviours)
            {
                if (mb is IEquipable eq) _equipables.Add(eq);
            }
        }

        /// <summary>
        /// Toggle equip: if nothing is equipped, equip the first item. Otherwise, unequip current.
        /// Could be extended to cycle through multiple items.
        /// </summary>
        public void ToggleEquip()
        {
            if (_equipables.Count == 0) return;
            if (_currentIndex < 0)
            {
                // Equip the first item
                _currentIndex = 0;
                _equipables[_currentIndex].OnEquip(GetComponent<NetBehaviourBase>());
            }
            else
            {
                // Unequip current item
                _equipables[_currentIndex].OnUnequip(GetComponent<NetBehaviourBase>());
                _currentIndex = -1;
            }
        }

        /// <summary>
        /// Updates the currently equipped item, if any. Should be called each frame.
        /// </summary>
        public void Tick(NetBehaviourBase owner)
        {
            if (_currentIndex >= 0 && _currentIndex < _equipables.Count)
            {
                _equipables[_currentIndex].Tick(owner);
            }
        }
    }
}