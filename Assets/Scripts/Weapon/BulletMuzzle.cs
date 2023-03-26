using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMuzzle : MonoBehaviour
{
    public GameObject bulletPrefab;
    protected Bullet[] bullets;
    public int bulletsLength = 20;

    public float speed = 0.3f; 
    Vector3 buleltVelosity = new Vector3(0, 0, 0.3f);
    int bulletTimer = 60;

    void Start()
    {
        this.bullets = new Bullet[this.bulletsLength];
        for(int i=0; i < this.bullets.Length; i++){
        	GameObject bullet = Instantiate(this.bulletPrefab, Vector3.zero, Quaternion.identity);
			this.bullets[i] = new Bullet(bullet);
        	bullet.SetActive(false);
        }
        this.buleltVelosity = (this.transform.rotation * Vector3.forward * this.speed) + Player.Instance.forwardVelosity;
    }

    public void Shot(){
        
        foreach (var bullet in this.bullets)
        {
            if(!bullet.isActive){
                bullet.obj.SetActive(true);
                bullet.isActive = true;
                bullet.transform.position = this.transform.position;
                bullet.transform.rotation = this.transform.rotation;
                bullet.velocity = this.buleltVelosity;
                bullet.timer = this.bulletTimer;
                break;
            }
        }
    }

    void FixedUpdate()
    {
        foreach (var bullet in this.bullets)
        {
            if(bullet.isActive){
                bullet.transform.position += bullet.velocity;
                bullet.timer--;
                if(bullet.timer <= 0){
                    bullet.obj.SetActive(false);
                    bullet.isActive = false;
                }
                //攻撃確認
                foreach (var obstacle in ObstacleManager.Instance.obstacles)
                {
                    // if(enemy.isDead) continue;
                    var isHit = false;
                    foreach (var coll in obstacle.colliders)
                    {
                        var hitRange = coll.size / 2f;
                        var range = obstacle.transform.position + coll.center - bullet.transform.position;
                        if(Mathf.Abs(range.x) <= hitRange.x 
                        && Mathf.Abs(range.y) <= hitRange.y
                        && Mathf.Abs(range.z) <= hitRange.z){
                            isHit = true;
                            break;
                        }
                    }
                    if(isHit){
                        bullet.obj.SetActive(false);
                        bullet.isActive = false;
                        break;
                    }
                }
            }
        }
    }

    public void PositionReset(){
        var positionResetRange = Player.Instance.positionResetRange;
        foreach (var bullet in this.bullets)
        {
            bullet.transform.position -= Vector3.forward * positionResetRange;
        }
    }
}
