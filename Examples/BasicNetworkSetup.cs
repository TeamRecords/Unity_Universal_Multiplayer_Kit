using UnityEngine;
using UMK.Core;

/// <summary>
/// Demonstrates how to start a host, client or server using UMK_NetworkService from the UI.
/// Attach this script to a GUI canvas or any GameObject in your bootstrap scene.
/// </summary>
public class BasicNetworkSetup : MonoBehaviour
{
    private string clientAddress = "127.0.0.1";
    private void OnGUI()
    {
        const int width = 150;
        const int height = 30;
        int y = 10;
        // Host button
        if (GUI.Button(new Rect(10, y, width, height), "Start Host"))
        {
            UMK_NetworkService.Instance.StartHost();
        }
        y += height + 5;
        // Server button
        if (GUI.Button(new Rect(10, y, width, height), "Start Server"))
        {
            UMK_NetworkService.Instance.StartServer();
        }
        y += height + 5;
        // Client address input
        GUI.Label(new Rect(10, y, width, height), "Client IP:");
        clientAddress = GUI.TextField(new Rect(70, y, width, height), clientAddress);
        y += height + 5;
        // Client button
        if (GUI.Button(new Rect(10, y, width, height), "Connect Client"))
        {
            UMK_NetworkService.Instance.StartClient(clientAddress);
        }
    }
}