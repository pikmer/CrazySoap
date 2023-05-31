using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    public static Shop Instance;

    [SerializeField] GameObject content;
    
    [SerializeField] Text coinText;

    [SerializeField] RectTransform shieldButton;
    [SerializeField] Text shieldText;
    int shieldPrice = 100;
    int shieldOverCount;
    int ShieldOverCount = 7;

    void Awake(){
        Instance = this;
    }

    void FixedUpdate(){
        if(this.shieldOverCount > 0){
            this.shieldOverCount--;
            if(this.shieldOverCount > this.ShieldOverCount / 2){
                this.shieldButton.localScale = Vector3.one * (1.05f + (float)(this.ShieldOverCount - this.shieldOverCount) * 0.05f);
            }else{
                this.shieldButton.localScale = Vector3.one * (1.05f + (float)this.shieldOverCount * 0.05f);
            }
        }
    }


    public void ShieldButtonEnter(){
        this.shieldButton.localScale = Vector3.one * 1.05f;
    }
    public void ShieldButtonExit(){
        this.shieldButton.localScale = Vector3.one;
    }

    public void ShieldButtonDown(){
        if(CoinParent.Instance.Use(this.shieldPrice)){
            this.shieldOverCount = this.ShieldOverCount;
            Player.Instance.ShieldCountSet(+1);
            this.shieldText.text = Player.Instance.shieldUseCount.ToString();
            this.coinText.text = CoinParent.Instance.money.ToString();
            AudioManager.Instance.PlaySE(4);
        }
    }

    public void SetActive(bool isActive){
        this.content.SetActive(isActive);
        AdsManager.Instance.BannerDisplay(!isActive);
        AudioManager.Instance.PlaySE(4);
        this.shieldText.text = Player.Instance.shieldUseCount.ToString();
        this.coinText.text = CoinParent.Instance.money.ToString();
    }

    public void ShopCoinText(){
        this.coinText.text = CoinParent.Instance.money.ToString();
    }
}
