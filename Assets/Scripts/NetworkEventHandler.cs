using UnityEngine;
public class NetworkEvent
{
    public const byte Spawn = 20;
}

public class NetworkEventHandler : MonoBehaviour
{

    public delegate Transform OnSyncSpawnNode(Teams.Team team);
    public static event OnSyncSpawnNode SyncSpawnNodeEvent;

    public delegate Transform OnTeamBasedRespawn(Teams.Team team);
    public static event OnTeamBasedRespawn TeamBasedRespawnEvent;

    public delegate Transform OnRespawnRandom();
    public static event OnRespawnRandom RespawnRandomEvent;



    public static void SyncSpawnNode(Teams.Team team, PhotonPlayer player)
    {
        if (SyncSpawnNodeEvent != null)
        {
            Transform spawnNode = SyncSpawnNodeEvent(team);

            PhotonNetwork.RaiseEvent(NetworkEvent.Spawn,
                new object[] { spawnNode.position, spawnNode.rotation }, true,
                new RaiseEventOptions
                {
                    TargetActors = new int[] { player.ID },
                    CachingOption = EventCaching.AddToRoomCache
                });
        }
    }

    public static void TeamBasedRespawn(Teams.Team team, PhotonPlayer player)
    {
        if (SyncSpawnNodeEvent != null)
        {
            Transform spawnNode = TeamBasedRespawnEvent(team);

            PhotonNetwork.RaiseEvent(NetworkEvent.Spawn,
                new object[] { spawnNode.position, spawnNode.rotation }, true,
                new RaiseEventOptions
                {
                    TargetActors = new int[] { player.ID },
                    CachingOption = EventCaching.AddToRoomCache
                });
        }
    }

    public static void RespawnRandomSpawnNode(PhotonPlayer player)
    {
        if (SyncSpawnNodeEvent != null)
        {
            Transform spawnNode = RespawnRandomEvent();

            PhotonNetwork.RaiseEvent(NetworkEvent.Spawn,
                new object[] { spawnNode.position, spawnNode.rotation }, true,
                new RaiseEventOptions
                {
                    TargetActors = new int[] { player.ID },
                    CachingOption = EventCaching.AddToRoomCache
                });
        }
    }
}

