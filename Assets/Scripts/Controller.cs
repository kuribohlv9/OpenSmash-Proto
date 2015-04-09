using UnityEngine;
using System.Collections;

public class Controller : MonoBehaviour {
	private Transform rightHand;
	private Transform leftHand;
	private Transform attackPoint;
	private Transform spawn;

	private Rigidbody rigidbody;
	private CapsuleCollider collider;
	private Animator animator;

	public int playerNumber = 0;

	private MoveDatabase moveDatabase;
	public Move[] moves = new Move[3];
	public float comboGap = 0;
	public int comboChain = 0;

	public float damage = 0;
	public int lives = 4;
	public bool isAlive = true;

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
	public bool hasAirControl = true;
	public bool canAttack = true;

	void Start () {
		moveDatabase = Camera.main.GetComponent<MoveDatabase>();
		moves[0] = moveDatabase.Get("Punch Left");
		moves[1] = moveDatabase.Get("Punch Left");
		moves[2] = moveDatabase.Get("Kick");

		rigidbody = GetComponent<Rigidbody>();
		collider = GetComponent<CapsuleCollider>();
		animator = GetComponent<Animator>();
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
		spawn = GameObject.Find("Player Spawn").transform;

		horizontal = "Horizontal " + playerNumber;
		jump = "Jump " + playerNumber;
		crouch = "Crouch " + playerNumber;
		attack = "Attack " + playerNumber;
		special = "Special " + playerNumber;
		shield = "Shield " + playerNumber;
	}
	
	void Update () {
		if (comboGap > 0) comboGap -= Time.deltaTime;
		else comboChain = 0;
		if (!isAlive) return;
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
			moveSpeed = moveSpeedBase;
		}
		else if ((Input.GetButtonUp(crouch)))
		{
			crouching = false;
			collider.height = standHeight;
			collider.center = Vector3.up * collider.height/2;
			moveSpeed = moveSpeedBase * crouchSpeed;
		}
		attackPoint.localPosition = Vector3.up * collider.height * 0.6f;

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
			if (hasAirControl || onGround)
			{
				rigidbody.velocity = new Vector3(0, rigidbody.velocity.y, 0);
			}
		}

	//Attacking
		if (Input.GetButtonDown(attack) && canAttack)
		{
			if (comboGap > 0) comboChain = (comboChain+1) % moves.Length;
			StartCoroutine("Attack");
		}

		if (transform.position.y < -20)
		{
			StartCoroutine("Respawn");
		}

	//Animations
		animator.SetBool("Forward", moving);
		//animator.SetBool("Crouch", crouching);
		animator.SetBool("In Air", !onGround);
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

	public void OnAnimatorMove()
	{
		if (onGround && Time.deltaTime > 0)
		{
			Vector3 v = (animator.deltaPosition * moveSpeed) / Time.deltaTime;
			v.y = rigidbody.velocity.y;
			//rigidbody.velocity = v;
		}
	}


	void AddDamage (float amount) {
		damage += amount;
	}

	private IEnumerator Respawn () {
		isAlive = false;
		damage = 0;
		lives--;
		yield return new WaitForSeconds(1.2f);
		transform.position = spawn.position;
		transform.rotation = Quaternion.identity;
		isAlive = true;
	}

	private IEnumerator Attack () {
		Move currentMove = moves[comboChain];
		canAttack = false;
		if (name.Contains("Kickchan"))
		{
			if (comboChain > 0) animator.CrossFade("Idle", 0);
			animator.SetTrigger(currentMove.name);
		}
		yield return new WaitForSeconds(currentMove.hitTime);
		Ray ray = new Ray(attackPoint.position, attackPoint.forward);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, currentMove.reach))
		{
			Controller target = hit.transform.gameObject.GetComponent<Controller>();
			float dmg = target.damage;
			hit.rigidbody.AddForce((attackPoint.forward * currentMove.forwardForce + Vector3.up * currentMove.upForce) * dmg);
			hit.transform.gameObject.GetComponent<Controller>().AddDamage(currentMove.damage / 100f);
			target.hasAirControl = !currentMove.restrictAirControl;
		}
		Debug.DrawRay(ray.origin, ray.direction * currentMove.reach, Color.red);
		yield return new WaitForSeconds(currentMove.duration - currentMove.hitTime);
		comboGap = 1;
		canAttack = true;
	}
}