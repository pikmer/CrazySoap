using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDuckCross : Obstacle
{
    float referenceX;

    int moveCount = 0;
    public int MoveCount = 60;

    public float moveDistance;
    
    public float moveZ;

    [SerializeField] int MaxMoveCount = 60;
    [SerializeField] int MinMoveCount = 60;

    public override void Init(Vector3 position){
        base.Init(position);
        this.referenceX = position.x;
        this.moveCount = 0;
        this.MoveCount = Random.Range(MinMoveCount, MaxMoveCount);
        this.graphics.transform.rotation = Quaternion.Euler(0, 90, 0);
    }

    protected override void EachUpdate()
    {
        if(!this.isActive && this.flyCount > 0) return;
        
        this.moveCount++;
        if(this.moveCount >= this.MoveCount * 2){
            this.moveCount = 0;
            this.graphics.transform.rotation = Quaternion.Euler(0, 90, 0);
        }else if(this.moveCount == this.MoveCount){
            this.graphics.transform.rotation = Quaternion.Euler(0, -90, 0);
        }

        float move = 0;
        float moveZ = 0;
        if(this.moveCount < this.MoveCount){
            move = moveDistance * ((float)this.moveCount / (float)this.MoveCount);
            moveZ = -this.moveZ;
        }else{
            move = moveDistance * (1f - ((float)(this.moveCount - this.MoveCount) / (float)this.MoveCount));
            moveZ = this.moveZ;
        }

        var point = this.transform.position;
        point.x = referenceX + move;
        point.z += moveZ;
        this.transform.position = point;
    }
}