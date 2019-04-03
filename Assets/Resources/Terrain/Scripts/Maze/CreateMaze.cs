using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using Assets.Inventorys;

namespace Assets.Other
{
    public class CreateMaze : MonoBehaviour
    {
        public GameObject ground;
        public GameObject groundMaze;
        public GameObject wall;

        public int heightWall = 3;
        public int width = 41, height = 41;

        public static bool isCreated = false;

        private char[][] maze;
        private Vector3 startPositionMaze = new Vector3(10, 0, 10);
        private int countCell = 0;
        private Cell currentCell = new Cell(1, 1);
        private List<Cell> stack = new List<Cell>();


        /// ---------------------------------------------------------------

        void Start()
        {
            mazeArray();

            createMaze();
            createEntry(11, 1);
            addHeightWall();
            createTarget(height - 2, width - 2);
            isCreated = true;
        }


        /// ---------------------------------------------------------------

        //Создание входа
        void createEntry(int x, int z)
        {
            maze[x][z] = 's';
            bool perm = false;
            if (z == 1)
            {
                perm = true;
                z--;
            }
            else if (x == 1)
            {
                perm = true;
                x--;
            }
            if (z == width - 1)
            {
                perm = true;
                z++;
            }
            else if (x == height - 1)
            {
                perm = true;
                x++;
            }
            if (perm)
                maze[x][z] = 'e';
        }

        //Создание Цели
        void createTarget(int x, int z)
        {
            maze[x][z] = 't';
        }

        /// ---------------------------------------------------------------

        //Прорисовка лабиринта
        void createMaze()
        {
            maze[currentCell.x][currentCell.z] = 'v';
            MyTerrain.CreateBlock((int)Id.Block.Wood, new Vector3(currentCell.x + startPositionMaze.x, startPositionMaze.y + startPositionMaze.y, currentCell.z + startPositionMaze.z));
            for (int i = 0; i < height; i++)
            {
                for (int j = 0; j < width; j++)
                {
                    if (maze[i][j] == 'c')
                        countCell++;
                }
            }
            while (countCell != 0)
            {
                CellString neighbourCell = getNeighbours(currentCell);
                if (neighbourCell.size != 0)
                {
                    int randNeighbor = (int)Mathf.Floor(Random.Range(0, neighbourCell.size));
                    Cell nextItem = neighbourCell.arr[randNeighbor];
                    removeWall(currentCell, nextItem);
                    countCell--;
                    currentCell = nextItem;
                    stack.Add(currentCell);
                }
                else if (stack.Count != 0)
                {
                    stack.RemoveAt(stack.Count - 1);
                    if (stack.Count != 0)
                    {
                        currentCell = stack[stack.Count - 1];
                    }
                }
            }
        }

        //Высота стен
        void addHeightWall()
        {
            Transform parent = gameObject.transform.Find("Walls");
            for (int l = 1; l <= heightWall; l++)
            {
                for (int x = 0; x < height; x++)
                {
                    for (int z = 0; z < width; z++)
                    {
                        if (maze[x][z] == 'w')
                        {
                            MyTerrain.CreateBlock((int)Id.Block.RedHoneyCumb, new Vector3(x + startPositionMaze.x, l + startPositionMaze.y, z + startPositionMaze.z));
                        }
                    }
                }
            }
        }

        /// ---------------------------------------------------------------

        //Подготовка текстового лабиринта
        void mazeArray()
        {
            if (width % 2 == 0)
                width++;
            if (height % 2 == 0)
                height++;
            maze = new char[height][];
            for (int i = 0; i < height; i++)
            {
                maze[i] = new char[width];
                for (int j = 0; j < width; j++)
                {
                    if ((i % 2 != 0 && j % 2 != 0) && (i < height - 1 && j < width - 1))
                        maze[i][j] = 'c';
                    else
                        maze[i][j] = 'w';
                }
            }
        }

        //Поиск пустых соседей ячейки лабиринта
        CellString getNeighbours(Cell c)
        {
            Cell up = new Cell(c.x, c.z - 2);
            Cell rt = new Cell(c.x + 2, c.z);
            Cell dw = new Cell(c.x, c.z + 2);
            Cell lt = new Cell(c.x - 2, c.z);
            Cell[] d = new Cell[] { dw, rt, up, lt };
            int size = 0;

            CellString cells = new CellString();
            cells.arr = new List<Cell>();

            for (int i = 0; i < 4; i++)
            {
                if (d[i].x > 0 && d[i].x < height && d[i].z > 0 && d[i].z < width)
                {
                    char mazeCellCurrent = maze[d[i].x][d[i].z];
                    if (mazeCellCurrent != 'w' && mazeCellCurrent != 'v')
                    {
                        cells.arr.Add(d[i]);
                        size++;
                    }
                }
            }
            cells.size = size;
            return cells;
        }

        //Установка ячеек лабиринта
        void removeWall(Cell first, Cell second)
        {
            int x = second.x - first.x;
            int y = second.z - first.z;
            int addX, addY;
            Cell target = new Cell();

            addX = (x != 0) ? (x / Mathf.Abs(x)) : 0;
            addY = (y != 0) ? (y / Mathf.Abs(y)) : 0;

            target.x = first.x + addX;
            target.z = first.z + addY;

            maze[second.x][second.z] = 'v';
            maze[target.x][target.z] = 'v';

            Transform parent = gameObject.transform.Find("Ground");

            MyTerrain.CreateBlock((int)Id.Block.Wood, new Vector3(second.x + startPositionMaze.x, startPositionMaze.y, second.z + startPositionMaze.z));
            MyTerrain.CreateBlock((int)Id.Block.Wood, new Vector3(target.x + startPositionMaze.x, startPositionMaze.y, target.z + startPositionMaze.z));
        }

        /// ---------------------------------------------------------------

        public struct Cell
        {
            public int x;
            public int z;
            public Cell(int x, int z)
            {
                this.x = x;
                this.z = z;
            }
        }

        struct CellString
        {
            public int size;
            public List<Cell> arr;
        }

    }

}
