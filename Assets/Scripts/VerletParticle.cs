using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class VerletParticle
{
    private Rigidbody2D boundRb;
    private Vector2 position;
    private Vector2 oldPosition;

    public VerletParticle(Vector2 pos, Rigidbody2D rb)
    {
        position = oldPosition = pos;
        boundRb = rb;
    }

    public void Step(float interval)
    {
        oldPosition = position;
        position += boundRb.velocity * interval;
    }
}
