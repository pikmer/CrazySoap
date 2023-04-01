using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [HideInInspector][System.NonSerialized]
    public bool isGame = false;

    void Awake(){
        Instance = this;
    }

    void Start()
    {
        
    }

    void Update()
    {
        
    }

    public void PositionReset(){
        Player.Instance.PositionReset();
        BulletMuzzle.PositionReset();
        ObstacleManager.Instance.PositionReset();
        UpgradeItem.Instance.PositionReset();
        SupportItem.Instance.PositionReset();
        CoinParent.Instance.PositionReset();
    }

    public void GameStart(){
        this.isGame = true;
        Player.Instance.GameStart();
        UIManager.Instance.GameStart();
        ObstacleManager.Instance.GameStart();
    }

    public void GameOver(){
        this.isGame = false;
        Player.Instance.GameOver();
        UIManager.Instance.GameOver();
    }

    public void Retry(){
        Player.Instance.Retry();
        UIManager.Instance.Retry();
        ObstacleManager.Instance.Retry();
        UpgradeItem.Instance.Retry();
        SupportItem.Instance.Retry();
        CoinParent.Instance.Retry();
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
