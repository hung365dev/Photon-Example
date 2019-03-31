using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Implements teams in a room/game with help of player properties. Access them by PhotonPlayer.GetTeam extension.
/// </summary>
/// <remarks>
/// Teams are defined by enum Team. Change this to get more / different teams.
/// There are no rules when / if you can join a team. You could add this in JoinTeam or something.
/// </remarks>
public class Teams : MonoBehaviour
{
    /// <summary>Enum defining the teams available. First team should be neutral (it's the default value any field of this enum gets).</summary>
    public enum Team : byte { none, black, white, aggressive };

    /// <summary>The main list of teams with their player-lists. Automatically kept up to date.</summary>
    /// <remarks>Note that this is static. Can be accessed by PunTeam.PlayersPerTeam. You should not modify this.</remarks>
    public static Dictionary<Team, List<PhotonPlayer>> PlayersPerTeam;



    #region Events by Unity and Photon

    public void Start()
    {

        PlayersPerTeam = new Dictionary<Team, List<PhotonPlayer>>();
        Array enumVals = Enum.GetValues(typeof(Team));
        foreach (var enumVal in enumVals)
        {
            PlayersPerTeam[(Team)enumVal] = new List<PhotonPlayer>();
        }
    }

    public void OnDisable()
    {
        PlayersPerTeam = new Dictionary<Team, List<PhotonPlayer>>();
    }

    /// <summary>Needed to update the team lists when joining a room.</summary>
    /// <remarks>Called by PUN. See enum PhotonNetworkingMessage for an explanation.</remarks>
    public void OnJoinedRoom()
    {

        this.UpdateTeams();
    }

    public void OnLeftRoom()
    {
        Start();
    }

    /// <summary>Refreshes the team lists. It could be a non-team related property change, too.</summary>
    /// <remarks>Called by PUN. See enum PhotonNetworkingMessage for an explanation.</remarks>
    public void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
    {
        this.UpdateTeams();
    }

    public void OnPhotonPlayerDisconnected(PhotonPlayer otherPlayer)
    {
        this.UpdateTeams();
    }

    public void OnPhotonPlayerConnected(PhotonPlayer newPlayer)
    {
        this.UpdateTeams();
    }

    #endregion


    public void UpdateTeams()
    {
        Array enumVals = Enum.GetValues(typeof(Team));
        foreach (var enumVal in enumVals)
        {
            PlayersPerTeam[(Team)enumVal].Clear();
        }

        for (int i = 0; i < PhotonNetwork.playerList.Length; i++)
        {
            PhotonPlayer player = PhotonNetwork.playerList[i];
            Team playerTeam = player.GetPlayerTeam();
            PlayersPerTeam[playerTeam].Add(player);
        }
    }
}