using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageInfo
{
    ObstacleManager manager;

    public void SetManager(ObstacleManager manager){
        this.manager = manager;
    }

    public void SetObstacle(StageName name, float offsetZ){
        switch (name)
        {
            case StageName.Test1:
                this.Test1(offsetZ);
                break;
            case StageName.Test2:
                this.Test2(offsetZ);
                break;
        }
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
            manager.SetObstacle(new Vector3(-8 + 4 * i, 0, offsetZ + 100f), 0);
        }
        // for (int i = 0; i < 4; i++)
        // {
        //     manager.SetObstacle(new Vector3(-6 + 4 * i, 0, offsetZ + 70f), 0);
        // }
        //強化アイテム
        var upgradePos = new Vector3(-6 + 4 * Random.Range(0, 4), 0, offsetZ + 100f);
        manager.SetObstacle(upgradePos, 1);
        UpgradeItem.Instance.SetItem(upgradePos);

        //コイン
        for (int i = 0; i < 10; i++)
        {
            CoinParent.Instance.SetCoin(new Vector3(0, 0, offsetZ + 10 * i));
        }
    }
}

public enum StageName{
    Test1,
    Test2,
}
