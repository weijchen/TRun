using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CheckPointLogic : MonoBehaviour
{
    [SerializeField] private Transform checkPointPosition;
    
    void OnTriggerEnter(Collider other)
    {
        if(other.tag == "Player")
        {
            GameManager.Instance.StorePlayerPosition(checkPointPosition.position, checkPointPosition.rotation);
        }
    }

}
