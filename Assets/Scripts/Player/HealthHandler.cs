using System;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class HealthHandler : MonoBehaviour
{

    [SerializeField] float m_maxHealth;
    float m_currentHealth;

    //Lets just subscribe e.g. the HealthBar to this
    //to get only the information when the health changed
    //or some hiteffects
    public event Action<float, float> OnChangeHealthEvent;

    public delegate void OnPlayerDeath();
    public event OnPlayerDeath PlayerDeathEvent;


    [SerializeField] PhotonView m_pView;
    public PhotonView PhotonView { get { return m_pView; } set { m_pView = value; } }

    //will be assigned from HealthBarHandler.cs
    public Teams.Team CurrentTeam { get; set; }
    private Player m_lastHit;



    private void Awake()
    {
        if (PhotonView == null)
        {
            PhotonView = GetComponent<PhotonView>();
        }
    }
    private void Start()
    {
        m_currentHealth = m_maxHealth;
        HealthChanged();
    }

    void AddDamage(float value)
    {
        m_currentHealth = Mathf.Clamp(m_currentHealth -= value, 0, m_maxHealth);
        if (m_currentHealth == 0)
        {
            if (PhotonView.IsMine)
            {

                OnDeath();
            }
        }
        HealthChanged();

    }
    void AddHealth(float value)
    {
        m_currentHealth = Mathf.Clamp(m_currentHealth += value, 0, m_maxHealth);
        HealthChanged();
    }

    void HealthChanged()
    {
        if (OnChangeHealthEvent != null)
        {
            OnChangeHealthEvent(m_currentHealth, m_maxHealth);
            CameraShake.Instance.StartShake(0);
        }
    }
    public void ResetHealth()
    {
        PhotonView.RPC("OnAddHealth", RpcTarget.Others);
    }
    /// <summary>Add Damage</summary>
    /// <param name="value">Damage</param>
    /// <param name="lastShoot">Player who shoot.</param>
    public void OnReceiveDamage(float value, Player lastShoot)
    {
        PhotonView.RPC("OnAddDamage", RpcTarget.All, value, lastShoot);
    }
    [PunRPC]
    void OnResetHealth()
    {
        m_currentHealth = m_maxHealth;
        HealthChanged();
    }
    [PunRPC]
    void OnAddDamage(float value, Player lastShoot)
    {
        AddDamage(value);
        m_lastHit = lastShoot;
    }
    [PunRPC]
    void OnAddHealth(float value)
    {
        AddHealth(value);
    }
    void OnDeath()
    {
        m_lastHit.AddPlayerScore(1);

        if (PlayerDeathEvent != null)
        {
            PlayerDeathEvent();
        }

        PhotonNetwork.Destroy(gameObject);
    }
}
