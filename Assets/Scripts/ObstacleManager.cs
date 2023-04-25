using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
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
    [SerializeField] Text waveText;

    StageInfo stageInfo = new StageInfo();

    //waveごとの確立
    UnityEvent<float> waveEnemySpawn = new UnityEvent<float>();
    public Dictionary<int, UnityAction<float>[]> stages;
    public UnityAction<float>[] nowStages;

    //水流の表現
    public Transform[] currents;
    int currentIndex = 0;
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
    int currentArrowCount = 0;
    int CurrentArrowCount = 120;
    //
    public Material currentMat;
    float currentMatOffset;

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
            waterEffect.position = Vector3.back * 10f;
            this.waterEffects[i] = waterEffect;
        }

        this.stageInfo.SetManager(this);

        this.currentArrow.gameObject.SetActive(false);
    }

    void FixedUpdate()
    {
        //水流UI
        if(this.currentArrowCount > 0){
            this.currentArrowCount--;
            var value = this.currentArrowCount % 30 - 15;
            this.currentArrow.anchoredPosition3D = new Vector3((float)value * 1.1f, 0, 0);
            if(this.currentArrowCount <= 0){
                this.currentArrow.gameObject.SetActive(false);
            }
        }
        //水流
        this.currentMatOffset -= 0.05f;
        if(this.currentMatOffset <= 0){
            this.currentMatOffset += 1f;
        }
        this.currentMat.mainTextureOffset = new Vector2(this.currentMatOffset, 0);

        if(!GameManager.Instance.isGame) return;

        this.waveTime++;
        if(this.waveTime >= this.WaveTime){
            this.wave++;
            this.waveTime = 0;
            this.waveText.text = "level\u00A0" + this.wave;
        }
        //水流変更 2インナーウェーブ前
        if(this.innerWave >= 12 && (this.waveTime + this.WaveTime / 3) % (this.WaveTime / 2) == 0){
            if(Random.value < 0.5f){
                this.sideStream = Random.Range(this.minStream, this.maxStream);
            }else{
                this.sideStream = Random.Range(-this.maxStream, -this.minStream);
            }
        }
        //水流変更確定
        if(this.wave >= 3 && this.waveTime % (this.WaveTime / 2) == 0){
            Player.Instance.ChangesideStream(this.sideStream);
            this.currentArrowCount = this.CurrentArrowCount;
            this.currentArrow.gameObject.SetActive(true);
            var angle = this.sideStream < 0 ? 0 : 180f;
            this.currentArrow.parent.rotation = Quaternion.Euler(0, 0, angle);
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
            if(this.innerWave < 24){
                this.stageInfo.Upgrade(playerPosZ + 100 * z);
            }else{
                this.stageInfo.Wall(playerPosZ + 100 * z);
            }
        }
        //難しwave
        else if(this.innerWave == 59){
            this.stageInfo.MoveDuck(playerPosZ + 100 * z);
        }
        else if(this.innerWave == 95){
            this.stageInfo.MoveDuckBlack(playerPosZ + 100 * z);
        }
        else if(this.innerWave > 59 && this.innerWave < 95 && this.innerWave % 6 == 5 && Random.value < 0.5f){
            this.stageInfo.MoveDuck(playerPosZ + 100 * z);
        }
        else if(this.innerWave > 95 && this.innerWave % 6 == 5){
            if(Random.value < 0.5f){
                this.stageInfo.MoveDuck(playerPosZ + 100 * z);
            }else{
                this.stageInfo.MoveDuckBlack(playerPosZ + 100 * z);
            }
        }
        //ランダムセット
        else{
            this.nowStages[Random.Range(0, this.nowStages.Length)](playerPosZ + 100 * z);
        }

        //水流
        if(this.innerWave >= 13){
            if(this.sideStream > 0){
                this.currents[this.currentIndex].rotation = Quaternion.Euler(0, 180, 1f);
            }else{
                this.currents[this.currentIndex].rotation = Quaternion.Euler(0, 0, 1f);
            }
            this.currents[this.currentIndex].position = new Vector3(0, 0.15f, Player.Instance.transform.position.z + 250f);
            this.currentIndex++;
            if(this.currentIndex >= this.currents.Length) this.currentIndex = 0;
        }
    }

    public void SetObstacle(Vector3 position, int index, int random = -1){
        foreach (var obstacle in this.obstacles[index])
        {
            if(!obstacle.isActive){
                obstacle.Init(position);
                if(random != -1){
                    obstacle.Randomizer(random);
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
        foreach (var current in this.currents)
        {
            current.position -= Vector3.forward * positionResetRange;
        }
    }

    public void GameStart(){
        this.SetStage(0);
        this.SetStage(1);
        this.SetStage(2);
        UpgradeItem.Instance.SetStartItem(new Vector3(0, 0, 50));
        this.waveText.text = "level 1";
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
        foreach (var current in this.currents)
        {
            current.position = Vector3.down * 10f;
        }
        this.currentIndex = 0;

        this.sideStream = 0;
        this.currentArrow.gameObject.SetActive(false);

        this.effectIndex = 0;
        foreach (var effect in this.waterEffects)
        {
            effect.position = Vector3.back * 10f;
        }
    }

    public void JumpItemSet(float forwardSpeed, float JumpSpeed, float gravity){
        var playerPos = Player.Instance.transform.position;

        for (int i = 0; i < 20; i++)
        {
            var value = (float)i * 5f;
            CoinParent.Instance.SetCoin(playerPos + new Vector3(0, value * JumpSpeed - this.Drop((int)value) * gravity
                , value * forwardSpeed));
        }

        var itemPosCount = 150f;
        var itemPos = playerPos + new Vector3(0, itemPosCount * JumpSpeed - this.Drop((int)itemPosCount) * gravity
            , itemPosCount * forwardSpeed);
        itemPos.x = Random.value < 0.5f ? 5f : -5f;
        UpgradeItem.Instance.SetItem(itemPos);
        itemPos.x *= -1f;
        SupportItem.Instance.SetItemRandomJumpNone(itemPos);
    }
    float Drop(int frame){
        float temp = 0;
        for (int i = 0; i < frame; i++)
        {
            for (int j = 0; j < i+1; j++)
            {
            temp += 1;
            }
        }
        return temp;
    }
}
