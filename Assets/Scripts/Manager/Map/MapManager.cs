using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Rendering;

namespace ChenChen_Core
{
    [System.Serializable]
    public class Terrain
    {
        public HexagonObject hexagonObject;

        public Hexagon.Type type;

        [Range(0, 1)]
        public float probability = 1;
    }

    public class MapManager : SingletonMono<MapManager>
    {
        [Header("Grid")]
        public HexagonGrid HG;
        public int mapWidth = 10;
        public int mapHeight = 10;

        [Header("Hexagon")]
        public int initatieNumber = 10;     // ��ʼʱʵ��������Ȧ����
        public float createAnimationUpHeight = 0.5f;    // �������ɶ��������ĸ߶�
        public float createDelay = 0.01f;   // ����һȦ�����ʱ��

        protected override void Awake()
        {
            base.Awake();

            HG.InitHexagonMap(mapWidth, mapHeight);
        }

        /// <summary>
        /// �ҵ���Ŀ��������ˮ��
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Hexagon FindNearSeaHexagon(Vector3 pos)
        {
            Hexagon posHexagon = HG.FindHexagon(pos);

            if (posHexagon == null)
            {
                Debug.LogWarning("�õ��Ϊ��");
                return null;
            }

            HashSet<Hexagon> visited = new HashSet<Hexagon>();
            Queue<Hexagon> queue = new Queue<Hexagon>();
            queue.Enqueue(posHexagon);
            visited.Add(posHexagon);

            while (queue.Count > 0)
            {
                Hexagon hex = queue.Dequeue();

                if (hex.type == Hexagon.Type.water)
                {
                    return hex;
                }

                // û�ҵ������ھӽ�����
                foreach (var neibor in hex.neibor)
                {
                    // ���ھӺ��Ѿ�������Ĳ����ٽ���
                    if (neibor != null && !visited.Contains(neibor))
                    {
                        queue.Enqueue(neibor);
                        visited.Add(neibor);
                    }
                }
            }

            // ������ȫ��������Ȼû��ˮ��
            return null;
        }

        /// <summary>
        /// ����Ŀ��㷶Χ�ڵ�ˮ��
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="range">��Χ����Ȧ��</param>
        /// <returns></returns>
        public List<Hexagon> FindRangeSeaHexagon(Vector3 pos, int range)
        {
            Hexagon posHexagon = HG.FindHexagon(pos);

            if (posHexagon == null)
            {
                Debug.LogWarning("�õ��Ϊ��");
                return null;
            }

            List<Hexagon> result = new List<Hexagon>();
            result.Add(posHexagon);
            HashSet<Hexagon> visited = new HashSet<Hexagon>();
            visited.Add(posHexagon);
            Queue<Hexagon> nextQueue = new Queue<Hexagon>();
            nextQueue.Enqueue(posHexagon);


            for (int i = 0; i < range; i++)
            {
                List<Hexagon> thisRound = new List<Hexagon>();
                while (nextQueue.Count > 0)
                {
                    thisRound.Add(nextQueue.Dequeue());
                }

                foreach (var hex in thisRound)
                {
                    foreach (var neibor in hex.neibor)
                    {
                        if (neibor == null) continue;
                        if (visited.Contains(neibor)) { continue; }
                        if (neibor.type != Hexagon.Type.water) { continue; }
                        result.Add(neibor);
                        visited.Add(neibor);
                        nextQueue.Enqueue(neibor);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// ����Ŀ��㷶Χ�ڵĸ���
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public List<Hexagon> FindRangeHexagon(Vector3 pos, int range)
        {
            Hexagon posHexagon = HG.FindHexagon(pos);

            if (posHexagon == null)
            {
                Debug.LogWarning("�õ��Ϊ��");
                return null;
            }

            List<Hexagon> result = new List<Hexagon>();
            result.Add(posHexagon);
            HashSet<Hexagon> visited = new HashSet<Hexagon>();
            visited.Add(posHexagon);
            Queue<Hexagon> nextQueue = new Queue<Hexagon>();
            nextQueue.Enqueue(posHexagon);


            for (int i = 0; i < range; i++)
            {
                List<Hexagon> thisRound = new List<Hexagon>();
                while (nextQueue.Count > 0)
                {
                    thisRound.Add(nextQueue.Dequeue());
                }

                foreach (var hex in thisRound)
                {
                    foreach (var neibor in hex.neibor)
                    {
                        if (neibor == null) continue;
                        if (visited.Contains(neibor)) { continue; }
                        result.Add(neibor);
                        visited.Add(neibor);
                        nextQueue.Enqueue(neibor);
                    }
                }
            }

            return result;
        }

        /// <summary>
        /// ����һ��Բ�η�Χ������ļ���½��
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="range"> ��Χ </param>
        /// <param name="reNum"> �������� </param>
        /// <returns></returns>
        public Hexagon[] GetRandomLandHexagon(Vector3 pos, float range, int reNum)
        {
            if (!HG.IsOk)
            {
                Debug.LogError("��ͼ��δ��ʼ��");
                return null;
            }

            Hexagon posHexagon = HG.FindHexagon(pos);

            HashSet<Hexagon> visited = new HashSet<Hexagon>();
            Queue<Hexagon> queue = new Queue<Hexagon>();
            queue.Enqueue(posHexagon);
            visited.Add(posHexagon);

            List<Hexagon> result = new List<Hexagon>();// ��ŷ�Χ�����е�½�ظ���
            while (queue.Count > 0)
            {
                Hexagon hex = queue.Dequeue();

                if (Vector3.Distance(hex.center, pos) > range)
                {
                    continue;
                }

                if (hex.type == Hexagon.Type.grass)
                {
                    result.Add(hex);
                }

                // û�ҵ������ھӽ�����
                foreach (var neibor in hex.neibor)
                {
                    // ���ھ�,�Ѿ������,������Χ��,�Ĳ����ٽ���
                    if (neibor != null && !visited.Contains(neibor) && Vector3.Distance(neibor.center, pos) <= range)
                    {
                        queue.Enqueue(neibor);
                        visited.Add(neibor);
                    }
                }
            }

            if(reNum > result.Count)
            {
                Debug.LogError("���󣬸÷�Χ��û����ô��½�ظ���");
                return null;
            }

            // ������ȫ�����Ӻ�����б�������Ҫ������
            RandomList(result);

            return result.Take(reNum).ToArray();

            void RandomList(List<Hexagon> origin)
            {
                int n = origin.Count;
                for (int i = 0; i < n; i++)
                {
                    int j = UnityEngine.Random.Range(i, n);
                    if (!origin.TrySwap(i, j, out var error))
                    {
                        Debug.LogError(error);
                    }
                }
            }
        }
    }
}
