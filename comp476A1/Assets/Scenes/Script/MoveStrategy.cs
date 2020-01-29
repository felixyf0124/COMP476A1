using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveStrategy : MonoBehaviour
{
    /**
     * return velocity
     */
    public Vector3 seekKinematic(Vector3 positionTarget, Vector3 positionCurrent, float velocityMax)
    {

        Vector2 pTar = new Vector2(positionTarget.x, positionTarget.z);

        Vector2 pCur = new Vector2(positionCurrent.x, positionCurrent.z);

        float vMax = velocityMax;

        Vector2 vDir = pTar - pCur;

        Vector2 ksVelocity = vMax * vDir.normalized;

        return (new Vector3(ksVelocity.x, positionCurrent.y, ksVelocity.y));
    }


    /**
     * return velocity
     */
    public Vector3 arriveKinematic(Vector3 positionTarget, Vector3 positionCurrent, float velocityMax, float rSat, float t2t, bool isI)
    {
        Vector2 pTar = new Vector2(positionTarget.x, positionTarget.z);

        Vector2 pCur = new Vector2(positionCurrent.x, positionCurrent.z);

        float vMax = velocityMax;

        Vector2 vDir = pTar - pCur;

        Vector2 ksVelocity = new Vector2(0.0f,0.0f);

        if (isI)
        {//I
            if (vDir.magnitude > rSat)
            {
                ksVelocity = vMax * vDir.normalized;
            }
        }
        else
        {//II
            ksVelocity = Mathf.Min(vMax, vDir.magnitude/t2t) * vDir.normalized;
        }

        return (new Vector3(ksVelocity.x, positionCurrent.y, ksVelocity.y));

    }

    /**
     * return velocity
     */
    public Vector3 fleeKinematic(Vector3 pTar, Vector3 pCur, float velocityMax)
    {
        return -seekKinematic(pTar, pCur, velocityMax);
    }
}
