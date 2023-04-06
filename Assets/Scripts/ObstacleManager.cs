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

    int innerWave = 0;
    int wave = 1;
    int waveTime = 0;
    int WaveTime = 20 * 60;

    StageInfo stageInfo = new StageInfo();

    //waveごとの確立
    UnityEvent<float> waveEnemySpawn = new UnityEvent<float>();
    public Dictionary<int, UnityAction<float>[]> stages;
    public UnityAction<float>[] nowStages;


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
            this.waveTime = 0;
        }
        if(this.waveTime % (this.WaveTime / 2) == 0){
            Player.Instance.ChangesideStream();
        }
        if(this.waveTime % (this.WaveTime / 6) == 0){
            this.SetStage(5);
        }
    }

    void SetStage(float z){
        var playerPosZ = Player.Instance.transform.position.z;
        this.innerWave++;
        
        //抽選対象の切り替え
        if(this.stages.ContainsKey(this.innerWave)){
            this.nowStages = this.stages[this.innerWave];
        }

        //3の倍数は強化ポイント
        if(this.innerWave % 6 == 0){
            this.stageInfo.Upgrade(playerPosZ + 100 * z);
        }
        //ランダムセット
        else{
            this.nowStages[Random.Range(0, this.nowStages.Length)](playerPosZ + 100 * z);
        }
    }

    public void SetObstacle(Vector3 position, int index, Coin coin = null){
        foreach (var obstacle in this.obstacles[index])
        {
            if(!obstacle.isActive){
                obstacle.Init(position);
                if(coin != null){
                    obstacle.Protect(coin);
                    coin.Protect();
                }
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
        for (int i = 0; i < 6; i++)
        {
            this.SetStage(i);
        }
        UpgradeItem.Instance.SetItem(new Vector3(0, 0, 5));
    }

    public void Retry(){
        this.waveTime = 0;
        this.wave = 1;
        this.innerWave = 0;
        foreach (var obstacleArray in this.obstacles)
        {
            foreach (var obstacle in obstacleArray)
            {
                if(obstacle.isActive) obstacle.SetActive(false);
            }
        }
    }
}
