using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class Lobby : Photon.MonoBehaviour
{

    [Header("RoomList")]
    [SerializeField] GameObject roomList;
    [SerializeField] Button roomListButton;
    [Space(10)]
    [SerializeField] GameObject roomPanel;
    [SerializeField] Transform panelRect;
    [SerializeField] List<RoomPanel> panelList = new List<RoomPanel>();

    [Header("CreatRoom")]
    [SerializeField] GameObject createRoomCanvas;
    [SerializeField] Button createRoomCanvasButton;

    [Header("Map")]
    [SerializeField] Map waitingRoom;


    void Start()
    {
        ShowRoomList();

        roomListButton.onClick.AddListener(ShowRoomList);
        createRoomCanvasButton.onClick.AddListener(ShowCreateRoomCanvas);

    }
    void ShowRoomList()
    {
        createRoomCanvas.SetActive(false);
        roomList.SetActive(true);
    }
    void ShowCreateRoomCanvas()
    {
        roomList.SetActive(false);
        createRoomCanvas.SetActive(true);
    }
    /// <summary>Called from Photon API.</summary>
    public void OnReceivedRoomListUpdate()
    {
        RoomInfo[] rooms = PhotonNetwork.GetRoomList();
        for (int i = 0; i < rooms.Length; i++)
        {
            OnRoomReceived(rooms[i]);
        }

        RemoveOldRoomPanels(rooms);

    }

    private void RemoveOldRoomPanels(RoomInfo[] rooms)
    {
        var t = rooms.ToList();

        int roomIndex = -1;
        for (int i = 0; i < panelList.Count; i++)
        {
            roomIndex = t.FindIndex(x => x.Name == panelList[i].RoomName);
            if (roomIndex == -1)
            {
                var oldPanel = panelList[i].gameObject;
                panelList.RemoveAt(i);
                Destroy(oldPanel);
            }

        }
    }

    void OnRoomReceived(RoomInfo room)
    {
        int index = panelList.FindIndex(x => x.RoomName == room.Name);

        if (index == -1)//Room is not in the panelList
        {
            GameObject gbj = Instantiate(roomPanel, panelRect, false);
            RoomPanel panel = gbj.GetComponent<RoomPanel>();

            panel.SetRoom(room);
            panel.JoinButton.onClick.AddListener(() => OnClickJoinRoom(panel.RoomName, room));
            panelList.Add(panel);
            index = (panelList.Count - 1);
        }
    }
    void OnClickJoinRoom(string roomName, RoomInfo roomInfo)
    {
        if (PhotonNetwork.JoinRoom(roomName))
        {
            MapDatabase.LoadMap(waitingRoom);
        }
        else
        {
            print("Join room failed");
        }
    }
}
