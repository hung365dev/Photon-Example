using UnityEngine;

public class InGameGizmos : MonoBehaviour
{


    [SerializeField] Color m_gizmoColor = Color.green;
    [SerializeField] bool m_drawCircle = false;
    [SerializeField] Vector3 m_circlePoistion;
    [SerializeField] float m_circleRadius;

    [SerializeField] bool m_drawWireCube = false;
    [SerializeField] Vector3 m_wireCubePosition;
    [SerializeField] Vector3 m_wireCubeSize;

    private void OnDrawGizmos()
    {
        Gizmos.color = m_gizmoColor;
        if (m_drawCircle)
        {

            Gizmos.DrawWireSphere(transform.position + m_circlePoistion, m_circleRadius);
        }
        if (m_drawWireCube)
        {
            Gizmos.DrawWireCube(transform.position + m_wireCubePosition, m_wireCubeSize);

        }
    }
}
