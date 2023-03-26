using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    bool isDead = true;

    Vector3 moveDirection = Vector3.zero;
    [HideInInspector][System.NonSerialized]
    public Vector3 forwardVelosity = new Vector3(0, 0, 0.3f);
    Vector3 rightVelosity = new Vector3(0.15f, 0, 0);

    [HideInInspector][System.NonSerialized]
    public float positionResetRange = 70f;

    //攻撃
    int attackInterval = 0;
    int AttackInterval = 8;
    public Weapon[] weapons;


    //判定
    public BoxCollider normalColl;
    
    void Awake()
    {
        Instance = this;
    }

    void FixedUpdate()
    {
        if(this.isDead) return;

        var transform = this.transform;

        //移動
        this.moveDirection += this.forwardVelosity;
        if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow)){
            this.moveDirection -= this.rightVelosity;
        }
        if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow)){
            this.moveDirection += this.rightVelosity;
        }
        if(transform.position.z >= this.positionResetRange){
            GameManager.Instance.PositionReset();
        }
        //移動実行
        transform.position += this.moveDirection;
        this.moveDirection = Vector3.zero;

        //接触確認
        foreach (var obstacle in ObstacleManager.Instance.obstacles)
        {
            var isHit = false;
            foreach (var coll in obstacle.colliders)
            {
                if(this.CheckBoxColl(obstacle.transform.position, coll)){
                    isHit = true;
                    break;
                }
            }
            if(isHit){
                GameManager.Instance.GameOver();
                break;
            }
        }

        //攻撃実行
        this.attackInterval++;
        if(this.attackInterval >= this.AttackInterval){
            this.attackInterval = 0;
            foreach (var weapon in this.weapons)
            {
                weapon.Shot();
            }
        }
    }

    public void PositionReset(){
        this.transform.position -= Vector3.forward * this.positionResetRange;
        foreach (var weapon in this.weapons)
        {
            weapon.PositionReset();
        }
    }

    //回転してないボックス同士の判定
    bool CheckBoxColl(Vector3 position, BoxCollider coll){
        var hitRange = (this.normalColl.size + coll.size) / 2f;
        var range = position + coll.center - this.transform.position - this.normalColl.center;
        if(Mathf.Abs(range.x) <= hitRange.x 
        && Mathf.Abs(range.y) <= hitRange.y
        && Mathf.Abs(range.z) <= hitRange.z){
            return true;
        }else{
            return false;
        }
    }

    public void GameStart(){
        this.isDead = false;
    }

    public void GameOver(){
        this.isDead = true;
    }

    public void Retry(){
        this.transform.position = Vector3.zero;
    }
}
