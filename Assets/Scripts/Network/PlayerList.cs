using System;
using System.Collections.Generic;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;


//small attempt to Custom PlayerList
//mainly because I want a reference to each Client controllled Player Object

public class PlayerList : MonoBehaviourPunCallbacks
{

    [Serializable]
    public class CustomPlayer
    {
        public string Name;
        public Player PhotonPlayer;
        public Teams.Team Team;
        public PhotonView PhotonView;
        public GameObject PlayerObject;

    }

    //only accsess it throw Method - Add or Remove
    [SerializeField] List<CustomPlayer> m_customPlayerList = new List<CustomPlayer>();


    /// <summary>Use this to add a new Player to the List.</summary>
    public void Add(CustomPlayer newPlayer)
    {
        int m_index = -1;
        //if a CustomPlayer contains newPlayer means hee already exists
        //lets just update him
        m_index = m_customPlayerList.FindIndex(x => x.PhotonPlayer == newPlayer.PhotonPlayer);

        if (m_index != -1)
        {
            CustomPlayer m_updatedPlayer = m_customPlayerList[m_index];
            m_updatedPlayer.PhotonView = newPlayer.PhotonView;
            m_updatedPlayer.PlayerObject = newPlayer.PlayerObject;
            m_customPlayerList[m_index] = m_updatedPlayer;
            return;
        }

        m_customPlayerList.Add(newPlayer);
        Debug.LogFormat("Added {0} to PlayerList. ", newPlayer.PhotonPlayer.NickName);

    }
    /// <summary>Remove a CustomPlayer from List with a PhotonPlayer</summary>
	public void Remove(Player player)
    {
        var m_target = m_customPlayerList.Find(x => x.PhotonPlayer == player);
        m_customPlayerList.Remove(m_target);
        Debug.LogFormat("{0} removed from PlayerList.", player.NickName);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Remove(otherPlayer);
    }

}
