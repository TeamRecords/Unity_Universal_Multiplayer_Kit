using UnityEngine;

namespace UMK.Core
{
    /// <summary>
    /// Base class for network-aware behaviours. When Mirror is present (HAS_MIRROR define), this class
    /// inherits from Mirror.NetworkBehaviour to enable RPCs, SyncVars and other features. Otherwise it
    /// falls back to MonoBehaviour so your code still compiles and runs in offline/local only mode.
    /// </summary>
#if HAS_MIRROR
    public class NetBehaviourBase : Mirror.NetworkBehaviour
    {
        /// <summary>
        /// Returns true when running on the server or in offline (non-Mirror) mode. In Mirror builds this is equivalent to isServer;
        /// in non-Mirror builds it always returns true. Use this to guard server-only logic.
        /// </summary>
        public bool IsServerOrOffline()
        {
            return isServer;
        }
    }
#else
    public class NetBehaviourBase : MonoBehaviour
    {
        // Stubs for Mirror properties
        public bool isServer => true;
        public bool isClient => true;
        public bool isLocalPlayer => true;
        /// <summary>
        /// Returns true when running on the server or in offline mode. In non-Mirror builds this always returns true.
        /// </summary>
        public bool IsServerOrOffline()
        {
            return true;
        }
    }
#endif
}