using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {
	private Transform rightHand;
	private Transform leftHand;
	private Transform attackPoint;

	private Rigidbody rigidbody;
	private CapsuleCollider collider;
	private Animation animation;

	public int playerNumber = 0;

	public float damage = 0;

	private string horizontal;
	private string jump;
	private string crouch;
	private string attack;
	private string special;
	private string shield;

	public float moveSpeedBase = 1;
	private float moveSpeed = 1;
	public float dashSpeed = 1.33f;
	public float crouchSpeed = 0.66f;

	private float standHeight;
	public float crouchHeight = 1f;
	public float jumpPower = 400f;

	public bool onGround = true;
	public bool canJump = true;
	public bool moving = false;
	public bool crouching = false;

	void Start () {
		rigidbody = GetComponent<Rigidbody>();
		collider = GetComponent<CapsuleCollider>();
		animation = GetComponent<Animation>();
		standHeight = collider.height;

		foreach (Transform child in GetComponentsInChildren<Transform>())
		{
			if (child.name.Contains("Hand"))
			{
				if (child.name.Contains("Left"))
				{
					leftHand = child;
				}
				else if (child.name.Contains("Right"))
				{
					rightHand = child;
				}
			}
		}
		attackPoint = transform.Find("AttackPoint");

		horizontal = "Horizontal " + playerNumber;
		jump = "Jump " + playerNumber;
		crouch = "Crouch " + playerNumber;
		attack = "Attack " + playerNumber;
		special = "Special " + playerNumber;
		shield = "Shield " + playerNumber;
	}
	
	void Update () {
	//Jumping
		if (Input.GetButtonDown(jump) && canJump)
		{
			onGround = false;
			rigidbody.AddForce(new Vector3(0, 1, 0) * jumpPower);
		}

	//Crouching
		if (Input.GetButton(crouch))
		{
			crouching = true;
			collider.height = crouchHeight;
			collider.center = Vector3.up * collider.height/2;
		}
		else if ((Input.GetButtonUp(crouch)))
		{
			crouching = false;
			collider.height = standHeight;
			collider.center = Vector3.up * collider.height/2;
		}

	//Running
		if (Input.GetButton(horizontal))
		{
			moving = true;
			rigidbody.velocity = new Vector3(Input.GetAxis(horizontal) * moveSpeed * Time.deltaTime * 250, rigidbody.velocity.y, 0);

			if (Input.GetAxis(horizontal) > 0)
			{
				transform.eulerAngles = Vector3.up * 90;
			}
			else
			{
				transform.eulerAngles = Vector3.up * 270;
			}
		}
		else
		{
			moving = false;
			if (onGround)
			{
				rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, 0);
			}
		}

	//Attacking
		if (Input.GetButtonDown(attack))
		{
			Ray ray = new Ray(attackPoint.position, attackPoint.forward);
			RaycastHit hit;
			if (Physics.Raycast(ray, out hit, 0.7f))
			{
				print(hit.transform.name);
				float damage = hit.transform.gameObject.GetComponent<Controller>().damage;
				hit.rigidbody.AddForce((attackPoint.forward + Vector3.up*0.7f) * 200 * damage);
				hit.transform.gameObject.GetComponent<Controller>().AddDamage(0.1f);
			}
			Debug.DrawRay(ray.origin, ray.direction * 0.7f, Color.red);
		}

	
		if (!crouching)
		{
			moveSpeed = moveSpeedBase;
		}
		else
		{
			moveSpeed = moveSpeedBase * crouchSpeed;
		}

	//Animations
		if (crouching && (onGround || !moving))
		{
			//crouch idle
		}
		else if (!moving && !crouching && onGround)
		{
			//idle
		}
		else if (moving && onGround)
		{
			//run
			animation.Play("HumanoidRun");
		}
		else if (!onGround)
		{
			//airborne
		}
	}

	void OnCollisionStay (Collision col) {
		if (col.gameObject.tag == "Platform")
		{
			onGround = true;
			if (!Input.GetButton(jump))
			{
				canJump = true;
			}			
		}
	}

	void OnCollisionExit (Collision col) {
		if (col.gameObject.tag == "Platform")
		{
			onGround = false;
			canJump = false;
		}
	}

	void AddDamage (float amount) {
		damage += amount;
	}
}