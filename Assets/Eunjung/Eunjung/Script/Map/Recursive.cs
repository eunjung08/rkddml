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
            MovePlayer.transform.position = new Vector3(x * scale, 1,y * scale);
            GameObject.Find("Main Camera").GetComponent<FollowCamera>().Set();

            GameObject MoveEnemy = GameObject.Find("GameManager").GetComponent<GameManager>().CrateMoveEnemy();
        }

        void Generate(int x, int z)
        {
            //4방위 주 2방위 이상이 복도일경우 || 자신이 복도일 때를 추가
            if (CountSquareNeighbours(x, z) >= 2 || map[x, z] == 0)
            {
                return;
            }

            map[x, z] = 0; //복도
            directions.Shuffle();//4방위 섞기

            Generate(x + directions[0].x, z + directions[0].z); //6.5
            Generate(x + directions[1].x, z + directions[1].z); //5.6
            Generate(x + directions[2].x, z + directions[2].z); //4.5
            Generate(x + directions[3].x, z + directions[3].z); //5.4
        }
    }
}
