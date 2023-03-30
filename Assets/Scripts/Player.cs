using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public static Player Instance;

    bool isDead = true;

    Vector3 moveDirection = Vector3.zero;
    [HideInInspector][System.NonSerialized]
    public Vector3 forwardVelosity = new Vector3(0, 0, 0.5f);
    Vector3 rightVelosity = new Vector3(0.25f, 0, 0);

    //左右の壁の距離
    float wallDistance = 10f;

    //横の探され
    float sideStream = 0;
    float minStream = 0.015f;
    float maxStream = 0.03f;

    [HideInInspector][System.NonSerialized]
    public float positionResetRange = 100f;

    //攻撃
    int attackInterval = 0;
    int AttackInterval = 8;
    public Weapon weapon;

    //判定
    public BoxCollider normalColl;
    
    void Awake()
    {
        Instance = this;

        this.wallDistance -= normalColl.size.x / 2f;
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
        //横の流れ
        this.moveDirection += Vector3.right * this.sideStream;
        //移動実行
        transform.position += this.moveDirection;
        this.moveDirection = Vector3.zero;

        //左右の移動上限
        if(transform.position.x > this.wallDistance){
            transform.position = new Vector3(this.wallDistance, transform.position.y, transform.position.z);
        }else if(transform.position.x < -this.wallDistance){
            transform.position = new Vector3(-this.wallDistance, transform.position.y, transform.position.z);
        }

        //接触確認
        var position = transform.position + this.normalColl.center;
        var size = this.normalColl.size;
        foreach (var obstacleArray in  ObstacleManager.Instance.obstacles)
        {
            foreach (var obstacle in obstacleArray)
            {
                if(obstacle.isActive){
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
                        GameManager.Instance.GameOver();
                        break;
                    }
                }
            }
        }

        //攻撃実行
        this.attackInterval++;
        if(this.attackInterval >= this.AttackInterval){
            this.attackInterval = 0;
            this.weapon.Shot();
        }
    }

    //アップグレード
    public void WeaponUpgrade(){
        //シングルショットを取ってないと、最優先でとる
        if(!this.weapon.isSingleShot){
            this.weapon.SingleShotGet();
            return;
        }else{
            this.weapon.Upgrade();
        }
    }

    public void ChangesideStream(){
        if(Random.value < 0.5f){
            this.sideStream = Random.Range(this.minStream, this.maxStream);
        }else{
            this.sideStream = Random.Range(-this.maxStream, -this.minStream);
        }
    }

    public void PositionReset(){
        this.transform.position -= Vector3.forward * this.positionResetRange;
    }

    public void GameStart(){
        this.isDead = false;
    }

    public void GameOver(){
        this.isDead = true;
    }

    public void Retry(){
        this.transform.position = Vector3.zero;
        this.sideStream = 0;
        weapon.Retry();
    }
}
