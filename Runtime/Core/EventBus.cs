using System;
using System.Collections.Generic;

namespace UMK.Core
{
    /// <summary>
    /// Lightweight pub/sub bus used by various UMK subsystems to decouple senders from listeners.
    /// Any subsystem (characters, objects, audio emitters, terrain modifiers, etc.) can publish messages
    /// and subscribe to specific message types. Messages are dispatched on the main thread.
    /// </summary>
    public static class EventBus
    {
        // Internal dictionary mapping message types to subscriber lists
        private static readonly Dictionary<Type, List<Delegate>> _subscribers = new Dictionary<Type, List<Delegate>>();

        /// <summary>
        /// Subscribes to messages of a given type. When a message is published, the provided handler
        /// will be invoked with the message instance. Use in OnEnable and remove in OnDisable.
        /// </summary>
        /// <typeparam name="T">Message type</typeparam>
        /// <param name="handler">Callback to invoke when a message of type T is published</param>
        public static void Subscribe<T>(Action<T> handler)
        {
            if (handler == null) return;
            var type = typeof(T);
            if (!_subscribers.TryGetValue(type, out var list))
            {
                list = new List<Delegate>();
                _subscribers[type] = list;
            }
            if (!list.Contains(handler)) list.Add(handler);
        }

        /// <summary>
        /// Unsubscribes a handler from messages of a given type.
        /// </summary>
        public static void Unsubscribe<T>(Action<T> handler)
        {
            if (handler == null) return;
            var type = typeof(T);
            if (_subscribers.TryGetValue(type, out var list))
            {
                list.Remove(handler);
            }
        }

        /// <summary>
        /// Publishes a message to all subscribers of its type. Handlers execute synchronously.
        /// </summary>
        public static void Publish<T>(T msg)
        {
            var type = typeof(T);
            if (_subscribers.TryGetValue(type, out var list))
            {
                // Copy list to avoid issues if subscribers modify subscription during iteration
                var temp = new List<Delegate>(list);
                foreach (var del in temp)
                {
                    if (del is Action<T> cb)
                    {
                        try
                        {
                            cb(msg);
                        }
                        catch (Exception e)
                        {
                            UnityEngine.Debug.LogException(e);
                        }
                    }
                }
            }
        }
    }
}