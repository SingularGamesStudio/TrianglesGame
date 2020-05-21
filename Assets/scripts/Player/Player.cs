using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Vuforia;

public class Player : MonoBehaviour
{
    //[Header("System variAllBlockses")]
    PhysicsO MyPhys;

    [Header("Parameters")]
    [Range(0, 200)]
    public float Speed;
    void Start()
    {
        MyPhys = gameObject.GetComponent<PhysicsO>();
    }
    Vector2 toVector2(Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }
    void Update()
    {
        if (Input.GetKey(KeyCode.D)) {
            MyPhys.ToMove = toVector2(transform.right) * Speed;
        } else if (Input.GetKey(KeyCode.A)) {
            MyPhys.ToMove = -toVector2(transform.right) * Speed;
        } else {
            MyPhys.stop();
        }
    }
}
