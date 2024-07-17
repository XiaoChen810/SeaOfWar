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
        public int initatieNumber = 10;     // 初始时实例化多少圈网格
        public float createAnimationUpHeight = 0.5f;    // 网格生成动画上升的高度
        public float createDelay = 0.01f;   // 创建一圈网格的时延

        protected override void Awake()
        {
            base.Awake();

            HG.InitHexagonMap(mapWidth, mapHeight);
        }

        /// <summary>
        /// 找到离目标点最近的水域
        /// </summary>
        /// <param name="pos"></param>
        /// <returns></returns>
        public Hexagon FindNearSeaHexagon(Vector3 pos)
        {
            Hexagon posHexagon = HG.FindHexagon(pos);

            if (posHexagon == null)
            {
                Debug.LogWarning("该点块为空");
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

                // 没找到则将其邻居进队列
                foreach (var neibor in hex.neibor)
                {
                    // 空邻居和已经进入过的不会再进入
                    if (neibor != null && !visited.Contains(neibor))
                    {
                        queue.Enqueue(neibor);
                        visited.Add(neibor);
                    }
                }
            }

            // 遍历完全部格子仍然没有水域
            return null;
        }

        /// <summary>
        /// 返回目标点范围内的水域
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="range">范围，按圈数</param>
        /// <returns></returns>
        public List<Hexagon> FindRangeSeaHexagon(Vector3 pos, int range)
        {
            Hexagon posHexagon = HG.FindHexagon(pos);

            if (posHexagon == null)
            {
                Debug.LogWarning("该点块为空");
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
        /// 返回目标点范围内的格子
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="range"></param>
        /// <returns></returns>
        public List<Hexagon> FindRangeHexagon(Vector3 pos, int range)
        {
            Hexagon posHexagon = HG.FindHexagon(pos);

            if (posHexagon == null)
            {
                Debug.LogWarning("该点块为空");
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
        /// 返回一个圆形范围内随机的几块陆地
        /// </summary>
        /// <param name="pos"></param>
        /// <param name="range"> 范围 </param>
        /// <param name="reNum"> 返回数量 </param>
        /// <returns></returns>
        public Hexagon[] GetRandomLandHexagon(Vector3 pos, float range, int reNum)
        {
            if (!HG.IsOk)
            {
                Debug.LogError("地图还未初始化");
                return null;
            }

            Hexagon posHexagon = HG.FindHexagon(pos);

            HashSet<Hexagon> visited = new HashSet<Hexagon>();
            Queue<Hexagon> queue = new Queue<Hexagon>();
            queue.Enqueue(posHexagon);
            visited.Add(posHexagon);

            List<Hexagon> result = new List<Hexagon>();// 存放范围内所有的陆地格子
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

                // 没找到则将其邻居进队列
                foreach (var neibor in hex.neibor)
                {
                    // 空邻居,已经进入过,超出范围的,的不会再进入
                    if (neibor != null && !visited.Contains(neibor) && Vector3.Distance(neibor.center, pos) <= range)
                    {
                        queue.Enqueue(neibor);
                        visited.Add(neibor);
                    }
                }
            }

            if(reNum > result.Count)
            {
                Debug.LogError("错误，该范围内没有那么多陆地格子");
                return null;
            }

            // 遍历完全部格子后打乱列表并返回需要的数量
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
