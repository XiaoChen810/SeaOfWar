using UnityEngine;

[CreateAssetMenu(fileName = "BuildingDef", menuName = "ScriptableObject/建筑定义")]
public class BuildingDef : ScriptableObject
{
    public new string name;

    public string description;

    public Sprite preview;

    public GameObject prefab;
}