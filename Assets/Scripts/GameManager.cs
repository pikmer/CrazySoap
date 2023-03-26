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
        ObstacleManager.Instance.PositionReset();
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
    }
}
