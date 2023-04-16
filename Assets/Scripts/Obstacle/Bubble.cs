using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : HealthObstacle
{
    Coin coin;

    public override void Damage(int damage){
        this.HP -= damage;
        if(this.HP <= 0){
            this.SetActive(false);
            if(this.coin != null){
                this.coin.ProtectBreak();
            }
            BubbleEffect.Instance.Play(this.transform.position + this.center);
        }else if(this.isHPbar){
            var size = this.HPbar.localScale;
            this.HPbar.localScale = new Vector3((float)this.HP / (float)this.MaxHP, size.y, size.z);
            this.HPbarBack.localScale = new Vector3(1f - this.HPbar.localScale.x, size.y, size.z);
        }
    }

    public override void Protect(Coin coin){
        this.coin = coin;
    }
    
    public override void Fly(Vector3 flyVec){
        this.Damage(this.MaxHP);
    }
}
