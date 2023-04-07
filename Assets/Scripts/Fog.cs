using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Fog : MonoBehaviour
{
    public Transform player;

    static public Fog Instance;

    void Start(){
        Instance = this;
    }

    void FixedUpdate()
    {
        if(!GameManager.Instance.isGame) return;

        var pos = this.transform.position ;
        pos.z = this.player.position.z + 100f;
        this.transform.position = pos;
    }

    public void Retry(){
        var pos = this.transform.position ;
        pos.z = this.player.position.z + 100f;
        this.transform.position = pos;
    }

    public void PositionReset(){
        var pos = this.transform.position ;
        pos.z = this.player.position.z + 100f;
        this.transform.position = pos;
    }
}
