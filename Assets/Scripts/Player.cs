using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player Instance;

    bool isDead = true;
	//無敵モード
	bool isInvincible = false;

    Vector3 moveDirection = Vector3.zero;
    [HideInInspector][System.NonSerialized]
    public Vector3 forwardVelosity = new Vector3(0, 0, 0.5f);
    Vector3 rightVelosity = new Vector3(0.25f, 0, 0);

    //ジャンプ
    bool isJump = false;
    float jumpSpeed;
    float JumpSpeed = 0.5f;
    float gravity = 0.004f;

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
    //ウィングマン
    bool isWingman;
    public GameObject wingmanObj;
    public BulletMuzzle[] wingmans;

    //判定
    public BoxCollider normalColl;
    //バリア
    bool isShield = false;
    public GameObject shieldObj;
    int shieldInterval;
    int ShieldInterval = 20 * 60;
    int shieldCount;
    int ShieldCount = 15 * 60;
    public Counter shieldCounter;

    //スコア
    int posResetMil;
    int itemScore;
    int score;
    public Text scoreText;
    
    void Awake()
    {
        Instance = this;

        this.wallDistance -= normalColl.size.x / 2f;
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Space) && this.shieldCount <= 0 && this.shieldInterval <= 0){
            this.isShield = true;
            this.shieldCount = this.ShieldCount;
            this.shieldObj.SetActive(true);
        }
        //ジャンプ仮
        // if(Input.GetKeyDown(KeyCode.W)){
        //     this.Jump();
        // }
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
        //ジャンプ
        if(this.isJump){
            this.moveDirection += Vector3.up * this.jumpSpeed;
            this.jumpSpeed -= this.gravity;
        }
        //横の流れ
        else{
            this.moveDirection += Vector3.right * this.sideStream;
        }
        
        //移動実行
        transform.position += this.moveDirection;
        this.moveDirection = Vector3.zero;

        //左右の移動上限
        if(transform.position.x > this.wallDistance){
            transform.position = new Vector3(this.wallDistance, transform.position.y, transform.position.z);
        }else if(transform.position.x < -this.wallDistance){
            transform.position = new Vector3(-this.wallDistance, transform.position.y, transform.position.z);
        }
        //ジャンプの着地
        if(this.isJump && transform.position.y <= 0){
            this.isJump = false;
            transform.position = new Vector3(transform.position.x, 0, transform.position.z);
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
                        if(this.isShield){
                            obstacle.SetActive(false);
                            this.isShield = false;
                            this.shieldObj.SetActive(false);
                            this.shieldCount = 0;
                            this.shieldInterval = this.ShieldInterval;
                        }else{
                            if(!this.isInvincible) GameManager.Instance.GameOver();
                        }
                        break;
                    }
                }
            }
        }

        //スコア表示更新
        this.scoreText.text = this.GetScore().ToString();

        //攻撃実行
        this.attackInterval++;
        if(!this.isJump && this.attackInterval >= this.AttackInterval){
            this.attackInterval = 0;
            this.weapon.Shot();
            //
            if(this.isWingman){
                foreach (var wingman in wingmans)
                {
                    wingman.Shot();
                }
            }
        }

        //シールド
        if(this.shieldCount > 0){
            this.shieldCount--;
            Counter.Display((float)this.shieldCount / (float)this.ShieldCount);
            if(this.shieldCount <= 0){
                this.isShield = false;
                this.shieldObj.SetActive(false);
                this.shieldInterval = this.ShieldInterval;
            }
        }
        if(this.shieldInterval > 0){
            this.shieldInterval--;
            if(this.shieldInterval <= 0){
            }
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

    public void Jump(){
        this.isJump = true;
        this.jumpSpeed = this.JumpSpeed;
    }

    public void ChangesideStream(){
        if(Random.value < 0.5f){
            this.sideStream = Random.Range(this.minStream, this.maxStream);
        }else{
            this.sideStream = Random.Range(-this.maxStream, -this.minStream);
        }
    }

    //スコア計算
    int GetScore(){
        return (this.posResetMil + (int)transform.position.z) + this.itemScore;
    }
    public void ItemScore(int score){
        this.itemScore += score;
    }

    //ウィングマン
    public void Wingman(bool isWingman){
        this.isWingman = isWingman;
        this.wingmanObj.SetActive(isWingman);
    }

    public void PositionReset(){
        this.transform.position -= Vector3.forward * this.positionResetRange;
        this.posResetMil += (int)this.positionResetRange;
    }

    public void GameStart(){
        this.isDead = false;
        this.scoreText.text = "";
    }

    public void GameOver(){
        this.isDead = true;
    }

    public void Retry(){
        this.transform.position = Vector3.zero;
        this.sideStream = 0;
        weapon.Retry();
        this.isWingman = false;
        this.wingmanObj.SetActive(false);
        //
        this.isJump = false;
        this.jumpSpeed = 0;
        //
        this.isShield = false;
        this.shieldCount = 0;
        this.shieldInterval = 0;
        this.shieldObj.SetActive(false);
        //スコア関係
        this.posResetMil = 0;
        this.itemScore = 0;
        this.score = 0;
    }
}
