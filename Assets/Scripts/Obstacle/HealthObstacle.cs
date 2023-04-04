using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HealthObstacle : Obstacle
{
    public int MaxHP = 10;
    protected int HP;

    public bool isHPbar = true;
    public GameObject HPbarObj;
    Transform HPbarTrf;
    public Transform HPbar;
    public Transform HPbarBack;

    void Start(){
        this.HPbarTrf = this.HPbarObj.transform;
        if(!this.isHPbar){
            this.HPbarObj.SetActive(false);
        }
    }

    public override void Init(Vector3 position){
        base.Init(position);
        if(this.isHPbar){
            this.HP = this.MaxHP;
            var size = this.HPbar.localScale;
            this.HPbar.localScale = new Vector3(1f, size.y, size.z);
            this.HPbarBack.localScale = new Vector3(0, size.y, size.z);
        }
    }

    public override void SetActive(bool active){
        base.SetActive(active);
        if(this.isHPbar){
            this.HPbarObj.SetActive(active);
        }
    }
}
