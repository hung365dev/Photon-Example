using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Login : MonoBehaviour
{

    [Header("Photon Settings")]
    [SerializeField] string version = "0.0";             //client GameVersion
    [SerializeField] bool automaticallySyncScene;       //sync scenes with masterclient or nor
    [SerializeField] GameObject connectionPanel;        //Connection Display
    [SerializeField] Text connectionState;

    [Header("Input Fields")]
    [SerializeField] InputField nameField;              //Login Name
    [SerializeField] InputField passwordField;          //Login Passoword
    [SerializeField] Toggle rememberToggle;
    [SerializeField] Toggle autoLoginToggle;

    //Login.data_remember for loading right Playerprefs from other Scripts
    const string data_remember = "cb_remember";        //PlayerPrefs key as bool
    const string data_autoLogin = "cb_autoLogin";      //PlayerPrefs key as bool
    const string data_name = "cb_name";                //Player Name
    const string data_pass = "cb_password";            //Player Password

    [Header("Buttons")]
    [SerializeField] Button loginButton;
    [SerializeField] Button registerButton;
    [SerializeField] Button createAccoutButton;

    [Header("Panels")]
    [SerializeField] GameObject loginPanel;
    [SerializeField] GameObject registerPanel;
    [SerializeField] GameObject lobbyPanel;
    [SerializeField] GameObject m_joinOptions;



    [Header("Server Message")]
    [SerializeField] Transform messageCanvas;
    [SerializeField] GameObject serverMessage;



    public void Start()
    {
        //Add Mehtod to Button to turn on/off the Register Panel
        registerButton.onClick.AddListener(() => registerPanel.SetActive(!registerPanel.activeInHierarchy));
        //default disabled
        connectionPanel.SetActive(false);
        //load Messages from Database or something
        LoadMessages();

        //Fetch login Data if avaible
        OnStart();
    }
    void OnStart()
    {
        //Read PlayerPrefs
        //Load Name and Password
        //AutoLogin 

        //Thats just as simple Example using playerprefs

        int rememberData = PlayerPrefs.GetInt(data_remember);
        bool canRemember = rememberData == 1 ? true : false;
        if (canRemember)
        {
            rememberToggle.isOn = canRemember;
            nameField.text = PlayerPrefs.GetString(data_name);
            passwordField.text = PlayerPrefs.GetString(data_pass);
        }

        int autoLoginData = PlayerPrefs.GetInt(data_autoLogin);
        bool canAutoLogin = autoLoginData == 1 ? true : false;
        if (canAutoLogin)
        {
            OnLogin();
        }

        loginButton.onClick.AddListener(OnLogin);
    }
    IEnumerator OnConnecting()
    {
        connectionPanel.SetActive(true);
        PhotonNetwork.ConnectUsingSettings(version);                    //Connect to Photon Cloud
        PhotonNetwork.automaticallySyncScene = automaticallySyncScene;  //set scene sync
        PhotonNetwork.autoJoinLobby = true;
        PhotonNetwork.JoinLobby(TypedLobby.Default);                    //Check out the docs for more infos about TypedLobby's

        while (PhotonNetwork.connectionState != ConnectionState.Connected)  //wait until we are connected
        {
            yield return null;
            connectionState.text = PhotonNetwork.connectionState.ToString();
        }
        connectionPanel.SetActive(false);                               //clean up the Screen from useless connection infos

    }
    void StoreData()
    {

        //Player Name and Password will be saved localy
        //thats only the easiest Solution
        bool shouldRemember = rememberToggle.isOn;
        // store data to Playerprefs
        if (shouldRemember)
        {
            PlayerPrefs.SetString(data_name, nameField.text);
            PlayerPrefs.SetString(data_pass, passwordField.text);
            PlayerPrefs.SetInt(data_remember, 1);
        }
        else
        {
            //clean up playerPrefs if it should not remember
            PlayerPrefs.SetString(data_name, string.Empty);
            PlayerPrefs.SetString(data_pass, string.Empty);
            PlayerPrefs.SetInt(data_remember, 0);
        }
        bool canAutoLogin = autoLoginToggle.isOn;
        if (canAutoLogin)
        {
            PlayerPrefs.SetInt(data_autoLogin, 1);
        }
        else
        {
            PlayerPrefs.SetInt(data_autoLogin, 0);
        }

    }
    public void OnLogin()
    {
        #region PhotonRelated
        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //!
        //! Photon Authentication
        //!
        //!
        //!     HERE!
        //!
        //!
        //!
        //!
        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!

        //PhotonNetwork.AuthValues = new AuthenticationValues();
        //PhotonNetwork.AuthValues.AuthType = CustomAuthenticationType.Custom;
        //PhotonNetwork.AuthValues.AddAuthParameter("user",nameField.text);
        //PhotonNetwork.AuthValues.AddAuthParameter("pass",passwordField.text);
        //PhotonNetwork.AuthValues.UserId = id;



        if (!PhotonNetwork.connectedAndReady)
        {
            StartCoroutine(OnConnecting());
        }

        #endregion PhotonRelated

        StoreData();
        SetPlayerName(nameField.text);

        m_joinOptions.SetActive(true);

        loginPanel.SetActive(false);
    }
    void SetPlayerName(string name)
    {
        PhotonNetwork.playerName = name;
    }
    public void CreateAccount()
    {

        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
        //!
        //!     ACCOUNT CREATION!
        //!
        //!
        //!     
        //!
        //!
        //!
        //!
        //!!!!!!!!!!!!!!!!!!!!!!!!!!!!!   
        registerPanel.SetActive(false);

    }
    public void LoadMessages()
    {
        //Load Messages from a Database or something
        //Create for each Message a new GameObject


        List<string> example = new List<string>
        {
            {" Admin : Photon Example Project." },
            {" Admin : Have Fun and give me your Money!!"},
            {"Admin : Report any Error."}
        };
        for (int i = 0; i < example.Count; i++)
        {
            GameObject newMessage = Instantiate(serverMessage, messageCanvas);
            ServerMessage sm = newMessage.GetComponent<ServerMessage>();
            sm.SetMessage(example[i]);

        }
    }
}

