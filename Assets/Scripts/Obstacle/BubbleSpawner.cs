using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleSpawner : Obstacle
{
    public Vector3 spawnPoint;

    int interval;
    public int Interval = 6;

    void FixedUpdate()
    {
        if(!this.isActive) return;

        this.interval++;
        if(this.interval >= this.Interval){
            ObstacleManager.Instance.SetObstacle(this.transform.position + spawnPoint 
            + new Vector3(0, 0, Random.Range(-0.5f, 0.5f)), 9);
            this.interval = 0;
        }
    }
}
