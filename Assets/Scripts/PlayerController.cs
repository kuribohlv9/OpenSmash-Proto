using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public Animator m_Animator;

    public Rigidbody m_Rigidbody;

    public bool m_NormalControl = true;

    public float m_MovementSpeed = 1.0f;

    public Vector3 m_MaxVelocity;
    public float jumpHeight = 1.0f;

    public KeyCode m_LeftKey;
    public KeyCode m_RightKey;
    public KeyCode m_JumpKey;

    public bool canJump;

    void Start() {
        if (m_Animator == null) m_Animator = transform.gameObject.GetComponent<Animator>();
        if (m_Rigidbody == null) m_Rigidbody = transform.gameObject.GetComponent<Rigidbody>();
    }

    void Update() {
        if (m_NormalControl) {
            bool rightArrow = Input.GetKey(m_RightKey);
            bool leftArrow = Input.GetKey(m_LeftKey);
            if (rightArrow ^ leftArrow) {
                transform.rotation = Quaternion.Euler(new Vector3(0, leftArrow ? -90 : 90, 0));
                transform.position += transform.forward * Time.deltaTime * m_MovementSpeed;
                m_Animator.SetBool("Forward", true);
            } else {
                m_Animator.SetBool("Forward", false);
            }

            bool jumpButton = Input.GetKeyDown(m_JumpKey);
            if(jumpButton && canJump){
                m_Rigidbody.AddForce(new Vector3(0, jumpHeight/8000, 0));
                canJump = false;
                m_Animator.SetBool("In Air", true);
            }

        }

        var newVel = m_Rigidbody.velocity;
        if (!Mathf.Approximately(0f, m_MaxVelocity.x)) {
            newVel.x = Mathf.Clamp(newVel.x, -m_MaxVelocity.x, m_MaxVelocity.x);
        }
        if (!Mathf.Approximately(0f, m_MaxVelocity.y)) {
            newVel.y = Mathf.Clamp(newVel.y, -m_MaxVelocity.y, m_MaxVelocity.y);
        }
        if (!Mathf.Approximately(0f, m_MaxVelocity.z)) {
            newVel.z = Mathf.Clamp(newVel.z, -m_MaxVelocity.z, m_MaxVelocity.z);
        }
        m_Rigidbody.velocity = newVel;
        
    }

    void OnCollisionEnter(Collision collision) {
        if (collision.gameObject.tag == "Platform")
        {
            canJump = true;
            m_Animator.SetBool("In Air", false);
        }
    }

}
