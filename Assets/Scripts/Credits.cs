using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Credits : MonoBehaviour
{
    [SerializeField]
    GameObject creditsUI;

    public void SetActive(bool active)
    {
        this.creditsUI.SetActive(active);
        AdsManager.Instance.BannerDisplay(!active);
        AudioManager.Instance.PlaySE(4);
    }

    //ぴくまーリンク
    public void PikmerLink(){
        Application.OpenURL("https://pikmer.com/");
    }
}
