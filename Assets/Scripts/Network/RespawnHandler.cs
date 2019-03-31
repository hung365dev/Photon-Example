using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class RespawnHandler : MonoBehaviour
{

    [SerializeField] private int RespawnTime = 5;
    [SerializeField] private Text Counter;
    private HealthHandler m_healthHandler;
    private Mode m_currenGameMode;


    public void Init(HealthHandler healthHandler)
    {
        m_healthHandler = healthHandler;
        healthHandler.PlayerDeathEvent += StartCountDown;
    }

    void OnDisable()
    {
        if (m_healthHandler != null)
        {
            m_healthHandler.PlayerDeathEvent -= StartCountDown;
        }
        
    }
    void StartCountDown()
    {
        if (!Counter.gameObject.activeInHierarchy)
        {
            Counter.gameObject.SetActive(true);
        }

        StartCoroutine(Countdown());
    }

    private IEnumerator Countdown()
    {
        int duration = RespawnTime;
        while (duration != -1)
        {
            Counter.text = duration.ToString("F0");
            yield return new WaitForSeconds(1);
            duration--;
            if (duration == 0)
            {
                RespawnPlayer();
            }
        }
    }
    private void RespawnPlayer()
    {
        m_currenGameMode = (Mode)PhotonNetwork.CurrentRoom.GetGamemode();
        Player localPlayer = PhotonNetwork.LocalPlayer;
        Teams.Team team = localPlayer.GetPlayerTeam();

        switch (m_currenGameMode)
        {
            case Mode.TeamDeathMatch:
                NetworkEvents.TeamBasedRespawn(team, localPlayer);
                break;
            case Mode.DeatchMatch:
                NetworkEvents.RespawnRandomSpawnNode(localPlayer);
                break;
        }

        if (Counter.gameObject.activeInHierarchy)
        {
            Counter.gameObject.SetActive(false);
        }
    }
}
