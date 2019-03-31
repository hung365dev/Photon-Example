using UnityEngine;


public class PlayerHandler : MonoBehaviour
{

    [SerializeField] private PlayerList m_playerList;
    [SerializeField] private GameObject m_playerCountryball;
    [SerializeField] private GameObject m_playerCamera;
    [SerializeField] private RespawnHandler m_respawnHandler;
    private GameObject m_spawnedCam;

    private void OnEnable()
    {
        PhotonNetwork.OnEventCall += OnEvent;
    }

    private void OnDisable()
    {
        PhotonNetwork.OnEventCall -= OnEvent;
    }

    #region CreatePlayer

    private void CreatePlayer(Vector3 spawnNode, Quaternion rotationNode)
    {
        GameObject playerObject = PhotonNetwork.Instantiate(m_playerCountryball.name, spawnNode, Quaternion.identity, 0);
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
    void OnEvent(byte eventcode, object content, int senderid)
    {
        switch (eventcode)
        {
            case NetworkEvent.Spawn:
                object[] data = (object[])content;
                Vector3 position = (Vector3)data[0];
                Quaternion rotation = (Quaternion)data[1];

                CreatePlayer(position, rotation);

                break;
        }
    }
}
