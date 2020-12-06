using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testTarget : MonoBehaviour
{
    public float health;
    public GameObject ui;
    private float maxLife;
    private GUIHandler uiHandler;
    List<GameObject> observerEnemies;

     void Awake()
    {
        observerEnemies = new List<GameObject>();
        maxLife = health;
        uiHandler = ui.GetComponent<GUIHandler>();
    }

    public void hitTarget(float hitpoints)
    {
        Debug.Log("Target hit");
        health -= hitpoints;
        
        if(health <= 0)
        {
            Debug.Log("Target Death");
            informAllObserver(TargetState.TargetDead);
            death();
        }

        uiHandler.updateLifeBar(health / maxLife);
    }

    private void death()
    {
        Destroy(this.gameObject);
    }


    public void subscribeEnemy(GameObject observerEnemy)
    {
        observerEnemies.Add(observerEnemy.gameObject);
    }
    public bool desubscribeEnemy(GameObject observerEnemy)
    {
        if (observerEnemies.Contains(observerEnemy))
        {
            observerEnemies.Remove(observerEnemy);
            return true;
        }
        else
        {
            return false;
        }
    }

    private void informAllObserver(TargetState state)
    {
        foreach(GameObject observer in observerEnemies)
        {
            observer.GetComponent<Enemy>().inform(state);
        }
    }
        
    
}
