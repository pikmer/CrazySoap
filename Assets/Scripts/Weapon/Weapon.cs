using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Weapon : MonoBehaviour
{
    // [HideInInspector][System.NonSerialized]
    public int level;
    public int maxLevel = 1;
    int interval = 0;
    public int Interval = 10;

    public BulletMuzzle[] muzzles;

    //出る確率
    public float baseProb = 10f;

    void start(){
        if(this.maxLevel > this.Interval){
            this.maxLevel = this.Interval;
        }
    }

    public void Shot(){

        this.interval += this.level;
        
        if(this.interval >= this.Interval){
            this.interval -= this.Interval;
            foreach (var muzzle in this.muzzles)
            {
                muzzle.Shot();
            }
        }
    }

    public void Upgrade(){
        this.level++;
        if(this.level > this.maxLevel){
            this.level = this.maxLevel;
        }

        this.interval = this.Interval - this.level;
    }

    public void PositionReset(){
        foreach (var muzzle in this.muzzles)
        {
            muzzle.PositionReset();
        }
    }

    public void Retry(){
        this.level = 0;
    }
}
