using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;

public class RoomOverview : MonoBehaviourPunCallbacks
{

    [Header("RoomList")]
    [SerializeField] GameObject m_roomList;
    [SerializeField] Button m_roomListButton;
    
    [SerializeField] GameObject m_roomPanel;
    [SerializeField] Transform m_panelRect;
    [SerializeField] Dictionary<string,RoomPanel> m_roomPanelList = new Dictionary<string,RoomPanel>();

    [Header("CreatRoom")]
    [SerializeField] GameObject m_createRoomCanvas;
    [SerializeField] Button m_createRoomCanvasButton;

    [Header("Map")]
    [SerializeField] Map m_waitingRoom;

    void Start()
    {
        ShowRoomList();

        m_roomListButton.onClick.AddListener(ShowRoomList);
        m_createRoomCanvasButton.onClick.AddListener(ShowCreateRoomCanvas);
    }
    void ShowRoomList()
    {
        m_createRoomCanvas.SetActive(false);
        m_roomList.SetActive(true);
    }
    void ShowCreateRoomCanvas()
    {
        m_roomList.SetActive(false);
        m_createRoomCanvas.SetActive(true);
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
        var gbj = Instantiate(m_roomPanel, m_panelRect, false);
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
