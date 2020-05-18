using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class main : MonoBehaviour
{
    public static main _m;
    public float maxpd;
    public GameObject plr;
    [Header("Ingame things lists")]
    public List<BInit> blocks = new List<BInit>();

    [Header("Loading screen")]
    public Text loadtxt;
    public RectTransform loadimg;
    public RectTransform loadfill;
    
    [System.Serializable]
    public class BInit
    {
        public string name;
        public Sprite spbase;
        public Sprite spleft;
        public Sprite spright;
        public Sprite spup;
        public Sprite spdown;
    }
    public void load_set(float perc)
    {
        loadfill.sizeDelta = new Vector3(loadimg.sizeDelta.x * perc, 0);
        loadfill.anchoredPosition = new Vector3(loadimg.sizeDelta.x * perc / 2f, 0);
    }
    public void load_txt(string txt)
    {
        loadtxt.transform.parent.gameObject.SetActive(true);
        loadfill.gameObject.SetActive(true);
        loadtxt.text = txt;
    }
    public void load_stop()
    {
        loadtxt.transform.parent.gameObject.SetActive(false);
        loadimg.gameObject.SetActive(false);
    }
    // Start is called before the first frame update
    void Awake()
    {
        _m = this;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
