using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FirstExample : MonoBehaviour
{
    [SerializeField] private int _numberOfItem;
    
    [HideInInspector] public int HidedenValue = 0;
    
    [SerializeField]
    [Range(-273f, 10000)] private float _temperature;
    
    [Tooltip("This is a tooltip. Don't touch me!")] 
    public int TestField = 10;
    
    [Header("NAME ON INSPECTOR")] public int Title;
    

    
    private void Start()
    {
        Debug.Log(_numberOfItem);
    }

    [ContextMenu("Your Name For function")]
    private void StartFromContextMenu()
    {
        Debug.LogAssertion("COOL!");
    }
}