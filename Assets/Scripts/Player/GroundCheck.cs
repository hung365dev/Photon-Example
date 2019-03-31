using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GroundCheck : MonoBehaviour
{
    public RaycastHit Hit
    {
        get { return m_hit; }
    }

    [SerializeField] private LayerMask GroundLayer;
    [SerializeField] private CharacterController CharacterController;


    private RaycastHit m_hit;

    private void Awake()
    {
        CharacterController = GetComponent<CharacterController>();
    }

    public bool IsGrounded()
    {
        var p1 = this.transform.position + CharacterController.center + Vector3.up * -CharacterController.height * 0.5f;
        var p2 = p1 + Vector3.up * CharacterController.height;
        if (Physics.CapsuleCast(p1, p2, CharacterController.radius, transform.forward, out m_hit, 0, GroundLayer))
        {
            return true;
        }

        return false;
    }


}
