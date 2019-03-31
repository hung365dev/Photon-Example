using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.UI;


//Manage Health Bar
//Local Player using ScreenSpace HealthBar
//other Players using World HealthBars
public class HealthBarHandler : MonoBehaviour
{

    [SerializeField] PhotonView m_pView;
    [SerializeField] HealthHandler m_healthHandler;
    enum HealthBarMode { WorldSpace, ScreenSpace };
    [SerializeField, Tooltip("Read Only!")] HealthBarMode m_healthBarMode;

    [Header("Overhead Health Bar")]
    [SerializeField] GameObject m_worldSpaceHealthBar;
    [SerializeField] Canvas m_worldSpaceCanvas;
    [SerializeField] Image m_worldFillBar;
    [SerializeField] Image m_worldBarBoarder;
    [SerializeField] Color m_friendly = Color.green;
    [SerializeField] Color m_enemy = Color.green;

    [Header("Screen Health Bar")]
    [SerializeField] GameObject m_screenSpaceCanvas;
    [SerializeField] Image m_screenFillBar;

    [Header("Name Plate")]
    [SerializeField] TextMesh m_nameField;



    Teams.Team m_currentTeam;
    Player m_player;

    private void Start()
    {
        this.m_player = PhotonNetwork.LocalPlayer;
        m_nameField.text = m_player.NickName;
        m_currentTeam = m_player.GetPlayerTeam();
        m_healthHandler.CurrentTeam = m_currentTeam;
        m_healthHandler.OnChangeHealthEvent += OnHealthEventChanged;



        if (m_pView.IsMine)
        {
            m_healthBarMode = HealthBarMode.ScreenSpace;
        }
        else
        {
            m_healthBarMode = HealthBarMode.WorldSpace;
        }
        Initialize();
    }
    void Initialize()
    {
        switch (m_healthBarMode)
        {
            case HealthBarMode.ScreenSpace:
                m_screenSpaceCanvas.SetActive(true);
                m_worldSpaceHealthBar.SetActive(false);

                break;

            case HealthBarMode.WorldSpace:
                m_screenSpaceCanvas.SetActive(false);
                m_worldSpaceHealthBar.SetActive(true);

                if (m_currentTeam != PhotonNetwork.LocalPlayer.GetPlayerTeam() || m_currentTeam == Teams.Team.aggressive)
                {
                    m_worldBarBoarder.color = m_enemy;
                }

                break;
        }
    }
    private void Update()
    {
        CameraSearch();
        Billboard();
    }
    void CameraSearch()
    {
        if (m_worldSpaceCanvas.worldCamera == null)
        {
            var t = Camera.main;
            if (t != null)
            {
                m_worldSpaceCanvas.worldCamera = t;
            }
        }
    }
    void Billboard()
    {
        if (m_worldSpaceCanvas.worldCamera != null)
        {
            var t = m_worldSpaceCanvas.worldCamera;
            var m_lookAtPosition = t.transform.position;
            m_worldSpaceCanvas.transform.LookAt(m_lookAtPosition);
        }
    }
    void OnHealthEventChanged(float currentHealth, float maxHealth)
    {
        var m_fillamount = currentHealth / maxHealth;
        switch (m_healthBarMode)
        {
            case HealthBarMode.ScreenSpace:
                m_screenFillBar.fillAmount = m_fillamount;

                break;

            case HealthBarMode.WorldSpace:
                m_worldFillBar.fillAmount = m_fillamount;


                break;
        }
    }
    private void OnDisable()
    {
        m_healthHandler.OnChangeHealthEvent -= OnHealthEventChanged;
    }
}