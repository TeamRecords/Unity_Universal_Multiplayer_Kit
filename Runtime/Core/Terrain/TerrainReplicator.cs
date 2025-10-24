using UnityEngine;

namespace UMK.Core.Terrain
{
    /// <summary>
    /// Example network replicator for terrain modifications. Subscribes to TerrainModifyEvent and, when Mirror
    /// is present, sends modifications to the server which then replicates to all clients. This ensures
    /// consistent terrain state across the network. Without Mirror, it simply consumes the events.
    /// </summary>
    public class TerrainReplicator : NetBehaviourBase
    {
        [Tooltip("Reference to the TerrainModifier this replicator synchronises")]
        public TerrainModifier modifier;
        void OnEnable()
        {
            EventBus.Subscribe<TerrainModifier.TerrainModifyEvent>(OnModifyEvent);
        }
        void OnDisable()
        {
            EventBus.Unsubscribe<TerrainModifier.TerrainModifyEvent>(OnModifyEvent);
        }
        private void OnModifyEvent(TerrainModifier.TerrainModifyEvent ev)
        {
            // Only send modifications that originate from our own modifier
            if (ev.modifier != modifier) return;
#if HAS_MIRROR
            if (isLocalPlayer || !isServer)
            {
                // Send to server for replication
                CmdReplicateTerrain(ev.position, ev.amount);
            }
#endif
        }
#if HAS_MIRROR
        [Mirror.ServerRpc]
        private void CmdReplicateTerrain(Vector3 pos, float amt)
        {
            // Perform the modification on the server so it's authoritative
            modifier.ModifyTerrain(pos, amt);
            RpcReplicateTerrain(pos, amt);
        }
        [Mirror.ClientRpc]
        private void RpcReplicateTerrain(Vector3 pos, float amt)
        {
            if (isServer) return; // server already applied
            modifier.ModifyTerrain(pos, amt);
        }
#endif
    }
}