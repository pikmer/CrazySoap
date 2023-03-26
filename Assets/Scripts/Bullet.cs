using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class Bullet{
	public GameObject obj;
	public Transform transform;
	public bool isActive = false;
    public Vector3 velocity;
	public int timer = 120;

	public Bullet(GameObject obj){
		this.obj = obj;
		this.transform = obj.transform;
	}
}