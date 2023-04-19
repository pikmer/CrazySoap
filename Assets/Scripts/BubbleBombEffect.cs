using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BubbleBombEffect : MonoBehaviour
{
    public static BubbleBombEffect Instance;

    ParticleSystem[] effects = new ParticleSystem[3];
    public GameObject prefab;
    int playIndex = 0;

    void Start()
    {
        Instance = this;
        
        //アイテム作成
        for(int i=0; i < this.effects.Length; i++){
        	GameObject effect = Instantiate(this.prefab, Vector3.zero, Quaternion.identity);
			effect.transform.SetParent(this.transform);
			this.effects[i] = effect.GetComponent<ParticleSystem>();
        }
    }

    public void Play(Vector3 position)
    {
        this.effects[this.playIndex].Play();
        this.effects[this.playIndex].transform.position = position;
        this.playIndex++;
        if(this.playIndex >= this.effects.Length){
            this.playIndex = 0;
        }
    }

    public void PositionReset(){
        var positionResetRange = Player.Instance.positionResetRange;
		foreach (var effect in this.effects)
		{
            if(effect.isPlaying){
                effect.transform.position -= Vector3.forward * positionResetRange;
            }
        }
    }

    public void Pause(){
		foreach (var effect in this.effects)
		{
			if(effect.isPlaying) effect.Pause();
		}
    }

    public void Resume(){
		foreach (var effect in this.effects)
		{
			if(effect.isPaused) effect.Play();
		}
    }
}
