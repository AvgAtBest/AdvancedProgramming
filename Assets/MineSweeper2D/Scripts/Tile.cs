using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace MineSweeper2D
{

    public class Tile : MonoBehaviour
    {

        public int x, y;
        public bool isMine = false;
        public bool isRevealed = false;
        [Header("Reference")]
        public Sprite[] emptySprites;
        public Sprite[] mineSprites;
        public Sprite flagSprite;
        private SpriteRenderer rend;


        private void Awake()
        {
            rend = GetComponent<SpriteRenderer>();
        }
        // Use this for initialization
        void Start()
        {
            isMine = Random.value < .10f; //randomly decide if a time is a mine, using 5% chance (0.0 = 0, 1 = 100%)


        }

        //Reveals the contents of a tile once selected
        public void Reveal(int adjacentMines, int mineState = 0)
        {
            //flag a tile as being revealed
            isRevealed = true;

            if (isMine)//if its a mine
            {
                //show mine sprite on board
                rend.sprite = mineSprites[mineState];
            }
            else
            {
                //show empty sprite on board
                rend.sprite = emptySprites[adjacentMines];
            }
        }
        public void FlagSprite(int flaggableTiles)
        {
            //if it hasnt been revealed
            if(isRevealed == false)
            {
                //render sprite
                rend.sprite = flagSprite;
            }
        }
    }
}


