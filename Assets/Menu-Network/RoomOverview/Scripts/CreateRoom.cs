
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class CreateRoom : MonoBehaviour
{
    [Header("Create Room")]
    [SerializeField] InputField roomNameField;
    [SerializeField] Dropdown gameMode;
    [SerializeField] Dropdown playercount;
    [SerializeField] Text playercountLabel;
    [SerializeField] Button createButton;

    [Header("Map")]
    [SerializeField] Text mapName;
    [SerializeField] Text mapDescription;
    [SerializeField] MapDatabase mapDatabase;
    [SerializeField] Transform mapCanvas;
    [SerializeField] MapButton mapButton;
    [SerializeField] Map waitingRoom;

    Map m_pickedMap;

    private void Start()
    {
        gameMode.ClearOptions();
        gameMode.AddOptions(new List<string> { "Pick a Map" });
        playercount.ClearOptions();
        CreateMapButtons();
    }
    /// <summary>Set Gamemode depends on the picked Map.</summary>
    private void SetGamemode()
    {
        List<string> modeToString = new List<string>();
        for (int i = 0; i < m_pickedMap.allowedGamemode.Count; i++)
        {
            if (m_pickedMap.allowedGamemode[i].allow)
            {
                string mode = m_pickedMap.allowedGamemode[i].mode.ToString();
                //From String to Enum use
                //Mode parsedEnum = (Mode)System.Enum.Parse(typeof(Mode),mode);
                modeToString.Add(mode);
            }
        }

        gameMode.AddOptions(modeToString);
        gameMode.onValueChanged.AddListener(SetPlayerCount);
    }
    void CreateMapButtons()
    {
        for (int i = 0; i < mapDatabase.Count; i++)
        {
            if (!mapDatabase.Maps[i].IsMenuScene)
            {
                GameObject gbj = Instantiate(mapButton.gameObject, mapCanvas);
                MapButton mb = gbj.GetComponent<MapButton>();
                mb.map = mapDatabase.Maps[i];
                mb.image.sprite = mb.map.MapImage;
                mb.button.onClick.AddListener(() => SetMap(mb.map));
            }
        }
    }
    void SetMap(Map map)
    {
        gameMode.ClearOptions();
        m_pickedMap = map;
        mapName.text = map.GetMapName;
        mapDescription.text = map.GetMapDescription;
        SetGamemode();
        SetPlayerCount(0);
        createButton.onClick.AddListener(OnClickCreateRoom);
    }
    void SetPlayerCount(int dropdownIndex)
    {
        Mode parsedEnum = (Mode)System.Enum.Parse(typeof(Mode), gameMode.options[dropdownIndex].text);
        List<string> m_playercount = new List<string>()
        {
            "1","2","3","4","5","6","7","8"
        };
        List<string> m_teamSize = new List<string>()
        {
            "2","3","4"
        };
        switch (parsedEnum)
        {
            case Mode.DeatchMatch:
                playercountLabel.text = "Playercount :";
                playercount.ClearOptions();
                playercount.AddOptions(m_playercount);
                break;
            case Mode.TeamDeathMatch:
                playercountLabel.text = "Team Size :";
                playercount.ClearOptions();
                playercount.AddOptions(m_teamSize);
                break;
        }

    }
    void OnClickCreateRoom()
    {
        Mode parsedEnum = (Mode)System.Enum.Parse(typeof(Mode), gameMode.options[gameMode.value].text);

        int m_maxPlayer = 1;

        switch (parsedEnum)
        {
            case Mode.DeatchMatch:
                m_maxPlayer = System.Convert.ToInt32(playercount.options[playercount.value].text);
                break;
            case Mode.TeamDeathMatch:
                m_maxPlayer = System.Convert.ToInt32(playercount.options[playercount.value].text) * 2;
                break;
        }

        PhotonNetwork.automaticallySyncScene = true;

        //Create a new Hashtable and store room Properties
        Hashtable m_customRoomOption = new Hashtable()
        {
            {RoomProperties.Map,m_pickedMap.GetMapName },
            {RoomProperties.Gamemode, parsedEnum},
        };
        //set Room Options and store the properties in it
        //To get/set properties in the Looby set CustomRoomPropertiesForLobby
        RoomOptions m_roomOptions = new RoomOptions()
        {
            IsVisible = true,
            IsOpen = true,
            MaxPlayers = (byte)m_maxPlayer,
            CustomRoomPropertiesForLobby = new string[] { RoomProperties.Map, RoomProperties.Gamemode, RoomProperties.Ping },
            CustomRoomProperties = m_customRoomOption,

        };

        List<RoomInfo> roomInfo = new List<RoomInfo>();
        roomInfo.AddRange(PhotonNetwork.GetRoomList());
        var doubleRoom = roomInfo.Find(x => x.Name == roomNameField.text);
        string newRoomName = roomNameField.text + "#" + Random.Range(0, 999);

        if (doubleRoom == null)
        {
            if (PhotonNetwork.CreateRoom(newRoomName, m_roomOptions, TypedLobby.Default))
            {
                MapDatabase.LoadMap(waitingRoom);
            }
        }
        else
        {
            newRoomName = roomNameField.text + "#" + Random.Range(0, 999);
            if (PhotonNetwork.CreateRoom(newRoomName, m_roomOptions, TypedLobby.Default))
            {
                MapDatabase.LoadMap(waitingRoom);
            }
        }

    }
    private void OnPhotonCreateRoomFailed(Object[] codeAndMessage)
    {
        print("Create room failed: " + codeAndMessage[1]);
    }

}
