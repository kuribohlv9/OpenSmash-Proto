using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

	public Animator m_Animator;
	public Rigidbody m_Rigidbody;
	public CapsuleCollider m_Collider;

	public bool m_NormalControl = true;

	public float m_MovementSpeed = 1.0f;
	public float m_CrouchSpeed = 0.5f;

	public Vector3 m_MaxVelocity;
	public float jumpHeight = 1.0f;
	public float standHeight = 1.8f;
	public float crouchHeight = 1.0f;

	public bool crouching = false;

	public KeyCode m_LeftKey;
	public KeyCode m_RightKey;
	public KeyCode m_JumpKey;
	public KeyCode m_CrouchKey;

	public bool m_IsGrounded;

	void Start() {
		if (m_Animator == null) m_Animator = GetComponent<Animator>();
		if (m_Rigidbody == null) m_Rigidbody = GetComponent<Rigidbody>();
		if (m_Collider == null) m_Collider = GetComponent<CapsuleCollider>();
	}

	void Update() {
		if (m_NormalControl) 
		{
			bool rightArrow = Input.GetKey(m_RightKey);
			bool leftArrow = Input.GetKey(m_LeftKey);
			if (rightArrow ^ leftArrow)
			{
				transform.rotation = Quaternion.Euler(new Vector3(0, leftArrow ? -90 : 90, 0));
				if (!crouching)
				{
					transform.position += transform.forward * Time.deltaTime * m_MovementSpeed;
					m_Animator.SetBool("Forward", true);
				}
				else
				{
					transform.position += transform.forward * Time.deltaTime * m_CrouchSpeed;
					m_Animator.SetBool("Forward", true);
				}
			} else {
				m_Animator.SetBool("Forward", false);
			}

			bool jumpButton = Input.GetKeyDown(m_JumpKey);
			if(jumpButton && m_IsGrounded)
			{
				m_Rigidbody.AddForce(new Vector3(0, jumpHeight * 1000, 0));
				m_IsGrounded = false;
				m_Animator.SetBool("In Air", true);
			}

			bool crouchButton = Input.GetKeyDown(m_CrouchKey);  
			if(crouchButton)
			{
				crouching = true;
				m_Collider.height = crouchHeight;
				m_Collider.center = Vector3.up * m_Collider.height/2;
			}
			else if(Input.GetKeyUp(m_CrouchKey))
			{
				crouching = false;
				m_Collider.height = standHeight;
				m_Collider.center = Vector3.up * m_Collider.height/2;
			}
		}

		var newVel = m_Rigidbody.velocity;
		if (!Mathf.Approximately(0f, m_MaxVelocity.x)) 
		{
			newVel.x = Mathf.Clamp(newVel.x, -m_MaxVelocity.x, m_MaxVelocity.x);
		}
		if (!Mathf.Approximately(0f, m_MaxVelocity.y)) 
		{
			newVel.y = Mathf.Clamp(newVel.y, -m_MaxVelocity.y, m_MaxVelocity.y);
		}
		m_Rigidbody.velocity = newVel;
		
	}

	void OnCollisionEnter(Collision collision) {
		if (collision.gameObject.tag == "Platform")
		{
			m_IsGrounded = true;
			m_Animator.SetBool("In Air", false);
		}
	}
}