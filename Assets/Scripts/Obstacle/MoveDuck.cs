using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveDuck : MoveObstacle
{
    [SerializeField] int MaxMoveCount = 60;
    [SerializeField] int MinMoveCount = 60;

    public override void Init(Vector3 position){
        base.Init(position);
        this.MoveCount = Random.Range(MinMoveCount, MaxMoveCount);
    }
}
