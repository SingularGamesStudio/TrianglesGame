using System;
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
    public GameObject BaseBlock;
    System.Random rnd;
    public GameObject plr;
    

    [Header("Planet parameters")]
    public List<Layer> Layers = new List<Layer>();
    [Range(0, 1000)]
    public int Depth;
    [Tooltip("Any string")]
    public string Seed;
    [Range(0, 5)]
    public int CavesWidth;
    [Tooltip("Value grows => Count of stones grows")]
    public int StonesShuffled;
    public bool RenderAll;
    public int MountainsHeight;
    public float CavesSize;
    public float CavesNumber;
    List<List<Block.BlockInit>> PlanetStruct = new List<List<Block.BlockInit>>();
    SortedSet<LightedBlock> ls = new SortedSet<LightedBlock>();
    public class LightedBlock: IComparable
    {
        public float Lightness;
        public Block.BlockInit b;
        public int k;
        public LightedBlock(Block.BlockInit _b)
        {
            Lightness = _b.Lightness;
            b = _b;
            k = b.ObjectName;
        }
        public LightedBlock(Block.BlockInit _b, float _l)
        {
            Lightness = _l;
            b = _b;
            k = b.ObjectName;
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
    int LoadState = 0;
    int mod(int a, int b)
    {
        return ((a % b) + b) % b;
    }
    [System.Serializable]
    public class Layer
    {
        public List<int> basicBlocks;
        public List<int> metals;
        public int maxdp;
    };

    //temporary variables
    List<List<Block.BlockInit>> PlanetTemp = new List<List<Block.BlockInit>>();
    List<Block.BlockInit> AllBlocks = new List<Block.BlockInit>();
    List<int> AllCaves = new List<int>();
    int MaxLen;
    int BlockNum;
    Block.BlockInit[] temp;
    int LrNow = 0;
    int INow;
    int LayerNow;
    int LastEdge;
    bool fst;
    int LastX = 0;
    void Start()
    {
        int sh = 0;
        for (int i = 0; i < Seed.Length; i++) {
            sh = mod((sh * 239 + ((int)Seed[i]) * 42), 115249);
        }
        rnd = new System.Random(sh);
        for (int i = 0; i <= Depth; i++)
            PlanetStruct.Add(new List<Block.BlockInit>());
        for (int i = 0; i <= Depth; i++)
            PlanetTemp.Add(new List<Block.BlockInit>());
        MaxLen = (Depth - 1) * 2 + 2;
        BlockNum = 0;
        fst = true;
        main._m.loadTxtSet("Loading Planet");
        main._m.loadSet(0);
        temp = new Block.BlockInit[MaxLen * 2 + 3];
        LrNow = Depth;
        LoadState = 1;
    }
    void Update()
    {
        if (LoadState == 1) {
            planetBaseGen(true);
        } else
        if (LoadState == 2) {
            planetBaseGen(false);
        } else
        if (LoadState == 3) {
            soilInit();
        } else
        if (LoadState == 4) {
            surfaceInit();
        } else
        if (LoadState == 5) {
            cavesInit();
            //TODO: write loading bar from this moment
        } else
        if (LoadState == 6) {
            polish();
        } else
        if (LoadState == 7) {
            calcLight();
            main._m.loadTxtSet("Instancing");
            LoadState++;
        } else
        if (LoadState == 8) {
            instantiateInCam();
            LoadState++;
            main._m.loadStop();
        } else {
            //If pLayer leaves the Planet
            if (LoadState > 10) {
                if (transform.childCount == 0) {
                    instantiateInCam();
                }
            }
            LoadState++;
        }
    }

    void planetBaseGen(bool Upper)
    {
        if (Upper) {
            bool EndCond = true;
            main._m.loadSet(((float)(Depth - LrNow)) / Depth / 2f);
            for (int lv = LrNow; lv >= 0; lv--) {
                int len = lv + (Depth - lv - 1) * 2 + 2;
                Block.BlockInit Left = null;
                for (int i = -len; i <= len; i++) {
                    Vector3 pos = new Vector3(i * 0.11f, lv * 0.2f + 0.1f, 0);
                    Block.BlockInit BNew = new Block.BlockInit();
                    BNew.pos = pos;
                    BNew.BaseBlock = BaseBlock;
                    BNew.Parent = this;
                    BNew.ObjectName = BlockNum++;
                    if (mod(i + lv, 2) == 0) {
                        BNew.flipY();
                    }
                    BNew.Left = Left;
                    BNew.Up = temp[i + MaxLen];
                    if (temp[i + MaxLen] != null)
                        temp[i + MaxLen].Down = BNew;
                    temp[i + MaxLen] = BNew;
                    if (Left != null)
                        Left.Right = BNew;
                    Left = BNew;
                    if (Mathf.Abs(i) < lv + 2) {
                        PlanetStruct[lv].Add(BNew);
                        BNew.Layer = lv;
                        BNew.ResLayer = lv;
                    } else {
                        if (i > 0) {
                            PlanetStruct[lv + ((Mathf.Abs(i) - lv) / 2)].Add(BNew);
                            BNew.Layer = lv + ((Mathf.Abs(i) - lv) / 2);
                            BNew.ResLayer = lv + ((Mathf.Abs(i) - lv) / 2);
                        } else {
                            if (mod(i + lv, 2) == 0)
                                PlanetStruct[lv + ((Mathf.Abs(i) - lv) / 2)].Insert(1, BNew);
                            else PlanetStruct[lv + ((Mathf.Abs(i) - lv) / 2)].Insert(0, BNew);
                            BNew.Layer = lv + ((Mathf.Abs(i) - lv) / 2);
                            BNew.ResLayer = lv + ((Mathf.Abs(i) - lv) / 2);
                        }
                    }
                }
                if (lv < LrNow - 10) {
                    LrNow = lv - 1;
                    EndCond = false;
                    break;
                }
            }
            if (EndCond) {
                LoadState++;
                LrNow = 0;
            }
        }
        else {
            bool EndCond = true;
            main._m.loadSet(((float)LrNow) / Depth / 2f + 0.5f);
            for (int lv = LrNow; lv <= Depth; lv++) {
                int len = lv + (Depth - lv - 1) * 2 + 2;
                Block.BlockInit Left = null;
                for (int i = -len; i <= len; i++) {
                    Vector3 pos = new Vector3(i * 0.11f, -lv * 0.2f - 0.1f, 0);
                    Block.BlockInit BNew = new Block.BlockInit();
                    BNew.pos = pos;
                    BNew.BaseBlock = BaseBlock;
                    BNew.Parent = this;
                    BNew.ObjectName = BlockNum++;
                    if (mod(i + lv, 2) == 1) {
                        BNew.flipY();
                    }
                    BNew.Up = temp[i + MaxLen];
                    if (temp[i + MaxLen] != null)
                        temp[i + MaxLen].Down = BNew;
                    temp[i + MaxLen] = BNew;
                    BNew.Left = Left;
                    if (Left != null)
                        Left.Right = BNew;
                    Left = BNew;
                    if (Mathf.Abs(i) < lv + 2) {
                        if (fst)
                            PlanetStruct[lv].Add(BNew);
                        else PlanetStruct[lv].Insert(PlanetStruct[lv].Count - 1, BNew);
                        fst = false;
                        BNew.Layer = lv;
                        BNew.ResLayer = lv;
                    } else {
                        if (i > 0) {
                            if (mod(i + lv, 2) == 0)
                                PlanetStruct[lv + ((Mathf.Abs(i) - lv) / 2)].Add(BNew);
                            else PlanetStruct[lv + ((Mathf.Abs(i) - lv) / 2)].Insert(PlanetStruct[lv + ((Mathf.Abs(i) - lv) / 2)].Count - 1, BNew);
                            BNew.Layer = lv + ((Mathf.Abs(i) - lv) / 2);
                            BNew.ResLayer = lv + ((Mathf.Abs(i) - lv) / 2);
                        } else {
                            PlanetTemp[lv + ((Mathf.Abs(i) - lv) / 2)].Add(BNew);
                            BNew.Layer = lv + ((Mathf.Abs(i) - lv) / 2);
                            BNew.ResLayer = lv + ((Mathf.Abs(i) - lv) / 2);
                        }
                    }
                }
                if (lv > LrNow + 10) {
                    LrNow = lv + 1;
                    EndCond = false;
                    break;
                }
            }
            if (EndCond) {
                for (int i = 0; i <= Depth; i++) {
                    for (int j = PlanetTemp[i].Count - 1; j >= 0; j--) {
                        PlanetStruct[i].Add(PlanetTemp[i][j]);
                    }
                    if (i != Depth)
                    foreach (Block.BlockInit b in PlanetStruct[i]) {
                        b.Active = true;
                    }
                }
                main._m.loadSet(0);
                main._m.loadTxtSet("Loading Blocks");
                LoadState++;
                LayerNow = 0;
                LrNow = Depth - 1;
                LastEdge = Depth - 1;
            }
        }
    }

    void soilInit()
    {
        bool EndCond = true;
        main._m.loadSet(((float)(Depth - 1 - LrNow)) / (Depth - 1));
        for (int i = LrNow; i >= 0; i--) {
            if (Depth - i > Layers[LayerNow].maxdp) {
                LayerNow++;
                LastEdge = i;
            }
            for (int j = 0; j < PlanetStruct[i].Count; j++) {
                if (i == Depth - 1) {
                    int rn1 = rnd.Next(Layers[LayerNow].basicBlocks.Count);
                    PlanetStruct[i][j].set(Layers[LayerNow].basicBlocks[rn1]);
                } else
                if (j == 0) {
                    bool NextLvl = false;
                    int Upper = (j / (2 * i + 1)) * (2 * i + 3) + 1 + (j % (2 * i + 1));
                    if (PlanetStruct[i + 1][Upper - 1].ResLayer >= LastEdge)
                        NextLvl = true;
                    if (PlanetStruct[i + 1][Upper].ResLayer >= LastEdge)
                        NextLvl = true;
                    if (PlanetStruct[i + 1][Upper + 1].ResLayer >= LastEdge)
                        NextLvl = true;
                    int rn1 = 0;
                    if (!NextLvl)
                        rn1 = rnd.Next(StonesShuffled * 3);
                    else rn1 = rnd.Next(10 * 3);
                    if (rn1 < 3) {
                        rn1 = rnd.Next(Layers[LayerNow].basicBlocks.Count);
                        PlanetStruct[i][j].set(Layers[LayerNow].basicBlocks[rn1]);
                    } else {
                        rn1 %= 3;
                        if (rn1 == 0) {
                            PlanetStruct[i][j].set(PlanetStruct[i + 1][Upper - 1].num);
                            PlanetStruct[i][j].ResLayer = PlanetStruct[i + 1][Upper - 1].ResLayer;
                        }
                        if (rn1 == 1) {
                            PlanetStruct[i][j].set(PlanetStruct[i + 1][Upper].num);
                            PlanetStruct[i][j].ResLayer = PlanetStruct[i + 1][Upper].ResLayer;
                        }
                        if (rn1 == 2) {
                            PlanetStruct[i][j].set(PlanetStruct[i + 1][Upper + 1].num);
                            PlanetStruct[i][j].ResLayer = PlanetStruct[i + 1][Upper + 1].ResLayer;
                        }
                    }
                } else if (j == PlanetStruct[i].Count - 1) {
                    bool NextLvl = false;
                    if (PlanetStruct[i][j - 1].ResLayer >= LastEdge)
                        NextLvl = true;
                    if (PlanetStruct[i][j - 2].ResLayer >= LastEdge)
                        NextLvl = true;
                    int Upper = (j / (2 * i + 1)) * (2 * i + 3) + 1 + (j % (2 * i + 1));
                    if (PlanetStruct[i + 1][Upper - 1].ResLayer >= LastEdge)
                        NextLvl = true;
                    if (PlanetStruct[i + 1][Upper].ResLayer >= LastEdge)
                        NextLvl = true;
                    if (PlanetStruct[i + 1][Upper + 1].ResLayer >= LastEdge)
                        NextLvl = true;
                    if (PlanetStruct[i][0].ResLayer >= LastEdge)
                        NextLvl = true;
                    if (PlanetStruct[i][1].ResLayer >= LastEdge)
                        NextLvl = true;
                    int rn1 = 0;
                    if (!NextLvl)
                        rn1 = rnd.Next(StonesShuffled * 7);
                    else rn1 = rnd.Next(10 * 7);
                    if (rn1 < 7) {
                        rn1 = rnd.Next(Layers[LayerNow].basicBlocks.Count);
                        PlanetStruct[i][j].set(Layers[LayerNow].basicBlocks[rn1]);
                    } else {
                        rn1 %= 7;
                        if (rn1 == 0) {
                            PlanetStruct[i][j].set(PlanetStruct[i][j - 1].num);
                            PlanetStruct[i][j].ResLayer = PlanetStruct[i][j - 1].ResLayer;
                        }
                        if (rn1 == 1) {
                            PlanetStruct[i][j].set(PlanetStruct[i][j - 2].num);
                            PlanetStruct[i][j].ResLayer = PlanetStruct[i][j - 2].ResLayer;
                        }
                        if (rn1 == 2) {
                            PlanetStruct[i][j].set(PlanetStruct[i + 1][Upper - 1].num);
                            PlanetStruct[i][j].ResLayer = PlanetStruct[i + 1][Upper - 1].ResLayer;
                        }
                        if (rn1 == 3) {
                            PlanetStruct[i][j].set(PlanetStruct[i + 1][Upper].num);
                            PlanetStruct[i][j].ResLayer = PlanetStruct[i + 1][Upper].ResLayer;
                        }
                        if (rn1 == 4) {
                            PlanetStruct[i][j].set(PlanetStruct[i + 1][Upper + 1].num);
                            PlanetStruct[i][j].ResLayer = PlanetStruct[i + 1][Upper + 1].ResLayer;
                        }
                        if (rn1 == 5) {
                            PlanetStruct[i][j].set(PlanetStruct[i][0].num);
                            PlanetStruct[i][j].ResLayer = PlanetStruct[i][0].ResLayer;
                        }
                        if (rn1 == 6) {
                            PlanetStruct[i][j].set(PlanetStruct[i][1].num);
                            PlanetStruct[i][j].ResLayer = PlanetStruct[i][1].ResLayer;
                        }
                    }
                } else if (j == 1) {
                    bool NextLvl = false;
                    if (PlanetStruct[i][j - 1].ResLayer >= LastEdge)
                        NextLvl = true;
                    int Upper = (j / (2 * i + 1)) * (2 * i + 3) + 1 + (j % (2 * i + 1));
                    if (PlanetStruct[i + 1][Upper - 1].ResLayer >= LastEdge)
                        NextLvl = true;
                    if (PlanetStruct[i + 1][Upper].ResLayer >= LastEdge)
                        NextLvl = true;
                    if (PlanetStruct[i + 1][Upper + 1].ResLayer >= LastEdge)
                        NextLvl = true;
                    int rn1 = 0;
                    if (!NextLvl)
                        rn1 = rnd.Next(StonesShuffled * 4);
                    else rn1 = rnd.Next(10 * 4);
                    if (rn1 < 4) {
                        rn1 = rnd.Next(Layers[LayerNow].basicBlocks.Count);
                        PlanetStruct[i][j].set(Layers[LayerNow].basicBlocks[rn1]);
                    } else {
                        rn1 %= 4;
                        if (rn1 == 0) {
                            PlanetStruct[i][j].set(PlanetStruct[i][j - 1].num);
                            PlanetStruct[i][j].ResLayer = PlanetStruct[i][j - 1].ResLayer;
                        }
                        if (rn1 == 1) {
                            PlanetStruct[i][j].set(PlanetStruct[i + 1][Upper - 1].num);
                            PlanetStruct[i][j].ResLayer = PlanetStruct[i + 1][Upper - 1].ResLayer;
                        }
                        if (rn1 == 2) {
                            PlanetStruct[i][j].set(PlanetStruct[i + 1][Upper].num);
                            PlanetStruct[i][j].ResLayer = PlanetStruct[i + 1][Upper].ResLayer;
                        }
                        if (rn1 == 3) {
                            PlanetStruct[i][j].set(PlanetStruct[i + 1][Upper + 1].num);
                            PlanetStruct[i][j].ResLayer = PlanetStruct[i + 1][Upper + 1].ResLayer;
                        }
                    }
                } else if (j == PlanetStruct[i].Count - 2) {
                    bool NextLvl = false;
                    if (PlanetStruct[i][j - 1].ResLayer >= LastEdge)
                        NextLvl = true;
                    if (PlanetStruct[i][j - 2].ResLayer >= LastEdge)
                        NextLvl = true;
                    int Upper = (j / (2 * i + 1)) * (2 * i + 3) + 1 + (j % (2 * i + 1));
                    if (PlanetStruct[i + 1][Upper - 1].ResLayer >= LastEdge)
                        NextLvl = true;
                    if (PlanetStruct[i + 1][Upper].ResLayer >= LastEdge)
                        NextLvl = true;
                    if (PlanetStruct[i + 1][Upper + 1].ResLayer >= LastEdge)
                        NextLvl = true;
                    if (PlanetStruct[i][0].ResLayer >= LastEdge)
                        NextLvl = true;
                    int rn1 = 0;
                    if (!NextLvl)
                        rn1 = rnd.Next(StonesShuffled * 6);
                    else rn1 = rnd.Next(10 * 6);
                    if (rn1 < 6) {
                        rn1 = rnd.Next(Layers[LayerNow].basicBlocks.Count);
                        PlanetStruct[i][j].set(Layers[LayerNow].basicBlocks[rn1]);
                    } else {
                        rn1 %= 6;
                        if (rn1 == 0) {
                            PlanetStruct[i][j].set(PlanetStruct[i][j - 1].num);
                            PlanetStruct[i][j].ResLayer = PlanetStruct[i][j - 1].ResLayer;
                        }
                        if (rn1 == 1) {
                            PlanetStruct[i][j].set(PlanetStruct[i][j - 2].num);
                            PlanetStruct[i][j].ResLayer = PlanetStruct[i][j - 2].ResLayer;
                        }
                        if (rn1 == 2) {
                            PlanetStruct[i][j].set(PlanetStruct[i + 1][Upper - 1].num);
                            PlanetStruct[i][j].ResLayer = PlanetStruct[i + 1][Upper - 1].ResLayer;
                        }
                        if (rn1 == 3) {
                            PlanetStruct[i][j].set(PlanetStruct[i + 1][Upper].num);
                            PlanetStruct[i][j].ResLayer = PlanetStruct[i + 1][Upper].ResLayer;
                        }
                        if (rn1 == 4) {
                            PlanetStruct[i][j].set(PlanetStruct[i + 1][Upper + 1].num);
                            PlanetStruct[i][j].ResLayer = PlanetStruct[i + 1][Upper + 1].ResLayer;
                        }
                        if (rn1 == 5) {
                            PlanetStruct[i][j].set(PlanetStruct[i][0].num);
                            PlanetStruct[i][j].ResLayer = PlanetStruct[i][0].ResLayer;
                        }
                    }
                } else {
                    bool NextLvl = false;
                    if (PlanetStruct[i][j - 1].ResLayer >= LastEdge)
                        NextLvl = true;
                    if (PlanetStruct[i][j - 2].ResLayer >= LastEdge)
                        NextLvl = true;
                    int Upper = (j / (2 * i + 1)) * (2 * i + 3) + 1 + (j % (2 * i + 1));
                    if (PlanetStruct[i + 1][Upper - 1].ResLayer >= LastEdge)
                        NextLvl = true;
                    if (PlanetStruct[i + 1][Upper].ResLayer >= LastEdge)
                        NextLvl = true;
                    if (PlanetStruct[i + 1][Upper + 1].ResLayer >= LastEdge)
                        NextLvl = true;
                    int rn1 = 0;
                    if (!NextLvl)
                        rn1 = rnd.Next(StonesShuffled * 5);
                    else rn1 = rnd.Next(10 * 5);
                    if (rn1 < 5) {
                        rn1 = rnd.Next(Layers[LayerNow].basicBlocks.Count);
                        PlanetStruct[i][j].set(Layers[LayerNow].basicBlocks[rn1]);
                    } else {
                        rn1 %= 5;
                        if (rn1 == 0) {
                            PlanetStruct[i][j].set(PlanetStruct[i][j - 1].num);
                            PlanetStruct[i][j].ResLayer = PlanetStruct[i][j - 1].ResLayer;
                        }
                        if (rn1 == 1) {
                            PlanetStruct[i][j].set(PlanetStruct[i][j - 2].num);
                            PlanetStruct[i][j].ResLayer = PlanetStruct[i][j - 2].ResLayer;
                        }
                        if (rn1 == 2) {
                            PlanetStruct[i][j].set(PlanetStruct[i + 1][Upper - 1].num);
                            PlanetStruct[i][j].ResLayer = PlanetStruct[i + 1][Upper - 1].ResLayer;
                        }
                        if (rn1 == 3) {
                            PlanetStruct[i][j].set(PlanetStruct[i + 1][Upper].num);
                            PlanetStruct[i][j].ResLayer = PlanetStruct[i + 1][Upper].ResLayer;
                        }
                        if (rn1 == 4) {
                            PlanetStruct[i][j].set(PlanetStruct[i + 1][Upper + 1].num);
                            PlanetStruct[i][j].ResLayer = PlanetStruct[i + 1][Upper + 1].ResLayer;
                        }
                    }
                }
            }
            if (i < LrNow - 5) {
                LrNow = i - 1;
                EndCond = false;
                break;
            }
        }
        if (EndCond) {
            INow = 0;
            LoadState++;
            main._m.loadSet(0);
            main._m.loadTxtSet("Loading Surface");
        }
    }

    void surfaceInit()
    {
        if (INow == 0) {
            for (int i = Depth - MountainsHeight; i < Depth; i++) {
                foreach (Block.BlockInit b in PlanetStruct[i]) {
                    b.Active = false;
                }
            }
        }
        int lnow = Depth - MountainsHeight - 1;
        int[] HeightMap = new int[2 * lnow + 1];

        {
            int i = INow;
            for (int j = 0; j < 2 * lnow + 1; j++) {
                HeightMap[j] = -1;
            }
            HeightMap[0] = 0;
            HeightMap[2 * lnow] = 0;
            createSurface(ref HeightMap, 0, 2 * lnow);
            for (int j = 0; j < 2 * lnow + 1; j++) {
                buildSurface(HeightMap[j], lnow, i * (2 * lnow + 1) + j);
            }
            INow++;
        }
        main._m.loadSet(((float)INow) / 6);
        if (INow == 6) {
            main._m.loadSet(0);
            main._m.loadTxtSet("Loading Caverns");
            LoadState++;
            LrNow = 0;
        }
    }
    void buildSurface(int d, int i, int j)
    {
        if (d == 0)
            return;
        if (d > 0) {
            PlanetStruct[i][j].Active = false;
            if (j % (2 * i + 1) == 0 || j % (2 * i + 1) == 2 * i)
                return;
            int Upper = (j / (2 * i + 1)) * (2 * i - 1) - 1 + (j % (2 * i + 1));
            buildSurface(d - 1, i - 1, Upper);
        } else {
            PlanetStruct[i][j].Active = true;
            int Upper = (j / (2 * i + 1)) * (2 * i + 3) + 1 + (j % (2 * i + 1));
            buildSurface(d + 1, i + 1, Upper);
        }
    }
    private void createSurface(ref int[] HeightMap, int l, int r)
    {
        if (r - l > 1) {
            HeightMap[(r + l) / 2] = Mathf.Min(MountainsHeight, Mathf.Max(-MountainsHeight, (HeightMap[r] + HeightMap[l]) / 2 + getRnd(r - l)));
            createSurface(ref HeightMap, l, (l + r) / 2);
            createSurface(ref HeightMap, (l + r) / 2, r);
        }
    }
    int getRnd(int sz)
    {
        return rnd.Next(2 * sz) - sz;
    }

    void cavesInit()
    {
        if (LrNow == 0) {
            int lnow = Depth - MountainsHeight * 2 - 1;
            int CalcCount = Mathf.RoundToInt(lnow * lnow * CavesNumber);
            int CalcSize = Mathf.RoundToInt(lnow * lnow * CavesSize);
            for (int i = 0; i < CalcCount; i++) {
                AllCaves.Add(rnd.Next(CalcSize));
            }
            for (int i = 0; i < lnow; i++) {
                foreach (Block.BlockInit b in PlanetStruct[i]) {
                    AllBlocks.Add(b);
                }
            }
            AllCaves.Sort();
            AllCaves.Add(CalcSize);
        }
        bool endd = true;
        for (int i = LrNow; i < AllCaves.Count; i++) {
            caveGen(AllBlocks[rnd.Next(AllBlocks.Count)], AllCaves[i] - LastX);
            LastX = AllCaves[i];
            if (i > LrNow + 5) {
                LrNow = i++;
                endd = false;
                break;
            }
        }
        if (endd) {
            main._m.loadSet(0);
            main._m.loadTxtSet("Polishing edges");
            LoadState++;
        }
    }

    void caveGen(Block.BlockInit BNew, int Size)
    { 
        BNew.Active = false;
        for(; Size>0; Size--) {
            int temp = rnd.Next(4);
            if (temp == 0) {
                if (BNew.Up != null) {
                    BNew = BNew.Up;
                    BNew.Active = false;
                } else Size++;
            } else if (temp == 1) {
                if (BNew.Down != null) {
                    BNew = BNew.Down;
                    BNew.Active = false;
                } else Size++;
            }
            else if(temp == 2) {
                if (BNew.Right != null) {
                    BNew = BNew.Right;
                    BNew.Active = false;
                } else Size++;
            }
            else if(temp == 3) {
                if (BNew.Left != null) {
                    BNew = BNew.Left;
                    BNew.Active = false;
                } else Size++;
            }
        }
        
    }

    void polish()
    {
        for (int i = 0; i < Depth; i++)
            foreach (Block.BlockInit b in PlanetStruct[i]) {
                int cnt1 = 0;
                {
                    if (b.Left.Left!=null && !b.Left.Left.Active)
                        cnt1++;
                    if (b.Right.Right != null && !b.Right.Right.Active)
                        cnt1++;
                    if (b.Up.Left != null && !b.Up.Left.Active)
                        cnt1++;
                    if (b.Up.Right != null && !b.Up.Right.Active)
                        cnt1++;
                    if (b.Down.Left != null && !b.Down.Left.Active)
                        cnt1++;
                    if (b.Down.Right != null && !b.Down.Right.Active)
                        cnt1++;
                    if (!b.Right.Active)
                        cnt1++;
                    if (!b.Left.Active)
                        cnt1++;
                    if (!b.Down.Active)
                        cnt1++;
                    if (!b.Up.Active)
                        cnt1++;
                    if (b.FlippedY) {
                        if (b.Up.Right != null && b.Up.Right.Right != null && !b.Up.Right.Right.Active)
                            cnt1++;
                        if (b.Up.Left != null && b.Up.Left.Left != null && !b.Up.Left.Left.Active)
                            cnt1++;
                    } else {
                        if (b.Down.Right != null && b.Down.Right.Right != null && !b.Down.Right.Right.Active)
                            cnt1++;
                        if (b.Down.Left != null && b.Down.Left.Left != null && !b.Down.Left.Left.Active)
                            cnt1++;
                    }
                }
                if (cnt1>=12-CavesWidth)
                    b.Active = false;
                if (cnt1 <= CavesWidth)
                    b.Active = true;
            }
        ls.Clear();
        for (int i = 0; i <= Depth; i++) {
            foreach (Block.BlockInit b in PlanetStruct[i]) {
                b.LightPenetr = main._m.Blocks[b.num].LightPenetr;
                b.BasicLight = main._m.Blocks[b.num].BasicLight;
                b.Lightness = b.getBasicLight();
                b.it = new LightedBlock(b);
                ls.Add(b.it);
            }
        }
        main._m.loadSet(0);
        main._m.loadTxtSet("Baking Lights");
        LoadState++;
    }
    public void calcLight()
    {
        AllBlocks.Clear();
        while (ls.Count != 0) {
            LightedBlock now = ls.Max;
            if (now.b.Left != null) {
                checkLightUpd(now, now.b.Left);
            }
            if (now.b.Right != null) {
                checkLightUpd(now, now.b.Right);
            }
            if (now.b.FlippedY) {
                if (now.b.Up != null) {
                    checkLightUpd(now, now.b.Up);
                }
            } else {
                if (now.b.Down != null) {
                    checkLightUpd(now, now.b.Down);
                }
            }
            AllBlocks.Add(now.b);
            ls.Remove(now);
        }
        foreach (Block.BlockInit b in AllBlocks)
            b.UpdateLightness();
        AllBlocks.Clear();
    }
    
    void checkLightUpd(LightedBlock now, Block.BlockInit nn)
    {
        if (nn != null && nn.Lightness < now.b.Lightness * nn.getLightCoeff()) {
            nn.updby += " "+now.b.ObjectName.ToString();
            ls.Remove(nn.it);
            nn.Lightness = now.b.Lightness * nn.getLightCoeff();
            nn.it = new LightedBlock(nn);
            ls.Add(nn.it);
        }
    }

    public void updLight(Block.BlockInit b)
    {
        main._m.FrameNum++;
        b.used = main._m.FrameNum;
        Queue<LightedBlock> bfs = new Queue<LightedBlock>();
        if(b.Lightness > b.getBasicLight())
            bfs.Enqueue(new LightedBlock(b, b.Lightness));
        else bfs.Enqueue(new LightedBlock(b, b.getBasicLight()));
        b.Lightness = b.getBasicLight();
        ls.Clear();
        List<Block.BlockInit> edge = new List<Block.BlockInit>();
        while (bfs.Count != 0) {
            LightedBlock BNow = bfs.Peek();
            bfs.Dequeue();
            BNow.b.it = new LightedBlock(BNow.b);
            
            ls.Add(BNow.b.it);
            if (BNow.Lightness <= 0.01f) {
                edge.Add(BNow.b);
                continue;
            }
            if (BNow.b.Left!=null && BNow.b.Left.used < main._m.FrameNum) {
                BNow.b.Left.Lightness = BNow.b.Left.getBasicLight();
                BNow.b.Left.used = main._m.FrameNum;
                bfs.Enqueue(new LightedBlock(BNow.b.Left, BNow.Lightness * BNow.b.Left.getLightCoeff()));
            }
            if (BNow.b.Right != null && BNow.b.Right.used < main._m.FrameNum) {
                BNow.b.Right.Lightness = BNow.b.Right.getBasicLight();
                BNow.b.Right.used = main._m.FrameNum;
                bfs.Enqueue(new LightedBlock(BNow.b.Right, BNow.Lightness * BNow.b.Right.getLightCoeff()));
            }
            if (BNow.b.FlippedY) {
                if (BNow.b.Up != null && BNow.b.Up.used < main._m.FrameNum) {
                    BNow.b.Up.Lightness = BNow.b.Up.getBasicLight();
                    BNow.b.Up.used = main._m.FrameNum;
                    bfs.Enqueue(new LightedBlock(BNow.b.Up, BNow.Lightness * BNow.b.Up.getLightCoeff()));
                }
            } else {
                if (BNow.b.Down != null && BNow.b.Down.used < main._m.FrameNum) {
                    BNow.b.Down.Lightness = BNow.b.Down.getBasicLight();
                    BNow.b.Down.used = main._m.FrameNum;
                    bfs.Enqueue(new LightedBlock(BNow.b.Down, BNow.Lightness * BNow.b.Down.getLightCoeff()));
                }
            }
        }
        foreach(Block.BlockInit b1 in edge) {
            if (b1.Left != null && b1.Left.used < main._m.FrameNum) {
                b1.Left.used = main._m.FrameNum;
                b1.Left.it = new LightedBlock(b1.Left);
                ls.Add(b1.Left.it);
            }
            if (b1.Right != null && b1.Right.used < main._m.FrameNum) {
                b1.Right.used = main._m.FrameNum;
                b1.Right.it = new LightedBlock(b1.Right);
                ls.Add(b1.Right.it);
            }
            if (b1.FlippedY) {
                if (b1.Up != null && b1.Up.used < main._m.FrameNum) {
                    b1.Up.used = main._m.FrameNum;
                    b1.Up.it = new LightedBlock(b1.Up);
                    ls.Add(b1.Up.it);
                }
            } else {
                if (b1.Down != null && b1.Down.used < main._m.FrameNum) {
                    b1.Down.used = main._m.FrameNum;
                    b1.Down.it = new LightedBlock(b1.Down);
                    ls.Add(b1.Down.it);
                }
            }
        }
        calcLight();
    }

    void instantiateInCam()
    {
        for (int i = 0; i <= Depth; i++) {
            foreach (Block.BlockInit b in PlanetStruct[i]) {
                if (Vector3.Distance(b.pos, plr.transform.position) < main._m.MinRenderDistance || RenderAll) {
                    b.createInstance();
                    if (!RenderAll)
                        b.BlockConnected.Watch = true;
                }
            }
        }
    }
}
