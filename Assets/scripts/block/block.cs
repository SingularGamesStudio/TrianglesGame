using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

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
    public float Lightness;
    public bool Watch;
    public main.LightedBlock it;
    [System.Serializable]
    public class BlockInit
    {
        public Block BlockConnected;
        public Vector3 pos;
        public bool Flippedy = false;
        public BlockInit Left;
        public BlockInit Right;
        public BlockInit Up;
        public BlockInit Down;
        public int num;
        public int ResLayer;
        public int Layer;
        public bool Active;
        public int ObjectName;
        public GameObject BaseBlock;
        public GameObject Parent;
        public void createInstance()
        {
            if (BaseBlock != null) {
                GameObject g = Instantiate(BaseBlock);
                Block b = g.GetComponent<Block>();
                main._m.ActiveBlocks.Add(b);
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
                if (Flippedy) {
                    b.img.flipX = true;
                    b.l.flipY = true;
                    b.r.flipY = true;
                    b.u.flipY = true;
                    b.d.flipY = true;
                    b.img.transform.rotation *= Quaternion.AngleAxis(180, Vector3.forward);
                    b.Shade.transform.rotation *= Quaternion.AngleAxis(180, Vector3.forward);
                }
                if (Active)
                    b.activate();
                else b.unactivate();
            }
        }
        public void destroyInstance()
        {
            main._m.ActiveBlocks.Remove(BlockConnected);
            Destroy(BlockConnected.gameObject);
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
            return 0.7f;
        }
        public void flipY()
        {
            Flippedy = !Flippedy;
        }
        public void set(int k)
        {
            num = k;
        }
    }

    
    public void unactivate()
    {
        Coll.isTrigger = true;
        Params.Active = false;
        img.enabled = false;
        l.gameObject.SetActive(false);
        r.gameObject.SetActive(false);
        u.gameObject.SetActive(false);
        d.gameObject.SetActive(false);
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
        l.gameObject.SetActive(false);
        r.gameObject.SetActive(false);
        
        u.gameObject.SetActive(false);
        d.gameObject.SetActive(false);
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
            if ((!Params.Up.Active && !Params.Flippedy) || (!Params.Down.Active && Params.Flippedy))
                u.gameObject.SetActive(true);
            if ((!Params.Down.Active && !Params.Flippedy) || (!Params.Up.Active && Params.Flippedy))
                d.gameObject.SetActive(true);
        }
    }
    void Start()
    {
    }
    void Update()
    {
        if (Watch) {
            checkWatch();
        }
    }
    void FixedUpdate()
    {
        //Shade.color = new Color(Shade.color.r, Shade.color.g, Shade.color.b, Mathf.Max(1-Lightness, 0));
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
        unactivate();
    }
}
