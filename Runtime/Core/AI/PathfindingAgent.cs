using UnityEngine;
using UnityEngine.AI;

#if HAS_MIRROR
using Mirror;
#endif

namespace UMK.Core.AI
{
    /// <summary>
    /// Wrapper around NavMeshAgent that replicates destination changes across the network when Mirror is present.
    /// Supports simple movement to target points. Clients can request a move via Interact; server/offline has authority.
    /// </summary>
    [RequireComponent(typeof(NavMeshAgent))]
    public class PathfindingAgent : NetBehaviourBase
    {
        private NavMeshAgent agent;

        #if HAS_MIRROR
        [SyncVar]
        #endif
        private Vector3 targetPosition;

        private void Awake()
        {
            agent = GetComponent<NavMeshAgent>();
        }

        private void Update()
        {
            // On server/offline, update agent destination when target changes
            if (IsServerOrOffline())
            {
                if (Vector3.Distance(agent.destination, targetPosition) > 0.1f)
                {
                    agent.SetDestination(targetPosition);
                }
            }
        }

        /// <summary>
        /// Requests movement to a target point. On server/offline, sets the target; on client, this should
        /// be routed through a Command (not implemented here for brevity).
        /// </summary>
        public void MoveTo(Vector3 pos)
        {
            if (IsServerOrOffline())
            {
                targetPosition = pos;
                agent.SetDestination(targetPosition);
            }
            else
            {
                // In a real game, send a Command to the server to update targetPosition
            }
        }
    }
}