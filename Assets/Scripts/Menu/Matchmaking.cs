using System;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class Matchmaking : MonoBehaviourPunCallbacks
{
    [Header("Matchmaking")]
    [SerializeField] private Text MatchmakingInfo;

    [SerializeField] private GameObject GamemodeButton;
    [SerializeField] private Transform GamemodePanel;
    
    [Header("Map")]
    [SerializeField] private Map WaitingRoom;
    
    private int m_pickedMode;

    private void Start()
    {
        CreateGameModeButtons();
    }
    private void CreateGameModeButtons()
    {
        //Create for each GameMode a Button
        //Each Button should call OnClickMode with the index
        //the index is a ref to the gameMode
        //ignore default/none(0) start with 1
        Array value = Enum.GetValues(typeof(Mode));
        for (var i = 1; i < value.Length; i++)
        {
            GameObject t = Instantiate(GamemodeButton.gameObject, GamemodePanel, false);
            Button btn = t.GetComponent<Button>();
            Text btnText = t.GetComponentInChildren<Text>();
            int mode = i;
            btn.onClick.AddListener(() => OnPickMode(mode));
            btnText.text = ((Mode)i).ToString();

        }
    }
    public void OnPickMode(int mode)
    {
        m_pickedMode = mode;
        GamemodePanel.gameObject.SetActive(false);
        Join();
    }

    public void OnClickMatchmaking()
    {
        GamemodePanel.gameObject.SetActive(true);
    }

    private void Join()
    {
        if (PhotonNetwork.IsConnectedAndReady)
        {

            //Filter for Gamemodes
            //Room Player count will be ignored
            Hashtable expectedProperties = new Hashtable() { { RoomProperties.Gamemode, m_pickedMode } };

            MatchmakingInfo.text = "looking for match...";
            //If the operation got queued and will be sent - returns true
            if (PhotonNetwork.CountOfRooms > 0)
            {
                PhotonNetwork.JoinRandomRoom(expectedProperties, 0);
            }
            else
            {
                MatchmakingInfo.text = "No Open Room found.";
            }
        }
        else
        {
            MatchmakingInfo.text = "Not Connected.";
        }
    }

    //Called form Photons API after successfully Joint a Room
    public override void OnJoinedRoom()
    {
        MatchmakingInfo.text = "Successful!";
        LoadingScreen.LoadScene(WaitingRoom.SceneIndex);
    }
}
