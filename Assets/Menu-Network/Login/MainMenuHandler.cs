using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;




public class MainMenuHandler : MonoBehaviour
{

    [Header("Panels")]
    [SerializeField] Login m_login;
    [SerializeField] GameObject m_joinOptions;
    [SerializeField] GameObject m_loginPanel;
    [SerializeField] GameObject m_roomOverviewPanel;
    [SerializeField] GameObject m_serverMessages;




    [Header("Join Options")]
    [SerializeField] Button m_roomOverview;
    [SerializeField] Button m_mode;
    [SerializeField] Button m_matchmaking;
    [SerializeField] Text m_matchmakingInfo;

    [Header("Mode Options")]
    [SerializeField] GameObject m_modePanel;
    [SerializeField] GameObject m_modeTemplate;


    [SerializeField] Map m_waitingRoom;
    int m_pickedMode;



    void Start()
    {
        m_roomOverview.onClick.AddListener(OnClickRoomOverview);

        m_mode.onClick.AddListener(OnClickMode);

        m_matchmaking.onClick.AddListener(OnClickMatchmaking);

        System.Array value = System.Enum.GetValues(typeof(Mode));

        //Create for each GameMode a Button
        //Each Button should call OnClickMode and with the index
        //the index ref also to the gameMode
        //ignore default(0) start with 1
        for (int i = 1; i < value.Length; i++)
        {
            var t = Instantiate(m_modeTemplate.gameObject, m_modePanel.transform, false);
            var btn = t.GetComponent<Button>();
            var btnText = t.GetComponentInChildren<Text>();
            int mode = i;
            btn.onClick.AddListener(() => OnPickMode(mode));
            btnText.text = ((Mode)i).ToString();

        }
    }
    public void OnClickRoomOverview()
    {
        m_loginPanel.SetActive(false);
        m_serverMessages.SetActive(false);
        m_roomOverviewPanel.SetActive(true);
        m_joinOptions.SetActive(false);

    }
    public void OnClickMode()
    {
        m_modePanel.SetActive(!m_modePanel.activeInHierarchy);
    }
    public void OnPickMode(int mode)
    {
        m_pickedMode = mode;
        Debug.LogFormat("Picked Mode : {0}", m_pickedMode);
        m_modePanel.SetActive(false);
    }
    public void OnClickMatchmaking()
    {
        if (PhotonNetwork.connectedAndReady)
        {
            //Filter for Gamemodes
            //Room Player count will be ignored
            Hashtable expectedProperties = new Hashtable() { { RoomProperties.Gamemode, m_pickedMode } };

            m_matchmakingInfo.text = "looking for match...";
            //If the operation got queued and will be sent - returns true
            if (PhotonNetwork.GetRoomList().Length > 0)
            {
                PhotonNetwork.JoinRandomRoom(expectedProperties, 0);
                MapDatabase.LoadMap(m_waitingRoom);
            }
            else
            {
                m_matchmakingInfo.text = "No Open Room found.";
            }
        }
        else
        {
            m_matchmakingInfo.text = "Not Connected.";
        }
    }
    //Called form Photons API after successfully Joint a Room
    public void OnJoinedRoom()
    {
        Debug.Log("Joined Room through Matchamking - Room Filter");
        m_matchmakingInfo.text = "Successful!";
    }
    void OnPhotonRandomJoinFailed(object[] codeAndMsg)
    {
        Debug.Log((string)codeAndMsg[1]);
        m_matchmakingInfo.text = (string)codeAndMsg[1];
    }
}
