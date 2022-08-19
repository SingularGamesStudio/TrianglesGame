using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PhysicsO : MonoBehaviour
{
    [Header("System variables")]
    public GameObject Planet;
    int State = 0;
    [HideInInspector]
    public Vector2 ToMove = Vector2.zero;
    Rigidbody2D rb;
    Vector3 RotateTo = Vector3.zero;
    Vector2 LastGravity = new Vector2(0, -2);
    float RSpeed = 1.2f;
    float MaxDist = 0.25f;
    public bool active;
    void Start()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }
    void FixedUpdate()
    {
        if (active) {
            Quaternion NewRot = Quaternion.LookRotation(new Vector3(0, 0, 1), Vector3.RotateTowards(transform.up, RotateTo, Time.deltaTime * RSpeed, 0.0f));
            transform.rotation = NewRot;
            Vector3 dist = transform.position - Planet.transform.position;
            //float sq3 = Mathf.Sqrt(3);
            float sq3 = 2 * (1f / 1.1f);
            Vector2 Gravity = new Vector2(0, 0);
            int NState = -1;
            if (dist.y > 0) {
                if (dist.x > 0) {
                    if (Mathf.Abs(dist.y / dist.x) <= sq3) {
                        Gravity = new Vector2(-sq3, -1);
                        NState = 1;
                    } else {
                        Gravity = new Vector2(0, -2);
                        NState = 0;
                    }
                } else {
                    if (dist.x == 0 || Mathf.Abs(dist.y / dist.x) <= sq3) {
                        Gravity = new Vector2(sq3, -1);
                        NState = 5;
                    } else {
                        Gravity = new Vector2(0, -2);
                        NState = 0;
                    }
                }
            } else {
                if (dist.x > 0) {
                    if (Mathf.Abs(dist.y / dist.x) <= sq3) {
                        Gravity = new Vector2(-sq3, 1);
                        NState = 2;
                    } else {
                        Gravity = new Vector2(0, 2);
                        NState = 3;
                    }
                } else {
                    if (dist.x == 0 || Mathf.Abs(dist.y / dist.x) <= sq3) {
                        Gravity = new Vector2(sq3, 1);
                        NState = 4;
                    } else {
                        Gravity = new Vector2(0, 2);
                        NState = 3;
                    }
                }
            }
            if (NState == State) {
                rb.AddForce(Gravity * 2000 * Time.deltaTime, ForceMode2D.Force);
                LastGravity = Gravity;
            } else if (Mathf.Abs(NState - State) > 1 && Mathf.Abs(NState - State) != 5) {
                RotateTo = -Gravity;
                State = NState;
                LastGravity = Gravity;
                rb.AddForce(Gravity * 2000 * Time.deltaTime, ForceMode2D.Force);
            } else {
                if ((State == 1 && NState == 2) || (State == 2 && NState == 1) || (State == 4 && NState == 5) || (State == 5 && NState == 4)) {
                    if (Mathf.Abs(dist.y) > MaxDist) {
                        RotateTo = -Gravity;
                        State = NState;
                        LastGravity = Gravity;
                        rb.AddForce(Gravity * 2000 * Time.deltaTime, ForceMode2D.Force);
                    } else {
                        rb.AddForce(LastGravity * 2000 * Time.deltaTime, ForceMode2D.Force);
                    }
                } else {
                    float tmp = (Mathf.PI / 3f) - Mathf.Atan2(Mathf.Abs(dist.y), Mathf.Abs(dist.x));
                    float len = 0;
                    if (tmp >= 0)
                        len = Mathf.Sin(tmp) * (Mathf.Sqrt(dist.y * dist.y + dist.x * dist.x));
                    else len = Mathf.Sin(-tmp) * (Mathf.Sqrt(dist.y * dist.y + dist.x * dist.x)) / 2f;
                    if (Mathf.Abs(len) > MaxDist) {
                        RotateTo = -Gravity;
                        State = NState;
                        LastGravity = Gravity;
                        rb.AddForce(Gravity * 2000 * Time.deltaTime, ForceMode2D.Force);
                    } else {
                        rb.AddForce(LastGravity * 2000 * Time.deltaTime, ForceMode2D.Force);
                    }
                }
            }
            if (rb.velocity == Vector2.zero) {
                rb.velocity = ToMove * Time.deltaTime;
            } else if (ToMove != Vector2.zero) {
                float cosa = (rb.velocity.x * ToMove.x + rb.velocity.y * ToMove.y) / rb.velocity.magnitude / ToMove.magnitude;
                rb.velocity = rb.velocity - rb.velocity.magnitude * cosa * ToMove.normalized + ToMove * Time.deltaTime;
            }
        }
    }
    public void stop()
    {
        if (ToMove != Vector2.zero && rb.velocity != Vector2.zero) {
            float cosa = (rb.velocity.x * ToMove.x + rb.velocity.y * ToMove.y) / rb.velocity.magnitude / ToMove.magnitude;
            rb.velocity = rb.velocity - rb.velocity.magnitude * cosa * ToMove.normalized;
        }
        ToMove = Vector2.zero;
    }
}
