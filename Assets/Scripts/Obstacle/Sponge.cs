using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sponge : HealthObstacle
{
    Vector3 attackSize = new Vector3(20, 20, 30);
    Vector3 attackCenter = new Vector3(0, 0, 15);

    int scaleCount = 0;

    public override void Init(Vector3 position){
        base.Init(position);
        this.scaleCount = 0;
        this.graphics.transform.localScale = Vector3.one;
    }

    protected override void EachUpdate(){
        if(this.scaleCount > 0){
            this.scaleCount--;
            this.graphics.transform.localScale = Vector3.one * (1f + (float)this.scaleCount / 5f);
        }
    }

    public override void Damage(int damage){
        if(this.HP <= 0) return;

        this.HP -= damage;
        if(this.HP <= 0){
            this.HP = 0;
            //バブルアタックで障害物を吹き飛ばす
            this.AttackCheck();
            this.scaleCount = 10;
            BubbleBombEffect.Instance.Play(this.transform.position);
        }else{
            this.scaleCount = 3;
        }

        if(this.isHPbar){
            var size = this.HPbar.localScale;
            this.HPbar.localScale = new Vector3((float)this.HP / (float)this.MaxHP, size.y, size.z);
            this.HPbarBack.localScale = new Vector3(1f - this.HPbar.localScale.x, size.y, size.z);
        }
    }

    void AttackCheck(){
        var position = transform.position + this.attackCenter;
        var size = this.attackSize;
        foreach (var obstacleArray in  ObstacleManager.Instance.obstacles)
        {
            foreach (var obstacle in obstacleArray)
            {
                if(obstacle.isActive && obstacle.flyCount <= 0){
                    var isHit = false;
                    foreach (var coll in obstacle.colliders)
                    {
                        if(GameManager.CheckBoxColl(position, size
                        , obstacle.transform.position + coll.center, coll.size)){
                            isHit = true;
                            break;
                        }
                    }
                    if(isHit){
                        var flyVec = obstacle.transform.position + obstacle.center - this.transform.position;
                        obstacle.Fly(flyVec.normalized * 0.7f + Vector3.up * 0.2f);
                    }
                }
            }
        }
    }
}