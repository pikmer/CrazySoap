using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObstacleManager : MonoBehaviour
{
    public static ObstacleManager Instance;

    [HideInInspector][System.NonSerialized]
    public Obstacle[][] obstacles;
    public GameObject[] prefabs;
    public int[] objLength;

    int wave = 1;
    int waveTime = 0;
    int WaveTime = 20 * 60;


    void Awake()
    {
        Instance = this;

        this.obstacles = new Obstacle[this.prefabs.Length][];
        for (int i = 0; i < this.prefabs.Length; i++)
        {
            this.obstacles[i] = new Obstacle[this.objLength[i]];
            for (int j = 0; j < this.objLength[i]; j++)
            {
                GameObject obstacle = Instantiate(this.prefabs[i], Vector3.zero, Quaternion.identity);
                obstacle.transform.SetParent(this.transform);
                obstacle.SetActive(true);
                Obstacle sc = obstacle.GetComponent<Obstacle>();
                this.obstacles[i][j] = sc;
            }
        }
    }

    void FixedUpdate()
    {
        if(!GameManager.Instance.isGame) return;

        this.waveTime++;
        if(this.waveTime >= this.WaveTime){
            this.wave++;
            this.SetStage();
            this.waveTime = 0;
        }
    }

    void SetStage(){
        var playerPosZ = Player.Instance.transform.position.z;
        for (int i = 0; i < this.obstacles[0].Length; i++)
        {
            var obstacle = this.obstacles[0][i];
            var lange = i / 10;
            obstacle.Init(new Vector3(Random.Range(-20f, 20f), 0, playerPosZ + 100 * (1 + lange)));
        }
        //強化アイテム
        this.SetObstacle(new Vector3(0, 0, playerPosZ + 250f), 1);
        UpgradeItem.Instance.SetItem(new Vector3(0, 0, playerPosZ + 250f));
        this.SetObstacle(new Vector3(0, 0, playerPosZ + 550f), 1);
        UpgradeItem.Instance.SetItem(new Vector3(0, 0, playerPosZ + 550f));
        //コイン
        for (int i = 0; i < 10; i++)
        {
            CoinParent.Instance.SetCoin(new Vector3(0, 0, playerPosZ + 300f + 10 * i));
        }
    }

    public void SetObstacle(Vector3 position, int index){
        foreach (var obstacle in this.obstacles[index])
        {
            if(!obstacle.isActive){
                obstacle.Init(position);
                return;
            }
        }
    }

    public void PositionReset(){
        var positionResetRange = Player.Instance.positionResetRange;
        foreach (var obstacleArray in this.obstacles)
        {
            foreach (var obstacle in obstacleArray)
            {
                obstacle.transform.position -= Vector3.forward * positionResetRange;
            }
        }
    }

    public void GameStart(){
        this.SetStage();
    }

    public void Retry(){
        this.waveTime = 0;
        this.wave = 1;
        foreach (var obstacleArray in this.obstacles)
        {
            foreach (var obstacle in obstacleArray)
            {
                if(obstacle.isActive) obstacle.SetActive(false);
            }
        }
    }
}
