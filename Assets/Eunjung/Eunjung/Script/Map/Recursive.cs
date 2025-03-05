using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Eunjung
{
    public class Recursive : Maze
    {
        // Start is called before the first frame update
        public override void Generate()
        {
            Generate(5, 5);

            int x = Random.Range(0, map.GetLength(0));
            int y = Random.Range(0, map.GetLength(1));
            int cnt = 0;
            while(map[x, y] == 1)
            {
                x = Random.Range(0, map.GetLength(0));
                y = Random.Range(0, map.GetLength(1));
                cnt++;
                if(cnt > 100)
                {
                    Debug.LogError("error");
                }
            }
            GameObject MovePlayer = GameObject.Find("GameManager").GetComponent<GameManager>().CrateMovePlayer();
            MovePlayer.transform.position = new Vector3(x * scale, 400,y * scale);
            GameObject.Find("Main Camera").GetComponent<FollowCamera>().Set();

            //for (int i = 0; i < 10; i++)
            //{
                int xi = Random.Range(0, map.GetLength(0));
                int yi = Random.Range(0, map.GetLength(1));
                int cnti = 0;
                while (map[xi, yi] == 1 || (xi == x && yi == y))
                {
                    xi = Random.Range(0, map.GetLength(0));
                    yi = Random.Range(0, map.GetLength(1));
                    cnti++;
                    if (cnti > 100)
                    {
                        Debug.LogError("error");
                    }
                }
                GameObject MoveEnemy = GameObject.Find("GameManager").GetComponent<GameManager>().CrateMoveEnemy();
                MoveEnemy.transform.position = new Vector3(xi * scale, 400, yi * scale);
            //}
        }

        void Generate(int x, int z)
        {
            //4���� �� 2���� �̻��� �����ϰ�� || �ڽ��� ������ ���� �߰�
            if (CountSquareNeighbours(x, z) >= 2 || map[x, z] == 0)
            {
                return;
            }

            map[x, z] = 0; //����
            directions.Shuffle();//4���� ����

            Generate(x + directions[0].x, z + directions[0].z); //6.5
            Generate(x + directions[1].x, z + directions[1].z); //5.6
            Generate(x + directions[2].x, z + directions[2].z); //4.5
            Generate(x + directions[3].x, z + directions[3].z); //5.4
        }
    }
}
