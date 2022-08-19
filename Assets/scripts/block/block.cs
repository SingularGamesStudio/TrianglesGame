using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Block : MonoBehaviour
{
    [Header("System variAllBlockses")]
    
    public Collider2D Coll;
    public SpriteRenderer img;
    public SpriteRenderer l;
    public SpriteRenderer r;
    public SpriteRenderer u;
    public SpriteRenderer d;
    public BlockInit Params;
    public SpriteRenderer Shade;
    
    public bool Watch;
    
    [System.Serializable]
    public class BlockInit
    {
        public Block BlockConnected;
        public Vector3 pos;
        public bool FlippedY = false;
        public BlockInit Left;
        public BlockInit Right;
        public BlockInit Up;
        public BlockInit Down;
        public int num;
        public int ResLayer;
        public int Layer;
        public bool Active;
        public int ObjectName;
        public float Lightness;
        public float BasicLight;
        public float LightPenetr;
        public PlanetInit.LightedBlock it;
        public GameObject BaseBlock;
        public PlanetInit Parent;
        public int used = 0;
        public void createInstance()
        {
            if (BaseBlock != null) {
                GameObject g = null;
                Block b = null;
                if (main._m.Pool.Count == 0) {
                    g = Instantiate(BaseBlock);
                    b = g.GetComponent<Block>();
                } else {
                    b = main._m.Pool.Pop();
                    g = b.gameObject;
                    g.SetActive(true);
                }
                b.Params = this;
                g.transform.parent = Parent.transform;
                g.name = ObjectName.ToString();
                BlockConnected = b;
                g.transform.position = pos;
                main.BInit nb = main._m.Blocks[num];
                b.img.sprite = nb.SpBase;
                b.l.sprite = nb.SpLeft;
                b.r.sprite = nb.SpRight;
                b.u.sprite = nb.SpUp;
                b.d.sprite = nb.SpDown;
                BasicLight = nb.BasicLight;
                LightPenetr = nb.LightPenetr;
                b.Shade.color = new Color(b.Shade.color.r, b.Shade.color.g, b.Shade.color.b, Mathf.Max(1 - Lightness, 0));
                b.img.transform.rotation = Quaternion.identity;
                b.Shade.transform.rotation = Quaternion.identity;
                if (FlippedY) {
                    b.img.flipX = true;
                    b.l.flipY = true;
                    b.r.flipY = true;
                    b.u.flipY = true;
                    b.d.flipY = true;
                    b.img.transform.rotation *= Quaternion.AngleAxis(180, Vector3.forward);
                    b.Shade.transform.rotation *= Quaternion.AngleAxis(180, Vector3.forward);
                } else {
                    b.img.flipX = false;
                    b.l.flipY = false;
                    b.r.flipY = false;
                    b.u.flipY = false;
                    b.d.flipY = false;
                }
                if (Active)
                    b.activate();
                else b.unActivate();
            }
        }
        public void UpdateLightness()
        {
            if(BlockConnected!=null)
                BlockConnected.Shade.color = new Color(BlockConnected.Shade.color.r, BlockConnected.Shade.color.g, BlockConnected.Shade.color.b, Mathf.Max(1 - Lightness, 0));
        }
        public void destroyInstance()
        {
            main._m.Pool.Push(BlockConnected);
            BlockConnected.gameObject.SetActive(false);
            BlockConnected.gameObject.name = "afk";
            BlockConnected.Params = null;
            BlockConnected = null;
            if (Left != null && Left.BlockConnected != null) {
                Left.BlockConnected.Watch = true;
                Left.BlockConnected.checkWatch();
            }
            if (Right != null && Right.BlockConnected != null) {
                Right.BlockConnected.Watch = true;
                Right.BlockConnected.checkWatch();
            }
            if (Down != null && Down.BlockConnected != null) {
                Down.BlockConnected.Watch = true;
                Down.BlockConnected.checkWatch();
            }
            if (Up != null && Up.BlockConnected != null) {
                Up.BlockConnected.Watch = true;
                Up.BlockConnected.checkWatch();
            }
        }
        public float getLightCoeff()
        {
            if (!Active)
                return 0.8f;
            else return LightPenetr;
        }
        public float getBasicLight()
        {
            if (Layer > Parent.Depth - 3 * Parent.MountainsHeight && !Active)
                return 4;
            if (!Active)
                return 0;
            else return BasicLight;
        }
        public void flipY()
        {
            FlippedY = !FlippedY;
        }
        public void set(int k)
        {
            if (BlockConnected == null) {
                num = k;
            } else {
                num = k;
                main.BInit nb = main._m.Blocks[num];
                BlockConnected.img.sprite = nb.SpBase;
                BlockConnected.l.sprite = nb.SpLeft;
                BlockConnected.r.sprite = nb.SpRight;
                BlockConnected.u.sprite = nb.SpUp;
                BlockConnected.d.sprite = nb.SpDown;
                BasicLight = nb.BasicLight;
                LightPenetr = nb.LightPenetr;
            }
        }
    }

    
    public void unActivate()
    {
        Coll.isTrigger = true;
        Params.Active = false;
        img.enabled = false;
        if (Params.Left != null && Params.Left.BlockConnected!=null)
            Params.Left.BlockConnected.upd();
        if (Params.Right != null && Params.Right.BlockConnected != null)
            Params.Right.BlockConnected.upd();
        if (Params.Up != null && Params.Up.BlockConnected != null)
            Params.Up.BlockConnected.upd();
        if (Params.Down != null && Params.Down.BlockConnected != null)
            Params.Down.BlockConnected.upd();
        upd();
    }
    public void activate()
    {
        Params.Active = true;
        img.enabled = true;
        Coll.isTrigger = false;
        if (Params.Left != null && Params.Left.BlockConnected != null)
            Params.Left.BlockConnected.upd();
        if (Params.Right != null && Params.Right.BlockConnected != null)
            Params.Right.BlockConnected.upd();
        if (Params.Up != null && Params.Up.BlockConnected != null)
            Params.Up.BlockConnected.upd();
        if (Params.Down != null && Params.Down.BlockConnected != null)
            Params.Down.BlockConnected.upd();
        upd();
    }
    public void upd()
    {
        l.gameObject.SetActive(false);
        r.gameObject.SetActive(false);
        u.gameObject.SetActive(false);
        d.gameObject.SetActive(false);
        if (Params.Active) {
            if (!Params.Left.Active)
                l.gameObject.SetActive(true);
            if (!Params.Right.Active)
                r.gameObject.SetActive(true);
            if ((!Params.Up.Active && !Params.FlippedY) || (!Params.Down.Active && Params.FlippedY))
                u.gameObject.SetActive(true);
            if ((!Params.Down.Active && !Params.FlippedY) || (!Params.Up.Active && Params.FlippedY))
                d.gameObject.SetActive(true);
        }
    }
    void Update()
    {
        if (Watch) {
            checkWatch();
        }
    }
    public void checkWatch()
    {
        if (Vector3.Distance(main._m.plr.transform.position, transform.position) < main._m.MinRenderDistance) {
            if (Params.Left != null && Params.Left.BlockConnected == null) {
                Params.Left.createInstance();
                Params.Left.BlockConnected.Watch = true;
                Params.Left.BlockConnected.checkWatch();
            }
            if (Params.Right != null && Params.Right.BlockConnected == null) {
                Params.Right.createInstance();
                Params.Right.BlockConnected.Watch = true;
                Params.Right.BlockConnected.checkWatch();
            }
            if (Params.Down != null && Params.Down.BlockConnected == null) {
                Params.Down.createInstance();
                Params.Down.BlockConnected.Watch = true;
                Params.Down.BlockConnected.checkWatch();
            }
            if (Params.Up != null && Params.Up.BlockConnected == null) {
                Params.Up.createInstance();
                Params.Up.BlockConnected.Watch = true;
                Params.Up.BlockConnected.checkWatch();
            }
            Watch = false;
        } else if(Vector3.Distance(main._m.plr.transform.position, transform.position) > main._m.MaxRenderDistance) {
            Params.destroyInstance();
        }
    }
    public void click()
    {
        if (Params.Active) {
            unActivate();
            Params.Parent.updLight(Params);
        } else {
            Params.set(main._m.BlockInHand);
            activate();
            Params.Parent.updLight(Params);
        }
    }
}
