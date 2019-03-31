using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using ExitGames.Client.Photon;

public class Scoreboard : MonoBehaviour , IInRoomCallbacks
{
    /// <summary>
    /// Simple Scoreboard wich displays the Kills from all Players in the Room.
    /// </summary>

    [SerializeField] private GameObject m_scoreBoard;
    [SerializeField] private Text m_scoreBoardText;
    private string m_nameLabel = " Name : ";
    private string m_scoreLabel = " Kills : ";



    void OnEnable()
    {
        //Because this Script is on an DontDestroyOnLoad
        //and stored in the WaitingRoom
        //This script needs to be informed every Time the scene has changed
        //to disable the ScoreBoard and clear the Text
        SceneManager.sceneLoaded += OnLevelFinishedLoading;
    }
    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            //deactivate if is activate and vice versa
            m_scoreBoard.SetActive(!m_scoreBoard.activeInHierarchy);
        }
    }
    //callback of Photon
    //TODO  poor Implementaion, consider using abstraction for IInRoomCallbacks
    public void OnPlayerPropertiesUpdate(Player targetPlayer, Hashtable changedProps)
    {
        //Get PlayerList
        //Sort Player after Score
        //add to text

        Player[] pList = PhotonNetwork.PlayerList;
        System.Array.Sort(pList, delegate (Player p1, Player p2) { return p1.GetCurrentScore().CompareTo(p2.GetCurrentScore()); });
        System.Array.Reverse(pList);

        m_scoreBoardText.text = string.Empty;
        for (int i = 0; i < pList.Length; i++)
        {
            Player player = pList[i];
            m_scoreBoardText.text += m_nameLabel + player.NickName + "   " + m_scoreLabel + player.GetCurrentScore() + "\n";
        }
    }

    void OnLevelFinishedLoading(Scene scene, LoadSceneMode mode)
    {
        m_scoreBoardText.text = string.Empty;
        m_scoreBoard.SetActive(false);
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnLevelFinishedLoading;
    }

    public void OnPlayerEnteredRoom(Player newPlayer)
    {
        throw new System.NotImplementedException();
    }

    public void OnPlayerLeftRoom(Player otherPlayer)
    {
        throw new System.NotImplementedException();
    }

    public void OnRoomPropertiesUpdate(Hashtable propertiesThatChanged)
    {
        throw new System.NotImplementedException();
    }


    public void OnMasterClientSwitched(Player newMasterClient)
    {
        throw new System.NotImplementedException();
    }
}
