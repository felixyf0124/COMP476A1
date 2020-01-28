using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MoveStrategy : MonoBehaviour
{
    /**
     * return velocity
     */
    public Vector3 seekKinematic(Vector3 pTar, Vector3 pCur, Vector3 vMax)
    {

        Vector2 pT = new Vector2(pTar.x, pTar.z);

        Vector2 pC = new Vector2(pCur.x, pCur.z);

        Vector2 vM = new Vector2(vMax.x, vMax.z);

        Vector2 vDir = pT - pC;

        //Normalnize
        vDir.Normalize();

        Vector2 ksVelocity = vM * vDir;

        return (new Vector3(ksVelocity.x, 0.0f, ksVelocity.y));
    }


    public void arriveKinematic(Vector3 pos, Vector3 tar)
    {

    }


    public void fleeKinematic()
    {

    }
}
