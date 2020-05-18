using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Vuforia;

public class PhysicsO : MonoBehaviour
{
    [Header("System variables")]
    public GameObject planet;
    [HideInInspector]
    public int state = 0;
    [HideInInspector]
    public Vector2 tomove = Vector2.zero;
    Rigidbody2D rb;
    Vector3 rotateto = Vector3.zero;
    Vector2 lgravity = new Vector2(0, -2);
    float rspeed = 1.2f;
    float maxd = 0.25f;
    float abs(float a)
    {
        if (a < 0)
            return -a;
        return a;
    }
    Vector2 tov2(Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }
    int absi(int a)
    {
        if (a < 0)
            return -a;
        return a;
    }
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Quaternion newrot = Quaternion.LookRotation(new Vector3(0, 0, 1), Vector3.RotateTowards(transform.up, rotateto, Time.deltaTime * rspeed, 0.0f));
        transform.rotation = newrot;
        Vector3 dist = transform.position - planet.transform.position;
        //float sq3 = Mathf.Sqrt(3);
        float sq3 = 2*(1f/1.1f);
        Vector2 gravity = new Vector2(0, 0);
        int nstate = -1;
        if (dist.y > 0) {
            if (dist.x > 0) {
                if (abs(dist.y / dist.x) <= sq3) {
                    gravity = new Vector2(-sq3, -1);
                    nstate = 1;
                } else {
                    gravity = new Vector2(0, -2);
                    nstate = 0;
                }
            } else {
                if (dist.x == 0 || abs(dist.y / dist.x) <= sq3) {
                    gravity = new Vector2(sq3, -1);
                    nstate = 5;
                } else {
                    gravity = new Vector2(0, -2);
                    nstate = 0;
                }
            }
        } else {
            if (dist.x > 0) {
                if (abs(dist.y / dist.x) <= sq3) {
                    gravity = new Vector2(-sq3, 1);
                    nstate = 2;
                } else {
                    gravity = new Vector2(0, 2);
                    nstate = 3;
                }
            } else {
                if (dist.x == 0 || abs(dist.y / dist.x) <= sq3) {
                    gravity = new Vector2(sq3, 1);
                    nstate = 4;
                } else {
                    gravity = new Vector2(0, 2);
                    nstate = 3;
                }
            }
        }
        if (nstate == state) {
            rb.AddForce(gravity * 2000 * Time.deltaTime, ForceMode2D.Force);
            lgravity = gravity;
        } else if (absi(nstate - state) > 1  && absi(nstate - state)!=5) {
            rotateto = -gravity;
            state = nstate;
            lgravity = gravity;
            rb.AddForce(gravity * 2000 * Time.deltaTime, ForceMode2D.Force);
        } else {
            if((state==1 && nstate==2) || (state == 2 && nstate == 1) || (state == 4 && nstate == 5) || (state == 5 && nstate == 4)) {
                if (abs(dist.y) > maxd) {
                    rotateto = -gravity;
                    state = nstate;
                    lgravity = gravity;
                    rb.AddForce(gravity * 2000 * Time.deltaTime, ForceMode2D.Force);
                } else {
                    rb.AddForce(lgravity * 2000 * Time.deltaTime, ForceMode2D.Force);
                }
            } else {
                float tmp = (Mathf.PI / 3f) - Mathf.Atan2(abs(dist.y), abs(dist.x));
                float len = 0;
                if (tmp>=0)
                     len = Mathf.Sin(tmp)*(Mathf.Sqrt(dist.y*dist.y+dist.x*dist.x));
                else len = Mathf.Sin(-tmp) * (Mathf.Sqrt(dist.y * dist.y + dist.x * dist.x))/2f;
                if (abs(len) > maxd) {
                    rotateto = -gravity;
                    state = nstate;
                    lgravity = gravity;
                    rb.AddForce(gravity * 2000 * Time.deltaTime, ForceMode2D.Force);
                } else {
                    rb.AddForce(lgravity * 2000 * Time.deltaTime, ForceMode2D.Force);
                }
            }
        }
        if(rb.velocity == Vector2.zero) {
            rb.velocity = tomove*Time.deltaTime;
        } else if (tomove != Vector2.zero) {
            float cosa = (rb.velocity.x * tomove.x + rb.velocity.y * tomove.y) / rb.velocity.magnitude / tomove.magnitude;
            rb.velocity = rb.velocity - rb.velocity.magnitude * cosa * tomove.normalized + tomove * Time.deltaTime;
        }
        
    }
    public void stop()
    {
        if (tomove != Vector2.zero && rb.velocity != Vector2.zero) {
            float cosa = (rb.velocity.x * tomove.x + rb.velocity.y * tomove.y) / rb.velocity.magnitude / tomove.magnitude;
            rb.velocity = rb.velocity - rb.velocity.magnitude * cosa * tomove.normalized;
        }
        tomove = Vector2.zero;
    }
    bool cnum = false;
}
