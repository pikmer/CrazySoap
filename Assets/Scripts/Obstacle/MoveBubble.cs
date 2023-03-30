using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBubble : Bubble
{
    Vector3 velocity;
    float moveSpeedMax = 0.12f;
    float moveSpeedMin = 0.04f;

    public override void Init(Vector3 position){
        base.Init(position);
        this.velocity = Vector3.left * Random.Range(this.moveSpeedMin, this.moveSpeedMax);
    }

    void FixedUpdate()
    {
        this.transform.position += this.velocity;
        if(this.transform.position.x <= -10f){
            this.transform.position += Vector3.right * 20f;
        }
    }
}
