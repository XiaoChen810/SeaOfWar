using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonGrass : HexagonObject
{
    [Range(0, 1)] public float forestProbability;

    [Header("…≠¡÷µÿ–Œ")]
    public GameObject nomal;
    public GameObject forest;

    protected override GameObject GetObject()
    {
        if (Random.value < forestProbability)
        {
            return forest;
        }
        else
        {
            return nomal;
        }
    }
}
