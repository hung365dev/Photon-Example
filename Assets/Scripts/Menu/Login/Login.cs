using System;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Photon.Realtime;
using SradnickDev.FlexGUI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;


public class Login : FlexScreen
{
    private const string Autologin = "autoLogin";
    private const string Name = "accName";
    private const string Password = "accPassword";


    [Header("Connection")] 
    [SerializeField] private Text ConnectionInfo;
    [SerializeField] private Color DefaultInfo = Color.white;
    [SerializeField] private Color FailedInfo = Color.red;
    [SerializeField] private Color SuccessInfo = Color.green;
    public UnityEvent OnConnected;
    

    [Header("Login")] 
    [SerializeField] private InputField NameField; //Login Name
    [SerializeField] private InputField PasswordField; //Login Passoword
    [SerializeField] private Toggle AutoLoginToggle;
    [SerializeField] private Image Overlay;
    private bool m_autoLogin = false;

    private class Account
    {
        public readonly string Username;
        public readonly string UserPassowrd;

        public Account(string username, string userPassowrd)
        {
            this.Username = username;
            this.UserPassowrd = userPassowrd;
        }
    }

    private Account m_currentAccount;

    public override void Close()
    {
        if (onClose != null)
        {
            onClose.Invoke();
        }
            
        gameObject.SetActive(false);
    }
    public void AutoLogin()
    {
        int autoLoginValue = PlayerPrefs.GetInt(Autologin);
        m_autoLogin = autoLoginValue == 1 ? true : false;

        AutoLoginToggle.isOn = m_autoLogin;
        if (m_autoLogin)
        {
            OnLogin();
        }
    }

    public void OnLogin()
    {
        #region PhotonRelated

        if (Overlay)
        {
            Overlay.gameObject.SetActive(true);
        }

        if (PhotonNetwork.IsConnectedAndReady)
        {
            Connected();
        }

        if (m_autoLogin)
        {
            m_currentAccount = FetchAccountData();
            NameField.text = m_currentAccount.Username;
            PasswordField.text = m_currentAccount.UserPassowrd;
        }
        else
        {
            if (NameField.text == string.Empty || PasswordField.text == string.Empty)
            {
                LogInfo("Please fill out all fields!", FailedInfo);
                if (Overlay)
                {
                    Overlay.gameObject.SetActive(false);
                }
                return;
            }
            m_currentAccount = new Account(NameField.text,PasswordField.text);
        }
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.AuthValues = new AuthenticationValues
        {
            AuthType = CustomAuthenticationType.Custom
        };

        PhotonNetwork.AuthValues.AddAuthParameter("usernamePost",m_currentAccount.Username);
        PhotonNetwork.AuthValues.AddAuthParameter("passwordPost",m_currentAccount.UserPassowrd);

        SetPlayerName(NameField.text);

        StoreAccountData(m_currentAccount);

        if (!PhotonNetwork.IsConnectedAndReady)
        {
            StartCoroutine(OnConnecting());
        }
        #endregion PhotonRelated
    }

    private void StoreAccountData(Account account)
    {
        PlayerPrefs.SetString(Name, account.Username);
        PlayerPrefs.SetString(Password, account.UserPassowrd);
    }

    private Account FetchAccountData()
    {
        string username = PlayerPrefs.GetString(Name);
        string password = PlayerPrefs.GetString(Password);
        return new Account(username, password);
    }

    public void OnAutoLoginValueChanged(bool auto)
    {
        int value = AutoLoginToggle.isOn == true ? 1 : 0;
        PlayerPrefs.SetInt(Autologin, value);
        if (!AutoLoginToggle.isOn)
        {
            StoreAccountData(new Account(string.Empty, string.Empty));
        }
    }

    private IEnumerator OnConnecting()
    {
        PhotonNetwork.ConnectUsingSettings();
        ConnectionInfo.color = DefaultInfo;

        while (!PhotonNetwork.IsConnectedAndReady)
        {
            yield return null;
            ClientState state = PhotonNetwork.NetworkClientState;
            ConnectionInfo.text = state.ToString();
            if (state == ClientState.Disconnected)
            {
                ConnectionInfo.color = FailedInfo;
                ConnectionInfo.text = "Password or Username wrong!";
                if (Overlay)
                {
                    Overlay.gameObject.SetActive(false);
                }
            }
            
        }

        Connected();
    }
    private void Connected()
    {
        if (OnConnected != null)
        {
            OnConnected.Invoke();
        }
    }
    private void SetPlayerName(string nickName)
    {
        PhotonNetwork.NickName = nickName;
    }
    private void LogInfo(string txt, Color color)
    {
        ConnectionInfo.color = color;
        ConnectionInfo.text = txt;
    }
}

