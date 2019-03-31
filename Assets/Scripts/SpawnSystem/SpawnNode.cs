using System.Collections.Generic;
using UnityEngine;

public class SpawnNode : MonoBehaviour
{


    /// <summary>SpawnPoints</summary>
    [System.Serializable]
    public class Node
    {
        public Transform transform;
        public bool used = false;
    }
    /// <summary>Stored SpawnPoints for a Team</summary>
    [System.Serializable]
    public class Spawn
    {
        public Teams.Team teamArea;
        public List<Node> nodes;
    }

    [SerializeField] List<Spawn> spawnList;

    private void OnEnable()
    {
        // register SpawnObject()
        NetworkEventHandler.SyncSpawnNodeEvent += GetNodeBasedOnTeam;
        NetworkEventHandler.TeamBasedRespawnEvent += TeamBasedRespawn;
        NetworkEventHandler.RespawnRandomEvent += RandomSpawnPoint;

    }
    private void OnDisable()
    {
        //remove from register
        NetworkEventHandler.SyncSpawnNodeEvent -= GetNodeBasedOnTeam;
        NetworkEventHandler.TeamBasedRespawnEvent -= TeamBasedRespawn;
        NetworkEventHandler.RespawnRandomEvent -= RandomSpawnPoint;
    }
    /// <summary>Return a unused SpawnNode based on Team</summary>
    private Transform GetNodeBasedOnTeam(Teams.Team team)
    {
        var teamNode = spawnList.Find(x => x.teamArea == team);
        var notUsedNode = teamNode.nodes.Find(x => x.used == false);

        if (notUsedNode == null)
        {
            Debug.LogWarningFormat("Not enough Spawn Nodes for Team {0} created. Please add more.", team);
            return null;
        }
        notUsedNode.used = true;
        return notUsedNode.transform;
    }
    /// <summary>Return a Random SpawnNode Based on Team</summary>
    private Transform TeamBasedRespawn(Teams.Team team)
    {
        var teamNode = spawnList.Find(x => x.teamArea == team);
        return teamNode.nodes[Random.Range(0, teamNode.nodes.Count)].transform;
    }
    /// <summary>Return a Random SpawnNode from all Lists</summary>
    private Transform RandomSpawnPoint()
    {
        var teamNode = spawnList[Random.Range(0, spawnList.Count)];

        return teamNode.nodes[Random.Range(0, teamNode.nodes.Count)].transform;
    }
}
