using UnityEngine;
using System;
using System.Collections;

[RequireComponent(typeof(Rigidbody))]
[RequireComponent(typeof(CapsuleCollider))]
[RequireComponent(typeof(Animator))]
public class PlayerController : MonoBehaviour {

    private const float MAX_DAMAGE = 999f;
    private const float MIN_DAMAGE = 0f;

	private Transform _rightHand;
	private Transform _leftHand;
	private Transform _attackPoint;
	private Transform _respawn;
	private GameObject _spawnPlatform;

	private Rigidbody _rigidbody;
	private CapsuleCollider _hitbox;
	private Animator _animator;

	public int playerNumber = 0;

	private MoveDatabase moveDatabase;
	public Move[] moves = new Move[3];
	public float comboGap = 0;
	public int comboChain = 0;

	private int currentJumpCount = 1;

    [SerializeField]
	private int maxJumpCount = 1;

    [SerializeField]
    private float baseJumpPower = 400f;

    [SerializeField]
    private float maxFallingSpeed = -5;
    /// <summary>
    /// An animation curve to determine how powerful consecutive jump is. This value is a multiplier
    /// </summary>
    [SerializeField]
    [Tooltip("An animation curve to determine how powerful consecutive jump is. This value is a multiplier to Base Jump Power. 0.0 is the first jump. 1.0 is the last.")]
    private AnimationCurve jumpPower;

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

	private bool grounded = true;
    private bool onSpawningPlatform = false;
    private bool onLedge = false;
    private bool canJump = true;
    private bool canDrop = false;
    private bool moving = false;
    private bool crouching = false;
    private float noAirControl = 0;
    private bool canAttack = true;

    public event Action OnDeath;
    public event Action OnHit;
    public event Action OnJump;

    private bool fastFalling = false;

    private void Awake() {
        _rigidbody = GetComponent<Rigidbody>();
        _hitbox = GetComponent<CapsuleCollider>();
        _animator = GetComponent<Animator>();
    }

    private void Start () {
		GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

		for (var i = 0; i < players.Length; i++)
		{
		    if (players[i] != gameObject)
		        continue;
		    playerNumber = i;
		    break;
		}		

		moveDatabase = Camera.main.GetComponent<MoveDatabase>();
		moves[0] = moveDatabase.Get("Punch Left");
		moves[1] = moveDatabase.Get("Punch Left");
		moves[2] = moveDatabase.Get("Kick");

		standHeight = _hitbox.height;

        moveSpeed = moveSpeedBase;

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
		_attackPoint = transform.Find("AttackPoint");

		horizontal = "Horizontal " + playerNumber;
		jump = "Jump " + playerNumber;
		crouch = "Crouch " + playerNumber;
		attack = "Attack " + playerNumber;
		special = "Special " + playerNumber;
		shield = "Shield " + playerNumber;
	}
	
	void Update () {
	//Combo Chain Timer
		if (comboGap > 0) comboGap = Mathf.Clamp(comboGap - Time.deltaTime, 0, 1);
		else comboChain = 0;

	//Respawn Incincibility
		if (invincibility > 0)
		{
			invincibility = Mathf.Clamp(invincibility - Time.deltaTime, 0, 10);
			if (invincibility == 0)
			{
				SetSpawningPlatform(false);
			}
			//enable effect
		}

	//Airstun decrease
		if (noAirControl > 0)
		{
			noAirControl = Mathf.Clamp(noAirControl - Time.deltaTime, 0, 10);
			//play stunned-in-air animation
			if (noAirControl == 0)
			{
				//play recovery animation
			}
		}

		if (!isAlive) return; //if KO'd no input
	//Ledge Grab
		if (onLedge)
		{
			if (Input.GetButtonDown(horizontal))
			{
				if (Input.GetAxis(horizontal) < 0 && transform.eulerAngles.y > 180 || Input.GetAxis(horizontal) > 0 && transform.eulerAngles.y < 180)
				{
					//climb code
				}
				if (Input.GetAxis(horizontal) > 0 && transform.eulerAngles.y > 180 || Input.GetAxis(horizontal) < 0 && transform.eulerAngles.y < 180)
				{
					GrabLedge(false);
				}
			}

            if (Input.GetButtonDown(jump))
            {
                GrabLedge(false);
                Jump();
            }
			else if (Input.GetButtonDown(crouch))
			{
				GrabLedge(false);
			}
			return;
		}

	//Jumping
		if (Input.GetButtonDown(jump) && canJump && currentJumpCount <= maxJumpCount)
            Jump();

	//Jump off the Spawning Platform
		if (invincibility > 0 && GetAnyKey())
			SetSpawningPlatform(false);

	//Crouching
		if (Input.GetButton(crouch))
		{
			crouching = true;
			_hitbox.height = crouchHeight;
			_hitbox.center = Vector3.up * _hitbox.height/2;
			moveSpeed = moveSpeedBase * crouchSpeed;
		}
		else if ((Input.GetButtonUp(crouch)))
		{
			crouching = false;
			_hitbox.height = standHeight;
			_hitbox.center = Vector3.up * _hitbox.height/2;
			moveSpeed = moveSpeedBase;
		}
		_attackPoint.localPosition = Vector3.up * _hitbox.height * 0.6f;

	//Running
		if (Input.GetButton(horizontal) && noAirControl == 0)
		{
			moving = true;
            if(grounded)
            {
                //Kuri: New movement is not optimized, but it feels better than the previous.
			    _rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, new Vector3(Input.GetAxis(horizontal) * moveSpeed, _rigidbody.velocity.y, 0), 0.1f);
            }
            else
            {
                //Kuri: Less control in the air.
                _rigidbody.velocity = Vector3.Lerp(_rigidbody.velocity, new Vector3(Input.GetAxis(horizontal) * moveSpeed, _rigidbody.velocity.y, 0), 0.07f);
            }

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
			if (noAirControl == 0 && grounded)
			{
				_rigidbody.velocity = new Vector3(0, _rigidbody.velocity.y, 0);
			}
		}

	//Attacking
		if (Input.GetButtonDown(attack) && canAttack)
		{
			if (comboGap > 0) comboChain = (comboChain+1) % moves.Length;
			StartCoroutine(Attack());
		}

        //Find better solution
		if (transform.position.y < -10)
		    Die();

	//Animations
		_animator.SetBool("Forward", moving);
		//_animator.SetBool("Crouch", crouching);
		if (onSpawningPlatform) _animator.SetBool("In Air", false);
		else _animator.SetBool("In Air", !grounded);

    //KURI TESTING AREA

    //Handle FastFalling
        if(!grounded && Input.GetButton(crouch) && _rigidbody.velocity.y < 0)
        {
            //Fastfall only in the air and if we have downwards velocity
            fastFalling = true;
        }

    //Handle Gravity
        if(!grounded)
        {
            //Kuri: Gravity builds up to maximum fallspeed value.
            //Kuri: Current fallspeed matches Mario from smash 4 while using Unity-chan.
            //Kuri: Still need that gravity disabling a bit during the start of the jump...
            if(fastFalling)
            {
                _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, maxFallingSpeed * 2, _rigidbody.velocity.z);
            }
            else if(_rigidbody.velocity.y > maxFallingSpeed)
            {
                _rigidbody.velocity = new Vector3(_rigidbody.velocity.x, Mathf.Clamp(_rigidbody.velocity.y - 0.4f, maxFallingSpeed, 100), _rigidbody.velocity.z);
            }
        }
	}

	void OnCollisionStay (Collision col) {
	    if (!col.gameObject.CompareTag("Platform"))
	        return;
	    grounded = true;
	    noAirControl = 0;
	    if (!Input.GetButton(jump))
	    {
	        canJump = true;
	        currentJumpCount = 0;
	    }

	    if (col.gameObject.GetComponent<MeshCollider>())
	    {
	        canDrop = true;
	    }
	}

    void OnCollisionExit(Collision col)
    {
        if (col.gameObject.CompareTag("Platform"))
		{
			grounded = false;
		}
	}

    public void Jump() {
        if (onLedge)
            GrabLedge(false);
        if (!canJump || currentJumpCount >= maxJumpCount)
            return;
        grounded = false;
        //Kuri: noticed that smash does this thing below so added it
        _rigidbody.velocity = new Vector3(Input.GetAxis(horizontal) * moveSpeed, 0, _rigidbody.velocity.z);
        float actualJumpPower = baseJumpPower;

        if (maxJumpCount <= 1)
            actualJumpPower *= jumpPower.Evaluate(0f);
        else
            actualJumpPower *= jumpPower.Evaluate((float) currentJumpCount/(float) (maxJumpCount - 1));
                
        if(actualJumpPower < 0)
            actualJumpPower = 0;
        _rigidbody.AddForce(Vector3.up * actualJumpPower);
        currentJumpCount++;
        if(OnJump != null)
            OnJump();

        fastFalling = false;
    }

    public void OnAnimatorMove()
	{
	    if (!grounded || !(Time.deltaTime > 0))
	        return;
	    Vector3 v = (_animator.deltaPosition * moveSpeed) / Time.deltaTime;
	    v.y = _rigidbody.velocity.y;
	    //_rigidbody.velocity = v;
	}

    public void Die() {
        lives--;
        if(OnDeath != null)
            OnDeath();
        if (lives > 0)
            StartCoroutine(Respawn());
    }

    public void Damage(float amount) {
        damage = Mathf.Clamp(damage + amount, MIN_DAMAGE, MAX_DAMAGE);
    }

    public void Heal(float amount) {
        damage = Mathf.Clamp(damage - amount, MIN_DAMAGE, MAX_DAMAGE);
    }

	private IEnumerator Attack () {
		Move currentMove = moves[comboChain];
		canAttack = false;
		if (name == "Unity Chan")
		{
			if (comboChain > 0) _animator.CrossFade("Idle", 0);
			_animator.SetTrigger(currentMove.name);
		}
		yield return new WaitForSeconds(currentMove.hitTime);
		Ray ray = new Ray(_attackPoint.position, _attackPoint.forward);
		RaycastHit hit;
		if (Physics.Raycast(ray, out hit, currentMove.reach))
		{
			PlayerController target = hit.transform.gameObject.GetComponent<PlayerController>();
			if (target.invincibility == 0)
			{
				float dmg = target.damage;
				hit.rigidbody.AddForce((_attackPoint.forward * currentMove.forwardForce + Vector3.up * currentMove.upForce) * (1 + dmg));
				hit.transform.gameObject.GetComponent<PlayerController>().Damage(currentMove.damage / 100f);
				target.noAirControl = currentMove.restrictAirControl;
			}
		}
		Debug.DrawRay(ray.origin, ray.direction * currentMove.reach, Color.red);
		yield return new WaitForSeconds(currentMove.duration - currentMove.hitTime);
		comboGap = 1;
		canAttack = true;
	}
    private IEnumerator Respawn()
    {
        lives--;
        damage = MIN_DAMAGE;
        Camera.main.GetComponent<CameraController>().Shake();
        Camera.main.GetComponent<CameraController>().alivePlayers--;
        isAlive = false;
        yield return new WaitForSeconds(1.25f);
        SetSpawningPlatform(true);
        transform.position = _respawn.transform.position;
        transform.rotation = Quaternion.Euler(0, 180, 0);
        invincibility = invincibilityTimer;
        isAlive = true;
        _rigidbody.velocity = Vector3.zero;
        Camera.main.GetComponent<CameraController>().alivePlayers++;
    }

	void SetSpawningPlatform (bool active) {
		onSpawningPlatform = active;
		_rigidbody.useGravity = !active;
		_spawnPlatform.SetActive(active);
		canJump = active;
	}

	public void GrabLedge (bool active) {
		onLedge = active;		
		_rigidbody.useGravity = !active;
		_rigidbody.velocity = Vector3.zero;
		currentJumpCount = 0;
	}

	bool GetAnyKey () {		
		return Input.GetButton(horizontal) || Input.GetButton(jump) ||  Input.GetButton(crouch) ||  Input.GetButton(attack) ||  Input.GetButton(special) ||  Input.GetButton(shield);
	}
}