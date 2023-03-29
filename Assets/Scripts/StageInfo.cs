using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StageInfo
{
    ObstacleManager manager;

    public void SetManager(ObstacleManager manager){
        this.manager = manager;
    }

    void Update(float offsetZ)
    {
        manager.SetObstacle(new Vector3(0, 0, offsetZ + 250f), 1);
    }
}
