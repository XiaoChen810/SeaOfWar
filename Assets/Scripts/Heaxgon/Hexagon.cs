using ChenChen_Core;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Hexagon
{
    public enum Type
    {
        none,
        grass,
        water
    }

    public float radius;
    public Vector3 center;
    public GameObject obj = null;
    public Type type = Type.none;

    public List<Hexagon> neibor;

    public string key;
    public Vector3 upperRight;
    public Vector3 upperLeft;
    public Vector3 upperCenter;
    public Vector3 lowerRight;
    public Vector3 lowerLeft;
    public Vector3 lowerCenter;

    public Hexagon(Vector3 center, float radius, Type type)
    {
        this.center = center;
        this.radius = radius;
        this.type = type;
        this.neibor = new List<Hexagon>(new Hexagon[6]);  // 初始化邻居列表，包含6个元素

        key = HexagonGrid.TransPosToKey(center);

        // 计算边角点cos(30°) = 0.866
        upperCenter = center + new Vector3(0, 0, radius);
        lowerCenter = center + new Vector3(0, 0, -radius);
        upperLeft = center + new Vector3(-radius * 0.866f, 0, radius * 0.5f); 
        lowerLeft = center + new Vector3(-radius * 0.866f, 0, -radius * 0.5f);
        upperRight = center + new Vector3(radius * 0.866f, 0, radius * 0.5f);
        lowerRight = center + new Vector3(radius * 0.866f, 0, -radius * 0.5f);
    }

    public bool PointIsInternal(Vector3 point)
    {
        if (point.x < lowerLeft.x || point.x > lowerRight.x) return false;
        if (point.z > upperCenter.z || point.z < lowerCenter.z) return false;

        Vector3[] corners = new Vector3[] { upperRight, upperCenter, upperLeft, lowerLeft, lowerCenter, lowerRight };
        for (int i = 0; i < corners.Length; i++)
        {
            Vector3 current = corners[i];
            Vector3 next = corners[(i + 1) % corners.Length];
            Vector3 edge = next - current;
            Vector3 normal = Quaternion.Euler(0, -90, 0) * edge;
            Vector3 pointToCurrent = point - current;
            if (Vector3.Dot(pointToCurrent, normal) < 0) return false;
        }

        return true;
    }

}
