using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eunjung
{
    [SerializeField]
    public class MapLocation
    {

        public int x;
        public int z;

        public MapLocation(int _x, int _z)//������
        {
            x = _x;
            z = _z;
        }

        public Vector2 ToVector()
        {
            return new Vector2(x, z);
        }
        public static MapLocation operator +(MapLocation a, MapLocation b)
            => new MapLocation(a.x + b.x, a.z + b.z);

    }
    public class Maze : MonoBehaviour
    {
        public List<MapLocation> directions = new List<MapLocation>()
        {
             new MapLocation(1,0),
             new MapLocation(0,1),
             new MapLocation(-1,0),
             new MapLocation(0, -1)
        };

        public int width = 30;
        public int depth = 30;
        public byte[,] map;//�ʱ��� 2���� �迭
        public int scale = 6;
        public List<Vector3> hall = new List<Vector3>();
        public bool isSpawn;
        public GameObject objmap;

        // Start is called before the first frame update
        void Start()
        {
            objmap = GameObject.Find("Map");
            InitialiseMap();
            Generate();
            DrawMap();
            //StartCoroutine("SpawnEnemy");
        }

        void InitialiseMap()
        {
            map = new byte[width, depth];
            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    map[x, z] = 1; //1�� ��, 0�� ���
                }
            }
        }

        public virtual void Generate()//�ڵ尡 �ֵ縻�� ��� ����(����) ��ӹ��� �ֿ��� generator�� ����ϱ� ���� ������ �Լ�, ��ӹ��� �ְ� �������̵� �ؼ� ���� ��Ų��
        {
            /*

             */
        }

        void DrawMap()
        {
            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (map[x, z] == 1)
                    {
                        Vector3 pos = new Vector3(x * scale, 400, z * scale);
                        GameObject wall = GameObject.CreatePrimitive(PrimitiveType.Cube);
                        wall.transform.localScale = new Vector3(scale, scale, scale);
                        wall.transform.localPosition = pos;
                        wall.transform.parent = objmap.transform;
                    }
                    else if (map[x, z] == 0)
                    {
                        Vector3 pos = new Vector3(x * scale, 400, z * scale);
                        hall.Add(pos);
                    }

                }
            }
            for (int z = 0; z < depth; z++)
            {
                for (int x = 0; x < width; x++)
                {
                    if (map[x, z] == 1)
                    {
                        Vector3 pos = new Vector3(x * scale, 400, z * scale);
                        GameObject plane = GameObject.CreatePrimitive(PrimitiveType.Plane);
                        plane.transform.localScale = new Vector3(scale, scale, scale);
                        plane.transform.localPosition = new Vector3(pos.x, 400, pos.z);
                    }
                }
            }
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="x"></param>
        /// <param name="z"></param>
        /// <returns></returns>

        public int CountSquareNeighbours(int x, int z)
        {
            int count = 0;
            if (x <= 0 || x >= width - 1 || z <= 0 || z >= depth - 1)
            {
                return 5;
            }
            if (map[x - 1, z] == 0)
            {
                count++;
            }
            if (map[x + 1, z] == 0)
            {
                count++;
            }
            if (map[x, z + 1] == 0)
            {
                count++;
            }
            if (map[x, z - 1] == 0)
            {
                count++;
            }
            return count;
        }

        //IEnumerator SpawnEnemy()
        //{
        //    isSpawn = false;
        //    int number = Random.Range(0, hall.Count);
        //    GameObject Enemy = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        //    Enemy.transform.localScale = new Vector3(scale, scale, scale);
        //    Enemy.transform.localPosition = hall[number];
        //    hall.RemoveAt(number);
        //    yield return new WaitForSeconds(0.1f);
        //    if (hall.Count/100 <= 0)
        //    {
        //        yield return null;
        //    }
        //    StartCoroutine("SpawnEnemy");

        //}
    }
}
