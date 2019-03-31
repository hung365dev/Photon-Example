using UnityEngine;
using Hashtable = ExitGames.Client.Photon.Hashtable;

public class PlayerProperties
{
    public const string Ready = "rdy";
    public const string Team = "team";
    public const string Score = "scr";
}

public static class PlayerExtension
{
    #region PlayerScore
    /// <summary>
    /// Assign Score to Player Properties.Will Override the current Score.
    /// </summary>
    /// <param name="player">Target Photon Player</param>
    /// <param name="score">Score</param>
    public static void SetScore(this PhotonPlayer player, int score)
    {
        Hashtable scoreProp = new Hashtable()
        {
            {PlayerProperties.Score,score}
        };
        player.SetCustomProperties(scoreProp);
    }
    /// <summary>
    /// Add Score to PLayer Properties.
    /// </summary>
    public static void AddPlayerScore(this PhotonPlayer player, int scoreToAdd)
    {
        int current = player.GetCurrentScore();
        current = current + scoreToAdd;
        Hashtable scoreProp = new Hashtable()
        {
            {PlayerProperties.Score,current}
        };
        player.SetCustomProperties(scoreProp);
    }
    /// <summary>Get the current Player Score.</summary>
    public static int GetCurrentScore(this PhotonPlayer player)
    {
        object score;
        if (player.CustomProperties.TryGetValue(PlayerProperties.Score, out score))
        {
            return (int)score;
        }

        return 0;
    }

    #endregion

    #region PlayerReadyState
    /// <summary>Change PhotonPlayer Ready State.</summary>
    public static void SetReadyState(this PhotonPlayer player, bool value)
    {
        Hashtable score = new Hashtable();
        score[PlayerProperties.Ready] = value;
        player.SetCustomProperties(score);
    }

    /// <summary>Get PhotonPlayer Ready State, if null its not set.</summary>
    public static bool GetReadyState(this PhotonPlayer player)
    {
        object m_activState;
        if (player.CustomProperties.TryGetValue(PlayerProperties.Ready, out m_activState))
        {
            return (bool)m_activState;
        }

        return false;
    }
    #endregion

    #region Teams

    /// <summary>Extension for PhotonPlayer class to wrap up access to the player's custom property.</summary>
    /// <returns>PunTeam.Team.none if no team was found (yet).</returns>
    public static Teams.Team GetPlayerTeam(this PhotonPlayer player)
    {
        object teamId;
        if (player.CustomProperties.TryGetValue(PlayerProperties.Team, out teamId))
        {
            return (Teams.Team)teamId;
        }

        return Teams.Team.none;
    }

    /// <summary>Switch that player's team to the one you assign.</summary>
    /// <remarks>Internally checks if this player is in that team already or not. Only team switches are actually sent.</remarks>
    /// <param name="player"></param>
    /// <param name="team"></param>
    public static void SetPlayerTeam(this PhotonPlayer player, Teams.Team team)
    {
        if (!PhotonNetwork.connectedAndReady)
        {
            Debug.LogWarning("JoinTeam was called in state: " + PhotonNetwork.connectionStateDetailed + ". Not connectedAndReady.");
            return;
        }

        Teams.Team currentTeam = player.GetPlayerTeam();
        if (currentTeam != team)
        {
            player.SetCustomProperties(new Hashtable() { { PlayerProperties.Team, team } });
        }
    }
    #endregion
}
