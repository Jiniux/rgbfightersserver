﻿using System.Collections;
using System.Collections.Generic;
using System.Linq;
using LiteNetLib;
using UnityEngine;

public class NetworkPlayers : MonoBehaviour
{
    private NetworkManager _networkManager;

    private bool _teamSelect;
    public GameObject playerObject;

    public List<Player> players;


    private void Start()
    {
        _networkManager = GetComponent<NetworkManager>();

        players = new List<Player>(_networkManager.connectedPeerLimit);
    }

    public void StartPlayer(string[] playerData, NetPeer peer)
    {
        if (players.Find(x => x.Name.Equals(playerData[1])))
            NetworkManager.DisconnectClient("Username Already in use", peer);

        _networkManager.networkMap.SendMatchStatus(peer);
        _networkManager.SendChatMessage($"ChatMessage@Server:{playerData[1]} Si è Connesso!");
    }

    public void SpawnPlayer(string[] playerData, NetPeer peer)
    {
        var playerColor = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));

        var spawnPoint =
            _networkManager.networkMap.spawnPoints[Random.Range(0, _networkManager.networkMap.spawnPoints.Count)];

        var newPlayer = Instantiate(playerObject).AddComponent<Player>();

        newPlayer.Spawn(playerData[1], _teamSelect ? "etero" : "rgb", peer, spawnPoint, playerColor);
        _teamSelect = !_teamSelect;
        
        players.Add(newPlayer);

        _networkManager.SendMessageToClient(
            $"PlayerInformation@{peer.Id}@{newPlayer.Team}@{playerColor.r}@{playerColor.g}@{playerColor.b}@{spawnPoint.x}@{spawnPoint.y}@{spawnPoint.z}",
            peer);
        SendPlayerListToClients();
    }

    private void SendPlayerListToClients()
    {
        var playerInformation = new List<string>();

        playerInformation.AddRange(from p in players
            where p.Body
            select
                $"{p.Name}&{p.Peer.Id}&{p.Team}&{p.Color.r}&{p.Color.g}&{p.Color.b}&{p.Body.transform.position.x}&{p.Body.transform.position.y}&{p.Body.transform.position.z}");

        if (playerInformation.Count == 0) return;

        var playerList = string.Join("@", playerInformation);

        //Debug.Log(playerList);

        _networkManager.SendMessageToClient($"PlayersList@{playerList}");
    }

    public void MovePlayer(string[] playerData, NetPeer peer)
    {
        if (!_networkManager.networkMap.gameplayState.Equals(1)) return;

        var movingPlayer = FindPlayer(peer);

        movingPlayer?.netPlayer.MovePlayer(playerData);

        SendPlayerPositionToClients(peer, playerData);
    }

    public IEnumerator KillPlayer(string[] data, NetPeer peer)
    {
        _networkManager.SendMessageToClient($"PlayerDead@{peer.Id}");
        yield return new WaitForSeconds(5);
        _networkManager.SendMessageToClient($"PlayerRespawn@{peer.Id}");
    }
    
    public Player FindPlayer(NetPeer peer)
    {
        return players.Find(x => x.Peer == peer);
    }

    public void ClearPlayers()
    {
        players.Clear();
    }

    private void SendPlayerPositionToClients(NetPeer peer, IReadOnlyList<string> playerData)
    {
        _networkManager.SendMessageToClient(
            $"PlayerPosition@{peer.Id}@{playerData[1]}@{playerData[2]}@{playerData[3]}@{playerData[4]}@{playerData[5]}@{playerData[6]}@{playerData[7]}@{playerData[8]}@{playerData[9]}@{playerData[10]}@{playerData[11]}@{playerData[12]}");
    }
}