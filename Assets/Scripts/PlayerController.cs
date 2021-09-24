using UnityEngine;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private float jumpForce, movementSpeed;
    private Rigidbody _rb;

    [SerializeField] private GameObject fxPrefab;

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

    private void OnCollisionEnter(Collision other)
    {
        if(Rewinder.rew.isRewinding) return;
        if(!other.gameObject.CompareTag("Obstacle")) return;

        var point = other.transform.position + other.gameObject.transform.up * 2f;
        var look = Quaternion.LookRotation(Vector3.up, Vector3.forward);
        Instantiate(fxPrefab, point, look);
    }
}
