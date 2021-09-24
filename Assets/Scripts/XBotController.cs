using UnityEngine;

public class XBotController : MonoBehaviour
{
    [SerializeField] private float jumpForce, movementSpeed, rotationSlerpSpeed;

    private Rigidbody _rb;
    private Animator _animator;
    private Transform _cam;

    private Vector3 _forward, _right;

    private bool _canMove = true;
    
    private static readonly int JumpHash = Animator.StringToHash("jump");
    private static readonly int IsMovingHash = Animator.StringToHash("isWalking");
    private static readonly int PunchHash = Animator.StringToHash("punch");
    private static readonly int HitHash = Animator.StringToHash("takeHit");
    private Vector3 desiredMovementDirection = Vector3.zero;

    private void Start()
    {
        _rb = GetComponent<Rigidbody>();
        _animator = GetComponent<Animator>();
        _cam = Camera.main.transform;
    }
    
    private void Update()
    {
        if(Rewinder.rew.isRewinding) return;
        
        if (Input.GetButtonDown("Jump"))
        {
            StartJump();
            return;
        }
        if (Input.GetButtonDown("Fire1"))
        {
            Punch();
            return;
        }
        //if(_canMove)
            MoveFacingMovementDirection();
    }

    private void MoveFacingMovementDirection(float lerpMultiplier = 1f)
    {
        var inputX = Input.GetAxis("Horizontal");
        var inputZ = Input.GetAxis("Vertical");
        
        _forward = _cam.forward;
        _right = _cam.right;

        _forward.y = 0f;
        _right.y = 0f;

        _forward.Normalize();
        _right.Normalize();

        desiredMovementDirection = _forward * (inputZ * movementSpeed) + _right * (inputX * movementSpeed);

        if (!desiredMovementDirection.Equals(Vector3.zero))
        {
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMovementDirection),
                rotationSlerpSpeed * lerpMultiplier);
            _animator.SetBool(IsMovingHash, true);
        }
        else
        {
            _animator.SetBool(IsMovingHash, false);
        }

        transform.position += desiredMovementDirection * Time.deltaTime;
    }

    private void StartJump()
    {
        _animator.SetTrigger(JumpHash);
    }
    
    public void EndJump()
    {
        _canMove = true;
        rotationSlerpSpeed /= 0.1f;
    }

    public void JumpAnim()
    {
        _rb.AddForce(0f, jumpForce, 0f);
        _canMove = false;

        if(!desiredMovementDirection.Equals(Vector3.zero))
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.LookRotation(desiredMovementDirection), rotationSlerpSpeed * 2f);
        
        rotationSlerpSpeed *= 0.1f;
    }
    
    private void Punch()
    {
        _animator.SetTrigger(PunchHash);
    }

    private void OnCollisionEnter(Collision other)
    {
        if(Rewinder.rew.isRewinding) return;
        if(!other.gameObject.CompareTag("Obstacle")) return;
        
        //_animator.SetTrigger(HitHash);
    }
}
