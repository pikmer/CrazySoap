using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Counter : MonoBehaviour
{
    static List<Counter> counters = new List<Counter>();

    int index = 0;
    [HideInInspector][System.NonSerialized]
    public float value;

    public RectTransform dotsParent;
    Transform[] dots = new Transform[10];
    
    RectTransform rectTrf;
    Vector3 hidePosition = new Vector3(-220, 0, 0);

    void Start()
    {
        for (int i = 0; i < this.dots.Length; i++)
        {
            this.dots[i] = this.dotsParent.GetChild(i);
        }

        this.index = counters.Count;
        counters.Add(this);

        this.rectTrf = this.transform.GetComponent<RectTransform>();
    }

    void FixedUpdate(){
        if(this.value == 0){
            if(this.rectTrf.anchoredPosition3D != hidePosition){
                this.rectTrf.anchoredPosition3D = hidePosition;
            }
        }else{
            int point = (int)(this.value * 10f);

            for (int i = 0; i < this.dots.Length; i++)
            {
                var dot = this.dots[i];
                if(i == point){
                    this.dots[i].localScale = new Vector3(this.value * 10f - (float)i, 1, 1);
                }else if(i < point){
                    this.dots[i].localScale = new Vector3(1, 1, 1);
                }else{
                    this.dots[i].localScale = new Vector3(0, 1, 1);
                }
            }

            var pos = new Vector3(10, 10 + 60 * this.index, 0);
            if(this.rectTrf.anchoredPosition3D != pos){
                this.rectTrf.anchoredPosition3D = pos;
            }
        }
        this.value = 0;
    }

    static public void Display(float value){
        foreach (var counter in counters)
        {
            if(counter.value == 0){
                counter.value = value;
                break;
            }
        }
    }
}
