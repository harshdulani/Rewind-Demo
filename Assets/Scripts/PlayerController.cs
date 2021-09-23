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

        var point = other.contacts[0].point;
        var _direction = transform.position - point;
        var _angle = (Mathf.Atan2(_direction.x, _direction.z) * Mathf.Rad2Deg);
        Instantiate(fxPrefab, point, Quaternion.AngleAxis(_angle, Vector3.up));
    }
}
