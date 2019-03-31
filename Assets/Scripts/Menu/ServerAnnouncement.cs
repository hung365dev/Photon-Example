using UnityEngine;
using UnityEngine.UI;

public class ServerAnnouncement : MonoBehaviour
{
    [SerializeField] private Text m_content;

    /// <summary>Set Server Message</summary><param name="text">Message</param>
    public void SetAnnouncement(string text)
    {
        m_content.text = text;
        SetSize();
    }

    private void SetSize()
    {
        Rect rect = GetComponent<RectTransform>().rect;
        Rect childRect = m_content.GetComponent<RectTransform>().rect;
        rect.height = childRect.height;
    }

}
