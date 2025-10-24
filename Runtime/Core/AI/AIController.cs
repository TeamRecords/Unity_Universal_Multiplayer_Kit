using UnityEngine;

#if HAS_MIRROR
using Mirror;
#endif

namespace UMK.Core.AI
{
    /// <summary>
    /// Simple AI controller that uses a PathfindingAgent to move between waypoints.
    /// Waypoints are visited in order and looped. Only active on server/offline.
    /// </summary>
    public class AIController : NetBehaviourBase
    {
        [Tooltip("Pathfinding agent used for movement.")]
        public PathfindingAgent agent;

        [Tooltip("Points for the AI to visit.")]
        public Transform[] waypoints;

        [Tooltip("Distance threshold to consider a waypoint reached.")]
        public float waypointTolerance = 1f;

        private int currentIndex;

        private void Start()
        {
            if (IsServerOrOffline() && agent && waypoints != null && waypoints.Length > 0)
            {
                currentIndex = 0;
                agent.MoveTo(waypoints[currentIndex].position);
            }
        }

        private void Update()
        {
            if (IsServerOrOffline() && agent && waypoints != null && waypoints.Length > 0)
            {
                if (Vector3.Distance(transform.position, waypoints[currentIndex].position) <= waypointTolerance)
                {
                    currentIndex = (currentIndex + 1) % waypoints.Length;
                    agent.MoveTo(waypoints[currentIndex].position);
                }
            }
        }
    }
}