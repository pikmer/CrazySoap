using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance;

    [SerializeField]
    AudioClip[] seList;

    bool[] audioPlayList;
    int[] audioInterval;
    int AudioInterval = 4;

    public AudioSource audioSource;

    public Slider slider;

    public GameObject sliderUiObj;

    //セーブ関係
    string saveAudioVolumeKey = "AudioVolume";

    void Awake()
    {
        Instance = this;

		//スマホ対応
		if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer){
			this.sliderUiObj.SetActive(false);
            this.audioSource.volume = 1f;
		}
		//PC対応
        else{
            //音量セーブがあれば取得
            if(PlayerPrefs.HasKey(this.saveAudioVolumeKey)){
                float saveAudioVolume = PlayerPrefs.GetFloat(this.saveAudioVolumeKey, 0.2f);
                this.audioSource.volume = saveAudioVolume;
            }
        }

        //プレイリスト作成
        this.audioPlayList = new bool[this.seList.Length];
        this.audioInterval = new int[this.seList.Length];

        // this.slider.value = this.audioSource.volume;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        for (int i = 0; i < this.audioPlayList.Length; i++)
        {
            if(this.audioInterval[i] <= 0){
                if(this.audioPlayList[i]){
                    this.audioSource.PlayOneShot(this.seList[i]);
                    this.audioInterval[i] = this.AudioInterval - 1;
                }
                this.audioPlayList[i] = false;
            }else{
                this.audioInterval[i]--;
            }
        }
    }

    public void PlaySE(int index)
    {
        this.audioPlayList[index] = true;
    }

    //音量
    public void VolumeSet(){
        if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) return;
        this.audioSource.volume = this.slider.value;
    }
    public void VolumeCheck(){
        // AudioManager.Instance.PlaySE(7);
        PlayerPrefs.SetFloat(this.saveAudioVolumeKey, this.audioSource.volume);
    }
}
