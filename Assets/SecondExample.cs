using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Camera))]
public class SecondExample : MonoBehaviour
{
    private Camera _camera;
    private void Awake()
    {
        this.GetComponent<Camera>();
    }
   
}
