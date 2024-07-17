using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Core
{
    /// <summary>
    /// ʹ��A star�㷨����·��
    /// </summary>
    public class SeekManager : SingletonMono<SeekManager>
    {
        public class Node
        {
            public Hexagon hex;
            public float f;
            public float h;
            public float g;
            public Node parent;

            public Node(Hexagon hex, float g, float h, Node parent = null)
            {
                this.hex = hex;
                this.g = g;
                this.h = h;
                f = g + h;
                this.parent = parent;
            }
        }

        public HexagonGrid HG;

        public List<Vector3> GetPath(Vector3 startPosition, Vector3 endPosition, Hexagon.Type obstacle = Hexagon.Type.grass, bool useSmoothPath = true)
        {
            // ת��
            HG = MapManager.Instance.HG;

            Hexagon start = HG.FindHexagon(startPosition);
            Hexagon end = HG.FindHexagon(endPosition);

            List<Vector3> path = SeekPath(start, end, obstacle);

            if (path == null)
            {
                Debug.LogWarning("δ�ҵ�·��");
                return path;
            }

            if (useSmoothPath && path.Count > 2)
            {
                path = SmoothPath(path);
            }

            return path;
        }

        private List<Vector3> SmoothPath(List<Vector3> path)
        {
            List<Vector3> smoothPath = new List<Vector3>();

            for (int i = 0; i < path.Count - 1; i++)
            {
                Vector3 p0 = path[Mathf.Max(i - 1, 0)];
                Vector3 p1 = path[i];
                Vector3 p2 = path[Mathf.Min(i + 1, path.Count - 1)];
                Vector3 p3 = path[Mathf.Min(i + 2, path.Count - 1)];

                smoothPath.Add(p1);

                for (int t = 1; t <= 4; t++)
                {
                    float s = t / 4f;
                    Vector3 point = 0.5f * (
                        2 * p1 +
                        (-p0 + p2) * s +
                        (2 * p0 - 5 * p1 + 4 * p2 - p3) * s * s +
                        (-p0 + 3 * p1 - 3 * p2 + p3) * s * s * s);
                    smoothPath.Add(point);
                }
            }

            smoothPath.Add(path[path.Count - 1]);

            return smoothPath;
        }

        public List<Vector3> SeekPath(Hexagon start, Hexagon end, Hexagon.Type obstacle)
        {
            HashSet<Node> openSet = new();
            HashSet<Node> closeSet = new();

            Node startNode = new Node(start, 0, Heuristic(start, end));
            openSet.Add(startNode);

            while (openSet.Count > 0)
            {
                // ��ȡ������С�Ľڵ�
                Node currentNode = GetMinCostNode(openSet);

                // �жϸýڵ��Ƿ����յ�
                if (currentNode.hex == end)
                {
                    return ReconstructPath(currentNode);
                }

                // �������ƶ������ж��б�
                openSet.Remove(currentNode);
                closeSet.Add(currentNode);

                // ���ڵ���ھ�
                foreach (Hexagon neighbor in currentNode.hex.neibor)
                {
                    // ������
                    if (neighbor == null)
                    {
                        continue;
                    }

                    // �ϰ�������
                    if (neighbor.type == obstacle)
                    {
                        continue;
                    }

                    // �ж�closeSet���Ƿ��Ѿ����ڸýڵ�
                    if (SetContains(closeSet, neighbor))
                    {
                        continue;
                    }

                    float tentative_g = currentNode.g + Distance(currentNode.hex, neighbor);

                    // �ж������б������޸ýڵ㣬���û��������ӣ�����������
                    if (!SetContains(openSet, neighbor))
                    {
                        Node neighborNode = new Node(neighbor, tentative_g, Heuristic(neighbor, end), currentNode);
                        openSet.Add(neighborNode);
                    }
                    else
                    {
                        Node existingNode = GetNodeFromSet(openSet, neighbor);
                        if (tentative_g < existingNode.g)
                        {
                            existingNode.g = tentative_g;
                            existingNode.f = existingNode.g + existingNode.h;
                            existingNode.parent = currentNode;
                        }
                    }
                }
            }

            return null;
        }

        private static Node GetMinCostNode(HashSet<Node> openSet)
        {
            float min = float.MaxValue;
            Node result = null;
            foreach (Node node in openSet)
            {
                if (node.f < min)
                {
                    min = node.f;
                    result = node;
                }
            }
            return result;
        }

        private static bool SetContains(HashSet<Node> Set, Hexagon hexagon)
        {
            foreach (Node node in Set)
            {
                if (node.hex == hexagon)
                {
                    return true;
                }
            }
            return false;
        }

        private static float Heuristic(Hexagon a, Hexagon b)
        {
            return Vector3.Distance(a.center, b.center);
        }

        private static float Distance(Hexagon a, Hexagon b)
        {
            return Vector3.Distance(a.center, b.center);
        }

        private List<Vector3> ReconstructPath(Node endNode)
        {
            List<Vector3> path = new List<Vector3>();
            Node currentNode = endNode;
            while (currentNode != null)
            {
                path.Add(currentNode.hex.center);
                currentNode = currentNode.parent;
            }
            path.Reverse();
            return path;
        }

        private Node GetNodeFromSet(HashSet<Node> set, Hexagon hexagon)
        {
            foreach (var n in set)
            {
                if (n.hex == hexagon)
                {
                    return n;
                }
            }
            return null;
        }
    }
}