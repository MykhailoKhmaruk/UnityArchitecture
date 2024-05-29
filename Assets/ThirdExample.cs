using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[DisallowMultipleComponent]
// [ExecuteAlways]
[ExecuteInEditMode]
public class ThirdExample : MonoBehaviour
{
    private int _num = 0;
    private void Update()
    {
        Debug.Log(_num++);
    }
}
