using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveBubble : Bubble
{
    Vector3 velocity;
    public float moveSpeedMax = 0.12f;
    public float moveSpeedMin = 0.04f;

    public bool isLoop;

    public override void Init(Vector3 position){
        base.Init(position);
        this.velocity = Vector3.left * Random.Range(this.moveSpeedMin, this.moveSpeedMax);
    }

    protected override void EachUpdate()
    {
        if(!this.isActive) return;
        
        this.transform.position += this.velocity;
        if(this.transform.position.x <= -10f){
            if(this.isLoop){
                this.transform.position += Vector3.right * 20f;
            }else{
                this.SetActive(false);
            }
        }
    }
}
