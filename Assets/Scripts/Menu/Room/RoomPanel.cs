using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class RoomPanel : MonoBehaviourPunCallbacks
{

    [SerializeField] Text roomName;
    [SerializeField] Text mapName;
    [SerializeField] Text gameMode;
    [SerializeField] Text playerCount;
    [SerializeField] Button join;
    [SerializeField] Text ping;

    public Button JoinButton { get { return join; } set { join = value; } }
    public string RoomName { get { return roomName.text; } }

    RoomInfo m_room;

    public void SetRoom(RoomInfo room)
    {
        m_room = room;

        string map = room.CustomProperties[RoomProperties.Map].ToString();
        int gameMode = System.Convert.ToInt32(room.CustomProperties[RoomProperties.Gamemode]);
        SetPanel(m_room.Name, map, gameMode);
        SetPlayerCount();
    }
    void SetPanel(string name, string map, int modeIndex)
    {
        roomName.text = name;
        mapName.text = map;
        gameMode.text = ((Mode)modeIndex).ToString();
    }
    void SetPing(int value)
    {
        ping.text = value.ToString();
    }
    void SetPlayerCount()
    {
        playerCount.text = m_room.PlayerCount + "/" + m_room.MaxPlayers;
    }
    /// <summary>Called from Photon Api</summary>
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        SetPlayerCount();

        //int ping = (int)m_room.CustomProperties[RoomProperties.Ping];
        //SetPing(ping);
    }
}
