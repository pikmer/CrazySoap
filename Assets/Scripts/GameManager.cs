using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Runtime.InteropServices;
using CrazyGames;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [HideInInspector][System.NonSerialized]
    public bool isGame = false;

    bool isContinue = false;
    
    //ゲームオーバー
    int GameOverDelay = 60;
    int gameOverDelay = 0;
    
    //コンティニュー
    int continueDelay = 0;
    int ContinueDelay = 40 * 3;
    public Text continueDelayText;
    int continueMoney = 100;

    //プラットフォーム
    // [HideInInspector][System.NonSerialized]
    public bool isPC = true;
    #if (UNITY_WEBGL && !UNITY_EDITOR)
        [DllImport("__Internal")]
        private static extern bool CheckPlatform();
    #endif

    void Awake(){
        Instance = this;
        
        #if (UNITY_WEBGL && !UNITY_EDITOR)
            this.isPC = CheckPlatform();
        #endif
    }

    void Start()
    {
        // Application.targetFrameRate = 30;
    }

    void FixedUpdate()
    {
        //ゲームオーバーのディレイ
        if(this.gameOverDelay > 0){
            this.gameOverDelay--;
            if(this.gameOverDelay <= 0){
                CrazyEvents.Instance.GameplayStop();
                if(this.isContinue){
                    this.GameOver();
                }else{
                    this.ContinueCheck();
                }
            }
        }

        //コンティニューのディレイ
        if(this.continueDelay > 0){
            if(this.continueDelay % 40 == 0){
                this.continueDelayText.text = (this.continueDelay / 40).ToString();
            }
            this.continueDelay--;
            if(this.continueDelay <= 0){
                this.continueDelayText.text = "";
                this.Continue();
            }
        }
    }

    public void PositionReset(){
        Player.Instance.PositionReset();
        BulletMuzzle.PositionReset();
        ObstacleManager.Instance.PositionReset();
        UpgradeItem.Instance.PositionReset();
        SupportItem.Instance.PositionReset();
        CoinParent.Instance.PositionReset();
        Fog.Instance.PositionReset();
        BubbleEffect.Instance.PositionReset();
        BubbleBombEffect.Instance.PositionReset();
        ScoreManager.Instance.PositionReset();
    }

    public void GameStart(){
        this.isGame = true;
        CrazyEvents.Instance.GameplayStart();
        AudioManager.Instance.PlaySE(4);
        Player.Instance.GameStart();
        UIManager.Instance.GameStart();
        ObstacleManager.Instance.GameStart();
        ScoreManager.Instance.GameStart();
        AdsManager.Instance.GameStart();
    }

    public void PlayerKilled(){
        this.isGame = false;
        this.gameOverDelay = this.GameOverDelay;
        CoinParent.Instance.PlayerKilled();
        Player.Instance.PlayerKilled();
    }

    public void ContinueCheck(){
        UIManager.Instance.ContinueCheck();
    }

    public void ContinueDelaySet(){
        if(CoinParent.Instance.Use(this.continueMoney)){
            this.continueDelay = this.ContinueDelay;
            UIManager.Instance.ContinueDelaySet();
            AudioManager.Instance.PlaySE(1);
        }
    }

    public void Continue(){
        this.isContinue = true;
        this.isGame = true;
        CrazyEvents.Instance.GameplayStart();
        Player.Instance.Continue();
    }

    public void GameOver(){
        this.isGame = false;
        AudioManager.Instance.PlaySE(4);
        UIManager.Instance.GameOver();
        ScoreManager.Instance.GameOver();
    }

    public void Retry(){
        this.isContinue = false;
        AudioManager.Instance.PlaySE(4);

        Player.Instance.Retry();
        UIManager.Instance.Retry();
        ObstacleManager.Instance.Retry();
        UpgradeItem.Instance.Retry();
        SupportItem.Instance.Retry();
        CoinParent.Instance.Retry();
        Fog.Instance.Retry();
        ScoreManager.Instance.Retry();
        AdsManager.Instance.Retry();
    }

    public void DataDelete(){
        ScoreManager.Instance.DataDelete();
        CoinParent.Instance.DataDelete();
        Player.Instance.DataDelete();
    }

    //回転してないボックス同士の判定
    static public bool CheckBoxColl(Vector3 position1, Vector3 size1, Vector3 position2, Vector3 size2){
        var hitRange = (size1 + size2) / 2f;
        var range = position1 - position2;
        if(Mathf.Abs(range.x) <= hitRange.x 
        && Mathf.Abs(range.y) <= hitRange.y
        && Mathf.Abs(range.z) <= hitRange.z){
            return true;
        }else{
            return false;
        }
    }
}
