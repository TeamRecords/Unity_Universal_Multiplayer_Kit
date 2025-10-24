using UnityEngine;

namespace UMK.Core.Character
{
    /// <summary>
    /// Simple flashlight equipable. When equipped it activates a light and plays an optional sound.
    /// When unequipped it disables the light. Can be extended to sync state over network.
    /// </summary>
    public class FlashlightEquipable : MonoBehaviour, IEquipable
    {
        public string Name => "Flashlight";
        [Tooltip("Object with Light component to toggle when equipped")]
        public GameObject flashlightObj;
        [Tooltip("Optional audio source for toggle sound")]
        public AudioSource audioSource;
        public AudioClip equipClip;
        public AudioClip unequipClip;

        public void OnEquip(NetBehaviourBase owner)
        {
            if (flashlightObj) flashlightObj.SetActive(true);
            if (audioSource && equipClip) audioSource.PlayOneShot(equipClip);
        }
        public void OnUnequip(NetBehaviourBase owner)
        {
            if (flashlightObj) flashlightObj.SetActive(false);
            if (audioSource && unequipClip) audioSource.PlayOneShot(unequipClip);
        }
        public void Tick(NetBehaviourBase owner)
        {
            // Flashlight does not need per-frame logic in this simple implementation
        }
    }
}