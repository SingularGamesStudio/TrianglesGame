using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Vuforia;

public class Player : MonoBehaviour
{
    //[Header("System variables")]
    PhysicsO myphys;

    [Header("Parameters")]
    [Range(0, 200)]
    public float speed;
    // Start is called before the first frame update
    void Start()
    {
        myphys = gameObject.GetComponent<PhysicsO>();
    }
    Vector2 tov2(Vector3 v)
    {
        return new Vector2(v.x, v.y);
    }
    void flip()
    {
    }
    // Update is called once per frame
    void Update()
    {
        if (Input.GetKey(KeyCode.D)) {
            flip();
            myphys.tomove = tov2(transform.right) * speed;
        } else if (Input.GetKey(KeyCode.A)) {
            flip();
            myphys.tomove = -tov2(transform.right) * speed;
        } else {
            myphys.stop();
        }
    }
}
