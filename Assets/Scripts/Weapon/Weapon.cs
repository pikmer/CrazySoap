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

    public void Shot(){

        this.interval++;
        
        if(this.interval <= this.level){
            foreach (var muzzle in this.muzzles)
            {
                muzzle.Shot();
            }
        }

        if(this.interval >= this.Interval){
            this.interval = 0;
        }
    }

    public void PositionReset(){
        foreach (var muzzle in this.muzzles)
        {
            muzzle.PositionReset();
        }
    }
}
