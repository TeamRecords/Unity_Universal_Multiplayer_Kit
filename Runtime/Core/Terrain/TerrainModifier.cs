using UnityEngine;

namespace UMK.Core.Terrain
{
    /// <summary>
    /// Example implementation of ITerrainModifier. Calling ModifyTerrain publishes a TerrainModifyEvent
    /// on the EventBus with the position and intensity. You can hook this up to your terrain system to
    /// deform voxels, spawn particles or leave footprints. Network replication can be added via NetBehaviourBase.
    /// </summary>
    public class TerrainModifier : MonoBehaviour, ITerrainModifier
    {
        public void ModifyTerrain(Vector3 worldPosition, float amount)
        {
            EventBus.Publish(new TerrainModifyEvent { position = worldPosition, amount = amount, modifier = this });
            // Custom logic to modify terrain would go here
        }
        public struct TerrainModifyEvent
        {
            public Vector3 position;
            public float amount;
            public TerrainModifier modifier;
        }
    }
}