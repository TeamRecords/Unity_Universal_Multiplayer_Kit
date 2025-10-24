using UnityEngine;

namespace UMK.Core.ObjectAction
{
    /// <summary>
    /// Example implementation of IObjectAction. Invoking PerformAction will publish an ObjectActionEvent
    /// on the EventBus. You can extend this class or implement IObjectAction directly to perform custom
    /// behaviours on activation (e.g., explode, activate trap, toggle animation).
    /// </summary>
    public class ObjectAction : MonoBehaviour, IObjectAction
    {
        [Tooltip("Name or description of this action")]
        public string actionName = "Action";
        public void PerformAction(NetBehaviourBase caller)
        {
            EventBus.Publish(new ObjectActionEvent { actionName = actionName, caller = caller, target = this });
            // Custom logic goes here
        }
        public struct ObjectActionEvent
        {
            public string actionName;
            public NetBehaviourBase caller;
            public ObjectAction target;
        }
    }
}