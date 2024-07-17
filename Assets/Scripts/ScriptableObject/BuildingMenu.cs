using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "BuildingMenu", menuName = "ScriptableObject/建筑菜单")]
public class BuildingMenu : ScriptableObject
{
    public List<BuildingDef> buildingsDefs;
}
