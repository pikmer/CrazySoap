using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Shop : MonoBehaviour
{
    [SerializeField] GameObject content;
    
    [SerializeField] Text coinText;

    [SerializeField] RectTransform shieldButton;
    [SerializeField] Text shieldText;
    int shieldPrice = 100;
    int shieldOverCount;
    int ShieldOverCount = 7;


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
        }
    }

    public void SetActive(bool isActive){
        this.content.SetActive(isActive);
        this.shieldText.text = Player.Instance.shieldUseCount.ToString();
        this.coinText.text = CoinParent.Instance.money.ToString();
    }
}
