using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HexagonRiver : HexagonObject
{
    public List<GameObject> rulerObjs;

    protected override GameObject GetObject()
    {
        if (rulerObjs.Count > 0)
        {
            return rulerObjs[0];
        }
        return null;
    }
}
