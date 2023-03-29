using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : Obstacle
{
    int MaxHP = 10;
    int HP;

    public override void Init(Vector3 position){
        base.Init(position);
        this.HP = this.MaxHP;
    }

    public override void Damage(int damage){
        this.HP -= damage;
        if(this.HP <= 0){
            this.SetActive(false);
        }
    }
}
