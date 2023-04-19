using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CoinParent : MonoBehaviour
{
    public static CoinParent Instance;
    
    [HideInInspector][System.NonSerialized]
    public int money;
    public Text moneyText;
    public Image icon;

    //保存用のキー
    string moneyKey = "money";

    Coin[] coins = new Coin[100];
    public GameObject prefab;

    Vector3 collCenter = new Vector3(0, 0.25f, 0);
    Vector3 collSize = new Vector3(1.0f, 1.0f, 1.0f);

    //全体マグネット
    bool isMagnet = false;
    float magDistance = 20f;
    //2倍効果
    bool isDoubleGet = false;

    int NotEnoughCount = 60;
    int notEnoughCount = 0;
    public Color normalColor;
    public Color notEnoughColor;

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

        this.money = PlayerPrefs.GetInt(this.moneyKey, 0);
        this.moneyText.text = this.money.ToString();
    }

    void FixedUpdate(){

        if(this.notEnoughCount > 0){
            this.notEnoughCount--;

            float t = (float)(this.notEnoughCount % 10) / 10f;
            if(this.notEnoughCount % 20 == 10){
                this.moneyText.color = this.notEnoughColor;
                this.icon.color = this.notEnoughColor;
            }else if(this.notEnoughCount % 20 == 0){
                this.moneyText.color = this.normalColor;
                this.icon.color = Color.white;
            }
        }
        
        if(Player.Instance.isDead) return;

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
                coin.trf.Rotate(0, 3f, 0);
            }
        }
    }
    void CoinGet(){
        //倍率の係数
        var coef = 1;
        if(this.isDoubleGet) coef *= 2;
        //コインゲット
        this.money += 1 * coef;
        this.moneyText.text = this.money.ToString();
        //スコア
        Player.Instance.ItemScore(10 * coef, 0);
    }

    public void SetCoin(Vector3 position)
    {
		foreach (var coin in this.coins)
		{
            if(!coin.isActive){
                coin.isActive = true;
                coin.obj.SetActive(true);
                coin.blur.SetActive(true);
                coin.trf.position = position;
                coin.trf.rotation = Quaternion.identity;
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

    public bool IsUse(int useValue){
        if(this.money - useValue >= 0){
            return true;
        }else{
            this.notEnoughCount = this.NotEnoughCount;
            return false;
        }
    }

    public bool Use(int useValue){
        if(this.money - useValue >= 0){
            this.money -= useValue;
            PlayerPrefs.SetInt(this.moneyKey, this.money);
            this.moneyText.text = this.money.ToString();
            return true;
        }else{
            this.notEnoughCount = this.NotEnoughCount;
            return false;
        }
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

    public void PlayerKilled(){
        PlayerPrefs.SetInt(this.moneyKey, this.money);
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
}

public class Coin{
    public bool isActive;
    public GameObject obj;
    public Transform trf;
    public GameObject blur;
    public float speed;
    public bool isMagnet;

    public Coin(GameObject obj){
        this.obj = obj;
        this.trf = obj.transform;
        this.blur = this.trf.GetChild(1).gameObject;
    }
}