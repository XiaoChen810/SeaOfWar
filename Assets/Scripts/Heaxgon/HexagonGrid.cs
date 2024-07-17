using System.Collections.Generic;
using UnityEngine;

namespace ChenChen_Core
{
    public enum State
    {
        spare,
        doing,
    }

    public class HexagonGrid : MonoBehaviour
    {
        [Header("网格参数")]
        public float hexagonRadius = 0.6f;

        [Header("Terrain")]
        public List<Terrain> terrainList;
        public float lacunarrty = 0.08f;
        public float randomOffset = 0;
        public int seed = 2003810;

        [Header("FogOfWar")]
        public FogOfWar fogOfWar;

        [Header("检测鼠标点击的图层")]
        public LayerMask ground;

        [Header("状态")]
        public State state = State.spare;
        public bool IsOk;   // 已经初始化


        // 使用字符串防止浮点数差异
        private Dictionary<string, Hexagon> grids = new();
        private HashSet<string> viewed = new HashSet<string>();

        /// <summary>
        /// 初始化网格地图，给Dictionary赋值
        /// </summary>
        /// <param name="width"></param>
        /// <param name="height"></param>
        public void InitHexagonMap(float width, float height)
        {
            UnityEngine.Random.InitState(seed);
            randomOffset = UnityEngine.Random.Range(-1000, 1000);

            string key = TransPosToKey(Vector3.zero);
            Hexagon oringin = new Hexagon(Vector3.zero, hexagonRadius, JoggleType(Vector3.zero));
            grids.Add(key, oringin);
            CreateHexagonObject(Vector3.zero);

            Queue<Hexagon> queue = new Queue<Hexagon>();

            queue.Enqueue(oringin);

            while (queue.Count > 0)
            {
                Hexagon cur = queue.Dequeue();

                List<Hexagon> hex_new = LinkNeighborsAndReturnNew(cur);

                foreach (Hexagon it in hex_new)
                {
                    if (InRange(it.center))
                    {
                        queue.Enqueue(it);
                    }
                }
            }

            IsOk = true;

            bool InRange(Vector3 centerPosition)
            {
                if (centerPosition.x > width || centerPosition.x < -width) return false;
                if (centerPosition.z > height || centerPosition.z < -height) return false;
                return true;
            }
        }

        /// <summary>
        /// 清除迷雾
        /// </summary>
        /// <param name="origin"></param>
        public void SeeFlog(Vector3 origin, float radius)
        {
            // 生成出来时将周围迷雾清空
            fogOfWar.ClearFog(origin, radius);
        }

        /// <summary>
        /// 找到对应的六边形网格，判断这个点在哪个网格内部
        /// </summary>
        /// <param name="position"></param>
        /// <returns></returns>
        public Hexagon FindHexagon(Vector3 position)
        {
            foreach (var hex in grids.Values)
            {
                if (hex.PointIsInternal(position))
                {
                    return hex;
                }
            }

            return null;
        }

        /// <summary>
        /// 找到鼠标指向的六边形网格
        /// </summary>
        /// <returns></returns>
        public Hexagon FindHexagon_MousePoint()
        {
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out RaycastHit hit, Mathf.Infinity, ground))
            {
                Vector3 point = hit.point;
                return FindHexagon(point);
            }
            else
            {
                Debug.Log("不在地面范围");
            }
            return null;
        }

        private List<Hexagon> LinkNeighborsAndReturnNew(Hexagon hex)
        {
            Vector3 pos = hex.center;

            float offsetX = hexagonRadius * 0.866f;
            float offsetZ = hexagonRadius * 1.5f;

            Vector3[] directions = new Vector3[]
            {
            new Vector3(pos.x + offsetX, 0, pos.z + offsetZ),
            new Vector3(pos.x + 2 * offsetX, 0, pos.z),
            new Vector3(pos.x + offsetX, 0, pos.z - offsetZ),
            new Vector3(pos.x - offsetX, 0, pos.z - offsetZ),
            new Vector3(pos.x - 2 * offsetX, 0, pos.z),
            new Vector3(pos.x - offsetX, 0, pos.z + offsetZ)
            };

            List<Hexagon> result = new List<Hexagon>();

            for (int i = 0; i < directions.Length; i++)
            {
                Hexagon neibor = null;
                if (AddHexagonToGridsDictionary(directions[i], out neibor))
                {
                    result.Add(neibor);
                }
                hex.neibor[i] = neibor;
                neibor.neibor[(i + 3) % 6] = hex;
            }

            return result;
        }

        private bool AddHexagonToGridsDictionary(Vector3 pos, out Hexagon hexagon)
        {
            string key = TransPosToKey(pos);
            if (!grids.ContainsKey(key))
            {
                hexagon = new Hexagon(pos, hexagonRadius, JoggleType(pos));
                grids.Add(key, hexagon);
                CreateHexagonObject(pos);
                return true;
            }
            else
            {
                hexagon = grids[key];
                return false;
            }
        }

        //生成网格，实例化这个位置的实体
        private void CreateHexagonObject(Vector3 pos)
        {
            string key = TransPosToKey(pos);

            if (viewed.Contains(key)) return;

            if (grids.TryGetValue(key, out var hex))
            {
                GameObject prefab = GetObject(pos);
                if (prefab != null)
                {
                    hex.obj = prefab;
                    viewed.Add(key);
                }
                else
                {
                    Debug.LogWarning("CreateHexagon Error: Instantiation returned null");
                }
            }
            else
            {
                Debug.LogWarning("CreateHexagon Error: Hexagon not found in grid");
            }
        }

        private GameObject GetObject(Vector3 pos)
        {
            GameObject prefab = JoggleTerrain(pos);
            if (prefab == null)
            {
                return null;
            }
            GameObject go = Instantiate(prefab, pos, Quaternion.identity, transform);
            return go;
        }

        // 根据柏林噪声判断该点属于那种地形，返回一个对应地形物件
        private GameObject JoggleTerrain(Vector3 pos)
        {
            if (terrainList.Count <= 0)
            {
                Debug.LogWarning("TerrainList 不能为空");
                return null;
            }

            float noise = Mathf.PerlinNoise(pos.x * lacunarrty + randomOffset, pos.z * lacunarrty + randomOffset);

            for (int i = 0; i < terrainList.Count; i++)
            {
                if (terrainList[i].probability >= noise)
                {
                    return terrainList[i].hexagonObject.gameObject;
                }
            }

            return terrainList[terrainList.Count - 1].hexagonObject.gameObject;
        }

        // 根据柏林噪声判断该点属于那种地形，返回一个对应网格类型
        public Hexagon.Type JoggleType(Vector3 pos)
        {
            if (terrainList.Count <= 0)
            {
                Debug.LogWarning("TerrainList 不能为空");
                return Hexagon.Type.none;
            }

            float noise = Mathf.PerlinNoise(pos.x * lacunarrty + randomOffset, pos.z * lacunarrty + randomOffset);

            for (int i = 0; i < terrainList.Count; i++)
            {
                if (terrainList[i].probability >= noise)
                {
                    return terrainList[i].type;
                }
            }

            return Hexagon.Type.none;
        }
        public static string TransPosToKey(Vector3 pos)
        {
            string key = pos.x.ToString("0.##") + pos.y.ToString("0.##") + pos.z.ToString("0.##");
            return key;
        }
    }
}