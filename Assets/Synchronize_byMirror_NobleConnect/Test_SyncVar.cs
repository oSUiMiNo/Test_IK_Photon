using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Test_SyncVar : NetworkBehaviour
{
    [SyncVar]
    [SerializeField]
    float value = 0;

    [SerializeField]
    Slider slider;

    void Awake()
    {
        slider = GameObject.Find("Slider").GetComponent<Slider>();
        Debug.Log($"{slider.value}--------------------");
        transform.position = new Vector3(788, 181, 0);
        transform.parent = slider.gameObject.transform;
    }

    void Start()
    {
    
    }

    void Update()
    {
        value = slider.value;
    }
}
