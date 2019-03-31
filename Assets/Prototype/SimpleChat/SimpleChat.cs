using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/// <summary>
/// Simple Chat Example
/// Put this on a GameObject add all References
/// add a PhotonView to the GameObject and leave it in your Room/Scene (SceneObject)
///
/// Included a Message Limit
/// this will by default only allow 1 message per secound
/// If you send more,the messages will be queued and send after 1(default) secound
/// also you by default it allows only 10 messeges in the queue
///
/// It is to take care of the Photon Message Limit, so spamming wouldn't disconnect/kick you.
/// </summary>
public class SimpleChat : Photon.MonoBehaviour
{

    [SerializeField] Text m_messagePrefab;
    [SerializeField] Transform m_targetParent;
    [SerializeField] InputField m_messageInput;
    [SerializeField] Button m_messageSender;

    Queue<string> m_messageQueue = new Queue<string>();

    [SerializeField] int m_messagesInQueue;

    #region Message Limits
    const double m_sendTimeLimit = 1;
    bool m_canSend = true;
    double m_nextSendingTime;
    const int m_queueLimit = 10;
    #endregion

    private void Start()
    {
        //add a Method to the Send Button
        m_messageSender.onClick.AddListener(() => SendSimpleMessage(m_messageInput.text));
        //clear text component
        m_messageInput.text = string.Empty;
    }
    void Update()
    {
        PlayerInput();
    }

    private void PlayerInput()
    {
        //if Text-Component dont contain Text and Pressing Enter
        //selects the InputField, now the Player can type

        if (string.IsNullOrEmpty(m_messageInput.text) && UnityEngine.Input.GetKeyDown(KeyCode.Return))
        {
            EventSystem.current.SetSelectedGameObject(m_messageInput.gameObject);
        }
        //if Text-Component containts Text and pressing Enter
        //a message with the Text will be sended
        if (!string.IsNullOrEmpty(m_messageInput.text) && UnityEngine.Input.GetKeyDown(KeyCode.Return))
        {
            //do nothing if input field contains spacces or is empty
            if (IsEmptyOrAllWhiteSpace(m_messageInput.text))
            {
                return;
            }

            SendSimpleMessage(m_messageInput.text);
            //clean input Field
            m_messageInput.text = string.Empty;
            //deselect the InputField
            EventSystem.current.SetSelectedGameObject(null);
        }

    }

    public void SendSimpleMessage(string message)
    {

        if (m_canSend)
        {
            //Calculate next Time when the Message/s can be sended
            //if less than 0 nextout is 0
            double nextOut = m_nextSendingTime - Time.time < 0.0 ? 0.0 : m_nextSendingTime - Time.time;
            print("Next outgoin msg : " + nextOut);
            HandleQueueLimit(message);
            StartCoroutine(HandleMessageLimit(nextOut));
        }
        else
        {
            Debug.Log("Added Message to Queue.");
            HandleQueueLimit(message);
            RefreshQueueCount();
        }
    }
    /// <summary>
    /// Enqueue all Messages.
    /// If Message Queue is full stops adding and ignores Messages.
    /// </summary>
    /// <param name="msg">Message to Send.</param>
    void HandleQueueLimit(string msg)
    {
        if (m_messageQueue.Count < m_queueLimit)
        {
            m_messageQueue.Enqueue(msg);
        }
        else
        {
            Debug.Log("Message Queue is full.Wait a moment");
        }
    }
    /// <summary>
    /// Calculate and set the Time to send a Message.
    /// "iterate" through all Messages in the Queue
    /// and stores them in as one large string.
    /// Send the large string via RPC
    /// </summary>
    /// <param name="delay"></param>
    /// <returns></returns>
    IEnumerator HandleMessageLimit(double delay)
    {
        m_canSend = false;
        m_nextSendingTime = Time.time + m_sendTimeLimit + delay;
        yield return new WaitForSeconds((float)delay);

        var queuedMessages = string.Empty;

        while (m_messageQueue.Count > 0)
        {
            queuedMessages += m_messageQueue.Dequeue() + "\n";
        }

        RefreshQueueCount();
        this.photonView.RPC("SimpleMessage", PhotonTargets.All, queuedMessages);
        m_canSend = true;
    }

    void RefreshQueueCount()
    {
        m_messagesInQueue = m_messageQueue.Count != 0 ? m_messageQueue.Count + 1 : 0;
    }
    [PunRPC]
    public void SimpleMessage(string text, PhotonMessageInfo info)
    {
        string m_sender = FormatName(info.sender);
        var messageText = Instantiate(m_messagePrefab, m_targetParent, false);

        messageText.text = m_sender + " : " + text;
    }
    /// <summary>
    /// Adds some Colors and chars to the Player Name.
    /// </summary>
    /// <param name="photonPlayer"></param>
    /// <returns></returns>
    string FormatName(PhotonPlayer photonPlayer)
    {
        string senderName = photonPlayer.NickName;

        if (string.IsNullOrEmpty(senderName))
        {
            senderName = "Someone";
        }

        string name = photonPlayer == PhotonNetwork.player ? "<color=#add8e6ff> [" + senderName + "]</color>" : "<color=#808080ff>" + senderName + "</color>";
        return name;
    }

    /// <summary>Check if a string is empty or contains only spaces.</summary>
    /// <param name="text">Strint to check.</param>
    /// <returns>True if Empty/spaces, Otherwise false</returns>
    bool IsEmptyOrAllWhiteSpace(string text)
    {
        return null != text && text.All(x => x.Equals(' '));
    }
}

