using System;
using System.Collections;
using System.Collections.Generic;
using SradnickDev.FlexGUI;
using UnityEngine;
using UnityEngine.Networking;

public class Announcement : FlexScreen
{
    private const string Url = "https://sradnickdev.de/photon/announcement/getAnnouncement.php";

    [SerializeField] private GameObject m_serverAnnouncement;
    [SerializeField] private Transform m_content;

    public override void Open()
    {
        base.Open();
        StartCoroutine(LoadAnnouncement());
    }
    public override void Close()
    {
        base.Close();
        DeleteAnnouncements();
    }


    private IEnumerator LoadAnnouncement()
    {
        using (UnityWebRequest www = UnityWebRequest.Get(Url))
        {
            yield return www.SendWebRequest();

            if (www.isHttpError || www.isNetworkError)
            {
                Debug.Log(www.error);
            }
            else
            {
                CreateAnnouncement(www.downloadHandler.text);
            }
        }
    }

   private void CreateAnnouncement(string announcements)
   {
       var t = announcements.Split('\\');

       for (int i = t.Length - 1 - 1; i >= 0; i--)
       {
           var newAnnouncement = Instantiate(m_serverAnnouncement, m_content, false);
           newAnnouncement.GetComponent<ServerAnnouncement>().SetAnnouncement(t[i]);
       }
   }

   private void DeleteAnnouncements()
   {
       var idx = m_content.childCount;
       for (int i = 0; i < idx; i++)
       {
           var target = m_content.GetChild(i);
           Destroy(target);
       }
   }
}
