using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CoinParent : MonoBehaviour
{
    public static CoinParent Instance;

    GameObject[] coins = new GameObject[10];
    public GameObject prefab;

    Vector3 collCenter = new Vector3(0, 0.25f, 0);
    Vector3 collSize = new Vector3(1.0f, 1.0f, 1.0f);

    void Awake()
    {
        Instance = this;
        
        //アイテム作成
        for(int i=0; i < this.coins.Length; i++){
        	GameObject coin = Instantiate(this.prefab, Vector3.zero, Quaternion.identity);
			coin.transform.SetParent(this.transform);
        	coin.SetActive(false);
			this.coins[i] = coin;
        }
    }

    void FixedUpdate(){
        //接触確認
        var player = Player.Instance;
        var playerPosition = player.transform.position + player.normalColl.center;
        var playerSize = player.normalColl.size;
        foreach (var coin in this.coins)
        {
            if(coin.activeSelf){
                if(GameManager.CheckBoxColl(coin.transform.position + this.collCenter, this.collSize
                , playerPosition, playerSize)){
                    //コインゲット
                    coin.SetActive(false);
                    break;
                }
            }
        }
    }

    public void SetCoin(Vector3 position)
    {
		foreach (var coin in this.coins)
		{
            if(!coin.activeSelf){
                coin.SetActive(true);
                coin.transform.position = position;
                break;
            }
        }
    }

    public void PositionReset(){
        var positionResetRange = Player.Instance.positionResetRange;
		foreach (var coin in this.coins)
		{
            coin.transform.position -= Vector3.forward * positionResetRange;
        }
    }

    public void Retry(){
		foreach (var coin in this.coins)
		{
            if(coin.activeSelf){
                coin.SetActive(false);
            }
        }
    }
}
