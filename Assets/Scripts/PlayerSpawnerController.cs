using Fusion;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerSpawnerController : NetworkBehaviour, IPlayerJoined, IPlayerLeft
{
    [SerializeField] private NetworkPrefabRef playerNetworkPrefab = NetworkPrefabRef.Empty;
    [SerializeField] private Transform[] spawnPoints;
    public override void Spawned()
    {
        if(Runner.IsServer)
        {
            foreach(var player in Runner.ActivePlayers)
            {
                SpawnPlayer(player);
            }
        }
    }
    private void SpawnPlayer(PlayerRef player)
    {
        if(Runner.IsServer)
        {
            var playerObject = Runner.Spawn(playerNetworkPrefab, spawnPoints[Random.Range(0, spawnPoints.Length)].position, Quaternion.identity, player);
            Runner.SetPlayerObject(player, playerObject);
        }
    }
    public void PlayerJoined(PlayerRef player)
    {
        SpawnPlayer(player);
    }

    public void PlayerLeft(PlayerRef player)
    {
        if(Runner.IsServer)
        {
            if(Runner.TryGetPlayerObject(player, out var playerObject))
                Runner.Despawn(playerObject);
            Runner.SetPlayerObject(player, null);
        }
    }
}
