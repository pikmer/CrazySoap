using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SupportItem : MonoBehaviour
{
    public static SupportItem Instance;

    Item[] items = new Item[10];
    public GameObject prefab;

    Vector3 collCenter = new Vector3(0, 0.25f, 0);
    Vector3 collSize = new Vector3(1.0f, 1.0f, 1.0f);

    //磁石
    int magInterval;
    int MagInterval = 20 * 60;
    //2倍
    int doubleInterval;
    int DoubleInterval = 20 * 60;
    //ウィングマン
    int wingmanInterval;
    int WingmanInterval = 15 * 60;

    //見た目
    public Mesh[] meshes;
    public Material[] materials;

    void Awake()
    {
        Instance = this;
        
        //アイテム作成
        for(int i=0; i < this.items.Length; i++){
        	GameObject item = Instantiate(this.prefab, Vector3.zero, Quaternion.identity);
			item.transform.SetParent(this.transform);
        	item.SetActive(false);
			this.items[i] = new Item(item);
        }
    }

    void FixedUpdate(){
        var playerDead = Player.Instance.isDead;

        //インターバル
        if(this.magInterval > 0){
            if(!playerDead){
                this.magInterval--;
                if(this.magInterval <= 0){
                    CoinParent.Instance.Magnet(false);
                }
            }
            Counter.Display((float)this.magInterval / (float)this.MagInterval, 0);
        }
        if(this.doubleInterval > 0){
            if(!playerDead){
                this.doubleInterval--;
                if(this.doubleInterval <= 0){
                    CoinParent.Instance.DoubleGet(false);
                }
            }
            Counter.Display((float)this.doubleInterval / (float)this.DoubleInterval, 1);
        }
        if(this.wingmanInterval > 0){
            if(!playerDead){
                this.wingmanInterval--;
                if(this.wingmanInterval <= 0){
                    Player.Instance.Wingman(false);
                }
            }
            Counter.Display((float)this.wingmanInterval / (float)this.WingmanInterval, 2);
        }
        
        if(playerDead) return;

        //接触確認
        var player = Player.Instance;
        var playerPosition = player.transform.position + player.normalColl.center;
        var playerSize = player.normalColl.size;
        foreach (var item in this.items)
        {
            if(item.isActive){
                if(GameManager.CheckBoxColl(item.trf.position + this.collCenter, this.collSize
                , playerPosition, playerSize)){
                    //実行
                    this.Support(item.number);
                    item.obj.SetActive(false);
                    item.isActive = false;
                    break;
                }
            }
        }
    }

    //サポート実行
    void Support(int number){
        switch (number)
        {
            case 0:
                this.magInterval = this.MagInterval;
                CoinParent.Instance.Magnet(true);
                break;
            case 1:
                this.doubleInterval = this.DoubleInterval;
                CoinParent.Instance.DoubleGet(true);
                break;
            case 2:
                this.wingmanInterval = this.WingmanInterval;
                Player.Instance.Wingman(true);
                break;
            case 3:
                Player.Instance.Jump();
                break;
        }
    }

    public void SetItemRandom(Vector3 position)
    {
        this.SetItem(position, Random.Range(0, this.meshes.Length));
    }

    public void SetItem(Vector3 position, int number)
    {
		foreach (var item in this.items)
		{
            if(!item.isActive){
                item.obj.SetActive(true);
                item.trf.position = position;
                item.isActive = true;
                item.number = number;
                item.filter.mesh = this.meshes[number];
                item.renderer.material = this.materials[number];
                break;
            }
        }
    }

    public void PositionReset(){
        var positionResetRange = Player.Instance.positionResetRange;
		foreach (var item in this.items)
		{
            if(item.isActive){
                item.trf.position -= Vector3.forward * positionResetRange;
                if(item.trf.position.z <= -10f){
                    item.obj.SetActive(false);
                    item.isActive = false;
                }
            }
        }
    }

    public void Retry(){
		foreach (var item in this.items)
		{
            if(item.isActive){
                item.obj.SetActive(false);
                item.isActive = false;
            }
        }
        this.magInterval = 0;
        this.doubleInterval = 0;
        this.wingmanInterval = 0;
    }

    class Item{
        public bool isActive;
        public GameObject obj;
        public Transform trf;
        public int number = 0;
        public MeshFilter filter;
        public MeshRenderer renderer;

        public Item(GameObject obj){
            this.obj = obj;
            this.trf = obj.transform;
            this.filter = this.trf.GetChild(0).GetComponent<MeshFilter>();
            this.renderer = this.trf.GetChild(0).GetComponent<MeshRenderer>();
        }
    }
}
