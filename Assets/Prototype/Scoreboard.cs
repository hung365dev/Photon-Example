using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class Scoreboard : MonoBehaviour
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

    void OnPhotonPlayerPropertiesChanged(object[] playerAndUpdatedProps)
    {
        //Get PlayerList
        //Sort Player after Score
        //add to text

        PhotonPlayer[] pList = PhotonNetwork.playerList;
        System.Array.Sort(pList, delegate (PhotonPlayer p1, PhotonPlayer p2) { return p1.GetCurrentScore().CompareTo(p2.GetCurrentScore()); });
        System.Array.Reverse(pList);

        m_scoreBoardText.text = string.Empty;
        for (int i = 0; i < pList.Length; i++)
        {
            PhotonPlayer player = pList[i];
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
}
