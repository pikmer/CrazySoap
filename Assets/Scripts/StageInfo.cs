using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StageInfo
{
    ObstacleManager manager;

    public void SetManager(ObstacleManager manager){
        this.manager = manager;
        manager.stages = new Dictionary<int, UnityAction<float>[]>(){
            // {1, new UnityAction<float>[]{MoveDuck,}},

            {1, new UnityAction<float>[]{Mutual,}},
            {2, new UnityAction<float>[]{RightLeft,}},
            {3, new UnityAction<float>[]{Branch, Center, CenterBlock}},
            {5, new UnityAction<float>[]{
                Mutual, CenterBlock, RightLeft, Branch, Center,
                Bubble, MoveBubble,
            }},

            {7, new UnityAction<float>[]{
                Mutual, CenterBlock, RightLeft, Branch, Center,
                Bubble, MoveBubble, Upgrade, BubbleSpawner
            }},

            {16, new UnityAction<float>[]{UpgradeJump,}},

            {17, new UnityAction<float>[]{
                Mutual, CenterBlock, RightLeft, Branch, Center,
                Bubble, MoveBubble, Upgrade, BubbleSpawner
            }},

            {24, new UnityAction<float>[]{
                Mutual, CenterBlock, RightLeft, Branch, Center,
                Bubble, MoveBubble, Wall, BubbleSpawner3,
            }},

            {30, new UnityAction<float>[]{
                Mutual, CenterBlock, RightLeft, Branch, Center,
                BubbleLv2, MoveBubbleLv2, Wall, BubbleSpawner3, MoveObstacle,
            }},
        };
    }

    void Mutual(float offsetZ)
    {
        for (int i = 0; i < 5; i++)
        {
            manager.SetObstacle(new Vector3(-8 + 4 * i, 0, offsetZ + 100f), 0);
        }
        for (int i = 0; i < 4; i++)
        {
            manager.SetObstacle(new Vector3(-6 + 4 * i, 0, offsetZ + 70f), 0);
        }
    }

    public void Upgrade(float offsetZ)
    {
        for (int i = 0; i < 5; i++)
        {
            manager.SetObstacle(new Vector3(-8 + 4 * i, 0, offsetZ + 90f), 0);
        }
        //強化アイテム
        var upgradePos = new Vector3(-6 + 4 * Random.Range(0, 4), 0, offsetZ + 90f);
        manager.SetObstacle(upgradePos, 1);
        if(Random.value < 0.5f){
            UpgradeItem.Instance.SetItem(upgradePos);
        }else{
            SupportItem.Instance.SetItemRandomJumpNone(upgradePos);
        }

        //コイン
        for (int i = 0; i < 8; i++)
        {
            CoinParent.Instance.SetCoin(new Vector3(0, 0, offsetZ + 10 * (1 + i)));
        }
    }

    public void UpgradeJump(float offsetZ)
    {
        for (int i = 0; i < 5; i++)
        {
            manager.SetObstacle(new Vector3(-8 + 4 * i, 0, offsetZ + 90f), 0);
        }
        //強化アイテム
        var upgradePos = new Vector3(-6 + 4 * Random.Range(0, 4), 0, offsetZ + 90f);
        manager.SetObstacle(upgradePos, 1);
        SupportItem.Instance.SetItem(upgradePos, 3);

        //コイン
        for (int i = 0; i < 8; i++)
        {
            CoinParent.Instance.SetCoin(new Vector3(0, 0, offsetZ + 10 * (1 + i)));
        }
    }

    public void CenterBlock(float offsetZ){
        for (int i = 0; i < 3; i++)
        {
            manager.SetObstacle(new Vector3(0, 0, offsetZ + 30 * (1 + i)), 0);

            manager.SetObstacle(new Vector3(9, 0, offsetZ + 30 * (1 + i)), 0);
            manager.SetObstacle(new Vector3(-9, 0, offsetZ + 30 * (1 + i)), 0);
        }
        for (int i = 0; i < 3; i++)
        {
            manager.SetObstacle(new Vector3(4.5f, 0, offsetZ + 15 + 30 * i), 0);
            manager.SetObstacle(new Vector3(-4.5f, 0, offsetZ + 15 + 30 * i), 0);
        }

        //コイン
        for (int i = 0; i < 10; i++)
        {
            if(i % 3 != 0){
                CoinParent.Instance.SetCoin(new Vector3(0, 0, offsetZ + 10 * i));
            }
        }
    }

    public void RightLeft(float offsetZ){
        for (int i = 1; i < 5; i++)
        {
            manager.SetObstacle(new Vector3(0, 0, offsetZ + 10 * (1 + i)), 0);
            manager.SetObstacle(new Vector3(-3, 0, offsetZ + 10 * (1 + i)), 0);
            manager.SetObstacle(new Vector3(-6, 0, offsetZ + 10 * (1 + i)), 0);
            manager.SetObstacle(new Vector3(-9, 0, offsetZ + 10 * (1 + i)), 0);
            CoinParent.Instance.SetCoin(new Vector3(8, 0, offsetZ + 10 * (1 + i)));
        }
        for (int i = 6; i < 10; i++)
        {
            manager.SetObstacle(new Vector3(0, 0, offsetZ + 10 * (1 + i)), 0);
            manager.SetObstacle(new Vector3(3, 0, offsetZ + 10 * (1 + i)), 0);
            manager.SetObstacle(new Vector3(6, 0, offsetZ + 10 * (1 + i)), 0);
            manager.SetObstacle(new Vector3(9, 0, offsetZ + 10 * (1 + i)), 0);
            CoinParent.Instance.SetCoin(new Vector3(-8, 0, offsetZ + 10 * (1 + i)));
        }
    }

    float[] BranchPos = new float[]{1f, 2f, 3f, 8f, 9f, 10f};
    public void Branch(float offsetZ){
        for (int i = 0; i < this.BranchPos.Length; i++)
        {
            for (int j = 0; j < 3; j++)
            {
                var x = 5f + j * 2;
                manager.SetObstacle(new Vector3(x, 0, offsetZ + 10 * this.BranchPos[i]), 0);
                manager.SetObstacle(new Vector3(-x, 0, offsetZ + 10 * this.BranchPos[i]), 0);
            }
        }
        for (int i = 3; i < 7; i++)
        {
            manager.SetObstacle(new Vector3(0, 0, offsetZ + 10 * (1 + i)), 0);
            manager.SetObstacle(new Vector3(-2, 0, offsetZ + 10 * (1 + i)), 0);
            manager.SetObstacle(new Vector3(2, 0, offsetZ + 10 * (1 + i)), 0);
            CoinParent.Instance.SetCoin(new Vector3(8, 0, offsetZ + 10 * (1 + i)));
            CoinParent.Instance.SetCoin(new Vector3(-8, 0, offsetZ + 10 * (1 + i)));
        }
    }

    public void Wall(float offsetZ){
        for (int i = 0; i < 3; i++)
        {
            manager.SetObstacle(new Vector3(1 + 4 * i, 0, offsetZ + 90f), 0);
            manager.SetObstacle(new Vector3(-1 - 4 * i, 0, offsetZ + 90f), 0);
        }
        //強化アイテム
        var upgradeX1 = Random.Range(0, 4);
        var upgradeX2 = (upgradeX1 + 1 + Random.Range(0, 3)) % 4;
        for (int i = 0; i < 4; i++)
        {
            var x = (i < 2) ? (3 + 4 * i) : (-3 - 4 * (i-2));
            var pos = new Vector3(x, 0, offsetZ + 90f);
            if(upgradeX1 == i){
                manager.SetObstacle(pos, 1);
                UpgradeItem.Instance.SetItem(pos);
            }else if(upgradeX2 == i){
                manager.SetObstacle(pos, 1);
                SupportItem.Instance.SetItemRandom(pos);
            }else{
                manager.SetObstacle(pos, 0);
            }
        }

        //コイン
        for (int i = 0; i < 8; i++)
        {
            CoinParent.Instance.SetCoin(new Vector3(0, 0, offsetZ + 10 * (1 + i)));
        }
    }

    public void Center(float offsetZ){
        for (int i = 1; i < 9; i++)
        {
            manager.SetObstacle(new Vector3(3, 0, offsetZ + 10 * (1 + i)), 2);
            manager.SetObstacle(new Vector3(-3, 0, offsetZ + 10 * (1 + i)), 2);
        }
        manager.SetObstacle(new Vector3(6, 0, offsetZ + 20), 2);
        manager.SetObstacle(new Vector3(-6, 0, offsetZ + 20), 2);
        manager.SetObstacle(new Vector3(9, 0, offsetZ + 20), 2);
        manager.SetObstacle(new Vector3(-9, 0, offsetZ + 20), 2);
    }

    float[] BubblePos = new float[]{0, 1f, 2f, 3.5f, 5f, 8f};
    public void Bubble(float offsetZ){
        for (int i = 3; i < 8; i++)
        {
            for (int j = 0; j < this.BubblePos.Length - 1; j++)
            {
                manager.SetObstacle(new Vector3(this.BubblePos[j] + Random.Range(-0.7f, 0.7f), 0, offsetZ + 10 * (1 + i)), 3);
                manager.SetObstacle(new Vector3(-this.BubblePos[j] + Random.Range(-0.7f, 0.7f), 0, offsetZ + 10 * (1 + i)), 3);
            }
            CoinParent.Instance.SetCoin(new Vector3(0, 0, offsetZ + 10 * (1 + i)));
            CoinParent.Instance.SetCoin(new Vector3(0, 0, offsetZ + 10 * (1 + i) + 5));
        }
    }
    public void BubbleLv2(float offsetZ){
        for (int i = 3; i < 8; i++)
        {
            for (int j = 0; j < this.BubblePos.Length; j++)
            {
                manager.SetObstacle(new Vector3(this.BubblePos[j] + Random.Range(-0.7f, 0.7f), 0, offsetZ + 10 * (1 + i)), 3);
                manager.SetObstacle(new Vector3(-this.BubblePos[j] + Random.Range(-0.7f, 0.7f), 0, offsetZ + 10 * (1 + i)), 3);
            }
            CoinParent.Instance.SetCoin(new Vector3(0, 0, offsetZ + 10 * (1 + i)));
            CoinParent.Instance.SetCoin(new Vector3(0, 0, offsetZ + 10 * (1 + i) + 5));
        }
    }

    public void MoveBubble(float offsetZ){
        for (int i = 3; i < 16; i++)
        {
            manager.SetObstacle(new Vector3(10, 0, offsetZ + 5 * (1 + i)), 4);
        }
    }
    public void MoveBubbleLv2(float offsetZ){
        for (int i = 3; i < 16; i++)
        {
            manager.SetObstacle(new Vector3(10, 0, offsetZ + 5 * (1 + i)), 4);
            manager.SetObstacle(new Vector3(10, 0, offsetZ + 5 * (1 + i)), 4);
        }
    }

    public void MoveObstacle(float offsetZ){
        var x = (Random.value < 0.5f ? 1f : -1f) * Random.Range(4f, 8f);
        manager.SetObstacle(new Vector3(x, 0, offsetZ + 80), 7);
        
        manager.SetObstacle(new Vector3(0.5f, 0, offsetZ + 90), 5);
        manager.SetObstacle(new Vector3(-0.5f, 0, offsetZ + 90), 6);
        manager.SetObstacle(new Vector3(-9.5f, 0, offsetZ + 90), 5);
        manager.SetObstacle(new Vector3(9.5f, 0, offsetZ + 90), 6);

        manager.SetObstacle(new Vector3(5.5f, 0, offsetZ + 100), 5);
        manager.SetObstacle(new Vector3(4.5f, 0, offsetZ + 100), 6);
        manager.SetObstacle(new Vector3(-4.5f, 0, offsetZ + 100), 5);
        manager.SetObstacle(new Vector3(-5.5f, 0, offsetZ + 100), 6);

        //コイン
        for (int i = 0; i < 6; i++)
        {
            CoinParent.Instance.SetCoin(new Vector3(0, 0, offsetZ + 10 * (1 + i)));
        }
    }

    public void BubbleSpawner(float offsetZ){
        manager.SetObstacle(new Vector3(9, 0, offsetZ + 90), 8);
    }
    public void BubbleSpawner3(float offsetZ){
        manager.SetObstacle(new Vector3(9, 0, offsetZ + 90), 8);
        manager.SetObstacle(new Vector3(9, 0, offsetZ + 70), 8);
        manager.SetObstacle(new Vector3(9, 0, offsetZ + 50), 8);
    }

    public void MoveDuck(float offsetZ){
        
        for (int i = 3; i < 16; i++)
        {
            manager.SetObstacle(new Vector3(-9.25f, 0, offsetZ + 5 * (1 + i)), 10);
        }
    }
}
