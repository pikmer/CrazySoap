using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bubble : Obstacle
{
    public int MaxHP = 10;
    int HP;

    public GameObject HPbarObj;
    Transform HPbarTrf;
    public Transform HPbar;
    public Transform HPbarBack;

    void Start(){
        this.HPbarTrf = this.HPbarObj.transform;
    }

    public override void Init(Vector3 position){
        base.Init(position);
        this.HP = this.MaxHP;
        var size = this.HPbar.localScale;
        this.HPbar.localScale = new Vector3(1f, size.y, size.z);
        this.HPbarBack.localScale = new Vector3(0, size.y, size.z);
    }

    // void Update(){
    //     this.HPbarTrf.rotation = Quaternion.FromToRotation(Vector3.back, Camera.main.transform.position - this.HPbarTrf.position);
    // }

    public override void Damage(int damage){
        this.HP -= damage;
        if(this.HP <= 0){
            this.SetActive(false);
        }else{
            var size = this.HPbar.localScale;
            this.HPbar.localScale = new Vector3((float)this.HP / (float)this.MaxHP, size.y, size.z);
            this.HPbarBack.localScale = new Vector3(1f - this.HPbar.localScale.x, size.y, size.z);
        }
    }

    public override void SetActive(bool active){
        base.SetActive(active);
        this.HPbarObj.SetActive(active);
    }
}
