using System.Collections.Generic;
using UnityEngine;

public class SpawnNode : MonoBehaviour
{


    /// <summary>SpawnPoints</summary>
    [System.Serializable]
    public class Node
    {
        public Transform Transform;
        public bool Used = false;

        public Node(Transform transform)
        {
            Transform = transform;
        }
    }
    /// <summary>Stored SpawnPoints for a Team</summary>
    [System.Serializable]
    public class Spawn
    {
        [HideInInspector] public string Name;
        public Teams.Team TeamArea; 
        public Transform NodeContainer;
        public List<Node> Nodes;
    }

    [SerializeField] private List<Spawn> Nodes;

    private void OnEnable()
    {
        // register SpawnObject()
        NetworkEvents.SyncSpawnNodeEvent += GetNodeBasedOnTeam;
        NetworkEvents.TeamBasedRespawnEvent += TeamBasedRespawn;
        NetworkEvents.RespawnRandomEvent += RandomSpawnPoint;
    }
    private void OnDisable()
    {
        //remove from register
        NetworkEvents.SyncSpawnNodeEvent -= GetNodeBasedOnTeam;
        NetworkEvents.TeamBasedRespawnEvent -= TeamBasedRespawn;
        NetworkEvents.RespawnRandomEvent -= RandomSpawnPoint;
    }
    public void GetNodes()
    {
        Debug.LogWarning("Try to find Spawn Nodes.");
        foreach (Spawn node in Nodes)
        {
            node.Nodes.Clear();
            node.Name = node.TeamArea.ToString();
            Transform[] nodes = node.NodeContainer.transform.GetComponentsInChildren<Transform>();
            foreach (Transform t in nodes)
            {
                if(t != node.NodeContainer)
                    node.Nodes.Add(new Node(t));
            }
        }
    }
    
    
    /// <summary>Return a unused SpawnNode based on Team</summary>
    private Transform GetNodeBasedOnTeam(Teams.Team team)
    {
        var teamNode = Nodes.Find(x => x.TeamArea == team);
        var notUsedNode = teamNode.Nodes.Find(x => x.Used == false);

        if (notUsedNode == null)
        {
            Debug.LogWarningFormat("Not enough Spawn Nodes for Team {0} created.Used one which was already used .Please add more.", team);
            notUsedNode = teamNode.Nodes[Random.Range(0, teamNode.Nodes.Count - 1)];
        }
        notUsedNode.Used = true;
        return notUsedNode.Transform;
    }
    /// <summary>Return a Random SpawnNode Based on Team</summary>
    private Transform TeamBasedRespawn(Teams.Team team)
    {
        var teamNode = Nodes.Find(x => x.TeamArea == team);
        return teamNode.Nodes[Random.Range(0, teamNode.Nodes.Count)].Transform;
    }
    /// <summary>Return a Random SpawnNode from all Lists</summary>
    private Transform RandomSpawnPoint()
    {
        var teamNode = Nodes[Random.Range(0, Nodes.Count)];

        return teamNode.Nodes[Random.Range(0, teamNode.Nodes.Count)].Transform;
    }
}
