using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class NetworkEvent
{
    public const byte Spawn = 20;
}

public static class NetworkEvents
{

    public delegate Transform OnSyncSpawnNode(Teams.Team team);
    public static event OnSyncSpawnNode SyncSpawnNodeEvent;

    public delegate Transform OnTeamBasedRespawn(Teams.Team team);
    public static event OnTeamBasedRespawn TeamBasedRespawnEvent;

    public delegate Transform OnRespawnRandom();
    public static event OnRespawnRandom RespawnRandomEvent;



    public static void SyncSpawnNode(Teams.Team team, Player player)
    {
        if (SyncSpawnNodeEvent != null)
        {
            Transform spawnNode = SyncSpawnNodeEvent(team);

            PhotonNetwork.RaiseEvent(NetworkEvent.Spawn,
                new object[] { spawnNode.position, spawnNode.rotation },
                new RaiseEventOptions
                {
                    TargetActors = new int[] { player.ActorNumber },
                    CachingOption = EventCaching.AddToRoomCache
                },SendOptions.SendReliable);
        }
    }

    public static void TeamBasedRespawn(Teams.Team team, Player player)
    {
        if (SyncSpawnNodeEvent != null)
        {
            Transform spawnNode = TeamBasedRespawnEvent(team);

            PhotonNetwork.RaiseEvent(NetworkEvent.Spawn,
                new object[] { spawnNode.position, spawnNode.rotation },
                new RaiseEventOptions
                {
                    TargetActors = new int[] { player.ActorNumber },
                    CachingOption = EventCaching.AddToRoomCache
                }, SendOptions.SendReliable);
        }
    }

    public static void RespawnRandomSpawnNode(Player player)
    {
        if (SyncSpawnNodeEvent != null)
        {
            Transform spawnNode = RespawnRandomEvent();

            PhotonNetwork.RaiseEvent(NetworkEvent.Spawn,
                new object[] { spawnNode.position, spawnNode.rotation },
                new RaiseEventOptions
                {
                    TargetActors = new int[] { player.ActorNumber },
                    CachingOption = EventCaching.AddToRoomCache
                },SendOptions.SendReliable);
        }
    }
}

