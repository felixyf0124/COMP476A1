using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveStrategy : MonoBehaviour
{
    /**
     * return velocity
     */
    public Vector3 seekKinematic(Vector3 positionTarget, Vector3 positionCurrent, Vector3 velocityMax)
    {

        Vector2 pTar = new Vector2(positionTarget.x, positionTarget.z);

        Vector2 pCur = new Vector2(positionCurrent.x, positionCurrent.z);

        Vector2 vMax = new Vector2(velocityMax.x, velocityMax.z);

        Vector2 vDir = pTar - pCur;

        //Normalnize
        vDir.Normalize();

        Vector2 ksVelocity = vMax * vDir;

        return (new Vector3(ksVelocity.x, positionCurrent.y, ksVelocity.y));
    }


    public Vector3 arriveKinematic(Vector3 positionTarget, Vector3 positionCurrent, Vector3 velocityMax, float rSat)
    {
        Vector2 pTar = new Vector2(positionTarget.x, positionTarget.z);

        Vector2 pCur = new Vector2(positionCurrent.x, positionCurrent.z);

        Vector2 vMax = new Vector2(velocityMax.x, velocityMax.z);

        Vector2 vDir = pTar - pCur;

        Vector2 ksVelocity = new Vector2(0.0f,0.0f);

        if (vDir.magnitude > rSat)
        {
            vDir.Normalize();

            ksVelocity = vMax * vDir;
        }

        return (new Vector3(ksVelocity.x, positionCurrent.y, ksVelocity.y));

    }


    public Vector3 fleeKinematic(Vector3 pTar, Vector3 pCur, Vector3 vMax)
    {
        return -seekKinematic(pTar, pCur, vMax);
    }
}
