using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

public class MarkerController : MonoBehaviour
{
    private GameObject IKTarget;
    private GameObject part;

    void Start()
    {
        IKTarget = GameObject.Find("IKMarker");
        part = GameObject.Find("J_Bip_R_Hand");
    }

    void Update()
    {
        IKTarget.transform.position = part.transform.position;
        //IKTarget.transform.rotation = part.transform.rotation;
    }
}
