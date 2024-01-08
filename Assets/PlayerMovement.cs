 using System.Collections;
using System.Collections.Generic;
using System.Security.Cryptography;
using UnityEngine;
using static UnityEngine.ParticleSystem;

public class PlayerMovement : MonoBehaviour, IKillable
{
    Rigidbody2D rb;
    bool grounded;
    ParticleSystem particle;
    //AnimationCurve curve = new AnimationCurve();
    bool doubleJump = false;
    [SerializeField] GameObject giblet;

    // Start is called before the first frame update
    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        particle = GetComponent<ParticleSystem>();
        //var psMain = partical.main;


        //curve.AddKey(0.0f, 1.0f);
        //curve.AddKey(1.0f, 0.0f);
        particle.Play();
    }

    // Update is called once per frame
    void Update()
    {
        if (grounded)
        {
            if (Input.GetKeyDown(KeyCode.Space)&& rb.velocity.y < 10)
            {
                rb.velocity += new Vector2(0, 8 );
                rb.angularVelocity = (rb.velocity.x + GameControl.Instance.worldSpeed) * -10;
            }
            if (Input.GetKey("a")&&rb.velocity.x>-10)
            {
                rb.velocity += new Vector2(-35, 0) * Time.deltaTime;
            }
            else if (Input.GetKey("d")&&rb.velocity.y<10) 
            {
                rb.velocity += new Vector2(35, 0) * Time.deltaTime;
            }
        }
        else if (doubleJump)
            if (Input.GetKeyDown(KeyCode.Space) && rb.velocity.y < 10)
            {
                rb.velocity += new Vector2(0, 8);
                rb.angularVelocity = (rb.velocity.x + GameControl.Instance.worldSpeed) * -10;
                doubleJump = false;
            }
        particle.startRotation = - rb.rotation*Mathf.Deg2Rad;

        Vector3 worldDirection = Quaternion.Inverse(transform.rotation) * (new Vector3(-GameControl.Instance.worldSpeed,0,0));

        var velocity = particle.velocityOverLifetime;
        velocity.x = new ParticleSystem.MinMaxCurve(worldDirection.x, worldDirection.x);
        velocity.y = new ParticleSystem.MinMaxCurve(worldDirection.y, worldDirection.y);
        velocity.z = new ParticleSystem.MinMaxCurve(0, 0);

        //var psMain = partical.main;
        //psMain.startRotationZ = rb.rotation;
    }

    /*private void OnCollisionStay2D(Collision collision)
    {
        grounded = true;
        rb.drag = 3;
    }*/
    private void OnCollisionStay2D(Collision2D collision)
    {
        if (collision.gameObject.layer != 6)
        {
            grounded = true;
            doubleJump = true;
            rb.drag = 3;
        }
    }
    private void OnCollisionExit2D(Collision2D collision)
    { 
        grounded = false;
        rb.drag = 0.2f;
    }

    public void Damage()
    {
        Debug.Log("The player has died");
        GameControl.Instance.player.SetActive(false);
        GameControl.Instance.gameOver.SetActive(true);
        for (int i = 0; i < 9;i++)
        {
            GameObject go = Instantiate(giblet);
            go.GetComponent<Rigidbody2D>().velocity = rb.velocity + new Vector2(Random.Range(-5, 15), Random.Range(-5, 15));
            go.transform.position = transform.position;
        }
    }
}
