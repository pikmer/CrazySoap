using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class StageInfo
{
    ObstacleManager manager;

    public void SetManager(ObstacleManager manager){
        this.manager = manager;
        manager.stages = new UnityAction<float>[]{
            Test1, Test2, CenterBlock, RightLeft, Branch, Wall, Center,
            Bubble, MoveBubble,
        };
    }

    void Test1(float offsetZ)
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

    void Test2(float offsetZ)
    {
        for (int i = 0; i < 5; i++)
        {
            manager.SetObstacle(new Vector3(-8 + 4 * i, 0, offsetZ + 90f), 0);
        }
        // for (int i = 0; i < 4; i++)
        // {
        //     manager.SetObstacle(new Vector3(-6 + 4 * i, 0, offsetZ + 70f), 0);
        // }
        //強化アイテム
        var upgradePos = new Vector3(-6 + 4 * Random.Range(0, 4), 0, offsetZ + 90f);
        manager.SetObstacle(upgradePos, 1);
        if(Random.value < 0.5f){
            UpgradeItem.Instance.SetItem(upgradePos);
        }else{
            SupportItem.Instance.SetItemRandom(upgradePos);
        }

        //コイン
        for (int i = 0; i < 10; i++)
        {
            CoinParent.Instance.SetCoin(new Vector3(0, 0, offsetZ + 10 * i));
        }
    }

    public void CenterBlock(float offsetZ){
        for (int i = 0; i < 3; i++)
        {
            manager.SetObstacle(new Vector3(0, 0, offsetZ + 30 * (1 + i)), 0);
        }
        for (int i = 0; i < 3; i++)
        {
            manager.SetObstacle(new Vector3(5, 0, offsetZ + 15 + 30 * i), 0);
            manager.SetObstacle(new Vector3(-5, 0, offsetZ + 15 + 30 * i), 0);
        }

        //コイン
        for (int i = 0; i < 10; i++)
        {
            CoinParent.Instance.SetCoin(new Vector3(0, 0, offsetZ + 10 * i));
        }
    }

    public void RightLeft(float offsetZ){
        for (int i = 1; i < 5; i++)
        {
            manager.SetObstacle(new Vector3(0, 0, offsetZ + 10 * (1 + i)), 0);
            manager.SetObstacle(new Vector3(-3, 0, offsetZ + 10 * (1 + i)), 0);
            manager.SetObstacle(new Vector3(-6, 0, offsetZ + 10 * (1 + i)), 0);
            CoinParent.Instance.SetCoin(new Vector3(5, 0, offsetZ + 10 * (1 + i)));
        }
        for (int i = 6; i < 10; i++)
        {
            manager.SetObstacle(new Vector3(0, 0, offsetZ + 10 * (1 + i)), 0);
            manager.SetObstacle(new Vector3(3, 0, offsetZ + 10 * (1 + i)), 0);
            manager.SetObstacle(new Vector3(6, 0, offsetZ + 10 * (1 + i)), 0);
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
        }
    }

    public void Wall(float offsetZ){
        for (int i = 0; i < 5; i++)
        {
            manager.SetObstacle(new Vector3(-8 + 4 * i, 0, offsetZ + 90f), 0);
        }
        //強化アイテム
        var upgradeX = Random.Range(0, 4);
        for (int i = 0; i < 4; i++)
        {
            var pos = new Vector3(-6 + 4 * i, 0, offsetZ + 90f);
            if(upgradeX == i){
                manager.SetObstacle(pos, 1);
                if(Random.value < 0.5f){
                    UpgradeItem.Instance.SetItem(pos);
                }else{
                    SupportItem.Instance.SetItemRandom(pos);
                }
            }else{
                manager.SetObstacle(pos, 0);
            }
        }

        //コイン
        for (int i = 0; i < 10; i++)
        {
            CoinParent.Instance.SetCoin(new Vector3(0, 0, offsetZ + 10 * i));
        }
    }

    public void Center(float offsetZ){
        for (int i = 0; i < 5; i++)
        {
            manager.SetObstacle(new Vector3(3, 0, offsetZ + 20 * (1 + i)), 2);
            manager.SetObstacle(new Vector3(-3, 0, offsetZ + 20 * (1 + i)), 2);
        }
    }

    public void Bubble(float offsetZ){
        for (int i = 3; i < 8; i++)
        {
            manager.SetObstacle(new Vector3(0, 0, offsetZ + 10 * (1 + i)), 3);
            manager.SetObstacle(new Vector3(3, 0, offsetZ + 10 * (1 + i)), 3);
            manager.SetObstacle(new Vector3(-3, 0, offsetZ + 10 * (1 + i)), 3);
        }
    }

    public void MoveBubble(float offsetZ){
        for (int i = 3; i < 16; i++)
        {
            manager.SetObstacle(new Vector3(10, 0, offsetZ + 5 * (1 + i)), 4);
        }
    }
}
