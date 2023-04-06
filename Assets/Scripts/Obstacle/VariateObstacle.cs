using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class VariateObstacle : Obstacle
{

    public Mesh[] meshes;
    public Material[] materials;
    public MeshFilter filter;
    public MeshRenderer meshRender;

    public override void Init(Vector3 position){
        base.Init(position);
        this.filter.mesh = this.meshes[Random.Range(0, this.meshes.Length)];
        this.meshRender.material = this.materials[Random.Range(0, this.materials.Length)];
    }
}
