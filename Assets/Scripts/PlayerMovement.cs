using UnityEngine;

public class PlayerMovement : MonoBehaviour, IKillable
{
    [SerializeField] float _groundCheckLeniency;
    [SerializeField] float _jumpInputTime;

    Rigidbody2D _rb;
    bool _grounded;
    float _groundedTime;
    float _jumpRequest = -10;
    float _lastJump;

    ParticleSystem _particle;
    bool _doubleJump = false;
    Color _transperent = new Color(0, 0, 0, 0);
    [SerializeField] GameObject _giblet;

    // Start is called before the first frame update
    void Start()
    {
        _rb = GetComponent<Rigidbody2D>();
        _particle = GetComponent<ParticleSystem>();
        _particle.Play();
    }

    // Update is called once per frame
    void Update()
    {
        // record time the jump key was hit so we can still respond if player is slightly early
        if (Input.GetKeyDown(KeyCode.Space))
        {
            _jumpRequest = Time.time;
        }

        //player controls
        if (_groundedTime < _groundCheckLeniency) // allow some leniency on ground check when jumping to help feel responsive
        {
            if (Mathf.Abs(_jumpRequest - Time.time) < _jumpInputTime && _rb.velocity.y < 10 && Time.time - _lastJump > (_groundCheckLeniency + 0.025f))
            {
                _rb.velocity += new Vector2(0, 8);
                _rb.angularVelocity = (_rb.velocity.x + GameControl.Instance.WorldSpeed) * -14;
                _jumpRequest = -10;
                // this was allowing 2 "grounded" jumps if key was hit just before then during collision with terrain, need to make sure at least one physics tick + our input leniency have past since last jump to prevent this, also remove any groudcheck leniency
                _groundedTime = 10;
                _lastJump = Time.time;
            }
        }
        if (_grounded) // still need to use bool for left/right, otherwise we get a big movement boost on jumping with key held in
        { 
            if (Input.GetKey("a")&&_rb.velocity.x>-10)
            {
                _rb.velocity += new Vector2(-35, 0) * Time.deltaTime;
            }
            else if (Input.GetKey("d")&&_rb.velocity.y<10) 
            {
                _rb.velocity += new Vector2(35, 0) * Time.deltaTime;
            }
        }
        else if (_doubleJump)
            if (Mathf.Abs(_jumpRequest - Time.time) < _jumpInputTime && _rb.velocity.y < 10)
            {
                _rb.velocity += new Vector2(0, 8);
                _rb.angularVelocity = (_rb.velocity.x + GameControl.Instance.WorldSpeed) * -10;
                _doubleJump = false;
                _jumpRequest = -10;
            }

        // rotate the trailing particles to match current player rotation
        _particle.startRotation = - _rb.rotation*Mathf.Deg2Rad;

        // set particle speed to match world speed, accounting for the rotation above
        Vector3 worldDirection = Quaternion.Inverse(transform.rotation) * (new Vector3(-GameControl.Instance.WorldSpeed,0,0));
        var velocity = _particle.velocityOverLifetime;
        velocity.x = new ParticleSystem.MinMaxCurve(worldDirection.x, worldDirection.x);
        velocity.y = new ParticleSystem.MinMaxCurve(worldDirection.y, worldDirection.y);
        velocity.z = new ParticleSystem.MinMaxCurve(0, 0);

        // change particle colors
        _particle.startColor = GameControl.LevelColor;
        _groundedTime += Time.deltaTime;
    }

    private void OnCollisionStay2D(Collision2D collision) // using OnCollision for ground check, normally would check collision was at players "feet", but for this game its actually pretty fun to have any collision with the terrain work
    {
        if (collision.gameObject.layer != 6)
        {
            _grounded = true;
            _groundedTime = 0;
            _doubleJump = true;
            _rb.drag = 3;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    { 
        _grounded = false;
        _rb.drag = 0.2f;
    }

    public void Damage() // any damage kills the player
    {
        Debug.Log("The player has died");
        // disable the player 
        GameControl.Instance.Player.SetActive(false);
        //show the game over screen
        GameControl.Instance.GameOver.SetActive(true);
        // spawn giblets
        for (int i = 0; i < 9;i++)
        {
            GameObject go = Instantiate(_giblet);
            go.GetComponent<Rigidbody2D>().velocity = _rb.velocity + new Vector2(Random.Range(-5, 15), Random.Range(-5, 15));
            go.transform.position = transform.position;
        }
    }
}
