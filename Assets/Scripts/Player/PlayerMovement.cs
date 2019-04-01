using System.Collections;
using System.Collections.Generic;
using NaughtyAttributes;
using UnityEngine;

public class PlayerMovement : MonoBehaviour
{
    public Vector3 InputDirection { get; private set; }

    [SerializeField, BoxGroup("Movement Settings")]
    private float MovementSpeed = 10;

    [SerializeField, BoxGroup("Movement Settings")]
    private float SlideSpeed = 6;

    [SerializeField, BoxGroup("Gravity Settings")]
    private float Gravity = -9.8f;

    [SerializeField, BoxGroup("Gravity Settings")]
    private float GravityMultiplier = 2;

    [SerializeField] private AnimationCurve GravityBehaviour = AnimationCurve.Linear(0.0f, 0.0f, 1.0f, 1.0f);

    [SerializeField, BoxGroup("Jump Settings")]
    private float JumpTime = 1.0f;

    [SerializeField, BoxGroup("Jump Settings")]
    private float JumpHeight = 1.0f;

    [SerializeField, BoxGroup("Jump Settings"), Range(0, 1)]
    private float JumpControl = 1f;

    [SerializeField, BoxGroup("Force Debugging")]
    private float Force = 10;

    [SerializeField, BoxGroup("Force Debugging")]
    private float ForceDur = 1;


    public Vector3 Velocity
    {
        get { return m_characterController.velocity; }
    }

    private CharacterController m_characterController;
    private float m_horizontal;
    private bool m_isGrounded = false;
    private bool m_isJumping = false;
    private Coroutine m_jumpRoutine = null;

    [SerializeField, ReadOnly] private float m_slopeAngle = 0f;
    private float m_time = 0;

    private Vector3 m_normal;

    private Vector3 m_moveDir;
    private CollisionFlags m_collisionFlag;

    #region Force
    private bool m_activeForce = false;
    private float m_forceTimer = 0.0f;
    private float m_forceDuration = 0.0f;
    private Vector3 m_force = new Vector3();
    #endregion

    private void Awake()
    {
        m_characterController = GetComponent<CharacterController>();
    }

    private void Update()
    {
        PlayerInput();
        m_isGrounded = m_characterController.isGrounded;
    }

    private void FixedUpdate()
    {
        Movement();
        ExecuteForce();
    }

    private void Movement()
    {
        float speed = m_horizontal * MovementSpeed;
        
        if (HitSlopeLimit() && !m_isJumping)
        {
            m_moveDir = Slide();
            m_moveDir.z = 0;
            m_moveDir.x *= SlideSpeed;
        }
        else
        {
            m_moveDir.x = m_isJumping == true ? speed * JumpControl : speed;
        }



        if (m_characterController.isGrounded && !m_isJumping)
        {
            m_moveDir.y = Gravity;
        }
        else
        {
            EvaluateGravity();
        }

        m_collisionFlag =
            m_characterController.Move(transform.TransformDirection(m_moveDir) * Time.fixedDeltaTime);

    }

    private Vector3 Slide()
    {
        RaycastHit hitInfo;
        Physics.SphereCast(transform.position, m_characterController.radius, Vector3.down, out hitInfo,
                           m_characterController.height / 2f, Physics.AllLayers, QueryTriggerInteraction.Ignore);
        return Vector3.ProjectOnPlane(m_moveDir, hitInfo.normal).normalized;
    }

    private void EvaluateGravity()
    {
        if (!m_isJumping)
        {
            m_time += Time.fixedDeltaTime;
            float duration = GravityBehaviour.keys[GravityBehaviour.length - 1].time;
            float perc = m_time / duration;
            float evaluate = GravityBehaviour.Evaluate(perc);
            m_moveDir.y = Mathf.Lerp(m_moveDir.y, Gravity * GravityMultiplier, evaluate);
        }
        else
        {
            m_time = 0;
        }
    }

    /// <summary>return true if ground angel is bigger or equal to Character Controlers SlopeLimit.</summary>
    private bool HitSlopeLimit()
    {
        m_slopeAngle = Mathf.Floor(Vector3.Angle(transform.up, m_normal));
        return m_slopeAngle >= m_characterController.slopeLimit;
    }

    private void PlayerInput()
    {
        m_horizontal = Input.GetAxis("Horizontal");
        InputDirection = new Vector3(m_horizontal, 0, 0);

        //TODO : Delete, just for testing
        if (Input.GetKeyDown(KeyCode.F))
        {
            var rndDir = Random.insideUnitCircle;
            AddForce(rndDir, Force, ForceDur);
        }

        //No Jumping while on Slope Limit >> Sliding
        if (!HitSlopeLimit())
        {
            Jump();    
        }
    }

    private void OnControllerColliderHit(ControllerColliderHit hit)
    {
        m_normal = hit.normal;
    }

    private void Jump()
    {
        if (Input.GetKeyDown(KeyCode.Space) && m_isGrounded)
        {
            if (m_jumpRoutine != null)
            {
                StopCoroutine(m_jumpRoutine);
            }

            m_jumpRoutine = StartCoroutine(JumpRoutine());
        }
    }

    private IEnumerator JumpRoutine()
    {
        float timer = 0;
        m_isJumping = true;
        m_moveDir.y = 0;


        while (timer < JumpTime)
        {
            float perc = timer / JumpTime;
            float jumpVelo = Mathf.Sqrt(JumpHeight * -2 * Gravity);


            float jumpForce = Mathf.Lerp(jumpVelo, 0, perc);
            timer += Time.deltaTime;
            m_characterController.Move(transform.TransformDirection(new Vector3(0, jumpForce, 0)) * Time.deltaTime);

            //Stop Jump if Character Controller Hits a Collider Above
            if ((m_collisionFlag & CollisionFlags.Above) != 0)
            {
                Debug.Log("Headbutted the Wall Above...");
                m_isJumping = false;
                
                yield break;
            }

            yield return null;
        }

        m_isJumping = false;
    }

    private void ExecuteForce()
    {
        if (m_activeForce)
        {
            float perc = m_forceTimer / m_forceDuration;
            Vector3 force = Vector3.Lerp(m_force, Vector3.zero, perc);
            m_characterController.Move(force * Time.fixedDeltaTime);
            m_forceTimer += Time.fixedDeltaTime;
            if (m_forceTimer >= m_forceDuration)
            {
                m_activeForce = false;
                m_force = Vector3.zero;
            }
        }
    }

    public void AddForce(Vector3 direction, float force, float duration)
    {
        m_moveDir = Vector3.zero;
        m_forceDuration = duration;
        m_forceTimer = 0;
        m_force = direction * force;
        m_activeForce = true;
    }

    private void OnDisable()
    {
        m_moveDir = Vector3.zero;
        m_slopeAngle = 0;
        m_isJumping = false;
    }
}
