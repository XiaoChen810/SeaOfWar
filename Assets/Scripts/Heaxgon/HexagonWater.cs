using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonWater : HexagonObject
{
    [Header("Ë®¿é")]
    public GameObject water;

    [Header("HighLight")]
    public Color moveColor;
    public Color attackColor;
    public GameObject hightLight;
    public MeshRenderer mr;

    protected override GameObject GetObject()
    {
        return water;
    }

    public void Open(bool juggle)
    {
        mr.material.color = juggle ? moveColor : attackColor;
        hightLight.SetActive(true);      
    }

    public void Close()
    {
        hightLight?.SetActive(false);
    }
}
