using UnityEngine;

[CreateAssetMenu(fileName = "BoatDef", menuName = "ScriptableObject/船定义")]
public class BoatDef : ScriptableObject
{
    public new string name;

    public GameObject prefab;

    public int maxHealth;               // 生命上限

    public int damage;                  // 攻击力

    public int attackRange;             // 攻击距离

    public int defend;                  // 防御力

    public int costForProduce;          // 建筑所需的工作量

    public int costCoalOneStep = 1;     // 移动一步需要消耗的燃料

    public int needTechnologyLevel = 0; // 所需科技等级
}
