using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {
	private Transform rightHand;
	private Transform leftHand;
	private Transform attackPoint;
	private Transform spawn;
	private GameObject spawnPlatform;

	private Rigidbody m_Rigidbody = new Rigidbody();
	private CapsuleCollider m_Collider = new CapsuleCollider();
	private Animator animator;

	public int playerNumber = 0;

	private MoveDatabase moveDatabase;
	public Move[] moves = new Move[3];
	public float comboGap = 0;
	public int comboChain = 0;

	public float damage = 0;
	public int lives = 4;
	public float invincibilityTimer = 3;
	public bool isAlive = true;
	private float invincibility = 0;

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
	public bool onSpawningPlatform = true;
	public bool canJump = true;
	public bool canDrop = false;
	public bool moving = false;
	public bool crouching = false;
	public float noAirControl = 0;
	public bool canAttack = true;

	void Start () {
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

		for (int i = 0; i < players.Length; i++)
		{
			if (players[i] == gameObject)
			{
				playerNumber = i;
				break;
			}
		}		

		moveDatabase = Camera.main.GetComponent<MoveDatabase>();
		moves[0] = moveDatabase.Get("Punch Left");
		moves[1] = moveDatabase.Get("Punch Left");
		moves[2] = moveDatabase.Get("Kick");

		m_Rigidbody = GetComponent<Rigidbody>();
		m_Collider = GetComponent<CapsuleCollider>();
		animator = GetComponent<Animator>();
		standHeight = m_Collider.height;

		//maybe for fancy hit registration later
		/*foreach (Transform child in GetComponentsInChildren<Transform>())
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
		}*/
		attackPoint = transform.Find("AttackPoint");
		spawn = GameObject.Find("Spawn " + playerNumber).transform;
		spawnPlatform = spawn.Find("Platform").gameObject;

		horizontal = "Horizontal " + playerNumber;
		jump = "Jump " + playerNumber;
		crouch = "Crouch " + playerNumber;
		attack = "Attack " + playerNumber;
		special = "Special " + playerNumber;
		shield = "Shield " + playerNumber;

		GameObject identifier = Instantiate(Resources.Load<GameObject>("Prefabs/Identifier"), Vector3.zero, Quaternion.identity) as GameObject;
		identifier.GetComponent<SpriteRenderer>().sprite = Resources.Load<Sprite>("Textures/P" + (playerNumber + 1));
		identifier.GetComponent<StickToPlayer>().player = transform;
	}
	
	void Update () {
		if (comboGap > 0) comboGap = Mathf.Clamp(comboGap - Time.deltaTime, 0, 1);
		else comboChain = 0;

		if (invincibility > 0)
		{
			invincibility = Mathf.Clamp(invincibility - Time.deltaTime, 0, 10);
			if (invincibility == 0)
			{
				SetSpawningPlatform(false);
			}
			//enable effect
		}

		if (noAirControl > 0)
		{
			noAirControl = Mathf.Clamp(noAirControl - Time.deltaTime, 0, 10);
			//play stunned-in-air animation
			if (noAirControl == 0)
			{
				//play recovery animation
			}
		}

		if (!isAlive) return;
	//Jumping
		if (Input.GetButtonDown(jump) && canJump)
		{
			onGround = false;
			m_Rigidbody.AddForce(new Vector3(0, 1, 0) * jumpPower);
		}

		if (invincibility > 0)
		{
			if (GetAnyKey())
			{
				SetSpawningPlatform(false);
			}
		}

	//Crouching
		if (Input.GetButton(crouch))
		{
			crouching = true;
			m_Collider.height = crouchHeight;
			m_Collider.center = Vector3.up * m_Collider.height/2;
			moveSpeed = moveSpeedBase * crouchSpeed;
		}
		else if ((Input.GetButtonUp(crouch)))
		{
			crouching = false;
			m_Collider.height = standHeight;
			m_Collider.center = Vector3.up * m_Collider.height/2;
			moveSpeed = moveSpeedBase;
		}
		attackPoint.localPosition = Vector3.up * m_Collider.height * 0.6f;

	//Running
		if (Input.GetButton(horizontal) && noAirControl == 0)
		{
			moving = true;
			m_Rigidbody.velocity = new Vector3(Input.GetAxis(horizontal) * moveSpeed * Time.deltaTime * 250, m_Rigidbody.velocity.y, 0);

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
				m_Rigidbody.velocity = new Vector3(0, m_Rigidbody.velocity.y, 0);
			}
		}

	//Attacking
		if (Input.GetButtonDown(attack) && canAttack)
		{
			if (comboGap > 0) comboChain = (comboChain+1) % moves.Length;
			StartCoroutine("Attack");
		}

		if (transform.position.y < -10)
		{
			StartCoroutine("Respawn");
		}

	//Animations
		animator.SetBool("Forward", moving);
		//animator.SetBool("Crouch", crouching);
		if (onSpawningPlatform) animator.SetBool("In Air", false);
		else animator.SetBool("In Air", !onGround);
	}

	void OnCollisionStay (Collision col) {
		if (col.gameObject.tag == "Platform")
		{
			onGround = true;
			noAirControl = 0;
			if (!Input.GetButton(jump))
			{
				canJump = true;
			}

			if (col.gameObject.GetComponent<MeshCollider>())
			{
				canDrop = true;
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
			v.y = m_Rigidbody.velocity.y;
			//m_Rigidbody.velocity = v;
		}
	}


	void AddDamage (float amount) {
		damage += amount;
	}

	private IEnumerator Respawn () {		
		lives--;
		damage = 0;
		Camera.main.GetComponent<CameraController>().Shake();
		Camera.main.GetComponent<CameraController>().alivePlayers--;
		isAlive = false;
		yield return new WaitForSeconds(1.25f);
		SetSpawningPlatform(true);
		transform.position = spawn.transform.position;
		transform.rotation = Quaternion.Euler(0, 180, 0);
		invincibility = invincibilityTimer;
		isAlive = true;
		m_Rigidbody.velocity = Vector3.zero;
		Camera.main.GetComponent<CameraController>().alivePlayers++;
	}

	private IEnumerator Attack () {
		Move currentMove = moves[comboChain];
		canAttack = false;
		if (name == "Kick Chan")
		{
			if (comboChain > 0) animator.CrossFade("Idle", 0);
			animator.SetTrigger(currentMove.name);
		}
		yield return new WaitForSeconds(currentMove.hitTime);
		Ray ray = new Ray(attackPoint.position, attackPoint.forward);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, currentMove.reach))
		{
			PlayerController target = hit.transform.gameObject.GetComponent<PlayerController>();
			if (target.invincibility == 0)
			{
				float dmg = target.damage;
				hit.rigidbody.AddForce((attackPoint.forward * currentMove.forwardForce + Vector3.up * currentMove.upForce) * (1 + dmg));
				hit.transform.gameObject.GetComponent<PlayerController>().AddDamage(currentMove.damage / 100f);
				target.noAirControl = currentMove.restrictAirControl;
			}
		}
		Debug.DrawRay(ray.origin, ray.direction * currentMove.reach, Color.red);
		yield return new WaitForSeconds(currentMove.duration - currentMove.hitTime);
		comboGap = 1;
		canAttack = true;
	}

	void SetSpawningPlatform (bool active) {
		onSpawningPlatform = active;
		m_Rigidbody.useGravity = !active;
		spawnPlatform.SetActive(active);
		canJump = active;
	}

	bool GetAnyKey () {		
		return Input.GetButton(horizontal) || Input.GetButton(jump) ||  Input.GetButton(crouch) ||  Input.GetButton(attack) ||  Input.GetButton(special) ||  Input.GetButton(shield);
	}
}