#if !HAS_MIRROR
using UnityEngine;
namespace Mirror
{
    public class NetworkBehaviour : MonoBehaviour { public bool isServer=>true; public bool isClient=>true; public bool isLocalPlayer=>true; }
    public class NetworkIdentity : MonoBehaviour {}
    public class NetworkManager : MonoBehaviour
    {
        public static NetworkManager singleton;
        public string networkAddress = "localhost";
        void Awake(){ if (singleton == null) singleton = this; }
        public void StartHost(){ Debug.Log("[UMK Shim] StartHost (no Mirror present)"); }
        public void StartClient(){ Debug.Log("[UMK Shim] StartClient (no Mirror present)"); }
        public void StartServer(){ Debug.Log("[UMK Shim] StartServer (no Mirror present)"); }
    }
    public class NetworkClient { public static bool isConnected => false; }
    public static class NetworkTime { public static double rtt => 0; }
}
#endif