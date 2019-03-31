using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class PlayerHandler : MonoBehaviourPunCallbacks, IOnEventCallback
{

    [SerializeField] private PlayerList m_playerList;
    [SerializeField] private GameObject m_playerPrefab;
    [SerializeField] private GameObject m_playerCamera;
    [SerializeField] private RespawnHandler m_respawnHandler;
    private GameObject m_spawnedCam;

    public void OnEvent(EventData photonEvent)
    {

        switch (photonEvent.Code)
        {
            case NetworkEvent.Spawn:
                Debug.Log("Received Spawn Event from: " + photonEvent.Sender);
                object[] data = (object[])photonEvent.CustomData;
                Vector3 position = (Vector3)data[0];
                Quaternion rotation = (Quaternion)data[1];

                CreatePlayer(position, rotation);

                break;
        }
    }

    #region CreatePlayer

    private void CreatePlayer(Vector3 spawnNode, Quaternion rotationNode)
    {
        GameObject playerObject = PhotonNetwork.Instantiate(m_playerPrefab.name, spawnNode, Quaternion.identity, 0);
        if (m_spawnedCam == null)
        {
            GameObject cameraObject = Instantiate(m_playerCamera, new Vector3(0, 0, 0), Quaternion.identity);
            m_spawnedCam = cameraObject;
        }

        SetUpComponents(playerObject, m_spawnedCam);
    }

    private void SetUpComponents(GameObject playerObject, GameObject cameraObject)
    {
        var healthHandler = playerObject.GetComponent<HealthHandler>();
        m_respawnHandler.Init(healthHandler);

        var cameraBehaviour = cameraObject.GetComponent<BasicCameraBehaviour>();
        cameraBehaviour.SetTarget(playerObject.transform);
        cameraObject.SetActive(true);
    }

    #endregion CreatePlayer
}
