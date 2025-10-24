using UnityEngine;
using UnityEngine.UI;

namespace UMK.Core.UI
{
    /// <summary>
    /// Basic network chat component. Users can enter text into an InputField. When submitted, the message
    /// is sent to all clients via Mirror (if present) or shown locally if not networked. Received
    /// messages are appended to a chat log. Also publishes ChatMessageEvent via EventBus.
    /// </summary>
    public class NetworkChat : NetBehaviourBase
    {
        public InputField inputField;
        public Text chatLog;

        /// <summary>
        /// Call this from a button or on InputField endEdit event to send the current message.
        /// </summary>
        public void SendCurrent()
        {
            if (inputField == null) return;
            string message = inputField.text;
            if (string.IsNullOrWhiteSpace(message)) return;
            string author = gameObject.name;
#if HAS_MIRROR
            if (isLocalPlayer)
            {
                CmdSendChat(author, message);
            }
            else
            {
                // Local copy if offline
                AppendChat(author, message);
            }
#else
            AppendChat(author, message);
#endif
            inputField.text = string.Empty;
        }

        private void AppendChat(string author, string message)
        {
            if (chatLog != null)
            {
                chatLog.text += $"\n{author}: {message}";
            }
            EventBus.Publish(new ChatMessageEvent { author = author, message = message });
        }

#if HAS_MIRROR
        // Mirror RPCs
        [Mirror.ServerRpc]
        private void CmdSendChat(string author, string message)
        {
            RpcReceiveChat(author, message);
        }
        [Mirror.ClientRpc]
        private void RpcReceiveChat(string author, string message)
        {
            AppendChat(author, message);
        }
#endif
        public struct ChatMessageEvent
        {
            public string author;
            public string message;
        }
    }
}