using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class RespawnHandler : MonoBehaviour
{

    [SerializeField] private int m_respawnTime = 5;
    [SerializeField] private Text m_counter;
    private HealthHandler m_healthHandler;
    private float m_countDown;
    private Mode m_currenGameMode;


    public void Init(HealthHandler healthHandler)
    {
        m_countDown = m_respawnTime;
        m_healthHandler = healthHandler;
        healthHandler.PlayerDeathEvent += StartCountDown;
    }

    void OnDisable()
    {
        m_healthHandler.PlayerDeathEvent -= StartCountDown;
    }
    void StartCountDown()
    {
        if (!m_counter.gameObject.activeInHierarchy)
        {
            m_counter.gameObject.SetActive(true);
        }

        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        float duration = m_respawnTime;
        while (duration != -1)
        {
            m_counter.text = duration.ToString("F0");
            yield return new WaitForSeconds(1);
            duration--;
            if (duration == 0)
            {
                RespawnPlayer();
            }
        }
    }
    void RespawnPlayer()
    {
        m_currenGameMode = (Mode)PhotonNetwork.room.GetGamemode();
        PhotonPlayer localPlayer = PhotonNetwork.player;
        Teams.Team team = localPlayer.GetPlayerTeam();

        switch (m_currenGameMode)
        {
            case Mode.TeamDeathMatch:
                NetworkEventHandler.TeamBasedRespawn(team, localPlayer);
                break;
            case Mode.DeatchMatch:
                NetworkEventHandler.RespawnRandomSpawnNode(localPlayer);
                break;
        }
        m_countDown = m_respawnTime;

        if (m_counter.gameObject.activeInHierarchy)
        {
            m_counter.gameObject.SetActive(false);
        }
    }
}
