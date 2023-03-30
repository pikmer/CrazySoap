using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

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

    StageInfo stageInfo = new StageInfo();

    //waveごとの確立
    UnityEvent<float> waveEnemySpawn = new UnityEvent<float>();
    // protected Dictionary<int, UnityAction<float>[]> stages;
    public UnityAction<float>[] stages;


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
                sc.SetActive(false);
            }
        }

        this.stageInfo.SetManager(this);
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
        if(this.waveTime % (this.WaveTime / 2) == 0){
            Player.Instance.ChangesideStream();
        }
    }

    void SetStage(){
        var playerPosZ = Player.Instance.transform.position.z;

        for (int i = 0; i < 6; i++)
        {
            this.stages[Random.Range(0, this.stages.Length)](playerPosZ + 100 * i);
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
                if(obstacle.isActive){
                    obstacle.transform.position -= Vector3.forward * positionResetRange;
                    if(obstacle.transform.position.z <= -10f){
                        obstacle.SetActive(false);
                    }
                }
            }
        }
    }

    public void GameStart(){
        this.SetStage();
        UpgradeItem.Instance.SetItem(new Vector3(0, 0, 5));
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
