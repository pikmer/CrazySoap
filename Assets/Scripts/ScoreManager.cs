using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class ScoreManager : MonoBehaviour
{
    static public ScoreManager Instance;

    //スコア
    int posResetMil;
    int itemScore;
    int befourScore;
    public Text scoreText;
    public Text scorePlusText;
    public Text topScoreTextGame;
    int scorePlusCount;
    int ScorePlusCount = 40;
    public Color scorePlusColorCoin;
    public Color scorePlusColorBubble;

    //保存用のキー
    string topScoresKey = "topScores";
    string todayScoreKey = "todayScore";
    string todayScoreTimeKey = "todayScoreTime";

    string saveNotFound = "saveNotFound";

    [SerializeField] Text[] topScoresText;
    [SerializeField] Text[] topScoresText2;
    [SerializeField] Text todayScoreText;
    [SerializeField] Text todayScoreText2;

    [SerializeField] RectTransform topScoreEmp;
    [SerializeField] RectTransform topScoreEmp2;
    [SerializeField] GameObject todayScoreEmp;
    [SerializeField] GameObject todayScoreEmp2;

    [SerializeField]
    Transform playerTrf;

    //操作説明
    [SerializeField]
    GameObject ctrlImage;

    void Awake(){
        Instance = this;

        this.scorePlusText.text = "";

        var now = DateTime.Now.ToShortDateString();
        if(now != PlayerPrefs.GetString(this.todayScoreTimeKey, "")){
            PlayerPrefs.SetInt(this.todayScoreKey, 0);
        }
        this.todayScoreText.text = PlayerPrefs.GetInt(this.todayScoreKey, 0).ToString();
        this.todayScoreText2.text = PlayerPrefs.GetInt(this.todayScoreKey, 0).ToString();


        var topScores = this.GetTopScores();
        for (int i = 0; i < this.topScoresText.Length; i++)
        {
            this.topScoresText[i].text = topScores.scores[i].ToString();
            this.topScoresText2[i].text = topScores.scores[i].ToString();
        }
        this.topScoreTextGame.text = "top " + topScores.scores[0];
    }

    void FixedUpdate()
    {
        if(this.scorePlusCount > 0){
            this.scorePlusCount--;
            if(this.scorePlusCount >= this.ScorePlusCount - 5){
                var value = this.scorePlusCount - this.ScorePlusCount + 5;
                this.scorePlusText.transform.localScale = Vector3.one * (1f + (float)value / 5f);
            }
            if(this.scorePlusCount < 5){
                this.scorePlusText.transform.localScale = Vector3.one * (float)this.scorePlusCount / 5f;
            }
        }

        //スコア表示更新
        int score = this.GetScore();
        if(score != this.befourScore){
            this.befourScore = score;
            this.scoreText.text = score.ToString();

            //操作説明
            if(this.ctrlImage.activeSelf && score > 30){
                this.ctrlImage.SetActive(false);
            }
        }
    }

    TopScores GetTopScores(){
        string recordJson = PlayerPrefs.GetString(topScoresKey, this.saveNotFound);
        if(recordJson == this.saveNotFound){
            return new TopScores();
        }else{
            return JsonUtility.FromJson<TopScores>(recordJson);
        }
    }

    //スコア計算
    int GetScore(){
        return (this.posResetMil + (int)this.playerTrf.position.z) / 4 + this.itemScore;
    }
    public void ItemScore(int score, int colorIndex){
        this.itemScore += score;
        this.scorePlusText.text = "+" + score;
        this.scorePlusCount = this.ScorePlusCount;

        if(colorIndex == 0){
            this.scorePlusText.color = this.scorePlusColorCoin;
        }else if(colorIndex == 1){
            this.scorePlusText.color = this.scorePlusColorBubble;
        }
    }

    public void PositionReset(){
        this.posResetMil += (int)Player.Instance.positionResetRange;
    }

    public void GameStart(){
        this.scoreText.text = "";
        this.topScoreTextGame.text = "top " + this.GetTopScores().scores[0];
        if(GameManager.Instance.isPC) this.ctrlImage.SetActive(true);
    }

    public void GameOver(){
        var score = this.GetScore();

        //最高記録
        var topScores = this.GetTopScores();
        var isRecord = false;
        var shift = 0;
        for (int i = 0; i < topScores.scores.Length; i++)
        {
            if(isRecord){
                var scoreTemp = topScores.scores[i]; 
                topScores.scores[i] = shift;
                shift = scoreTemp;
                this.topScoresText[i].color = Color.white;
                this.topScoresText2[i].color = Color.white;
            }else if(score > topScores.scores[i]){
                shift = topScores.scores[i];
                topScores.scores[i] = score;
                isRecord = true;
                this.topScoreEmp.anchoredPosition3D = new Vector3(0, -20 - 40 * (i + 1), 0);
                this.topScoreEmp.gameObject.SetActive(true);
                this.topScoreEmp2.anchoredPosition3D = new Vector3(0, -20 - 40 * (i + 1), 0);
                this.topScoreEmp2.gameObject.SetActive(true);
                this.topScoresText[i].color = Color.yellow;
                this.topScoresText2[i].color = Color.yellow;
            }else{
                this.topScoresText[i].color = Color.white;
                this.topScoresText2[i].color = Color.white;
            }
            this.topScoresText[i].text = topScores.scores[i].ToString();
            this.topScoresText2[i].text = topScores.scores[i].ToString();
        }
        if(isRecord){
            PlayerPrefs.SetString(this.topScoresKey, JsonUtility.ToJson(topScores));
        }else{
            this.topScoreEmp.gameObject.SetActive(false);
            this.topScoreEmp2.gameObject.SetActive(false);
        }

        //本日の最高記録
        var now = DateTime.Now.ToShortDateString();
        if(now == PlayerPrefs.GetString(this.todayScoreTimeKey, "")){
            if(score > PlayerPrefs.GetInt(this.todayScoreKey, 0)){
                PlayerPrefs.SetInt(this.todayScoreKey, score);
                this.todayScoreEmp.SetActive(true);
                this.todayScoreEmp2.SetActive(true);
                this.todayScoreText.color = Color.yellow;
                this.todayScoreText2.color = Color.yellow;
            }else{
                this.todayScoreEmp.SetActive(false);
                this.todayScoreEmp2.SetActive(false);
                this.todayScoreText.color = Color.white;
                this.todayScoreText2.color = Color.white;
            }
        }else{
            PlayerPrefs.SetInt(this.todayScoreKey, score);
            PlayerPrefs.SetString(this.todayScoreTimeKey, now);
            this.todayScoreEmp.SetActive(true);
            this.todayScoreEmp2.SetActive(true);
            this.todayScoreText.color = Color.yellow;
            this.todayScoreText2.color = Color.yellow;
        }
        this.todayScoreText.text = PlayerPrefs.GetInt(this.todayScoreKey, 0).ToString();
        this.todayScoreText2.text = PlayerPrefs.GetInt(this.todayScoreKey, 0).ToString();
    }

    public void Retry(){
        //スコア関係
        this.posResetMil = 0;
        this.itemScore = 0;
        this.befourScore = 0;
        this.scoreText.text = "0";
        this.scorePlusText.text = "";
    }

    public void DataDelete()
    {
        PlayerPrefs.DeleteKey(this.topScoresKey);
        PlayerPrefs.DeleteKey(this.todayScoreKey);
        PlayerPrefs.DeleteKey(this.todayScoreTimeKey);
    }

    class TopScores{
        public int[] scores = new int[5];
    }
}
