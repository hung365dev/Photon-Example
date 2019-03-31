using SradnickDev.FlexGUI;
using System.Collections;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class Lobby : FlexScreen
{
    public UnityEvent OnLobbyJoined;
    [SerializeField] private FlexFader JoinLobbyFader;

    public override void Open()
    {
        base.Open();
        JoinLobby();
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        Debug.Log("Join Room Failed");
    }

    private void JoinLobby()
    {
        if (PhotonNetwork.InLobby)
        {
            JoinLobbyFader.gameObject.SetActive(false);
        }
        else
        {
            PhotonNetwork.JoinLobby();   
        }
    }


    public override void OnJoinedLobby()
    {
        if (OnLobbyJoined != null)
        {
            OnLobbyJoined.Invoke();
        }
        JoinLobbyFader.StartFade();
    }
}