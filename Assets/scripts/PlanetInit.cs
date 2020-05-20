using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using UnityEngine;
using UnityEngine.UI;

public class PlanetInit : MonoBehaviour
{
    //[Header(string title)]
    //[Range(float min, float max)]
    //[Tooltip(string tip)]
    [Header("System variables")]
    public GameObject baseb;
    System.Random rnd;
    public GameObject plr;
    

    [Header("Planet parameters")]
    public List<layer> layers = new List<layer>();
    [Range(0, 1000)]
    public int depth;
    [Tooltip("Any string")]
    public string seed;
    [Range(0, 5)]
    public int caveswidth;
    [Tooltip("Value grows => Count of stones grows")]
    public int stonesshuffled;
    public bool genall;
    public int mountainsheight;
    public float cavescoef;
    public float cavesnumcoef;
    List<List<block.blockInit>> pstruct = new List<List<block.blockInit>>();
    int loadstate = 0;
    int mod(int a, int b)
    {
        return ((a % b) + b) % b;
    }
    int abs(int a)
    {
        if (a < 0)
            return -a;
        return a;
    }
    [System.Serializable]
    public class layer
    {
        public List<int> basicblocks;
        public List<int> metals;
        public int maxdp;
    };

    //temporary variables
    List<List<block.blockInit>> ptmp = new List<List<block.blockInit>>();
    List<block.blockInit> abl = new List<block.blockInit>();
    List<int> allcaves = new List<int>();
    int maxlen;
    int cnt;
    block.blockInit[] temp;
    int lvnow = 0;
    int inow;
    int layernow;
    int lastedge;
    bool fst;
    int lastx = 0;
    void Start()
    {
        int sh = 0;
        for (int i = 0; i < seed.Length; i++) {
            sh = mod((sh * 239 + ((int)seed[i]) * 42), 115249);
        }
        rnd = new System.Random(sh);
        for (int i = 0; i <= depth; i++)
            pstruct.Add(new List<block.blockInit>());
        for (int i = 0; i <= depth; i++)
            ptmp.Add(new List<block.blockInit>());
        maxlen = (depth - 1) * 2 + 2;
        cnt = 0;
        fst = true;
        main._m.load_txt("Loading planet");
        main._m.load_set(0);
        temp = new block.blockInit[maxlen * 2 + 3];
        lvnow = depth;
        loadstate = 1;
    }
    void Update()
    {
        if (loadstate == 1) {

            bool endc = true;
            main._m.load_set(((float)(depth - lvnow)) / depth / 2f);
            for (int lv = lvnow; lv >= 0; lv--) {
                int len = lv + (depth - lv - 1) * 2 + 2;
                block.blockInit left = null;
                for (int i = -len; i <= len; i++) {
                    Vector3 pos = new Vector3(i * 0.11f, lv * 0.2f + 0.1f, 0);
                    block.blockInit bn = new block.blockInit();
                    bn.pos = pos;
                    bn.baseb = baseb;
                    bn.par = gameObject;
                    bn.Oname = (cnt++).ToString();
                    if (mod(i + lv, 2) == 0) {
                        bn.flipy();
                    }
                    bn.left = left;
                    bn.up = temp[i + maxlen];
                    if (temp[i + maxlen] != null)
                        temp[i + maxlen].down = bn;
                    temp[i + maxlen] = bn;
                    if (left != null)
                        left.right = bn;
                    left = bn;
                    if (abs(i) < lv + 2) {
                        pstruct[lv].Add(bn);
                        bn.layer = lv;
                        bn.reslayer = lv;
                    } else {
                        if (i > 0) {
                            pstruct[lv + ((abs(i) - lv) / 2)].Add(bn);
                            bn.layer = lv + ((abs(i) - lv) / 2);
                            bn.reslayer = lv + ((abs(i) - lv) / 2);
                        } else {
                            if (mod(i + lv, 2) == 0)
                                pstruct[lv + ((abs(i) - lv) / 2)].Insert(1, bn);
                            else pstruct[lv + ((abs(i) - lv) / 2)].Insert(0, bn);
                            bn.layer = lv + ((abs(i) - lv) / 2);
                            bn.reslayer = lv + ((abs(i) - lv) / 2);
                        }
                    }
                }
                if (lv < lvnow - 10) {
                    lvnow = lv - 1;
                    endc = false;
                    break;
                }
            }
            if (endc) {
                loadstate++;
                lvnow = 0;
            }
        } else
        if (loadstate == 2) {
            bool endc = true;
            main._m.load_set(((float)lvnow) / depth / 2f + 0.5f);
            for (int lv = lvnow; lv <= depth; lv++) {
                int len = lv + (depth - lv - 1) * 2 + 2;
                block.blockInit left = null;
                for (int i = -len; i <= len; i++) {
                    Vector3 pos = new Vector3(i * 0.11f, -lv * 0.2f - 0.1f, 0);
                    block.blockInit bn = new block.blockInit();
                    bn.pos = pos;
                    bn.baseb = baseb;
                    bn.par = gameObject;
                    bn.Oname = (cnt++).ToString();
                    if (mod(i + lv, 2) == 1) {
                        bn.flipy();
                    }
                    bn.up = temp[i + maxlen];
                    if (temp[i + maxlen] != null)
                        temp[i + maxlen].down = bn;
                    temp[i + maxlen] = bn;
                    bn.left = left;
                    if (left != null)
                        left.right = bn;
                    left = bn;
                    if (abs(i) < lv + 2) {
                        if (fst)
                            pstruct[lv].Add(bn);
                        else pstruct[lv].Insert(pstruct[lv].Count - 1, bn);
                        fst = false;
                        bn.layer = lv;
                        bn.reslayer = lv;
                    } else {
                        if (i > 0) {
                            if (mod(i + lv, 2) == 0)
                                pstruct[lv + ((abs(i) - lv) / 2)].Add(bn);
                            else pstruct[lv + ((abs(i) - lv) / 2)].Insert(pstruct[lv + ((abs(i) - lv) / 2)].Count - 1, bn);
                            bn.layer = lv + ((abs(i) - lv) / 2);
                            bn.reslayer = lv + ((abs(i) - lv) / 2);
                        } else {
                            ptmp[lv + ((abs(i) - lv) / 2)].Add(bn);
                            bn.layer = lv + ((abs(i) - lv) / 2);
                            bn.reslayer = lv + ((abs(i) - lv) / 2);
                        }
                    }
                }
                if (lv > lvnow + 10) {
                    lvnow = lv + 1;
                    endc = false;
                    break;
                }
            }
            if (endc) {
                loadstate++;
                lvnow = 0;
            }
        } else
        if (loadstate == 3) {
            for (int i = 0; i < depth; i++) {
                for (int j = ptmp[i].Count - 1; j >= 0; j--) {
                    pstruct[i].Add(ptmp[i][j]);
                }
                foreach (block.blockInit b in pstruct[i]) {
                    b.binact = true;
                }
            }
            main._m.load_set(0);
            main._m.load_txt("Loading blocks");
            loadstate++;
            layernow = 0;
            lvnow = depth - 1;
            lastedge = depth - 1;
        } else
        if (loadstate == 4) {

            bool endc = true;
            main._m.load_set(((float)(depth - 1 - lvnow)) / (depth - 1));
            for (int i = lvnow; i >= 0; i--) {
                if (depth - i > layers[layernow].maxdp) {
                    layernow++;
                    lastedge = i;
                }
                for (int j = 0; j < pstruct[i].Count; j++) {
                    if (i == depth - 1) {
                        int rn1 = rnd.Next(layers[layernow].basicblocks.Count);
                        pstruct[i][j].set(layers[layernow].basicblocks[rn1]);
                    } else
                    if (j == 0) {
                        bool nlv = false;
                        int upper = (j / (2 * i + 1)) * (2 * i + 3) + 1 + (j % (2 * i + 1));
                        if (pstruct[i + 1][upper - 1].reslayer >= lastedge)
                            nlv = true;
                        if (pstruct[i + 1][upper].reslayer >= lastedge)
                            nlv = true;
                        if (pstruct[i + 1][upper + 1].reslayer >= lastedge)
                            nlv = true;
                        int rn1 = 0;
                        if (!nlv)
                            rn1 = rnd.Next(stonesshuffled * 3);
                        else rn1 = rnd.Next(10 * 3);
                        if (rn1 < 3) {
                            rn1 = rnd.Next(layers[layernow].basicblocks.Count);
                            pstruct[i][j].set(layers[layernow].basicblocks[rn1]);
                        } else {
                            rn1 %= 3;
                            if (rn1 == 0) {
                                pstruct[i][j].set(pstruct[i + 1][upper - 1].num);
                                pstruct[i][j].reslayer = pstruct[i + 1][upper - 1].reslayer;
                            }
                            if (rn1 == 1) {
                                pstruct[i][j].set(pstruct[i + 1][upper].num);
                                pstruct[i][j].reslayer = pstruct[i + 1][upper].reslayer;
                            }
                            if (rn1 == 2) {
                                pstruct[i][j].set(pstruct[i + 1][upper + 1].num);
                                pstruct[i][j].reslayer = pstruct[i + 1][upper + 1].reslayer;
                            }
                        }
                    } else if (j == pstruct[i].Count - 1) {
                        bool nlv = false;
                        if (pstruct[i][j - 1].reslayer >= lastedge)
                            nlv = true;
                        if (pstruct[i][j - 2].reslayer >= lastedge)
                            nlv = true;
                        int upper = (j / (2 * i + 1)) * (2 * i + 3) + 1 + (j % (2 * i + 1));
                        if (pstruct[i + 1][upper - 1].reslayer >= lastedge)
                            nlv = true;
                        if (pstruct[i + 1][upper].reslayer >= lastedge)
                            nlv = true;
                        if (pstruct[i + 1][upper + 1].reslayer >= lastedge)
                            nlv = true;
                        if (pstruct[i][0].reslayer >= lastedge)
                            nlv = true;
                        if (pstruct[i][1].reslayer >= lastedge)
                            nlv = true;
                        int rn1 = 0;
                        if (!nlv)
                            rn1 = rnd.Next(stonesshuffled * 7);
                        else rn1 = rnd.Next(10 * 7);
                        if (rn1 < 7) {
                            rn1 = rnd.Next(layers[layernow].basicblocks.Count);
                            pstruct[i][j].set(layers[layernow].basicblocks[rn1]);
                        } else {
                            rn1 %= 7;
                            if (rn1 == 0) {
                                pstruct[i][j].set(pstruct[i][j - 1].num);
                                pstruct[i][j].reslayer = pstruct[i][j - 1].reslayer;
                            }
                            if (rn1 == 1) {
                                pstruct[i][j].set(pstruct[i][j - 2].num);
                                pstruct[i][j].reslayer = pstruct[i][j - 2].reslayer;
                            }
                            if (rn1 == 2) {
                                pstruct[i][j].set(pstruct[i + 1][upper - 1].num);
                                pstruct[i][j].reslayer = pstruct[i + 1][upper - 1].reslayer;
                            }
                            if (rn1 == 3) {
                                pstruct[i][j].set(pstruct[i + 1][upper].num);
                                pstruct[i][j].reslayer = pstruct[i + 1][upper].reslayer;
                            }
                            if (rn1 == 4) {
                                pstruct[i][j].set(pstruct[i + 1][upper + 1].num);
                                pstruct[i][j].reslayer = pstruct[i + 1][upper + 1].reslayer;
                            }
                            if (rn1 == 5) {
                                pstruct[i][j].set(pstruct[i][0].num);
                                pstruct[i][j].reslayer = pstruct[i][0].reslayer;
                            }
                            if (rn1 == 6) {
                                pstruct[i][j].set(pstruct[i][1].num);
                                pstruct[i][j].reslayer = pstruct[i][1].reslayer;
                            }
                        }
                    } else if (j == 1) {
                        bool nlv = false;
                        if (pstruct[i][j - 1].reslayer >= lastedge)
                            nlv = true;
                        int upper = (j / (2 * i + 1)) * (2 * i + 3) + 1 + (j % (2 * i + 1));
                        if (pstruct[i + 1][upper - 1].reslayer >= lastedge)
                            nlv = true;
                        if (pstruct[i + 1][upper].reslayer >= lastedge)
                            nlv = true;
                        if (pstruct[i + 1][upper + 1].reslayer >= lastedge)
                            nlv = true;
                        int rn1 = 0;
                        if (!nlv)
                            rn1 = rnd.Next(stonesshuffled * 4);
                        else rn1 = rnd.Next(10 * 4);
                        if (rn1 < 4) {
                            rn1 = rnd.Next(layers[layernow].basicblocks.Count);
                            pstruct[i][j].set(layers[layernow].basicblocks[rn1]);
                        } else {
                            rn1 %= 4;
                            if (rn1 == 0) {
                                pstruct[i][j].set(pstruct[i][j - 1].num);
                                pstruct[i][j].reslayer = pstruct[i][j - 1].reslayer;
                            }
                            if (rn1 == 1) {
                                pstruct[i][j].set(pstruct[i + 1][upper - 1].num);
                                pstruct[i][j].reslayer = pstruct[i + 1][upper - 1].reslayer;
                            }
                            if (rn1 == 2) {
                                pstruct[i][j].set(pstruct[i + 1][upper].num);
                                pstruct[i][j].reslayer = pstruct[i + 1][upper].reslayer;
                            }
                            if (rn1 == 3) {
                                pstruct[i][j].set(pstruct[i + 1][upper + 1].num);
                                pstruct[i][j].reslayer = pstruct[i + 1][upper + 1].reslayer;
                            }
                        }
                    } else if (j == pstruct[i].Count - 2) {
                        bool nlv = false;
                        if (pstruct[i][j - 1].reslayer >= lastedge)
                            nlv = true;
                        if (pstruct[i][j - 2].reslayer >= lastedge)
                            nlv = true;
                        int upper = (j / (2 * i + 1)) * (2 * i + 3) + 1 + (j % (2 * i + 1));
                        if (pstruct[i + 1][upper - 1].reslayer >= lastedge)
                            nlv = true;
                        if (pstruct[i + 1][upper].reslayer >= lastedge)
                            nlv = true;
                        if (pstruct[i + 1][upper + 1].reslayer >= lastedge)
                            nlv = true;
                        if (pstruct[i][0].reslayer >= lastedge)
                            nlv = true;
                        int rn1 = 0;
                        if (!nlv)
                            rn1 = rnd.Next(stonesshuffled * 6);
                        else rn1 = rnd.Next(10 * 6);
                        if (rn1 < 6) {
                            rn1 = rnd.Next(layers[layernow].basicblocks.Count);
                            pstruct[i][j].set(layers[layernow].basicblocks[rn1]);
                        } else {
                            rn1 %= 6;
                            if (rn1 == 0) {
                                pstruct[i][j].set(pstruct[i][j - 1].num);
                                pstruct[i][j].reslayer = pstruct[i][j - 1].reslayer;
                            }
                            if (rn1 == 1) {
                                pstruct[i][j].set(pstruct[i][j - 2].num);
                                pstruct[i][j].reslayer = pstruct[i][j - 2].reslayer;
                            }
                            if (rn1 == 2) {
                                pstruct[i][j].set(pstruct[i + 1][upper - 1].num);
                                pstruct[i][j].reslayer = pstruct[i + 1][upper - 1].reslayer;
                            }
                            if (rn1 == 3) {
                                pstruct[i][j].set(pstruct[i + 1][upper].num);
                                pstruct[i][j].reslayer = pstruct[i + 1][upper].reslayer;
                            }
                            if (rn1 == 4) {
                                pstruct[i][j].set(pstruct[i + 1][upper + 1].num);
                                pstruct[i][j].reslayer = pstruct[i + 1][upper + 1].reslayer;
                            }
                            if (rn1 == 5) {
                                pstruct[i][j].set(pstruct[i][0].num);
                                pstruct[i][j].reslayer = pstruct[i][0].reslayer;
                            }
                        }
                    } else {
                        bool nlv = false;
                        if (pstruct[i][j - 1].reslayer >= lastedge)
                            nlv = true;
                        if (pstruct[i][j - 2].reslayer >= lastedge)
                            nlv = true;
                        int upper = (j / (2 * i + 1)) * (2 * i + 3) + 1 + (j % (2 * i + 1));
                        if (pstruct[i + 1][upper - 1].reslayer >= lastedge)
                            nlv = true;
                        if (pstruct[i + 1][upper].reslayer >= lastedge)
                            nlv = true;
                        if (pstruct[i + 1][upper + 1].reslayer >= lastedge)
                            nlv = true;
                        int rn1 = 0;
                        if (!nlv)
                            rn1 = rnd.Next(stonesshuffled * 5);
                        else rn1 = rnd.Next(10 * 5);
                        if (rn1 < 5) {
                            rn1 = rnd.Next(layers[layernow].basicblocks.Count);
                            pstruct[i][j].set(layers[layernow].basicblocks[rn1]);
                        } else {
                            rn1 %= 5;
                            if (rn1 == 0) {
                                pstruct[i][j].set(pstruct[i][j - 1].num);
                                pstruct[i][j].reslayer = pstruct[i][j - 1].reslayer;
                            }
                            if (rn1 == 1) {
                                pstruct[i][j].set(pstruct[i][j - 2].num);
                                pstruct[i][j].reslayer = pstruct[i][j - 2].reslayer;
                            }
                            if (rn1 == 2) {
                                pstruct[i][j].set(pstruct[i + 1][upper - 1].num);
                                pstruct[i][j].reslayer = pstruct[i + 1][upper - 1].reslayer;
                            }
                            if (rn1 == 3) {
                                pstruct[i][j].set(pstruct[i + 1][upper].num);
                                pstruct[i][j].reslayer = pstruct[i + 1][upper].reslayer;
                            }
                            if (rn1 == 4) {
                                pstruct[i][j].set(pstruct[i + 1][upper + 1].num);
                                pstruct[i][j].reslayer = pstruct[i + 1][upper + 1].reslayer;
                            }
                        }
                    }
                }
                if (i < lvnow - 5) {
                    lvnow = i - 1;
                    endc = false;
                    break;
                }
            }
            if (endc) {
                inow = 0;
                loadstate++;
                main._m.load_set(0);
                main._m.load_txt("Loading Surface");
            }
        } else
        if (loadstate == 5) {
            if (inow == 0) {
                for (int i = depth - mountainsheight; i < depth; i++) {
                    foreach (block.blockInit b in pstruct[i]) {
                        b.binact = false;
                    }
                }
            }
            int lnow = depth - mountainsheight - 1;
            int[] heights = new int[2 * lnow + 1];

            {
                int i = inow;
                for (int j = 0; j < 2 * lnow + 1; j++) {
                    heights[j] = -1;
                }
                heights[0] = 0;
                heights[2 * lnow] = 0;
                createsurface(ref heights, 0, 2 * lnow);
                for (int j = 0; j < 2 * lnow + 1; j++) {
                    buildsurface(heights[j], lnow, i * (2 * lnow + 1) + j);
                }
                inow++;
            }
            main._m.load_set(((float)inow) / 6);
            if (inow == 6) {
                main._m.load_set(0);
                main._m.load_txt("Loading Caverns");
                loadstate++;
                lvnow = 0;
            }
        } else
        if (loadstate == 6) {
            if (lvnow == 0) {
                int lnow = depth - mountainsheight * 2 - 1;
                int pcnt = Mathf.RoundToInt(lnow * lnow * cavesnumcoef);
                int psz = Mathf.RoundToInt(lnow * lnow * cavescoef);
                for (int i = 0; i < pcnt; i++) {
                    allcaves.Add(rnd.Next(psz));
                }
                for (int i = 0; i < lnow; i++) {
                    foreach (block.blockInit b in pstruct[i]) {
                        abl.Add(b);
                    }
                }
                allcaves.Sort();
                allcaves.Add(psz);
            }
            bool endd = true;
            for (int i = lvnow; i < allcaves.Count; i++) {
                Cavegen(abl[rnd.Next(abl.Count)], allcaves[i] - lastx);
                lastx = allcaves[i];
                if (i > lvnow + 5) {
                    lvnow = i++;
                    endd = false;
                    break;
                }
            }
            if (endd) {
                main._m.load_set(0);
                main._m.load_txt("Polishing edges");
                loadstate++;
            }
        } else
        if (loadstate == 7) {
            CavesInit();
            main._m.load_set(0);
            main._m.load_txt("Instancing");
            loadstate++;
        } else
        if (loadstate == 8) {
            for (int i = 0; i <= depth; i++) {
                foreach (block.blockInit b in pstruct[i]) {
                    if (Vector3.Distance(b.pos, plr.transform.position) < main._m.maxpd || genall) {
                        b.CreateInstance();
                        if (!genall)
                            b.cn.watch = true;
                    }
                }
            }
            loadstate++;
            main._m.load_stop();
        } else {
            if (loadstate > 10) {
                if (transform.childCount == 0) {
                    for (int i = 0; i <= depth; i++) {
                        foreach (block.blockInit b in pstruct[i]) {
                            if (Vector3.Distance(b.pos, plr.transform.position) < main._m.maxpd) {
                                b.CreateInstance();
                                b.cn.watch = true;
                            }
                        }
                    }
                }
            }
            loadstate++;
        }
    }
    
    void Cavegen(block.blockInit bn, int size)
    { 
        bn.binact = false;
        for(; size>0; size--) {
            int temp = rnd.Next(4);
            if (temp == 0) {
                if (bn.up != null) {
                    bn = bn.up;
                    bn.binact = false;
                } else size++;
            } else if (temp == 1) {
                if (bn.down != null) {
                    bn = bn.down;
                    bn.binact = false;
                } else size++;
            }
            else if(temp == 2) {
                if (bn.right != null) {
                    bn = bn.right;
                    bn.binact = false;
                } else size++;
            }
            else if(temp == 3) {
                if (bn.left != null) {
                    bn = bn.left;
                    bn.binact = false;
                } else size++;
            }
        }
        
    }
    void buildsurface(int d, int i, int j)
    {
        if (d == 0)
            return;
        if (d > 0) {
                pstruct[i][j].binact = false;
            if (j % (2 * i + 1) == 0 || j % (2 * i + 1)==2*i)
                return;
            int upper = (j / (2 * i + 1)) * (2 * i - 1) - 1 + (j % (2 * i + 1));
            buildsurface(d - 1, i-1, upper);
        } else {
            pstruct[i][j].binact = true;
            int upper = (j / (2 * i + 1)) * (2 * i + 3) + 1 + (j % (2 * i + 1));
            buildsurface(d + 1, i + 1, upper);
        }
    }
    private void createsurface(ref int[] he, int l, int r)
    {
        if (r - l > 1) {
            he[(r + l) / 2] = Mathf.Min(mountainsheight, Mathf.Max(-mountainsheight, (he[r] + he[l]) / 2 + getrnd(r-l)));
            createsurface(ref he, l, (l + r) / 2);
            createsurface(ref he, (l + r) / 2, r);
        }
    }
    int getrnd(int sz)
    {
        return rnd.Next(2*sz) - sz;
    }
    void CavesInit()
    {
        for (int i = 0; i < depth; i++)
            foreach (block.blockInit b in pstruct[i]) {
                int cnt1 = 0;
                {
                    if (b.left.left!=null && !b.left.left.binact)
                        cnt1++;
                    if (b.right.right != null && !b.right.right.binact)
                        cnt1++;
                    if (b.up.left != null && !b.up.left.binact)
                        cnt1++;
                    if (b.up.right != null && !b.up.right.binact)
                        cnt1++;
                    if (b.down.left != null && !b.down.left.binact)
                        cnt1++;
                    if (b.down.right != null && !b.down.right.binact)
                        cnt1++;
                    if (!b.right.binact)
                        cnt1++;
                    if (!b.left.binact)
                        cnt1++;
                    if (!b.down.binact)
                        cnt1++;
                    if (!b.up.binact)
                        cnt1++;
                    if (b.flippedy) {
                        if (b.up.right != null && b.up.right.right != null && !b.up.right.right.binact)
                            cnt1++;
                        if (b.up.left != null && b.up.left.left != null && !b.up.left.left.binact)
                            cnt1++;
                    } else {
                        if (b.down.right != null && b.down.right.right != null && !b.down.right.right.binact)
                            cnt1++;
                        if (b.down.left != null && b.down.left.left != null && !b.down.left.left.binact)
                            cnt1++;
                    }
                }
                if (cnt1>=12-caveswidth)
                    b.binact = false;
                if (cnt1 <= caveswidth)
                    b.binact = true;
            }
    }
}
