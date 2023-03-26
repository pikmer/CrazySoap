using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    public static ObstacleManager Instance;

    public GameObject prefab;
    [HideInInspector][System.NonSerialized]
    public Obstacle[] obstacles;

    int wave = 1;
    int waveTime = 0;
    int WaveTime = 20 * 60;


    void Awake()
    {
        Instance = this;

        //生成
        this.obstacles = new Obstacle[60];
        for (int i = 0; i < this.obstacles.Length; i++)
        {
        	GameObject obstacle = Instantiate(prefab, Vector3.zero, Quaternion.identity);
			obstacle.transform.SetParent(this.transform);
            obstacle.SetActive(true);
            Obstacle sc = obstacle.GetComponent<Obstacle>();
            this.obstacles[i] = sc;
        }
    }

    void FixedUpdate()
    {
        if(!GameManager.Instance.isGame) return;

        this.waveTime++;
        if(this.waveTime >= this.WaveTime){
            this.wave++;
            this.SetObstacle();
            this.waveTime = 0;
        }
    }

    void SetObstacle(){
        var plauerPos = Player.Instance.transform.position;
        for (int i = 0; i < this.obstacles.Length; i++)
        {
            var obstacle = this.obstacles[i];
            var lange = i / 10;
            obstacle.transform.position = new Vector3(Random.Range(-10f, 10f), 0,  60 * (1 + lange)) + plauerPos;
        }
    }

    public void PositionReset(){
        var positionResetRange = Player.Instance.positionResetRange;
        foreach (var obstacle in this.obstacles)
        {
            obstacle.transform.position -= Vector3.forward * positionResetRange;
        }
    }

    public void GameStart(){
        this.SetObstacle();
    }

    public void Retry(){
        this.waveTime = 0;
        this.wave = 1;
    }
}
