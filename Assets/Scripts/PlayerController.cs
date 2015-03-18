using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    public Animator m_Animator;

    public bool m_NormalControl = true;

    public float m_MovementSpeed = 1.0f;

    public KeyCode m_LeftKey;
    public KeyCode m_RightKey;

    void Start() {
        if (m_Animator == null) m_Animator = transform.gameObject.GetComponent<Animator>();
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
        }
    }

}
