﻿using System.Collections;
using System.Collections.Generic;
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
    List<List<block.blockInit>> all = new List<List<block.blockInit>>();

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
    int loadstate = 0;
    public List<List<block.blockInit>> alltmp = new List<List<block.blockInit>>();
    [HideInInspector]
    public int maxlen;
    [HideInInspector]
    public int cnt;
    [HideInInspector]
    public block.blockInit[] temp;
    [HideInInspector]
    public int lvnow = 0;
    void Start()
    {
        int sh = 0;
        for (int i = 0; i < seed.Length; i++) {
            sh = mod((sh * 239 + ((int)seed[i]) * 42), 115249);
        }
        rnd = new System.Random(sh);
        for (int i = 0; i <= depth; i++)
            all.Add(new List<block.blockInit>());
        for (int i = 0; i <= depth; i++)
            alltmp.Add(new List<block.blockInit>());
        maxlen = (depth - 1) * 2 + 2;
        cnt = 0;
        main._m.load_txt("Loading planet");
        main._m.load_set(0);
        temp = new block.blockInit[maxlen * 2 + 3];
        lvnow = depth;
        loadstate = 1;
    }
    int layernow;
    int lastedge;
    void Update()
    {
        if (loadstate == 1) {
            
            bool endc = true;
            main._m.load_set(((float)(depth-lvnow))/depth/2f);
            for (int lv = lvnow; lv >= 0; lv--) {
                Debug.Log(lv);
                int len = lv + (depth - lv - 1) * 2 + 2;
                block.blockInit left = null;
                for (int i = -len; i <= len; i++) {
                    Vector3 pos = new Vector3(i * 0.11f, lv * 0.2f + 0.1f, 0);
                    block.blockInit bn = new block.blockInit();
                    bn.pos = pos;
                    bn.baseb = baseb;
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
                        all[lv].Add(bn);
                        bn.layer = lv;
                        bn.reslayer = lv;
                    } else {
                        all[lv + ((abs(i) - lv) / 2)].Add(bn);
                        bn.layer = lv + ((abs(i) - lv) / 2);
                        bn.reslayer = lv + ((abs(i) - lv) / 2);
                    }
                }
                if (lv < lvnow - 10) {
                    lvnow = lv-1;
                    endc = false;
                    break;
                }
            }
            if (endc) {
                loadstate++;
                lvnow = 0;
                
            }
        }
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
                        all[lv].Add(bn);
                        bn.layer = lv;
                        bn.reslayer = lv;
                    } else {
                        if (i > 0) {
                            alltmp[lv + ((abs(i) - lv) / 2)].Add(bn);
                            bn.layer = lv + ((abs(i) - lv) / 2);
                            bn.reslayer = lv + ((abs(i) - lv) / 2);
                        } else {
                            all[lv + ((abs(i) - lv) / 2)].Add(bn);
                            bn.layer = lv + ((abs(i) - lv) / 2);
                            bn.reslayer = lv + ((abs(i) - lv) / 2);
                        }
                    }
                }
                if (lv > lvnow + 10) {
                    lvnow = lv+1;
                    endc = false;
                    break;
                }
            }
            if (endc) {
                loadstate++;
                lvnow = 0;
            }
        }
        if (loadstate == 3) {
            for (int i = 0; i < depth; i++) {
                for (int j = alltmp[i].Count - 1; j >= 0; j--) {
                    all[i].Add(alltmp[i][j]);
                }
                foreach (block.blockInit b in all[i]) {
                    b.binact = true;
                }
            }
            main._m.load_set(0);
            main._m.load_txt("Loading blocks");
            loadstate++;
            layernow = 0;
            lvnow = depth - 1;
            lastedge = depth - 1;
        }
        
        if (loadstate == 4) {
            
            bool endc = true;
            main._m.load_set(((float)(depth-1-lvnow))/(depth-1));
            for (int i = lvnow; i >= 0; i--) {
                if (depth - i > layers[layernow].maxdp) {
                    layernow++;
                    lastedge = i;
                }
                for (int j = 0; j < all[i].Count; j++) {
                    int rn = 0;
                    bool nlv = false;
                    if (all[i][j].up.num != 0) {
                        rn++;
                        if (all[i][j].up.reslayer >= lastedge)
                            nlv = true;
                    }
                    if (all[i][j].down.num != 0) {
                        rn++;
                        if (all[i][j].down.reslayer >= lastedge)
                            nlv = true;
                    }
                    if (all[i][j].right.num != 0) {
                        rn++;
                        if (all[i][j].right.reslayer >= lastedge)
                            nlv = true;
                    }
                    if (all[i][j].left.num != 0) {
                        rn++;
                        if (all[i][j].left.reslayer >= lastedge)
                            nlv = true;
                    }
                    int rn1 = 0;
                    if (rn != 0) {
                        if (!nlv)
                            rn1 = rnd.Next(stonesshuffled * rn);
                        else rn1 = rnd.Next(10 * rn);
                    }
                    if (rn == 0 || rn1 < rn) {
                        rn1 = rnd.Next(layers[layernow].basicblocks.Count);
                        all[i][j].set(layers[layernow].basicblocks[rn1]);
                    } else {
                        rn1 %= rn;
                        rn1++;
                        if (all[i][j].up.num != 0) {
                            rn1--;
                            if (rn1 == 0) {
                                all[i][j].set(all[i][j].up.num);
                                all[i][j].reslayer = all[i][j].up.reslayer;
                            }
                        }
                        if (all[i][j].down.num != 0) {
                            rn1--;
                            if (rn1 == 0) {
                                all[i][j].set(all[i][j].down.num);
                                all[i][j].reslayer = all[i][j].down.reslayer;
                            }
                        }
                        if (all[i][j].right.num != 0) {
                            rn1--;
                            if (rn1 == 0) {
                                all[i][j].set(all[i][j].right.num);
                                all[i][j].reslayer = all[i][j].right.reslayer;
                            }
                        }
                        if (all[i][j].left.num != 0) {
                            rn1--;
                            if (rn1 == 0) {
                                all[i][j].set(all[i][j].left.num);
                                all[i][j].reslayer = all[i][j].left.reslayer;
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
                loadstate++;
                main._m.load_set(0);
                main._m.load_txt("Loading Caves");
            }
        }
        if (loadstate == 5) {
            /*int tmp = rnd.Next(depth);
            Cavegen(tmp, 1000);
            tmp = rnd.Next(depth);
            Cavegen(tmp, 500);
            tmp = rnd.Next(depth);
            Cavegen(tmp, 500);
            tmp = rnd.Next(depth);
            Cavegen(tmp, 100);
            CavesInit();*/
            for (int i = 0; i < depth; i++) {
                foreach (block.blockInit b in all[i]) {
                    if (Vector3.Distance(b.pos, plr.transform.position) < main._m.maxpd) {
                        b.CreateInstance();
                    }
                }
                foreach (block.blockInit b in all[i]) {
                    if (Vector3.Distance(b.pos, plr.transform.position) < main._m.maxpd && (b.left.cn == null || b.right.cn == null || b.up.cn == null || b.down.cn == null)) {
                        b.cn.watch = true;
                    }
                }
            }
            loadstate++;
            main._m.load_stop();
        }
    }
    void Cavegen(int layer, int size)
    {
        block.blockInit bn = all[layer][rnd.Next(all[layer].Count)];
        bn.binact = false;
        for(; size>0; size--) {
            int temp = rnd.Next(4);
            if (temp == 0) {
                if (bn.up.active) {
                    bn = bn.up;
                    bn.binact = false;
                } else size++;
            } else if (temp == 1) {
                if (bn.down.active) {
                    bn = bn.down;
                    bn.binact = false;
                } else size++;
            }
            else if(temp == 2) {
                if (bn.right.active) {
                    bn = bn.right;
                    bn.binact = false;
                } else size++;
            }
            else if(temp == 3) {
                if (bn.left.active) {
                    bn = bn.left;
                    bn.binact = false;
                } else size++;
            }
        }
        
    }
    void CavesInit()
    {
        for (int i = 0; i < depth; i++)
            foreach (block.blockInit b in all[i]) {
                int cnt = 0;
                {
                    if (b.left.left!=null && !b.left.left.binact)
                        cnt++;
                    if (b.right.right != null && !b.right.right.binact)
                        cnt++;
                    if (b.up.left != null && !b.up.left.binact)
                        cnt++;
                    if (b.up.right != null && !b.up.right.binact)
                        cnt++;
                    if (b.down.left != null && !b.down.left.binact)
                        cnt++;
                    if (b.down.right != null && !b.down.right.binact)
                        cnt++;
                    if (!b.right.binact)
                        cnt++;
                    if (!b.left.binact)
                        cnt++;
                    if (!b.down.binact)
                        cnt++;
                    if (!b.up.binact)
                        cnt++;
                    if (b.flippedy) {
                        if (b.up.right != null && b.up.right.right != null && !b.up.right.right.binact)
                            cnt++;
                        if (b.up.left != null && b.up.left.left != null && !b.up.left.left.binact)
                            cnt++;
                    } else {
                        if (b.down.right != null && b.down.right.right != null && !b.down.right.right.binact)
                            cnt++;
                        if (b.down.left != null && b.down.left.left != null && !b.down.left.left.binact)
                            cnt++;
                    }
                }
                //if (!b.binact || cnt>=12-caveswidth)
                //    b.UnAct();
            }
    }
}