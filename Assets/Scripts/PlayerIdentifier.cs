using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class PlayerIdentifier : MonoBehaviour {

    private SpriteRenderer _spriteRenderer;
    private Transform _playerTransform;
	private CapsuleCollider _capsCol;

    public Sprite Sprite {
        get { return _spriteRenderer.sprite; }
        set { _spriteRenderer.sprite = value; }
    }

    public Color Color {
        get { return _spriteRenderer.color; }
        set { _spriteRenderer.color = value; }
    }

    private void Awake() {
        _spriteRenderer = GetComponent<SpriteRenderer>();
    }

    // Update is called once per frame
	void Update () {
	    if (_capsCol == null)
	        return;
		transform.position = _playerTransform.position + Vector3.up * _capsCol.height;
	}

    /// <summary>
    /// Attaches the identifier to the specified Player.
    /// </summary>
    /// <param name="player"></param>
    public void Attach(PlayerController player)
    {
        if (player == null)
            return;
        _capsCol = player.GetComponent<CapsuleCollider>();
        if (_capsCol == null)
            throw new System.ArgumentException("player should have a attached capsule collider");
        _playerTransform = player.transform;
    }

}
