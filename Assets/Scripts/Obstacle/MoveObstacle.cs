using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveObstacle : Obstacle
{
    float referenceX;

    int moveCount = 0;
    public int MoveCount = 60;

    public float moveDistance;


    public override void Init(Vector3 position){
        base.Init(position);
        this.referenceX = position.x;
    }

    void FixedUpdate()
    {
        if(!this.isActive) return;
        
        this.moveCount++;
        if(this.moveCount >= this.MoveCount * 2){
            this.moveCount = 0;
        }

        float move = 0;
        if(this.moveCount < this.MoveCount){
            move = moveDistance * ((float)this.moveCount / (float)this.MoveCount);
        }else{
            move = moveDistance * (1f - ((float)(this.moveCount - this.MoveCount) / (float)this.MoveCount));
        }

        var point = this.transform.position;
        point.x = referenceX + move;
        this.transform.position = point;
    }
}
