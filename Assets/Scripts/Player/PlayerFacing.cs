using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerFacing : MonoBehaviour
{
	[SerializeField] private Transform Player;
	[SerializeField] private float DegreesPerSec = 10;

	private PlayerMovement m_playerMovement;
	private CharacterController m_characterController;
	private Transform m_transform;
    private Vector3 m_normal;


	private void Awake()
	{
		m_playerMovement = GetComponent<PlayerMovement>();
		m_transform = GetComponent<Transform>();
		m_characterController = GetComponent<CharacterController>();
	}

	private void Update()
	{
		var targetRot = Quaternion.identity;

		if (m_characterController.isGrounded)
		{
			targetRot = Quaternion.FromToRotation(transform.up, m_normal);

			Player.rotation = Quaternion.RotateTowards(Player.rotation, targetRot, Time.deltaTime * DegreesPerSec);
		}
		else
		{
			Player.rotation =
				Quaternion.RotateTowards(Player.rotation, Quaternion.Euler(0, 0, 0), Time.deltaTime * DegreesPerSec);
		}

		if (Mathf.Abs(m_playerMovement.InputDirection.x) == 0) return;

		Flip(m_playerMovement.InputDirection.x < 0);
	}

	private void Flip(bool left)
	{
		Vector3 scale = m_transform.localScale;
		scale.x = left == true ? -1 : 1;
		m_transform.localScale = scale;
	}

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        m_normal = hit.normal;
    }
}