using System.Collections.Generic;
using UnityEngine;

namespace UMK.Core.Character
{
    /// <summary>
    /// Simple inventory system for a character. Stores items implementing IInventoryItem. Supports adding,
    /// removing and using items. When Mirror is present, the inventory names list is synchronised via a SyncList.
    /// Events are published through EventBus on changes and usage.
    /// </summary>
    public class CharacterInventory : NetBehaviourBase
    {
        // Local list of items. This does not synchronise item instances across the network; it stores
        // references locally. Synchronisation is handled separately via names list for Mirror.
        private readonly List<IInventoryItem> _items = new List<IInventoryItem>();

        // Mirror SyncList to replicate inventory item names. Only used when HAS_MIRROR is defined.
#if HAS_MIRROR
        public class SyncStringList : Mirror.SyncList<string> { }
        [Mirror.SyncVar] public SyncStringList syncedNames = new SyncStringList();
#endif

        /// <summary>
        /// Adds an item to the inventory. Publishes an InventoryChangedEvent and replicates names if needed.
        /// </summary>
        public void AddItem(IInventoryItem item)
        {
            if (item == null) return;
            _items.Add(item);
            EventBus.Publish(new InventoryChangedEvent { inventory = this, item = item, added = true });
#if HAS_MIRROR
            syncedNames.Add(item.Name);
#endif
        }

        /// <summary>
        /// Removes an item from the inventory. Publishes an InventoryChangedEvent and replicates names if needed.
        /// </summary>
        public bool RemoveItem(IInventoryItem item)
        {
            if (item == null) return false;
            if (_items.Remove(item))
            {
                EventBus.Publish(new InventoryChangedEvent { inventory = this, item = item, added = false });
#if HAS_MIRROR
                // Remove only one occurrence
                syncedNames.Remove(item.Name);
#endif
                return true;
            }
            return false;
        }

        /// <summary>
        /// Uses an item. Calls OnUse on the item and publishes an ItemUsedEvent. Optionally removes
        /// the item if it should be consumed.
        /// </summary>
        public void UseItem(IInventoryItem item, bool consume = true)
        {
            if (item == null || !_items.Contains(item)) return;
            item.OnUse(this);
            EventBus.Publish(new ItemUsedEvent { inventory = this, item = item });
            if (consume) RemoveItem(item);
        }

        // Events
        public struct InventoryChangedEvent
        {
            public CharacterInventory inventory;
            public IInventoryItem item;
            public bool added;
        }
        public struct ItemUsedEvent
        {
            public CharacterInventory inventory;
            public IInventoryItem item;
        }
    }
}