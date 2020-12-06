using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FieldOfView : MonoBehaviour
{   [Range(2,10)]
    public float viewRadius;
    [Range(0,360)]
    public float viewAngle;
    public LayerMask targetMask;
    public LayerMask obstacleMask;
    private Enemy enemyScript;

    public List<Transform> listOfVisibleTargets = new List<Transform>();

     void Start()
    {
        StartCoroutine("FindTargetWithDelay", .2f);
        enemyScript= GetComponent<Enemy>();

        Debug.Log(enemyScript);
    }

    IEnumerator FindTargetWithDelay(float delay)
    {
        while (true)
        {
            yield return new WaitForSeconds(delay);
            FindVisibleTargets();
        }
    }
    void FindVisibleTargets()
    {
        listOfVisibleTargets.Clear();
        Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);
        for(int i =0; i < targetsInViewRadius.Length; i++)
        {
            Transform target = targetsInViewRadius[i].transform;
            Vector3 dirToTarget = (target.position - transform.position).normalized;

            if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
            {
                float dstToTarget = Vector3.Distance(transform.position, target.position);

                if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                {
                    listOfVisibleTargets.Add(target);
                }
            }
           

        }

        if (this.listOfVisibleTargets.Count == 0)
        {
           
           // Debug.Log(this.name + "List is empty, also no target in view angle");
            if (enemyScript.getCurrentState() == EnemyState.Verfolgen)
            {
                enemyScript.resetTarget();
            }
        }
        else if (this.listOfVisibleTargets.Count > 0)
        {
            //Debug.Log(this.name + "ListCount " + listOfVisibleTargets.Count);
            enemyScript.enemyIsSeen(listOfVisibleTargets.ToArray());
        }


    }

    public Vector3 DirFromAngle(float pAngleInDegrees,bool angleIsGlobal)
    {
        if (!angleIsGlobal)
        {
            pAngleInDegrees += transform.eulerAngles.y;
        }
        return new Vector3(Mathf.Sin(pAngleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(pAngleInDegrees * Mathf.Deg2Rad));
    }
}
