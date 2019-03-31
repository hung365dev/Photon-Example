using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;
using UnityEngine.Events;

public class Jetpack : MonoBehaviour
{
    [Header("Movement Settings")]
    [SerializeField, MinMaxSlider(1,15)] private Vector2 Acceleration = new Vector2(1, 15);

    private Vector3 m_velocity = new Vector3();

    [Header("Fuel Settings")]
    private float m_maxFuel = 10;
    [SerializeField, ProgressBar("Fuel", 10, ProgressBarColor.Yellow)]
    private float m_currentFuel = 5;

    private CharacterController m_characterController;
    private PlayerMovement m_playerMovement;

    private void Awake()
    {
        m_characterController = GetComponent<CharacterController>();
        m_playerMovement = GetComponent<PlayerMovement>();
        m_currentFuel = m_maxFuel;
    }

    private void FixedUpdate()
    {
        if (Input.GetMouseButton(1) && m_currentFuel >= 0)
        {
            if (m_playerMovement.isActiveAndEnabled)
            {
                m_playerMovement.enabled = false;
            }

            var horizontal = Input.GetAxis("Horizontal");

            m_velocity.x += horizontal * Time.deltaTime * Mathf.Lerp(0,Acceleration.y,Time.deltaTime);
            m_velocity.y += Time.deltaTime * Mathf.Lerp(0, Acceleration.y, Time.deltaTime); ;

            m_characterController.Move(m_velocity * Time.deltaTime);

            DecreaseFuel();
        }
        else
        {
            IncreaseFuel();
            m_velocity = Vector3.zero;
            if (!m_playerMovement.isActiveAndEnabled)
            {
                m_playerMovement.enabled = true;
            }

        }
    }

    private void DecreaseFuel()
    {
        m_currentFuel = Mathf.Clamp(m_currentFuel -= Time.deltaTime, 0, m_maxFuel);
    }

    private void IncreaseFuel()
    {
        m_currentFuel = Mathf.Clamp(m_currentFuel += Time.deltaTime, 0, m_maxFuel);
    }
}
