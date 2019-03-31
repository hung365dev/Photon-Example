using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Photon.Pun;
using Photon.Realtime;
using Unity.Collections;
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
public class SimpleChat : MonoBehaviour
{

    [SerializeField] private Text MessagePrefab;
    [SerializeField] private Transform ChatContent;
    [SerializeField] private InputField MessageInput;
    [SerializeField] private Button SendButton;
    [SerializeField] private PhotonView PhotonView;

    private Queue<string> m_messageQueue = new Queue<string>();

    [SerializeField] private int MessagesInQueue;

    #region Message Limits
    private const double m_sendTimeLimit = 1;
    private bool m_canSend = true;
    private double m_nextSendingTime;
    private const int m_queueLimit = 10;
    #endregion

    private void Start()
    {
        //add a Method to the Send Button
        SendButton.onClick.AddListener(() => SendSimpleMessage(MessageInput.text));
        //clear text component
        MessageInput.text = string.Empty;
        
        MessageInput.gameObject.SetActive(false);
        SendButton.gameObject.SetActive(false);
    }
    void Update()
    {
        PlayerInput();
    }

    private void PlayerInput()
    {
        if (string.IsNullOrEmpty(MessageInput.text) && UnityEngine.Input.GetKeyDown(KeyCode.Return))
        {
            MessageInput.gameObject.SetActive(true);
            SendButton.gameObject.SetActive(true);
            EventSystem.current.SetSelectedGameObject(MessageInput.gameObject);
        }

        if (!string.IsNullOrEmpty(MessageInput.text) && UnityEngine.Input.GetKeyDown(KeyCode.Return))
        {
            //do nothing if input field contains spaces or is empty
            if (IsEmptyOrAllWhiteSpace(MessageInput.text))
            {
                return;
            }

            SendSimpleMessage(MessageInput.text);
            //clean input Field
            MessageInput.text = string.Empty;
            //deselect the InputField
            EventSystem.current.SetSelectedGameObject(null);
            MessageInput.gameObject.SetActive(false);
            SendButton.gameObject.SetActive(false);
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
        this.PhotonView.RPC("SendMessage", RpcTarget.All, queuedMessages);
        m_canSend = true;
    }

    void RefreshQueueCount()
    {
        MessagesInQueue = m_messageQueue.Count != 0 ? m_messageQueue.Count + 1 : 0;
    }
    
    [PunRPC]
    public void SendMessage(string text, PhotonMessageInfo info)
    {
        string m_sender = FormatName(info.Sender);
        var messageText = Instantiate(MessagePrefab, ChatContent, false);

        messageText.text = m_sender + " : " + text;
    }
    /// <summary>
    /// Adds some Colors and chars to the Player Name.
    /// </summary>
    /// <param name="photonPlayer"></param>
    /// <returns></returns>
    string FormatName(Player photonPlayer)
    {
        string senderName = photonPlayer.NickName;

        if (string.IsNullOrEmpty(senderName))
        {
            senderName = "Someone";
        }

        string name = photonPlayer == PhotonNetwork.LocalPlayer ? "<color=#add8e6ff> [" + senderName + "]</color>" : "<color=#808080ff>" + senderName + "</color>";
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

