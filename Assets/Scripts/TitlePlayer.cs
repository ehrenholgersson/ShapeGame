using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TitlePlayer : MonoBehaviour
{
    Rigidbody2D _rb;
    ParticleSystem _particle;
    // Start is called before the first frame update
    void Awake()
    {
        _rb = GetComponent<Rigidbody2D>();
        _particle = GetComponent<ParticleSystem>();
    }

    private void Start()
    {
        _particle.Play();
    }

    // Update is called once per frame
    void Update()
    {
        _rb.angularVelocity = 80;

        // rotate the trailing particles to match current player rotation
        _particle.startRotation = -_rb.rotation * Mathf.Deg2Rad;

        // set particle speed to match world speed, accounting for the rotation above
        Vector3 worldDirection = Quaternion.Inverse(transform.rotation) * (new Vector3(-10, 0, 0));
        var velocity = _particle.velocityOverLifetime;
        velocity.x = new ParticleSystem.MinMaxCurve(worldDirection.x, worldDirection.x);
        velocity.y = new ParticleSystem.MinMaxCurve(worldDirection.y, worldDirection.y);
        velocity.z = new ParticleSystem.MinMaxCurve(0, 0);

        // change particle colors
        _particle.startColor = TitleScreen.LevelColor;
    }
}
