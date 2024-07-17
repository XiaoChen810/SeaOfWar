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
        [Header("�������")]
        public float hexagonRadius = 0.6f;

        [Header("Terrain")]
        public List<Terrain> terrainList;
        public float lacunarrty = 0.08f;
        public float randomOffset = 0;
        public int seed = 2003810;

        [Header("FogOfWar")]
        public FogOfWar fogOfWar;

        [Header("����������ͼ��")]
        public LayerMask ground;

        [Header("״̬")]
        public State state = State.spare;
        public bool IsOk;   // �Ѿ���ʼ��


        // ʹ���ַ�����ֹ����������
        private Dictionary<string, Hexagon> grids = new();
        private HashSet<string> viewed = new HashSet<string>();

        /// <summary>
        /// ��ʼ�������ͼ����Dictionary��ֵ
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
        /// �������
        /// </summary>
        /// <param name="origin"></param>
        public void SeeFlog(Vector3 origin, float radius)
        {
            // ���ɳ���ʱ����Χ�������
            fogOfWar.ClearFog(origin, radius);
        }

        /// <summary>
        /// �ҵ���Ӧ�������������ж���������ĸ������ڲ�
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
        /// �ҵ����ָ�������������
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
                Debug.Log("���ڵ��淶Χ");
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

        //��������ʵ�������λ�õ�ʵ��
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

        // ���ݰ��������жϸõ��������ֵ��Σ�����һ����Ӧ�������
        private GameObject JoggleTerrain(Vector3 pos)
        {
            if (terrainList.Count <= 0)
            {
                Debug.LogWarning("TerrainList ����Ϊ��");
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

        // ���ݰ��������жϸõ��������ֵ��Σ�����һ����Ӧ��������
        public Hexagon.Type JoggleType(Vector3 pos)
        {
            if (terrainList.Count <= 0)
            {
                Debug.LogWarning("TerrainList ����Ϊ��");
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