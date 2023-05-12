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
    [SerializeField] Text todayScoreText;

    [SerializeField]
    Transform playerTrf;

    void Awake(){
        Instance = this;

        this.scorePlusText.text = "";

        var now = DateTime.Now.ToShortDateString();
        if(now != PlayerPrefs.GetString(this.todayScoreTimeKey, "")){
            PlayerPrefs.SetInt(this.todayScoreKey, 0);
        }
        this.todayScoreText.text = "today top : " + PlayerPrefs.GetInt(this.todayScoreKey, 0);


        var topScores = this.GetTopScores();
        for (int i = 0; i < this.topScoresText.Length; i++)
        {
            this.topScoresText[i].text = topScores.scores[i].ToString();
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
            }else if(score > topScores.scores[i]){
                shift = topScores.scores[i];
                topScores.scores[i] = score;
                isRecord = true;
            }
            this.topScoresText[i].text = topScores.scores[i].ToString();
        }
        if(isRecord){
            PlayerPrefs.SetString(this.topScoresKey, JsonUtility.ToJson(topScores));
        }

        //本日の最高記録
        var now = DateTime.Now.ToShortDateString();
        if(now == PlayerPrefs.GetString(this.todayScoreTimeKey, "")){
            if(score > PlayerPrefs.GetInt(this.todayScoreKey, 0)){
                PlayerPrefs.SetInt(this.todayScoreKey, score);
            }
        }else{
            PlayerPrefs.SetInt(this.todayScoreKey, score);
            PlayerPrefs.SetString(this.todayScoreTimeKey, now);
        }
        this.todayScoreText.text = "today top : " + PlayerPrefs.GetInt(this.todayScoreKey, 0);
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
