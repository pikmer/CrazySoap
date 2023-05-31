using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using CrazyGames;

public class AdsManager : MonoBehaviour
{
    public static AdsManager Instance;

    public GameObject adsButtonObj;
    public GameObject rewardGetObj;
    public Text rewardGetText;
    int adsCoin = 200;

    public CrazyBanner banner;

    bool isFirstPlay = true;

    void Awake()
    {
        Instance = this;

        this.BannerDisplay(true);

        this.rewardGetText.text = "+" + adsCoin;
    }

    public void GameStart(){
        this.BannerDisplay(false);
    }

    public void Retry(){
        if(!this.isFirstPlay && UnityEngine.Random.value < 0.5f) CrazyAds.Instance.beginAdBreak();
        this.isFirstPlay = false;
        
        this.BannerDisplay(true);
        
        this.adsButtonObj.SetActive(true);
    }

    //バナー広告
    public void BannerDisplay(bool isActive){
        this.banner.MarkVisible(isActive);
        CrazyAds.Instance.updateBannersDisplay();
    }

    //リワード広告を見る処理
    public void ReadAds(){
        CrazyAds.Instance.beginAdBreakRewarded(AdsSuccessCallback, AdsErrorCallback);
    }
    //リワード広告成功
    void AdsSuccessCallback(){
        this.adsButtonObj.SetActive(false);
        this.rewardGetObj.SetActive(true);
        CoinParent.Instance.AdsCoinGet(this.adsCoin);
        Shop.Instance.ShopCoinText();
    }
    //リワード広告失敗
    void AdsErrorCallback(){
        Debug.Log("Ads error");
    }

    //リワード報告閉じる
    public void RewardWindowClose(){
        this.rewardGetObj.SetActive(false);
        AudioManager.Instance.PlaySE(4);
    }
}
