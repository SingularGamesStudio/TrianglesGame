using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;

public class main : MonoBehaviour
{
    public static main _m;
    public float MinRenderDistance;
    public float MaxRenderDistance;
    public int FrameNum = 0;
    public PlanetInit PlanetNow;
    public GameObject plr;
    [Header("Ingame things lists")]
    public List<BInit> Blocks = new List<BInit>();

    [Header("Loading screen")]
    public Text LoadTxt;
    public RectTransform LoadImg;
    public RectTransform LoadFill;
    [System.Serializable]
    public class LightedBlock : IComparable
    {
        public float Lightness;
        public Block b;
        public int k;
        public LightedBlock(float _Lightness, Block _b)
        {
            Lightness = _Lightness;
            b = _b;
            k = b.Params.ObjectName;
        }
        public int CompareTo(object o)
        {
            LightedBlock p = o as LightedBlock;
            if (this.Lightness != p.Lightness) {
                return this.Lightness.CompareTo(p.Lightness);
            } else if (this.k != p.k) {
                return this.k.CompareTo(p.k);
            }
            return 0;
        }
    }

    [System.Serializable]
    public class BInit
    {
        public string Name;
        public float BasicLight;
        public float LightPenetr;
        public Sprite SpBase;
        public Sprite SpLeft;
        public Sprite SpRight;
        public Sprite SpUp;
        public Sprite SpDown;
    }
    public void loadSet(float perc)
    {
        LoadFill.sizeDelta = new Vector3(LoadImg.sizeDelta.x * perc, 0);
        LoadFill.anchoredPosition = new Vector3(LoadImg.sizeDelta.x * perc / 2f, 0);
    }
    public void loadTxtSet(string txt)
    {
        LoadTxt.transform.parent.gameObject.SetActive(true);
        LoadFill.gameObject.SetActive(true);
        LoadTxt.text = txt;
    }
    public void loadStop()
    {
        LoadTxt.transform.parent.gameObject.SetActive(false);
        LoadImg.gameObject.SetActive(false);
    }
    void Awake()
    {
        _m = this;
    }

    void Update()
    {
        FrameNum++;
    }
}
