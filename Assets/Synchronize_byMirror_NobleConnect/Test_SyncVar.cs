using Mirror;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Networking;

public class Test_SyncVar : NetworkBehaviour
{
    [SyncVar]
    float value = 0;

    [SerializeField]
    Slider slider;

    TextMeshProUGUI textMeshProUGUI;

    void Awake()
    {
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
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
        if (isLocalPlayer)
        {
            Debug.Log("ローカルプレイヤー");
            A();
        }
        textMeshProUGUI.text = value.ToString();
    }

    [Command]
    void A()
    {
        value = slider.value;
    }
}
