using System.Collections;
using ExitGames.Client.Photon;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

/// <summary> Used for WaitingRoom.</summary>
public class WaitingRoomEvent
{
	public const byte CloseRoom = 26;
	public const byte StartMatch = 27;
}

public class WaitingRoom : MonoBehaviourPunCallbacks, IOnEventCallback
{
	

	[Header("Timer"), SerializeField]
	
	private float InternalTimerLength = 60;
	[SerializeField] private float FinalTimerLength = 10;
	[SerializeField] private TimerState TimerStateState = TimerState.None;
	

	[SerializeField] private Map LobbyMap;

	[Header("Database"), SerializeField] 
    
	private MapDatabase MapDatabase;

	private Coroutine m_pingroutine;
	private RoomStats m_roomStats;
	private bool m_timerIsRunning;

	[Header("UI"), SerializeField] 
    
	private Text PlayerInRoomDisplay;

	[SerializeField] private Text TimerDisplay;

	

	public void OnEvent(EventData photonEvent)
	{
		Debug.Log("Received Event");
		switch (photonEvent.Code)
		{
			case WaitingRoomEvent.CloseRoom:

				LockRoom();
				TimerStateState = TimerState.Final;

				object[] eventContent = (object[]) photonEvent.CustomData;

				var serverTimeStamp = (float) eventContent[0];
				float dif = (PhotonNetwork.ServerTimestamp - serverTimeStamp) / 1000.0f;

				FinalTimerLength -= dif;
				m_timerIsRunning = true;

				break;

			case WaitingRoomEvent.StartMatch:

				PhotonNetwork.LocalPlayer.SetReady(false);
				TimerStateState = TimerState.Final;

				LoadPickedMap();
				break;
		}
	}


	public void Start()
	{
		GetRoomStats();
		PhotonNetwork.IsMessageQueueRunning = true;
		//Send Spawn Event to MasterClient
		if (!PhotonNetwork.IsMasterClient) return;
		
		TimerStateState = TimerState.Internal;

		SendSpawnEvent(PhotonNetwork.MasterClient);

		m_pingroutine = StartCoroutine(SetPing());
		RoomUtilization();
		SetPlayerCount();
		m_timerIsRunning = true;
	}


	private void GetRoomStats()
	{
		Room currentRoom = PhotonNetwork.CurrentRoom;
		SetRoomStats(currentRoom);
	}

    /// <summary>Save Room Infos.</summary>
    /// <param name="room">Current Room</param>
    private void SetRoomStats(Room room)
	{
		byte maxPlayerCount = PhotonNetwork.CurrentRoom.MaxPlayers;
		byte currentPlayerCount = PhotonNetwork.CurrentRoom.PlayerCount;
		string mapName = room.GetMap();
		Map map = MapDatabase.GetMapWithMapName(mapName);
		int gameModeIndex = room.GetGamemode();

		m_roomStats = new RoomStats(maxPlayerCount, currentPlayerCount, map, (Mode) gameModeIndex);
	}

    /// <summary>From IInRoomCallbacks, using it to Spawn Events from MasterClient to new Player.</summary>
    /// <param name="newPlayer">new Joined Player</param>
    public override void OnPlayerEnteredRoom(Player newPlayer)
	{
		Debug.Log("Player Entered Room");
		//Only Masterclient send Spawn Event to new joined Player
		if (PhotonNetwork.IsMasterClient)
		{
			SendSpawnEvent(newPlayer);
			UpdateRoomStats();
			RoomUtilization();
		}
	}

	private void UpdateRoomStats()
	{
		m_roomStats.CurrentPlayerCount = PhotonNetwork.CurrentRoom.PlayerCount;
		SetPlayerCount();
	}

    /// <summary>Send Spawn Event to Player.</summary>
    /// <param name="player">New joined Player</param>
    private void SendSpawnEvent(Player player)
	{
		Debug.Log("Spawn new entered Player");
		Gamemode.AddPlayerToTeam(m_roomStats.GameMode, player);
		NetworkEvents.SyncSpawnNode(player.GetPlayerTeam(), player);
	}

	/// <summary>If Room is Full, change Timer and send next Event. </summary>
	private void RoomUtilization()
	{
		if (!PhotonNetwork.IsMasterClient) return;

		bool roomIsFull = m_roomStats.CurrentPlayerCount == m_roomStats.MaxPlayerCount ? true : false;

		if (roomIsFull && TimerStateState != TimerState.Final)
		{
			SendEvent(WaitingRoomEvent.CloseRoom);
			TimerStateState = TimerState.Final;
		}
	}

	private void Update()
	{
		WaitingRoomTimer();
	}

	/// <summary>If Countdown ends send next Event.</summary>
	private void WaitingRoomTimer()
	{
		switch (TimerStateState)
		{
			case TimerState.Internal:
				InternalTimerLength -= Time.deltaTime;

				//TODO: Remove when done !
				SetDisplayTime(InternalTimerLength);

				if (InternalTimerLength <= 0 && m_timerIsRunning)
					if (PhotonNetwork.IsMasterClient)
					{
						StopCoroutine(m_pingroutine);

						SendEvent(WaitingRoomEvent.CloseRoom);
						m_timerIsRunning = false;
					}

				break;
			case TimerState.Final:
				FinalTimerLength -= Time.deltaTime;

				SetDisplayTime(FinalTimerLength);

				if (FinalTimerLength <= 0 && m_timerIsRunning)
				{
					SendEvent(WaitingRoomEvent.StartMatch);
					m_timerIsRunning = false;
				}

				break;
		}
	}

	//Photon callback
	public override void OnPlayerLeftRoom(Player player)
	{
		Debug.Log(player.NickName + " Disconnected");
		UpdateRoomStats();
	}

	private void LoadPickedMap()
	{
		PhotonNetwork.DestroyPlayerObjects(PhotonNetwork.LocalPlayer);

		PhotonNetwork.IsMessageQueueRunning = false;

		MapDatabase.LoadMap(m_roomStats.Map);
	}

	private void LockRoom()
	{
		if (PhotonNetwork.IsMasterClient)
		{
			Debug.Log("Lock Room");
			PhotonNetwork.CurrentRoom.IsVisible = false;
			PhotonNetwork.CurrentRoom.IsOpen = false;
		}
	}

	public void LeaveRoom()
	{
		PhotonNetwork.LeaveRoom();
	}

	private void SendEvent(byte eventcode)
	{
		if (!PhotonNetwork.IsMasterClient) return;

		foreach (Player pp in PhotonNetwork.PlayerList)
		{
			Debug.Log("Send event to" + pp.NickName);
			PhotonNetwork.RaiseEvent(eventcode, new object[] {(float) PhotonNetwork.ServerTimestamp},
									new RaiseEventOptions {TargetActors = new[] {pp.ActorNumber}},
									SendOptions.SendReliable);
		}
	}

	private IEnumerator SetPing()
	{
		PhotonNetwork.CurrentRoom.SetPing(PhotonNetwork.GetPing());
		yield return new WaitForSeconds(1);
		m_pingroutine = StartCoroutine(SetPing());
	}

	public class RoomStats
	{
		public int CurrentPlayerCount;
		public Mode GameMode;
		public Map Map;
		public int MaxPlayerCount;

		public RoomStats(int maxPlayerCount, int currentPlayerCount, Map map, Mode gameMode)
		{
			MaxPlayerCount = maxPlayerCount;
			CurrentPlayerCount = currentPlayerCount;
			Map = map;
			GameMode = gameMode;
		}
	}

	[SerializeField]
	private enum TimerState
	{
		None,
		Internal,
		Final
	}

	#region UI stuff

	private void SetPlayerCount()
	{
		PlayerInRoomDisplay.text = m_roomStats.CurrentPlayerCount + " / " + m_roomStats.MaxPlayerCount;
	}

	private void SetDisplayTime(float time)
	{
		TimerDisplay.text = time.ToString("F0");
	}

	#endregion UI Stuff
}