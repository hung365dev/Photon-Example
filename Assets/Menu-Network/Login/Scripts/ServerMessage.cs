using UnityEngine;
using UnityEngine.UI;

public class ServerMessage : MonoBehaviour
{

    [SerializeField] GameObject serverMessage;
    [SerializeField] Image messageImage;
    public Image Image { get { return messageImage; } set { messageImage = value; } }
    [SerializeField] Text messageContent;

    /// <summary>Set Server Message</summary><param name="text">Message</param>
    public void SetMessage(string text)
    {
        messageContent.text = text;
    }

}
