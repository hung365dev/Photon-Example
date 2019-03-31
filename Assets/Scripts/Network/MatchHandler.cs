using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using Photon.Pun;
using Photon.Realtime;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class MatchHandler : MonoBehaviourPunCallbacks, IInRoomCallbacks
{

    public override void OnEnable()
    {
        base.OnEnable();
        //Register OnSceneLoaded to an Unity SceneManger Event
        //wich is called when the Scene is loaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    public override void OnDisable()
    {
        base.OnDisable();
        //unregister when Disabled or Destroyed
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log(scene.name + " is loaded.");
        //if the Scene is loaded
        //enable Message Queue
        // to recieve messages
        //we can set this Client to be Ready
        //and waiting for match start
        PhotonNetwork.IsMessageQueueRunning = true;
        PhotonNetwork.LocalPlayer.SetReady(true);
    }
    public override void OnPlayerPropertiesUpdate(Player targetPlayer,Hashtable changedProps)
    {
        
        if (!PhotonNetwork.IsMasterClient) return;
        Debug.Log("OnPlayerPropertiesUpdate : " + this);
        Player[] photonPLayerList = PhotonNetwork.PlayerList;

        bool notReadyPlayer = photonPLayerList.ToList().Exists(x => x.IsReady() == false);

        if (notReadyPlayer == false)
        {
            SpawnEvent();
        }
    }

    private void SpawnEvent()
    {
        Debug.Log("Match Handler Sends Spawn Event to  Player");
        if (!PhotonNetwork.IsMasterClient) return;
        
        Player[] photonPlayerList = PhotonNetwork.PlayerList;

        //sending Spawn Information to each Client
        foreach (Player pp in photonPlayerList)
        {
            NetworkEvents.SyncSpawnNode(pp.GetPlayerTeam(), pp);
        }
    }
}

