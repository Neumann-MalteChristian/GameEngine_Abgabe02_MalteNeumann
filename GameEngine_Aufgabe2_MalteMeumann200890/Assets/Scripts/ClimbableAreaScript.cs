using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClimbableAreaScript : MonoBehaviour
{
    


     void OnTriggerEnter(Collider other)
    {
      
        if (other.CompareTag("Player"))
        {
            Debug.Log(other.gameObject.name + " enter the area");
            
            Player script = other.GetComponent<Player>();
            
            if (script.getCurrentPlayerState() != PlayerState.Climb)
            {
               
               
                script.setCurrentPlayerState(PlayerState.Climb);
                
            }
        }
        //enable climb mode
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            Debug.Log(other.gameObject.name + " leave the area");
            Player script = other.GetComponent<Player>();

            if (script.getCurrentPlayerState() == PlayerState.Climb)
            {
               
                script.GetComponent<Rigidbody>().useGravity = true;
                script.setCurrentPlayerState(PlayerState.Normal);
            }
        }
       
        //disable climb mode
    }
}
