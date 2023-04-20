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

    [SerializeField] Slider slider;

    [SerializeField] GameObject sliderUiObj;

    //セーブ関係
    string saveAudioVolumeKey = "AudioVolume";

    //UI関係
    bool isUiOver;
    int uiOverCount;
    [SerializeField] RectTransform sliderTrf;

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

        this.slider.value = this.audioSource.volume;
        this.sliderTrf.anchoredPosition3D += Vector3.right * 205f;
    }

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

        if(this.isUiOver){
            if(this.sliderTrf.anchoredPosition3D.x > 0){
                this.sliderTrf.anchoredPosition3D += Vector3.left * 15f;
                if(this.sliderTrf.anchoredPosition3D.x < 0){
                    this.sliderTrf.anchoredPosition3D += Vector3.zero;
                }
            }
        }else{
            if(this.sliderTrf.anchoredPosition3D.x < 205f){
                this.sliderTrf.anchoredPosition3D += Vector3.right * 15f;
                if(this.sliderTrf.anchoredPosition3D.x > 205f){
                    this.sliderTrf.anchoredPosition3D += Vector3.right * 205f;
                }
            }
        }
    }

    public void PlaySE(int index)
    {
        this.audioPlayList[index] = true;
    }

    //音量
    public void VolumeChange(){
        if(Application.platform == RuntimePlatform.Android || Application.platform == RuntimePlatform.IPhonePlayer) return;
        this.audioSource.volume = this.slider.value;
    }
    public void VolumeSet(){
        // AudioManager.Instance.PlaySE(7);
        PlayerPrefs.SetFloat(this.saveAudioVolumeKey, this.audioSource.volume);
    }

    public void UiOver(bool value){
        this.isUiOver = value;
    }
}
