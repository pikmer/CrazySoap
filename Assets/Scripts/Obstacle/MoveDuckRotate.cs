using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDuckRotate : Obstacle
{
    float referenceX;

    int moveCount = 0;
    public int MoveCount = 60;

    public float moveDistance;

    // [SerializeField] int MaxMoveCount = 60;
    // [SerializeField] int MinMoveCount = 60;

    public override void Init(Vector3 position){
        base.Init(position);
        this.referenceX = position.x;
        this.moveCount = 0;
        // this.MoveCount = Random.Range(MinMoveCount, MaxMoveCount);
    }

    protected override void EachUpdate()
    {
        if(!this.isActive && this.flyCount > 0) return;
        
        this.moveCount++;
        if(this.moveCount >= this.MoveCount * 2){
            this.moveCount = 0;
        }else if(this.moveCount == this.MoveCount){
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
        
        this.graphics.transform.Rotate(0, 10, 0);
    }

    //ランダム化
    public override void Randomizer(int random){
        this.moveCount = this.MoveCount * (random % 4) / 4;
    }
}