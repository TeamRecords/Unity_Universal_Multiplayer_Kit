using System.Collections;
using UnityEngine;

namespace UMK.Core.ObjectAction
{
    /// <summary>
    /// Spawns networked objects at defined spawn points. On servers it instantiates and spawns via Mirror;
    /// on clients or offline it simply instantiates locally. Useful for spawning enemies, pickups, etc.
    /// </summary>
    public class NetworkSpawner : NetBehaviourBase
    {
        public GameObject[] spawnPrefabs;
        public Transform[] spawnPoints;
        public float spawnInterval = 5f;
        private void Start()
        {
            if (!isServer) return;
            if (spawnPrefabs == null || spawnPrefabs.Length == 0) return;
            if (spawnPoints == null || spawnPoints.Length == 0) return;
            StartCoroutine(SpawnLoop());
        }
        private IEnumerator SpawnLoop()
        {
            while (true)
            {
                yield return new WaitForSeconds(spawnInterval);
                SpawnRandom();
            }
        }
        private void SpawnRandom()
        {
            int prefabIndex = Random.Range(0, spawnPrefabs.Length);
            int pointIndex = Random.Range(0, spawnPoints.Length);
            var prefab = spawnPrefabs[prefabIndex];
            var point = spawnPoints[pointIndex];
            if (!prefab || !point) return;
            GameObject obj = Instantiate(prefab, point.position, point.rotation);
#if HAS_MIRROR
            Mirror.NetworkServer.Spawn(obj);
#endif
        }
    }
}