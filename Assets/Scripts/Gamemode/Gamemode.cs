using System.Collections.Generic;
using System.Linq;
using UnityEngine;

//is stored in Room Properties
public enum Mode { none, TeamDeathMatch, DeatchMatch }

public class Gamemode : Photon.PunBehaviour
{
    public static void AddPlayerToTeam(Mode mod, PhotonPlayer player)
    {
        switch (mod)
        {
            case Mode.TeamDeathMatch:
                AssignToTeam(player);

                break;
            case Mode.DeatchMatch:
                DeatchMatch(player);
                break;
        }

    }

    ///<summary>Player will be assign to the Team with the lowest count.</summary>
    private static void AssignToTeam(PhotonPlayer player)
    {
        List<int> m_sizeOfTeams = new List<int>();

        //create a list with each Team Size
        // start at 1 => Team None should be ignored
        // -1 because the Team Agressive should be ignored too

        for (int i = 1; i < Teams.PlayersPerTeam.Count - 1; i++)
        {
            m_sizeOfTeams.Add(Teams.PlayersPerTeam[(Teams.Team)i].Count);
        }

        //using Linq and Lambda ,get the index of the smallest Team
        int m_smallestTeamIndex = m_sizeOfTeams.FindIndex(x => x == m_sizeOfTeams.Min());
        //adding 1 to smallestTeamIndex will prevent to pick Team None
        m_smallestTeamIndex++;

        Debug.LogFormat("{0} is forced to Play in the {1} Team.", player.NickName, (Teams.Team)m_smallestTeamIndex);

        //Add Player to smallest Team
        AssignPlayerToTeam((Teams.Team)m_smallestTeamIndex, player);
    }
    ///<summary>Player will be assign to the Team Agressiv.</summary>
    private static void DeatchMatch(PhotonPlayer player)
    {
        AssignPlayerToTeam(Teams.Team.aggressive, player);
    }
    private static void AssignPlayerToTeam(Teams.Team team, PhotonPlayer player)
    {
        player.SetPlayerTeam(team);
        Teams.PlayersPerTeam[team].Add(player);
    }
}
