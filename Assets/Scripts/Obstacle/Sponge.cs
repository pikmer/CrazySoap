using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sponge : HealthObstacle
{
    Vector3 attackSize = new Vector3(20, 20, 30);
    Vector3 attackCenter = new Vector3(0, 0, 15);

    public override void Damage(int damage){
        if(this.HP <= 0) return;

        this.HP -= damage;
        if(this.HP <= 0){
            this.HP = 0;
            //バブルアタックで障害物を吹き飛ばす
            this.AttackCheck();
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
                        obstacle.Fly(flyVec.normalized * 1f);
                    }
                }
            }
        }
    }
}