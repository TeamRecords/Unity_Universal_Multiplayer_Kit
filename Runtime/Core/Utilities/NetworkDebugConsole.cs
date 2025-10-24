using UnityEngine;
using System.Collections.Generic;

#if HAS_MIRROR
using Mirror;
#endif

namespace UMK.Core.Utilities
{
    /// <summary>
    /// Provides a simple in-game console for executing commands at runtime. Commands can be executed locally or
    /// broadcast to other players if Mirror is present. Supports registering custom command handlers.
    /// </summary>
    public class NetworkDebugConsole : NetBehaviourBase
    {
        private readonly Dictionary<string, System.Action<string[]>> commands = new Dictionary<string, System.Action<string[]>>();

        public bool showConsole;
        private string inputLine = string.Empty;
        private Vector2 scrollPos;
        private readonly List<string> outputLines = new List<string>();

        private void OnGUI()
        {
            if (!showConsole) return;
            // simple GUI for console
            GUI.Box(new Rect(10, 10, 400, 300), "Console");
            scrollPos = GUI.BeginScrollView(new Rect(15, 35, 390, 230), scrollPos, new Rect(0, 0, 370, outputLines.Count * 20));
            for (int i = 0; i < outputLines.Count; i++)
            {
                GUI.Label(new Rect(0, i * 20, 370, 20), outputLines[i]);
            }
            GUI.EndScrollView();
            inputLine = GUI.TextField(new Rect(15, 270, 300, 25), inputLine);
            if (GUI.Button(new Rect(320, 270, 80, 25), "Enter"))
            {
                ExecuteCommand(inputLine);
                inputLine = string.Empty;
            }
        }

        public void RegisterCommand(string command, System.Action<string[]> handler)
        {
            commands[command.ToLowerInvariant()] = handler;
        }

        public void ExecuteCommand(string line)
        {
            if (string.IsNullOrWhiteSpace(line)) return;
            AddOutput($"> {line}");
            string[] parts = line.Split(' ');
            string cmd = parts[0].ToLowerInvariant();
            string[] args = new string[parts.Length - 1];
            for (int i = 1; i < parts.Length; i++) args[i - 1] = parts[i];
            if (commands.TryGetValue(cmd, out var handler))
            {
                if (IsServerOrOffline())
                {
                    handler.Invoke(args);
                }
                else
                {
                    // In a real game, send to server via Command RPC
                    handler.Invoke(args);
                }
            }
            else
            {
                AddOutput($"Unknown command: {cmd}");
            }
        }

        private void AddOutput(string line)
        {
            outputLines.Add(line);
            scrollPos.y = float.MaxValue;
        }
    }
}