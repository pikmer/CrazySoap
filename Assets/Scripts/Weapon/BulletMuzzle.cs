using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMuzzle : MonoBehaviour
{
    public GameObject bulletPrefab;
    protected Bullet[] bullets;
    int bulletsLength = 9;

    public float speed = 0.3f; 
    Vector3 buleltVelosity = new Vector3(0, 0, 0.6f);
    int bulletTimer = 70;

    static List<BulletMuzzle> muzzles = new List<BulletMuzzle>();

    Vector3 bulletSize = Vector3.one * 0.2f;

    void Start()
    {
        this.bullets = new Bullet[this.bulletsLength];
        for(int i=0; i < this.bullets.Length; i++){
        	GameObject bullet = Instantiate(this.bulletPrefab, Vector3.zero, Quaternion.identity);
			this.bullets[i] = new Bullet(bullet);
        	bullet.SetActive(false);
        }
        this.buleltVelosity = (this.transform.rotation * Vector3.forward * this.speed) + Player.Instance.forwardVelosity;

        muzzles.Add(this);
    }

    public void Shot(){
        
        foreach (var bullet in this.bullets)
        {
            if(!bullet.isActive){
                bullet.obj.SetActive(true);
                bullet.isActive = true;
                bullet.transform.position = this.transform.position + this.transform.right * Random.Range(-0.1f, 0.1f);
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
                foreach (var obstacleArray in  ObstacleManager.Instance.obstacles)
                {
                    foreach (var obstacle in obstacleArray)
                    {
                        if(obstacle.isActive && obstacle.flyCount <= 0){
                            var isHit = false;
                            foreach (var coll in obstacle.colliders)
                            {
                                if(GameManager.CheckBoxColl(bullet.transform.position, this.bulletSize
                                , obstacle.transform.position + coll.center, coll.size)){
                                    isHit = true;
                                    break;
                                }
                            }
                            if(isHit){
                                obstacle.Damage(1);
                                bullet.obj.SetActive(false);
                                bullet.isActive = false;
                                break;
                            }
                        }
                    }
                }
            }
        }
    }
    
    public static void PositionReset(){
        var positionResetRange = Player.Instance.positionResetRange;
        foreach (var muzzle in muzzles)
        {
            foreach (var bullet in muzzle.bullets)
            {
                bullet.transform.position -= Vector3.forward * positionResetRange;
            }
        }
    }
}
