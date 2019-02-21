using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MineSweeper2D
{
    public class Grid : MonoBehaviour
    {
        public GameObject tilePrefab;
        public int width = 10, height = 10;
        public float spacing = .155f;

        private Tile[,] tiles;

        private void Start()
        {
            GenerateTiles();
        }
        Tile SpawnTile(Vector3 pos)
        {
            //clones the tile prefab
            GameObject clone = Instantiate(tilePrefab);

           
            clone.transform.position = pos;

            Tile currentTile = clone.GetComponent<Tile>();
            
            return currentTile;
        }
        private void Update()
        {
            //Call mouse over function
            MouseOver();
        }
        void GenerateTiles()
        {
            //create a new 2d array of size width and height
            tiles = new Tile[width, height];
            //loop through entire tile list
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    //store half size for later use
                    Vector2 halfSize = new Vector2(width * 0.5f, height * 0.5f);
                    //pivot tiles around grid
                    Vector2 pos = new Vector2(x - halfSize.x, y - halfSize.y);

                    //apply offset
                    Vector2 offset = new Vector2(.5f, .5f);
                    pos += offset;

                    //apply spacing
                    pos *= spacing;
                    //spawn the tile
                    Tile tile = SpawnTile(pos);
                    //set transform of the tiles to the grid
                    tile.transform.SetParent(transform);
                    //store its array coords
                    tile.x = x;
                    tile.y = y;
                    //stores tile in array at those coordss
                    tiles[x, y] = tile;

                }
            }
        }
        public int GetAdjacentMineCount(Tile tile)
        {
            //set count to 0
            int count = 0;
            // Loop through all the adjacent tiles on the x
            for (int x = -1; x <= 1; x++)
            {
                //loop through all the adjacent tiles on the y
                for (int y = -1; y <= 1; y++)
                {
                    //calculate which adjacent tile to look at
                    int desiredX = tile.x + x;
                    int desiredY = tile.y + y;
                    //check if the desired x & y is outside bounds
                    if(desiredX < 0 || desiredX >= width || desiredY < 0 || desiredY >= height)
                    {
                        //Continue to next element in loop
                        continue;
                    }
                    //select current tile
                    Tile currentTile = tiles[desiredX, desiredY];
                    //if the selected tile is a mine
                    if (currentTile.isMine)
                    {
                        //increase increment by 1
                        count++;
                    }
                }
            }
            //remember to return the count
            return count;
        }
        void MouseOver()
        {
            //if left mouse button is clicked
            if (Input.GetMouseButtonDown(0))
            {
                //shoots a ray from the main camera to the input positon
                Ray mouseRay = Camera.main.ScreenPointToRay(Input.mousePosition);
                //hit data from raycast
                RaycastHit2D hit = Physics2D.Raycast(mouseRay.origin, mouseRay.direction);
                ///if we hit a collider
                if (hit.collider != null)
                {
                    //get Tile component from the collider
                    Tile hitTile = hit.collider.GetComponent<Tile>();
                    //if there is the tile component
                    if (hitTile != null)
                    {
                        SelectTile(hitTile);
                    }
                }
            }
        }
        //FF = Flood Fill Algorithm
        void FFunCover(int x, int y, bool[,] visited)
        {
            // is x and y within bounds of the grid
            if (x >= 0 && y >= 0 && x < width && y < height)
            {
                //have these coordinates been visited
                if (visited[x, y])
                    return;
                Tile tile = tiles[x, y];
                //reveal tile in that x and y coord
                int adjacentMines = GetAdjacentMineCount(tile);
                //Reveal tile in that x and y coord
                tile.Reveal(adjacentMines);
                //if there were no more adjacent tiles around that tile
                if (adjacentMines == 0)
                {
                    //this tile has been visited
                    visited[x, y] = true;
                    //visit all the other tiles around this tile
                    FFunCover(x - 1, y, visited);
                    FFunCover(x + 1, y, visited);
                    FFunCover(x, y - 1, visited);
                    FFunCover(x, y + 1, visited);
                }
            }
        }
        void UncoverMine(int mineState = 0)
        {
            //loop through 2d array
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    Tile tile = tiles[x, y];
                    //Check if tile is a mine
                    if (tile.isMine)
                    {
                        //reveal mine tile
                        int uncovMines = GetAdjacentMineCount(tile);
                        tile.Reveal(uncovMines, mineState);
                    }
                }
            }
        }
        //scans the grid to check if there are no more empty tiles
        bool NoMoreEmptyTiles()
        {
            //set empty tile count to xero
            int emptyTileCount = 0;
            //loop through 2d array
            for (int x = 0; x < width; x++)
            {
                for (int y = 0; y < height; y++)
                {
                    //if tile is not revealed and not a mine
                    Tile tile = tiles[x, y];
                    if(!tile.isRevealed && !tile.isMine)
                    {
                        //its a empty tile
                        emptyTileCount += 1;
                    }
                }
            }
            return emptyTileCount == 0;
        }
        //uncovers a selected tile
        void SelectTile(Tile selected)
        {
            
            int selectedTile = GetAdjacentMineCount(selected);
            selected.Reveal(selectedTile);

            //if the tile selected is a mine
            if (selected.isMine)
            {
                //function to uncover the mine
                UncoverMine();
                //lose
            }
            //otherwise, are there no mines around this tile
            else if (selectedTile == 0)
            {

                int x = selected.x;
                int y = selected.y;
                //use flood fill algorith to uncover all adjacent mines
                FFunCover(x, y, new bool[width, height]);
            }
            //if there are no more empty tiles
            if (NoMoreEmptyTiles())
            {
                //uncover the mines, the 1 being win state
                UncoverMine(1);
            }
        }
    }
}
