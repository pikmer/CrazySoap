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

    [HideInInspector][System.NonSerialized]
    public float positionResetRange = 100f;

    //攻撃
    int attackInterval = 0;
    int AttackInterval = 8;
    public Weapon[] weapons;

    //判定
    public BoxCollider normalColl;

    //アップグレード
    public List<ItemProbability> itemProbs = new List<ItemProbability>();
    
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
            foreach (var weapon in this.weapons)
            {
                weapon.Shot();
            }
        }
    }

    //アップグレード
    public void WeaponUpgrade(){
        this.itemProbs.Clear();
        float probSum = 0;
        //選べる武器と合計確立を出す
        for (int i = 0; i < this.weapons.Length; i++)
        {
            var weapon = this.weapons[i];
            //武器上限なしまたは、上限レベルが下の場合に追加
            if(weapon.level < weapon.maxLevel){
                this.itemProbs.Add(new ItemProbability(i, weapon.baseProb));
                probSum += weapon.baseProb;
            }
        }
        //抽選
        float selectProbability = UnityEngine.Random.Range(0, probSum);
        float probAdd = 0;
        for (int j = 0; j < this.itemProbs.Count; j++)
        {
            var itemProb = this.itemProbs[j];
            probAdd += itemProb.prob;
            if(probAdd >= selectProbability){
                this.weapons[itemProb.weaponIndex].Upgrade();
                break;
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

    public void GameStart(){
        this.isDead = false;
    }

    public void GameOver(){
        this.isDead = true;
    }

    public void Retry(){
        this.transform.position = Vector3.zero;
        foreach (var weapon in this.weapons)
        {
            weapon.Retry();
        }
    }

    //強化アイテムの確立用
    public struct ItemProbability{
        public int weaponIndex;
        public float prob;

        public ItemProbability(int weaponIndex, float prob) {
            this.weaponIndex = weaponIndex;
            this.prob = prob;
        }
    }
}
