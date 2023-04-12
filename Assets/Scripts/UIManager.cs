using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UIManager : MonoBehaviour
{
    public static UIManager Instance;

    public GameObject titleUI;
    public GameObject continueUI;
    public GameObject gameoverUI;

    void Awake()
    {
        Instance = this;
        
        this.Retry();
    }

    public void GameStart(){
        this.titleUI.SetActive(false);
    }

    public void ContinueCheck(){
        this.continueUI.SetActive(true);
    }

    public void ContinueDelaySet(){
        this.continueUI.SetActive(false);
    }

    public void GameOver(){
        this.gameoverUI.SetActive(true);
        this.continueUI.SetActive(false);
    }

    public void Retry(){
        this.titleUI.SetActive(true);
        this.gameoverUI.SetActive(false);
    }
}
