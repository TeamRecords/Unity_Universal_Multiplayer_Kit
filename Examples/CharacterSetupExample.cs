using UnityEngine;
using UMK.Core;
using UMK.Core.Character;

/// <summary>
/// Spawns a player character with UMK character components. Requires a prefab with CharacterAgent,
/// CharacterMotor, CharacterLook, CharacterEquipment and CharacterInput_Default already attached.
/// Attach this script to an empty GameObject in your bootstrap scene and assign the prefab.
/// </summary>
public class CharacterSetupExample : MonoBehaviour
{
    [Tooltip("Player prefab with UMK character components (CharacterAgent, CharacterMotor, CharacterLook, etc.)")]
    public GameObject playerPrefab;

    void Start()
    {
        // Only spawn on server/offline. Clients will receive spawn via Mirror if using a networked prefab.
        if (UMK_NetworkService.Instance.IsServerOrOffline && playerPrefab)
        {
            SpawnPlayer(Vector3.zero);
        }
    }

    void SpawnPlayer(Vector3 position)
    {
        var go = Instantiate(playerPrefab, position, Quaternion.identity);
            var agent = go.GetComponent<CharacterAgent>();
            if (agent != null)
            {
                // Assign a custom input source if desired; otherwise, CharacterInput_Default will be used automatically.
                // For example: agent.inputSource = new MyCustomInputSource();
            }
    }
}