using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Obstacle : MonoBehaviour
{
    public bool isActive;

    public GameObject graphics;

    public BoxCollider[] colliders;

    public virtual void Init(Vector3 position){
        this.transform.position = position;
        this.SetActive(true);
    }

    public virtual void Damage(int damage){}

    public void SetActive(bool active){
        this.isActive = active;
        this.graphics.SetActive(active);
    }
}