using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinParent : MonoBehaviour
{
    public static CoinParent Instance;

    Coin[] coins = new Coin[50];
    public GameObject prefab;

    Vector3 collCenter = new Vector3(0, 0.25f, 0);
    Vector3 collSize = new Vector3(1.0f, 1.0f, 1.0f);

    //全体マグネット
    bool isMagnet = false;
    float magDistance = 20f;
    //2倍効果
    bool isDoubleGet = false;

    void Awake()
    {
        Instance = this;
        
        //アイテム作成
        for(int i=0; i < this.coins.Length; i++){
        	GameObject coin = Instantiate(this.prefab, Vector3.zero, Quaternion.identity);
			coin.transform.SetParent(this.transform);
        	coin.SetActive(false);
			this.coins[i] = new Coin(coin);;
        }
    }

    void FixedUpdate(){
        //接触確認
        var player = Player.Instance;
        var playerPosition = player.transform.position + player.normalColl.center;
        var playerSize = player.normalColl.size;
        foreach (var coin in this.coins)
        {
            if(coin.isActive){
                //マグネット吸引
                if(this.isMagnet && !coin.isMagnet){
                    float distanceSqr = (playerPosition - coin.trf.position).sqrMagnitude;
                    if(distanceSqr <= this.magDistance * this.magDistance){
                        coin.isMagnet = true;
                        coin.speed = 0.7f;
                    }
                }
                if(coin.isMagnet){
                    var direction = playerPosition - coin.trf.position;
                    var magMove = direction.normalized * coin.speed;
                    coin.trf.position += magMove;
                    coin.speed += 0.02f;
                    //マグネット判定
                    if(direction.sqrMagnitude <= coin.speed * coin.speed){
                        //コインゲット
                        this.CoinGet();
                        coin.obj.SetActive(false);
                        coin.isActive = false;
                        break;
                    }
                }
                //判定
                if(GameManager.CheckBoxColl(coin.trf.position + this.collCenter, this.collSize
                , playerPosition, playerSize)){
                    //コインゲット
                    this.CoinGet();
                    coin.obj.SetActive(false);
                    coin.isActive = false;
                    break;
                }
            }
        }
    }
    void CoinGet(){
        //倍率の係数
        var coef = 1;
        if(this.isDoubleGet) coef *= 2;
        //コインゲット
        Player.Instance.ItemScore(10 * coef);
    }

    public void SetCoin(Vector3 position)
    {
		foreach (var coin in this.coins)
		{
            if(!coin.isActive){
                coin.isActive = true;
                coin.obj.SetActive(true);
                coin.trf.position = position;
                coin.isMagnet = false;
                break;
            }
        }
    }

    public void Magnet(bool isMagnet){
        this.isMagnet = isMagnet;
    }

    public void DoubleGet(bool isDoubleGet){
        this.isDoubleGet = isDoubleGet;
    }

    public void PositionReset(){
        var positionResetRange = Player.Instance.positionResetRange;
		foreach (var coin in this.coins)
		{
            if(coin.isActive){
                coin.trf.position -= Vector3.forward * positionResetRange;
                if(coin.trf.position.z <= -10f){
                    coin.obj.SetActive(false);
                    coin.isActive = false;
                }
            }
        }
    }

    public void Retry(){
		foreach (var coin in this.coins)
		{
            if(coin.isActive){
                coin.obj.SetActive(false);
                coin.isActive = false;
            }
        }
        this.isMagnet = false;
        this.isDoubleGet = false;
    }

    class Coin{
        public bool isActive;
        public GameObject obj;
        public Transform trf;
        public float speed;
        public bool isMagnet;

        public Coin(GameObject obj){
            this.obj = obj;
            this.trf = obj.transform;
        }
    }
}
