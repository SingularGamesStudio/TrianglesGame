using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class blockCollHolder : MonoBehaviour
{
    [Header("System variables")]
    public block par;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    private void OnMouseUp()
    {
        par.Click();
    }
}
