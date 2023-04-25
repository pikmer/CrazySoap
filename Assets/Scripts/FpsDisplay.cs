using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class FpsDisplay : MonoBehaviour {
	public Text text;
	int frameCount;
	float prevTime;
	float time;
	
	public Text fixedText;
	int fixedFrameCount;
	float FixedPrevTime;
	float FixedTime;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {

		frameCount++;
		time += Time.deltaTime;

		if (time >= 0.5f) {
			text.text = "fps " + ((float)frameCount / time).ToString("F0");

			frameCount = 0;
			time = 0;
		}
	}
	
	// Update is called once per frame
	void FixedUpdate () {
		// fixedFrameCount++;
		// FixedTime += Time.fixedDeltaTime;

		// if (FixedTime >= 0.5f) {
		// 	fixedText.text = "Ph:" + ((float)fixedFrameCount / FixedTime).ToString("F0");

		// 	fixedFrameCount = 0;
		// 	FixedTime = 0;
		// }
	}
}
