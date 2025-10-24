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
    }
#else
    public class NetBehaviourBase : MonoBehaviour
    {
        // Stubs for Mirror properties
        public bool isServer => true;
        public bool isClient => true;
        public bool isLocalPlayer => true;
    }
#endif
}