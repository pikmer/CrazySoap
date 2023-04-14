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

    //水流の表現
    Vector3 leftWaterPos = new Vector3(8.57f, 2.6f, 190f);     //10 + 180
    Vector3 rightWaterPos = new Vector3(-9.75f, 5.25f, 190f);
    public Transform waterEffect;
    Transform[] waterEffects = new Transform[10];
    int effectIndex = 0;
    float sideStream = 0;
    float minStream = 0.015f;
    float maxStream = 0.03f;
    //
    public RectTransform currentArrow;

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
        
        this.waterEffects[0] = this.waterEffect;
        for (int i = 1; i < this.waterEffects.Length; i++)
        {
            var waterEffect = Instantiate(this.waterEffect, Vector3.zero, Quaternion.identity);
            waterEffect.SetParent(this.waterEffect.parent);
            this.waterEffects[i] = waterEffect;
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
        //水流変更
        if((this.waveTime + this.WaveTime / 3) % (this.WaveTime / 2) == 0){
            if(Random.value < 0.5f){
                this.sideStream = Random.Range(this.minStream, this.maxStream);
            }else{
                this.sideStream = Random.Range(-this.maxStream, -this.minStream);
            }
        }
        //水流変更確定
        if(this.waveTime % (this.WaveTime / 2) == 0){
            Player.Instance.ChangesideStream(this.sideStream);
        }
        //水流演出
        if(this.waveTime % 40 == 0 && this.sideStream != 0){
            var pos = new Vector3(0, 0, Player.Instance.transform.position.z) 
                + (this.sideStream < 0 ? this.leftWaterPos : this.rightWaterPos);
            this.waterEffects[this.effectIndex].position = pos;
            this.effectIndex++;
            if(this.effectIndex >= this.waterEffects.Length){
                this.effectIndex = 0;
            }
        }
        //ステージ作成
        if(this.waveTime % (this.WaveTime / 6) == 0){
            this.SetStage(2);
        }

        this.currentArrow.anchoredPosition3D += Vector3.left;
        if(this.currentArrow.anchoredPosition3D.x <= -30f){
            this.currentArrow.anchoredPosition3D += Vector3.right * 30f;
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
                obstacle.Protect(coin);
                if(coin != null){
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
        foreach (var effect in this.waterEffects)
        {
            effect.position -= Vector3.forward * positionResetRange;
        }
    }

    public void GameStart(){
        this.SetStage(0);
        this.SetStage(1);
        this.SetStage(2);
        UpgradeItem.Instance.SetStartItem(new Vector3(0, 0, 50));
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

        this.sideStream = 0;

        this.effectIndex = 0;
        foreach (var effect in this.waterEffects)
        {
            effect.position = Vector3.back * 10f;
        }
    }
}
