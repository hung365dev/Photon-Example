using Photon.Pun;
using Photon.Realtime;
using UnityEngine;


//Add this Script to your Player
//it Register the Player to the PLayerList

public class RegisterPlayer : MonoBehaviour, IPunInstantiateMagicCallback
{

    [SerializeField] PlayerList m_playerList;


    //reference to the Player Objects PhotonView Component
    [SerializeField] PhotonView m_pView;

    //reference to the Player Object itself
    [SerializeField] GameObject m_playerObject;


    //OnPhotonInstantiate is called only on the (PhotonNetowk.)Instantiated GameObject > ChildObjects
    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        m_playerList = FindObjectOfType<PlayerList>();
        Register();
    }
    private void Register()
    {
        //Get The PhotonPlayer with the Owner Actor Nr
        Player m_thisPhotonPlayer = PhotonNetwork.LocalPlayer;//Player.Get(m_pView.OwnerActorNr);

        //Create new Custom Player
        PlayerList.CustomPlayer m_cp = new PlayerList.CustomPlayer()
        {
            Name = m_thisPhotonPlayer.NickName,
            PhotonPlayer = m_thisPhotonPlayer,
            Team = m_thisPhotonPlayer.GetPlayerTeam(),
            PhotonView = m_pView,
            PlayerObject = m_playerObject
        };
        m_playerList.Add(m_cp);
    }

}
