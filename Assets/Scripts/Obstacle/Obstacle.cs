using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public bool isActive;

    public GameObject graphics;

    public BoxCollider[] colliders;

    [HideInInspector][System.NonSerialized]
    public int flyCount = 0;
    int FlyCount = 30;
    Vector3 flyVec;
    Vector3 rotateAxis;
    public Vector3 center;

    public virtual void Init(Vector3 position){
        this.transform.position = position;
        this.graphics.transform.rotation = Quaternion.identity;
        this.SetActive(true);
    }

    void FixedUpdate()
    {
        if(!this.isActive) return;

        if(this.flyCount > 0){
            this.flyCount--;
            this.transform.position += this.flyVec;
            this.graphics.transform.Rotate(this.rotateAxis, 20f, Space.World);
            if(this.flyCount <= 0){
                this.SetActive(false);
            }
        }else{
            this.EachUpdate();
        }
    }

    protected virtual void EachUpdate(){}

    public virtual void Damage(int damage){}

    public virtual void SetActive(bool active){
        this.isActive = active;
        this.graphics.SetActive(active);
        this.flyCount = 0;
    }

    public virtual void Protect(Coin coin){}

    //プレイヤーと衝突
    public virtual void Fly(Vector3 flyVec){
        this.flyCount = this.FlyCount;
        this.flyVec = flyVec;
        if(this.flyVec.y < 0)this.flyVec.y = 0;
        this.rotateAxis = this.flyVec;
        this.rotateAxis.y = 0;
        this.rotateAxis = Quaternion.Euler(0, 90, 0) * this.rotateAxis;
    }
}
