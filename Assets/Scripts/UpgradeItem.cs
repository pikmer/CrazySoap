using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UpgradeItem : MonoBehaviour
{
    public static UpgradeItem Instance;

    GameObject[] items = new GameObject[10];
    public GameObject prefab;

    Vector3 collCenter = new Vector3(0, 0.25f, 0);
    Vector3 collSize = new Vector3(1.0f, 1.0f, 1.0f);

    void Awake()
    {
        Instance = this;
        
        //アイテム作成
        for(int i=0; i < this.items.Length; i++){
        	GameObject item = Instantiate(this.prefab, Vector3.zero, Quaternion.identity);
			item.transform.SetParent(this.transform);
        	item.SetActive(false);
			this.items[i] = item;
        }
    }

    void FixedUpdate(){
        //接触確認
        var player = Player.Instance;
        var playerPosition = player.transform.position + player.normalColl.center;
        var playerSize = player.normalColl.size;
        foreach (var item in this.items)
        {
            if(item.activeSelf){
                if(GameManager.CheckBoxColl(item.transform.position + this.collCenter, this.collSize
                , playerPosition, playerSize)){
                    //強化実行
                    Player.Instance.WeaponUpgrade();
                    item.SetActive(false);
                    break;
                }
            }
        }
    }

    public void SetItem(Vector3 position)
    {
		foreach (var item in this.items)
		{
            if(!item.activeSelf){
                item.SetActive(true);
                item.transform.position = position;
                break;
            }
        }
    }

    public void PositionReset(){
        var positionResetRange = Player.Instance.positionResetRange;
		foreach (var item in this.items)
		{
            item.transform.position -= Vector3.forward * positionResetRange;
        }
    }

    public void Retry(){
		foreach (var item in this.items)
		{
            if(item.activeSelf){
                item.SetActive(false);
            }
        }
    }
}
