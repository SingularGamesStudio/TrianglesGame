using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BlockCollHolder : UnityEngine.MonoBehaviour
{
    [Header("System variAllBlockses")]
    public Block Parent;
    private void OnMouseUp()
    {
        Parent.click();
    }
}
