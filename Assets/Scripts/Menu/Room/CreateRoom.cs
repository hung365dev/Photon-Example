
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Hashtable = ExitGames.Client.Photon.Hashtable;
using Photon.Pun;
using Photon.Realtime;
using SradnickDev.FlexGUI;
using Object = UnityEngine.Object;
using Random = UnityEngine.Random;

public class CreateRoom : MonoBehaviourPunCallbacks ,IFlexScreen
{

    [Header("Room Settings")]
    [SerializeField] private int m_maxPlayer = 20;

    [Header("Create Room Reference")]
    [SerializeField] private InputField m_roomNameField;
    [SerializeField] private  Dropdown m_gameMode;
    [SerializeField] private  Dropdown m_playercount;
    [SerializeField] private  Text m_playercountLabel;
    [SerializeField] private  Button m_createButton;

    [Header("Map Preview Reference")]
    [SerializeField] private Text m_mapName;
    [SerializeField] private Text m_mapDescription;
    [SerializeField] private MapDatabase m_mapDatabase;
    [SerializeField] private Transform m_mapCanvas;
    [SerializeField] private MapButton m_mapButton;
    [SerializeField] private Map m_waitingRoom;

    private Map m_pickedMap;

    private void Start()
    {
        m_gameMode.ClearOptions();
        m_gameMode.AddOptions(new List<string> { "Pick a Map" });
        m_playercount.ClearOptions();
        CreateMapButtons();
    }

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    /// <summary>Set Gamemode depends on the picked Map.</summary>
    private void SetGamemode()
    {
        List<string> modeToString = new List<string>();
        foreach (Map.AllowedGamemode allowedGameMode in m_pickedMap.allowedGamemode)
        {
            if (!allowedGameMode.allow) continue;
            string mode = allowedGameMode.mode.ToString();
            modeToString.Add(mode);
        }

        m_gameMode.AddOptions(modeToString);
        m_gameMode.onValueChanged.AddListener(SetPlayerCount);
    }
    private void CreateMapButtons()
    {
        for (var i = 0; i < m_mapDatabase.Count; i++)
        {
            if (!m_mapDatabase.Maps[i].IsMenuScene)
            {
                GameObject gbj = Instantiate(m_mapButton.gameObject, m_mapCanvas);
                MapButton mb = gbj.GetComponent<MapButton>();
                mb.map = m_mapDatabase.Maps[i];
                mb.image.sprite = mb.map.MapImage;
                mb.button.onClick.AddListener(() => SetMap(mb.map));
            }
        }
    }
    private void SetMap(Map map)
    {
        m_gameMode.ClearOptions();
        m_pickedMap = map;
        m_mapName.text = map.GetMapName;
        m_mapDescription.text = map.GetMapDescription;
        SetGamemode();
        SetPlayerCount(0);
        m_createButton.onClick.AddListener(OnClickCreateRoom);
    }
    private void SetPlayerCount(int dropdownIndex)
    {
        Mode parsedEnum = (Mode)System.Enum.Parse(typeof(Mode), m_gameMode.options[dropdownIndex].text);
        List<string> playercount = new List<string>();
        List<string> teamSize = new List<string>();

        for (var i = 1; i <= m_maxPlayer; i++)
        {
            if (i % 5 == 0)
            {
                playercount.Add(i.ToString());
            }

            if (i % 4 == 0)
            {
                teamSize.Add((i / 2).ToString());
            }
        }

        switch (parsedEnum)
        {
            case Mode.DeatchMatch:
                m_playercountLabel.text = "Playercount :";
                this.m_playercount.ClearOptions();
                this.m_playercount.AddOptions(playercount);
                break;
            case Mode.TeamDeathMatch:
                m_playercountLabel.text = "Team Size :";
                this.m_playercount.ClearOptions();
                this.m_playercount.AddOptions(teamSize);
                break;
            case Mode.none:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

    }
    private void OnClickCreateRoom()
    {
        Mode parsedEnum = (Mode)System.Enum.Parse(typeof(Mode), m_gameMode.options[m_gameMode.value].text);

        var maxPlayer = 1;

        switch (parsedEnum)
        {
            case Mode.DeatchMatch:
                maxPlayer = System.Convert.ToInt32(m_playercount.options[m_playercount.value].text);
                break;
            case Mode.TeamDeathMatch:
                maxPlayer = System.Convert.ToInt32(m_playercount.options[m_playercount.value].text) * 2;
                break;
            case Mode.none:
                break;
            default:
                throw new ArgumentOutOfRangeException();
        }

        //Create a new Hashtable and store room Properties
        Hashtable customRoomOption = new Hashtable()
        {
            {RoomProperties.Map,m_pickedMap.GetMapName },
            {RoomProperties.Gamemode, parsedEnum},
        };
        //set Room Options and store the properties in it
        //To get/set properties in the Looby set CustomRoomPropertiesForLobby
        RoomOptions roomOptions = new RoomOptions()
        {
            BroadcastPropsChangeToAll = true,
            IsVisible = true,
            IsOpen = true,
            PublishUserId = true,
            MaxPlayers = (byte)maxPlayer,
            CustomRoomPropertiesForLobby = new string[] { RoomProperties.Map, RoomProperties.Gamemode, RoomProperties.Ping },
            CustomRoomProperties = customRoomOption,

        };
        
        string t = m_roomNameField.text;
        t = t.Replace(" ",string.Empty);
        
        string newRoomName = m_roomNameField.text == "" ? RoomHash() : m_roomNameField.text;
        
        Debug.Log(newRoomName);

        //TypedLobby defautlLobby = new TypedLobby("PhotonSample",LobbyType.Default);

        bool created = PhotonNetwork.CreateRoom(newRoomName, roomOptions);
        Debug.Log(created);
        if (created) return;
        string rndRoomHash = RoomHash();
        Debug.Log(rndRoomHash);
        newRoomName = m_roomNameField.text + rndRoomHash;
        PhotonNetwork.CreateRoom(newRoomName, roomOptions, TypedLobby.Default);

        //Waiting for CallBack "OnJoinedRoom" to load the Map.
        //add some loading animation..
    }

    private string RoomHash()
    {
        return "#" + Random.Range(0, 999);
    }
    public override void OnJoinedRoom()
    {
        Debug.Log("Lobby <-> JoinedRoom");
        LoadingScreen.LoadScene(m_waitingRoom.SceneIndex);
    }
    private void OnPhotonCreateRoomFailed(Object[] codeAndMessage)
    {
        print("Create room failed: " + codeAndMessage[1]);
        //remove loading animtion
        //reset stuff if needed, so the client can try again
    }
}
