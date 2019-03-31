using Photon.Pun;
using SradnickDev.FlexGUI;
using UnityEditor;
using UnityEngine;

public class MenuPanel : FlexScreen
{
    [SerializeField] private GameObject m_serverAnnouncements;

    public override void Open()
    {
        base.Open();
        m_serverAnnouncements.SetActive(true);
    }
    public override void Close()
    {
        base.Close();
        m_serverAnnouncements.SetActive(false);
    }
    
    private void OnDisable()
    {
        m_serverAnnouncements.SetActive(false);
    }

    public void Quit()
    {
        PhotonNetwork.Disconnect();
#if UNITY_EDITOR
        if(EditorApplication.isPlaying) 
        {
            EditorApplication.isPlaying = false;
        }
#endif
        Application.Quit();
    }
}
