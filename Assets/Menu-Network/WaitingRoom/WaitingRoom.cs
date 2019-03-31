using System.Collections;
using UnityEngine;
using UnityEngine.UI;


public class WaitingRoomEvent
{
    public const byte InternalWaiting = 25;
    public const byte RoomIsFull = 26;
    public const byte StartMatch = 27;
}

public class WaitingRoom : MonoBehaviour
{

    [SerializeField]
    enum WaitingRoomTimer
    {
        none,
        Internal,
        Final,
        StartMatch
    }

    [SerializeField] WaitingRoomTimer m_waitingRoomState = WaitingRoomTimer.none;

    [Header("UI")] [SerializeField] Text m_playerInRoomDisplay;
    [SerializeField] Text m_timer;


    [Header("Database")] [SerializeField] MapDatabase m_mapDatabase;
    [SerializeField] Map m_lobby;
    Map m_roomMap;

    string m_loadedMapName; //Map Name from Photon Room Properties
    int m_playerInRoom = 0;
    int m_roomMaxPlayer;

    bool m_startInternalTimer = false;
    [SerializeField] float m_maxInternalWaitingLength = 60;
    float m_internalTimer = 0;
    [SerializeField] float m_finalCountdownLenght = 10;
    bool m_startFinalCountdown = false;
    float m_finalCountdown = 0;

    Mode m_currenGameMode;
    Coroutine m_pingroutine;

    private void OnEnable()
    {
        PhotonNetwork.OnEventCall += OnRecieveEvent;
        m_finalCountdown = m_finalCountdownLenght;

    }

    IEnumerator SetPing()
    {
        Room m_currentRoom = PhotonNetwork.room;
        m_currentRoom.SetPing(PhotonNetwork.GetPing());
        yield return new WaitForSeconds(1);
        m_pingroutine = StartCoroutine(SetPing());
    }

    #region Photon related

    public void OnJoinedRoom()
    {
        Room currentRoom = PhotonNetwork.room;

        string mapName = currentRoom.GetMap();
        m_roomMap = m_mapDatabase.GetMapWithMapName(mapName);

        int gamemodeIndex = currentRoom.GetGamemode();

        m_currenGameMode = (Mode)gamemodeIndex;

        Debug.Log("Gamemode : " + m_currenGameMode);

        //Instantiate MasterCLients Player
        if (PhotonNetwork.isMasterClient)
        {
            Gamemode.AddPlayerToTeam(m_currenGameMode, PhotonNetwork.masterClient);
            NetworkEventHandler.SyncSpawnNode(PhotonNetwork.player.GetPlayerTeam(), PhotonNetwork.masterClient);
            m_pingroutine = StartCoroutine(SetPing());
        }

        SetPlayerCount();
        IsRoomFull();
        m_startInternalTimer = true;
    }

    public void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        //Only MasterClient start Event to create Player
        //newPlayer is the new joind Client
        if (PhotonNetwork.isMasterClient)
        {
            Gamemode.AddPlayerToTeam(m_currenGameMode, newPlayer);
            NetworkEventHandler.SyncSpawnNode(newPlayer.GetPlayerTeam(), newPlayer);
        }

        SetPlayerCount();
        IsRoomFull();
    }

    void IsRoomFull()
    {
        if (PhotonNetwork.isMasterClient)
        {
            bool roomIsFull = m_playerInRoom == m_roomMaxPlayer ? true : false;
            if (roomIsFull)
            {
                SendEvent(WaitingRoomEvent.RoomIsFull);
            }
        }
    }

    private void Update()
    {
        //Start internal Timer for about 60secounds after that start the Match
        InternalTimer();
        FinalTimer();

    }

    private void FinalTimer()
    {
        if (m_startFinalCountdown)
        {
            m_finalCountdown -= Time.deltaTime;
            m_timer.text = m_finalCountdown.ToString("F0");
            if (m_finalCountdown <= 0)
            {
                m_finalCountdown = 0;
                m_startFinalCountdown = false;

                if (PhotonNetwork.isMasterClient)
                {
                    SendEvent(WaitingRoomEvent.StartMatch);
                }
            }
        }
    }

    private void InternalTimer()
    {
        if (m_startInternalTimer)
        {
            SendEvent(WaitingRoomEvent.InternalWaiting);

            m_internalTimer += Time.deltaTime;
            if (m_internalTimer >= m_maxInternalWaitingLength)
            {
                PhotonNetwork.room.MaxPlayers = m_playerInRoom;

                StopCoroutine(m_pingroutine);

                SendEvent(WaitingRoomEvent.RoomIsFull);
                m_internalTimer = 0;
                m_startInternalTimer = false;
            }
        }
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer player)
    {
        Debug.Log(player.NickName + " Disconnected");

        if (PhotonNetwork.isMasterClient)
        {
            PhotonNetwork.room.MaxPlayers = m_roomMaxPlayer - 1;
        }

        m_roomMaxPlayer = PhotonNetwork.room.MaxPlayers;

        SetPlayerCount();
    }

    #endregion Photon related


    #region UI stuff

    void SetPlayerCount()
    {
        m_playerInRoom = PhotonNetwork.room.PlayerCount;
        m_roomMaxPlayer = PhotonNetwork.room.MaxPlayers;
        m_playerInRoomDisplay.text = m_playerInRoom + " / " + m_roomMaxPlayer;
    }

    #endregion UI Stuff

    private bool m_mapLoaded = false;

    void LoadPickedMap()
    {
        if (m_mapLoaded)
        {
            return;
        }

        if (PhotonNetwork.player.IsLocal)
        {
            PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.player);
        }

        PhotonNetwork.isMessageQueueRunning = false;

        MapDatabase.LoadMap(m_roomMap);
        m_mapLoaded = true;

    }

    void LockRoom()
    {
        //IsVisible to false it wont show up in the Lobby List
        PhotonNetwork.room.IsVisible = false;
        //IsOpen nobody can join
        PhotonNetwork.room.IsOpen = false;
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
    }

    void SendEvent(byte eventcode)
    {
        var photonPlayerList = PhotonNetwork.playerList;

        foreach (PhotonPlayer pp in photonPlayerList)
        {
            PhotonNetwork.RaiseEvent(eventcode, new object[] { (float)PhotonNetwork.ServerTimestamp }, true,
                new RaiseEventOptions() { TargetActors = new int[] { pp.ID } });
        }
    }

    void OnRecieveEvent(byte eventcode, object content, int senderid)
    {
        switch (eventcode)
        {
            case WaitingRoomEvent.InternalWaiting:
                m_waitingRoomState = WaitingRoomTimer.Internal;
                break;

            case WaitingRoomEvent.RoomIsFull:
                LockRoom();
                m_waitingRoomState = WaitingRoomTimer.Final;
                //start small Timer


                object[] m_eventContent = (object[])content;

                float m_serverTimeStamp = (float)m_eventContent[0];
                float m_dif = (PhotonNetwork.ServerTimestamp - m_serverTimeStamp) / 1000.0f;


                m_startFinalCountdown = true;
                m_finalCountdown -= m_dif;

                break;

            case WaitingRoomEvent.StartMatch:

                PhotonNetwork.player.SetReadyState(false);
                m_waitingRoomState = WaitingRoomTimer.StartMatch;

                LoadPickedMap();
                break;
        }
    }

    private void OnDisable()
    {
        PhotonNetwork.OnEventCall -= OnRecieveEvent;
    }
}
