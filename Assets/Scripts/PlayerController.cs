using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float jumpForce, movementSpeed;
    private Rigidbody _rb;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if(Rewinder.rew.isRewinding) return;
        
        if (Input.GetButtonDown("Jump"))
        {
            Jump();
            return;
        }
        Movement();
    }

    private void Jump()
    {
        _rb.AddForce(0, jumpForce, 0f);
    }

    private void Movement()
    {
        var input = new Vector3(Input.GetAxis("Horizontal"), 0f, Input.GetAxis("Vertical"));

        transform.position += input * (movementSpeed * Time.deltaTime);
    }
}
