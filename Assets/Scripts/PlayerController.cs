using UnityEngine;
using System.Collections;

public class PlayerController : MonoBehaviour {

    Animator animator;

    public bool normalControl = true;

    public float speed = 1.0f;

    void Start()
    {
        animator = transform.gameObject.GetComponent<Animator>();
    }

    void Update()
    {
        if (normalControl)
        {
            if (Input.GetKey(KeyCode.RightArrow))
            {

                transform.rotation = Quaternion.Euler(new Vector3(0, -90, 0));

                transform.position += transform.forward * Time.deltaTime * speed;

                animator.SetBool("Forward", true);
            }
            if (Input.GetKeyUp(KeyCode.RightArrow))
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                animator.SetBool("Forward", false);

            }
            if (Input.GetKey(KeyCode.LeftArrow))
            {

                transform.rotation = Quaternion.Euler(new Vector3(0, 90, 0));

                transform.position += transform.forward * Time.deltaTime * speed;

                animator.SetBool("Forward", true);
            }
            if (Input.GetKeyUp(KeyCode.LeftArrow))
            {
                transform.rotation = Quaternion.Euler(new Vector3(0, 0, 0));
                animator.SetBool("Forward", false);
            }
        }
    }

}
