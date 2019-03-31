using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MatchHandler : MonoBehaviour
{
    [SerializeField] bool m_isSpawned = false;

    private void OnEnable()
    {
        //Register OnSceneLoaded to an Unity SceneManger Event
        //wich is called when the Scene is loaded
        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        Debug.Log(scene.name + " is loaded.");
        //if the Scene is loaded
        //enable Message Queue
        // to recieve messages
        //we can set this Client to be Ready
        //and waiting for
        PhotonNetwork.isMessageQueueRunning = true;
        PhotonNetwork.player.SetReadyState(true);
    }

    public void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
    {
        //PhotonPlayer m_photonPlayer = (PhotonPlayer)playerAndUpdatedProps[0];
        //Hashtable m_changedPropertie = (Hashtable)playerAndUpdatedProps[1];

        if (!m_isSpawned && PhotonNetwork.isMasterClient)
        {
            var m_photonPLayerList = PhotonNetwork.playerList;

            var m_notReadyPlayer = m_photonPLayerList.ToList().Exists(x => x.GetReadyState() == false);

            if (m_notReadyPlayer == false)
            {
                SpawnEvent();
            }
        }
    }

    void SpawnEvent()
    {
        Debug.Log("Match Handler Sends Spawn Event to  Player");
        if (PhotonNetwork.isMasterClient)
        {
            var m_photonPlayerList = PhotonNetwork.playerList;

            //sending Spawn Information to each Client
            foreach (PhotonPlayer pp in m_photonPlayerList)
            {
                NetworkEventHandler.SyncSpawnNode(pp.GetPlayerTeam(), pp);
            }
            m_isSpawned = true;
        }
    }

    private void OnDisable()
    {
        //unregister when Disabled or Destroyed
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }
}

