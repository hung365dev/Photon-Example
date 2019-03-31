using UnityEngine;
using UnityEngine.UI;

public class TopPanel : MonoBehaviour
{

    [SerializeField] GameObject m_ddols;			//ref to ddol parent
    [SerializeField] GameObject m_panel;		//to enable and disable
    [SerializeField] Button m_leave;			//the button
    [SerializeField] Map m_lobby;				//map we wanna load after pressing button


    void Start()
    {
        m_leave.onClick.AddListener(LeaveCurrentRoom);  //add Listener, if click on Button it will call the mehtod
        Cursor.lockState = CursorLockMode.Confined;     //bind cursor to screen
        Cursor.visible = false;							//dont show cursor
        m_panel.SetActive(false);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))           //waite for Input
        {
            m_panel.SetActive(!m_panel.GetActive());    //enable/disable
            Cursor.visible = m_panel.GetActive();       //if panel is enable cursor is visible
        }
    }
    void LeaveCurrentRoom()
    {
        PhotonNetwork.LeaveRoom();                      //only leave the Room no need to disconect
        MapDatabase.LoadMap(m_lobby);                   //go back to lobby
        Destroy(m_ddols);								//destroy all ddols
    }
}
