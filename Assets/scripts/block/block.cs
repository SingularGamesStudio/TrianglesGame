using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class block : MonoBehaviour
{
    [Header("System variables")]
    
    public Collider2D coll;
    public SpriteRenderer img;
    public SpriteRenderer l;
    public SpriteRenderer r;
    public SpriteRenderer u;
    public SpriteRenderer d;
    public blockInit prm;
    public bool watch;
    [System.Serializable]
    public class blockInit
    {
        public block cn;
        public Vector3 pos;
        public bool flippedy = false;
        public blockInit left;
        public blockInit right;
        public blockInit up;
        public blockInit down;
        public bool active;
        public int num;
        public int reslayer;
        public int layer;
        public bool binact;
        public string Oname;
        public GameObject baseb;
        public void CreateInstance()
        {
            GameObject g = Instantiate(baseb);
            block b = g.GetComponent<block>();
            b.prm = this;
            cn = b;
            g.transform.position = pos;
            main.BInit nb = main._m.blocks[num];
            b.img.sprite = nb.spbase;
            b.l.sprite = nb.spleft;
            b.r.sprite = nb.spright;
            b.u.sprite = nb.spup;
            b.d.sprite = nb.spdown;
            if (flippedy) {
                b.img.flipX = true;
                b.l.flipY = true;
                b.r.flipY = true;
                b.u.flipY = true;
                b.d.flipY = true;
                b.img.transform.rotation *= Quaternion.AngleAxis(180, Vector3.forward);
            }
            if (binact)
                b.Act();
            else b.UnAct();
        }
        public void flipy()
        {
            flippedy = !flippedy;
        }
        public void set(int k)
        {
            num = k;
        }
    }

    
    public void UnAct()
    {
        coll.isTrigger = true;
        prm.active = false;
        img.enabled = false;
        l.gameObject.SetActive(false);
        r.gameObject.SetActive(false);
        u.gameObject.SetActive(false);
        d.gameObject.SetActive(false);
        if (prm.left != null && prm.left.cn!=null)
            prm.left.cn.upd();
        if (prm.right != null && prm.right.cn != null)
            prm.right.cn.upd();
        if (prm.up != null && prm.up.cn != null)
            prm.up.cn.upd();
        if (prm.down != null && prm.down.cn != null)
            prm.down.cn.upd();
        upd();
    }
    public void Act()
    {
        prm.active = true;
        img.enabled = true;
        coll.isTrigger = false;
        l.gameObject.SetActive(false);
        r.gameObject.SetActive(false);
        u.gameObject.SetActive(false);
        d.gameObject.SetActive(false);
        if (prm.left != null && prm.left.cn != null)
            prm.left.cn.upd();
        if (prm.right != null && prm.right.cn != null)
            prm.right.cn.upd();
        if (prm.up != null && prm.up.cn != null)
            prm.up.cn.upd();
        if (prm.down != null && prm.down.cn != null)
            prm.down.cn.upd();
        upd();
    }
    public void upd()
    {
        l.gameObject.SetActive(false);
        r.gameObject.SetActive(false);
        u.gameObject.SetActive(false);
        d.gameObject.SetActive(false);
        if (prm.active) {
            if (!prm.left.active)
                l.gameObject.SetActive(true);
            if (!prm.right.active)
                r.gameObject.SetActive(true);
            if ((!prm.up.active && !prm.flippedy) || (!prm.down.active && prm.flippedy))
                u.gameObject.SetActive(true);
            if ((!prm.down.active && !prm.flippedy) || (!prm.up.active && prm.flippedy))
                d.gameObject.SetActive(true);
        }
    }
    // Start is called before the first frame update
    void Start()
    {
    }

    // Update is called once per frame
    void Update()
    {
        if (watch) {
            checkwatch();
        }
    }
    public void checkwatch()
    {
        if (Vector3.Distance(main._m.plr.transform.position, transform.position) < main._m.maxpd) {
            if (prm.left != null && prm.left.cn == null) {
                prm.left.CreateInstance();
                prm.left.cn.watch = true;
                prm.left.cn.checkwatch();
            }
            if (prm.right != null && prm.right.cn == null) {
                prm.right.CreateInstance();
                prm.right.cn.watch = true;
                prm.right.cn.checkwatch();
            }
            if (prm.down != null && prm.down.cn == null) {
                prm.down.CreateInstance();
                prm.down.cn.watch = true;
                prm.down.cn.checkwatch();
            }
            if (prm.up != null && prm.up.cn == null) {
                prm.up.CreateInstance();
                prm.up.cn.watch = true;
                prm.up.cn.checkwatch();
            }
            watch = false;
        }
    }
    public void Click()
    {
        UnAct();
    }
}
