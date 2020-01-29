using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LastFrozen : MonoBehaviour
{
    public static List<GameObject> lastFrozenList;

    private void Start()
    {
        lastFrozenList = new List<GameObject>();
    }
    void Update()
    {
        lastFrozenList.RemoveAll(obj => obj.tag != "frozen");
    }
}
