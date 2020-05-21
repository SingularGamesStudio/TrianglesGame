using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCollHolder : MonoBehaviour
{
    [Header("System variAllBlockses")]
    public Block Parent;
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
        Parent.click();
    }
}
