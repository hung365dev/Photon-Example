using Photon.Pun;
using Photon.Realtime;
using UnityEngine;

public class Shooting : MonoBehaviourPun
{

    [SerializeField] int m_damage = 10;
    [SerializeField] Transform m_rayStart;
    [SerializeField] LayerMask m_hitMask;
    [SerializeField] PhotonView m_pView;


    [Header("Effects")]
    [SerializeField] ParticleSystem m_bullet;
    [SerializeField] AudioSource m_shoot;


    float m_lastshoot;
    float m_shootDelay = 0.25f;

    RaycastHit m_hit;
    Ray m_ray;

    private void Update()
    {
        if (!m_pView.IsMine)
        {
            return;
        }

        Debug.DrawRay(m_rayStart.position, m_rayStart.forward * 100, Color.red);

        if (Input.GetMouseButton(0) && Time.time > m_lastshoot)
        {
            RaycastShooting();
            CameraShake.Instance.StartShake(0);
            this.photonView.RPC("ShootEffect", RpcTarget.All);
            m_lastshoot = Time.time + m_shootDelay;
        }

    }
    void RaycastShooting()
    {
        //Raycast from a custom StartPoint in the Cameras forward direction
        m_ray = new Ray(m_rayStart.position, m_rayStart.forward);

        if (Physics.Raycast(m_ray, out m_hit, 500, m_hitMask))
        {
            if (m_hit.transform != null)
            {
                OnRaycastHit(m_hit);
            }
        }

    }
    void OnRaycastHit(RaycastHit hit)
    {
        //On hit we try to get the Targets HealthHandler
        //check in what Team he is
        //Add Damage if right Team
        var m_healthHandler = hit.transform.GetComponent<HealthHandler>();
        if (m_healthHandler == null)
        {
            return;
        }

        if (m_healthHandler.CurrentTeam != PhotonNetwork.LocalPlayer.GetPlayerTeam()
        || m_healthHandler.CurrentTeam == Teams.Team.aggressive)
        {
            m_healthHandler.OnReceiveDamage(m_damage, PhotonNetwork.LocalPlayer);
        }

    }
    [PunRPC]
    void ShootEffect()
    {
        m_bullet.Play();
        m_shoot.Play();
    }


}
