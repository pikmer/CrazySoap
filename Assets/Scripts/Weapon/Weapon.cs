using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Weapon : MonoBehaviour
{
    static public Weapon Instance;

    WeaponInfo[] weaponInfos = new WeaponInfo[]{
        new WeaponInfo(8, 10f),
        new WeaponInfo(8, 10f),
        new WeaponInfo(8, 10f),
    };

    public bool isSingleShot = false;
    bool[] isShot;

    public BulletMuzzle singleShot;
    public BulletMuzzle[] wideShot;
    public BulletMuzzle wideFrontShot;
    public BulletMuzzle[] twinWideShot;
    public BulletMuzzle[] frontShot;
    public BulletMuzzle[] twinFrontShot;
    public BulletMuzzle[] twinShot;

    public RectTransform getUI;
    public Image getSprite;
    public Text getText;
    public Sprite[] getSprites;
    public string[] getTexts;
    int displayCount;
    int DisplayCount = 150;

    //アップグレード
    public List<ItemProbability> itemProbs = new List<ItemProbability>();

    void Awake(){
        this.isShot = new bool[this.weaponInfos.Length];

        Instance = this;
    }

    void FixedUpdate(){
        if(this.displayCount > 0){
            this.displayCount--;
            if(this.displayCount >= this.DisplayCount - 10){
                var pos = this.getUI.anchoredPosition3D;
                pos.x = -220f + 23f * (float)(this.DisplayCount - this.displayCount);
                this.getUI.anchoredPosition3D = pos;
            }else if(this.displayCount < 10){
                var pos = this.getUI.anchoredPosition3D;
                pos.x = -220f + 23f * (float)this.displayCount;
                this.getUI.anchoredPosition3D = pos;
            }
        }
    }

    public void Shot(){
        for (int i = 0; i < this.isShot.Length; i++)
        {
            this.isShot[i] = false;
        }
        //各武器打てるか確認
        for (int i = 0; i < this.weaponInfos.Length; i++)
        {
            var weaponInfo = this.weaponInfos[i];

            weaponInfo.interval += weaponInfo.level;
            if(weaponInfo.interval >= weaponInfo.Interval){
                weaponInfo.interval -= weaponInfo.Interval;
                this.isShot[i] = true;
            }
        }

        //状況に応じて発射
        //正面
        if(this.isShot[2] && this.isShot[1]){
            //クワッドショット
            this.MuzzleShot(this.twinFrontShot);
        }else if(this.isShot[2]){
            //ツインショット
            this.MuzzleShot(this.twinShot);
        }else if(this.isShot[1]){
            //フロントショット
            this.MuzzleShot(this.frontShot);
            this.singleShot.Shot();
        }else if(this.isShot[0]){
            //ワイドショット
            this.wideFrontShot.Shot();
        }else if(this.isSingleShot){
            //シングル
            this.singleShot.Shot();
        }
        //ワイド
        if(this.isShot[2] && this.isShot[0]){
            //クワッドワイドショット
            this.MuzzleShot(this.twinWideShot);
        }else if(this.isShot[0]){
            //ワイドショット
            this.MuzzleShot(this.wideShot);
        }
        
        // SE
        if(this.isSingleShot){
            AudioManager.Instance.PlaySE(0);
        }
    }

    void MuzzleShot(BulletMuzzle[] muzzles){
        foreach (var muzzle in muzzles)
        {
            muzzle.Shot();
        }
    }

    public void SingleShotGet(){
        this.isSingleShot = true;
        //UI
        this.getSprite.sprite = this.getSprites[0];
        this.getText.text = this.getTexts[0];
        this.displayCount = this.DisplayCount;
        
        // for (int i = 0; i < this.isShot.Length; i++)
        // {
        //     this.weaponInfos[i].level = this.weaponInfos[i].Interval;
        // }
    }

    public void Upgrade(){
        this.itemProbs.Clear();
        float probSum = 0;
        //選べる武器と合計確立を出す
        for (int i = 0; i < this.weaponInfos.Length; i++)
        {
            var weapon = this.weaponInfos[i];
            //武器上限なしまたは、上限レベルが下の場合に追加
            if(weapon.level < weapon.Interval){
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
                var select = this.weaponInfos[itemProb.weaponIndex];
                select.level++;
                select.interval = select.Interval - select.level;
                //UI
                this.getSprite.sprite = this.getSprites[itemProb.weaponIndex + 1];
                this.getText.text = this.getTexts[itemProb.weaponIndex + 1];
                this.displayCount = this.DisplayCount;
                break;
            }
        }
    }

    public bool CheckMaxLevel(){
        if(!this.isSingleShot){
            return false;
        }
        foreach (var weaponInfo in this.weaponInfos)
        {
            if(weaponInfo.level < weaponInfo.Interval){
                return false;
            }
        }
        return true;
    }

    public void Retry(){
        this.isSingleShot = false;
        
        for (int i = 0; i < this.isShot.Length; i++)
        {
            this.weaponInfos[i].level = 0;
        }
        
        var pos = this.getUI.anchoredPosition3D;
        pos.x = -220;
        this.getUI.anchoredPosition3D = pos;
        this.displayCount = 0;
    }

    class WeaponInfo{
        public int level;
        public int interval = 0;
        public int Interval = 8;
        //出る確率
        public float baseProb = 10f;

        public WeaponInfo(int Interval, float baseProb){
            this.Interval = Interval;
            this.baseProb = baseProb;
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
