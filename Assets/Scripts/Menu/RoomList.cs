using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;
using SradnickDev.FlexGUI;

public class RoomList : MonoBehaviourPunCallbacks, IFlexScreen
{
    [Header("Reference")]
    [SerializeField] private GameObject m_roomPanel;
    [SerializeField] private Transform m_content;
    [SerializeField] Dictionary<string,RoomPanel> m_roomPanelList = new Dictionary<string,RoomPanel>();


    [Header("Map")]
    [SerializeField] Map m_waitingRoom;

    public void Open()
    {
        gameObject.SetActive(true);
    }

    public void Close()
    {
        gameObject.SetActive(false);
    }

    /// <summary>Photon API Callback.</summary>
    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (var entry in roomList)
        {
            if (m_roomPanelList.ContainsKey(entry.Name))
            {
                
                if (entry.RemovedFromList)
                {
                    RemoveRoomPanel(entry);
                }
            }
            else
            {
                if (!entry.RemovedFromList)
                {
                    AddRoomPanel(entry);
                }
            }
        }
    }
    void AddRoomPanel(RoomInfo room)
    {
        var gbj = Instantiate(m_roomPanel, m_content, false);
        var panel = gbj.GetComponent<RoomPanel>();

        panel.SetRoom(room);
        panel.JoinButton.onClick.AddListener(() => OnClickJoinRoom(panel.RoomName, room));
        m_roomPanelList.Add(room.Name,panel);
    }

    private void RemoveRoomPanel(RoomInfo room)
    {
        var panel = m_roomPanelList[room.Name];
        m_roomPanelList.Remove(room.Name);
        Destroy(panel.gameObject);
    }
    void OnClickJoinRoom(string roomName, RoomInfo roomInfo)
    {
        PhotonNetwork.JoinRoom(roomName);
    }

    public override void OnJoinedRoom()
    {
        LoadingScreen.LoadScene(m_waitingRoom.SceneIndex);
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode,message);
    }
}
