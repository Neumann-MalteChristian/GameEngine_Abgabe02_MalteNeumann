using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DeadArea : MonoBehaviour
{
    [SerializeField]
    private GameObject playerSpwan;
     void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            //Debug.Log("Enter Dead Area");
            //other.gameObject.transform.position = playerSpwan.transform.position;

           /// other.gameObject.transform.position = new Vector3(94, 10, 0);
        }
    }
}
