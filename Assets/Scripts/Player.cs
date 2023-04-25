using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public static Player Instance;

    [HideInInspector][System.NonSerialized]
    public bool isDead = true;
	//無敵モード
	bool isInvincible = false;

    Vector3 startPos = new Vector3(0, 0, 0);

    Vector3 moveDirection = Vector3.zero;
    [HideInInspector][System.NonSerialized]
    public Vector3 forwardVelosity = new Vector3(0, 0, 0.5f);
    Vector3 rightVelosity = new Vector3(0.25f, 0, 0);
    int leftInputCount;
    int rightInputCount;

    //ジャンプ
    bool isJump = false;
    float jumpSpeed;
    float JumpSpeed = 0.5f;
    float gravity = 0.004f;

    //左右の壁の距離
    float wallDistance = 10f;

    //横の水流
    float sideStream = 0;

    [HideInInspector][System.NonSerialized]
    public float positionResetRange = 100f;

    //吹っ飛ぶ演出
    public Transform graphicsTrf;
    Vector3 graphicsOffset = new Vector3(0, 0.15f, 0);
    int flyCount = 0;
    int FlyCount = 30;
    Vector3 flyVec;
    Vector3 rotateAxis;

    //移動の演出
    float bodyTilt = 0;
    float BodyTilt = 10f;
    float befourBodyTilt = 0;
    float addBodyTilt = 1f;
    float returnBodyTilt = 2f;
    //ジャンプの演出
    float graphicsEulerAngleZ;

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
    //
    public Counter shieldCounter;
    public Text shieldText;
    public Image shieldImage;
    public Sprite shieldSprite;
    public Sprite watchSprite;
    public int shieldUseCount { get; private set;}
    string shieldUseCountKey = "shield";
    //
    public Text shieldTextSP;
    
    void Awake()
    {
        Instance = this;

        this.wallDistance -= normalColl.size.x / 2f;

        this.shieldText.text = this.shieldUseCount.ToString();

        this.transform.position = this.startPos;

        this.shieldUseCount = PlayerPrefs.GetInt(this.shieldUseCountKey, 3);

		//PC対応
		if(GameManager.Instance.isPC){
            this.shieldTextSP.transform.parent.gameObject.SetActive(false);
		}
		//スマホ対応
        else{
            this.shieldText.transform.parent.gameObject.SetActive(false);
            this.shieldText = this.shieldTextSP;
        }
    }

    void Update(){
        if(Input.GetKeyDown(KeyCode.Space)){
            this.Shield();
        }
        //ジャンプ仮
        // if(Input.GetKeyDown(KeyCode.W)){
        //     this.Jump();
        // }
    }

    void FixedUpdate()
    {
        if(this.isDead){
            if(this.flyCount > 0){
                this.flyCount--;
                this.graphicsTrf.position += this.flyVec;
                this.graphicsTrf.Rotate(this.rotateAxis, 20f, Space.World);
                if(this.flyCount <= 0){
                    this.graphicsTrf.gameObject.SetActive(false);
                }
            }
        }else{
            var transform = this.transform;

            //スマホ操作
            bool isSpLeft = false, isSpRight = false;
            if(!GameManager.Instance.isPC && Input.GetMouseButton(0)){
                if(Input.mousePosition.x <= Screen.width / 2){
                    isSpLeft = true;
                }else{
                    isSpRight = true;
                }
            }

            //移動
            this.moveDirection += this.forwardVelosity;
            if(Input.GetKey(KeyCode.A) || Input.GetKey(KeyCode.LeftArrow) || isSpLeft){
                this.leftInputCount++;
                var value = 1f;
                if(this.leftInputCount <= 3) value = Mathf.Pow(0.5f, 4 - this.leftInputCount);
                this.moveDirection -= this.rightVelosity * value;
                this.bodyTilt -= this.addBodyTilt;
                if(this.bodyTilt < -this.BodyTilt){
                    this.bodyTilt = -this.BodyTilt;
                }
            }else if(this.leftInputCount != 0){
                this.leftInputCount = 0;
            }
            if(Input.GetKey(KeyCode.D) || Input.GetKey(KeyCode.RightArrow) || isSpRight){
                this.rightInputCount++;
                var value = 1f;
                if(this.rightInputCount <= 3) value = Mathf.Pow(0.5f, 4 - this.rightInputCount);
                this.moveDirection += this.rightVelosity * value;
                this.bodyTilt += this.addBodyTilt;
                if(this.bodyTilt > this.BodyTilt){
                    this.bodyTilt = this.BodyTilt;
                }
            }else if(this.rightInputCount != 0){
                this.rightInputCount = 0;
            }
            if(transform.position.z >= this.positionResetRange){
                GameManager.Instance.PositionReset();
            }
            //ジャンプ
            if(this.isJump){
                this.moveDirection += Vector3.up * this.jumpSpeed;
                //アニメーション
                this.graphicsEulerAngleZ += 17f;
                var up = Quaternion.Euler(0, 0, this.graphicsEulerAngleZ) * Vector3.up;
                this.graphicsTrf.rotation = Quaternion.LookRotation(this.forwardVelosity + Vector3.up * this.jumpSpeed, up);
                //
                this.jumpSpeed -= this.gravity;
            }
            //横の流れ
            else{
                this.moveDirection += Vector3.right * this.sideStream;
                //移動のアニメーション
                if(this.bodyTilt != this.befourBodyTilt){
                    this.befourBodyTilt = this.bodyTilt;
                    this.graphicsTrf.rotation = Quaternion.Euler(0, this.bodyTilt, 0);
                }else if(this.bodyTilt != 0){
                    if(this.bodyTilt > 0){
                        this.bodyTilt -= this.returnBodyTilt;
                    }else{
                        this.bodyTilt += this.returnBodyTilt;
                    }
                    if(Mathf.Abs(this.bodyTilt) <= this.returnBodyTilt){
                        this.bodyTilt = 0;
                    }
                    this.graphicsTrf.rotation = Quaternion.Euler(0, this.bodyTilt, 0);
                }
            }
            
            //移動実行
            transform.position += this.moveDirection;

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
                //
                this.graphicsTrf.rotation = Quaternion.identity;
            }

            //接触確認
            var position = transform.position + this.normalColl.center;
            var size = this.normalColl.size;
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
                            this.flyVec = (this.transform.position - obstacle.transform.position - obstacle.center).normalized * 0.3f;
                            obstacle.Fly(-this.flyVec + this.moveDirection);
                            //
                            if(this.isShield){
                                this.isShield = false;
                                this.shieldObj.SetActive(false);
                                this.shieldCount = 0;
                                this.shieldInterval = this.ShieldInterval;
                                this.shieldText.color = new Color(1f, 0.5f, 0.5f);
                                this.shieldText.text = (this.shieldInterval / 60).ToString();
                                this.shieldImage.sprite = this.watchSprite;
                            }else if(!this.isInvincible){
                                GameManager.Instance.PlayerKilled();
                                this.flyCount = this.FlyCount;
                                if(this.flyVec.y < 0)this.flyVec.y = -this.flyVec.y;
                                this.rotateAxis = this.flyVec;
                                this.rotateAxis.y = 0;
                                this.rotateAxis = Quaternion.Euler(0, 90, 0) * this.rotateAxis;
                            }
                            break;
                        }
                    }
                }
            }

            //
            this.moveDirection = Vector3.zero;

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
                Counter.Display((float)this.shieldCount / (float)this.ShieldCount, 3);
                if(this.shieldCount <= 0){
                    this.isShield = false;
                    this.shieldObj.SetActive(false);
                    this.shieldInterval = this.ShieldInterval;
                    this.shieldText.color = new Color(1f, 0.5f, 0.5f);
                    this.shieldText.text = (this.shieldInterval / 60).ToString();
                    this.shieldImage.sprite = this.watchSprite;
                }
            }
            if(this.shieldInterval > 0){
                this.shieldInterval--;
                if(this.shieldInterval % 60 == 0){
                    this.shieldText.text = (this.shieldInterval / 60).ToString();
                }
                if(this.shieldInterval <= 0){
                    this.shieldText.color = Color.white;
                    this.shieldText.text = this.shieldUseCount.ToString();
                    this.shieldImage.sprite = this.shieldSprite;
                }
            }
        }
    }

    public void Jump(){
        this.isJump = true;
        this.jumpSpeed = this.JumpSpeed;
        ObstacleManager.Instance.JumpItemSet(this.forwardVelosity.z, this.JumpSpeed, this.gravity);
    }

    public void Shield(){
        if(this.shieldUseCount > 0 && this.shieldCount <= 0 && this.shieldInterval <= 0){
            this.isShield = true;
            this.shieldCount = this.ShieldCount;
            this.shieldObj.SetActive(true);

            this.ShieldCountSet(-1);
            this.shieldText.text = this.shieldUseCount.ToString();
        }
    }

    public void ChangesideStream(float sideStream){
        this.sideStream = sideStream;
    }

    //ウィングマン
    public void Wingman(bool isWingman){
        this.isWingman = isWingman;
        this.wingmanObj.SetActive(isWingman);
    }

    public void ShieldCountSet(int plus){
        if(this.shieldUseCount + plus < 0) return;
        this.shieldUseCount += plus;
        PlayerPrefs.SetInt(this.shieldUseCountKey, this.shieldUseCount);
    }

    public void PositionReset(){
        this.transform.position -= Vector3.forward * this.positionResetRange;
    }

    public void GameStart(){
        this.isDead = false;
        this.shieldText.text = this.shieldUseCount.ToString();
    }

    public void PlayerKilled(){
        this.isDead = true;
    }
    public void Continue(){
        this.isDead = false;
        this.graphicsTrf.localPosition = this.graphicsOffset;
        this.graphicsTrf.rotation = Quaternion.identity;
        this.graphicsTrf.gameObject.SetActive(true);
        this.flyCount = 0;
        BubbleBombEffect.Instance.Play(this.transform.position);
        //
        this.leftInputCount = 0;
        this.rightInputCount = 0;
        //周りを吹き飛ばす
        var position = this.transform.position;
        var size = new Vector3(15f, 10f, 70f);
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

    public void Retry(){
        this.transform.position = this.startPos;
        this.sideStream = 0;
        weapon.Retry();
        this.isWingman = false;
        this.wingmanObj.SetActive(false);
        //
        this.leftInputCount = 0;
        this.rightInputCount = 0;
        //
        this.isJump = false;
        this.jumpSpeed = 0;
        //
        this.graphicsTrf.localPosition = this.graphicsOffset;
        this.graphicsTrf.rotation = Quaternion.identity;
        this.graphicsTrf.gameObject.SetActive(true);
        this.flyCount = 0;
        //
        this.bodyTilt = 0;
        this.befourBodyTilt = 0;
        //
        this.isShield = false;
        this.shieldCount = 0;
        this.shieldInterval = 0;
        this.shieldObj.SetActive(false);
        this.shieldText.text = this.shieldUseCount.ToString();
        this.shieldText.color = Color.white;
        this.shieldImage.sprite = this.shieldSprite;
    }
}
