using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Collector : MonoBehaviour
{
    [SerializeField] private GameObject[] collect;
    public GameObject[] getCollect()
    {
        return collect;
    }
}
