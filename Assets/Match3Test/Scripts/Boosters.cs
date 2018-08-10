using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public  class Boosters : MonoBehaviour
{

    public GameObject PotencialBomb1 { get; set; }//gets the value in the script "Crystal"
    public GameObject PotencialBomb2 { get; set; }//gets the value in the script "Crystal"

    public Sprite radialBombSprite;
    public Sprite rowBombSprite;
    public Sprite columnBombSprite;

    private Board board;
    private GameScore gameScore;

    private void Start()
    {
    
        board = FindObjectOfType<Board>();
        gameScore = FindObjectOfType<GameScore>();
    }

    public void UseRowBomb(int row)
    {
        for(int i = 0; i < board.countOfColumns; i++)
        {
            for(int j = 0; j < board.countOfRows; j++)
            {
                if (j == row && !board.IsEmptySpaces[i, j])
                {
                    board.AllCrystals[i, j].GetComponent<Crystal>().IsMatched = true;
                }
            }
        }
    }
    public void UseColumnBomb(int column)
    {
        for (int i = 0; i < board.countOfColumns; i++)
        {
            for (int j = 0; j < board.countOfRows; j++)
            {
                if (i==column && !board.IsEmptySpaces[i,j])
                {
                    board.AllCrystals[i, j].GetComponent<Crystal>().IsMatched = true;
                }
            }
        }
    }
    public void UseRadialBomb(int column, int row)
    {

        HashSet<GameObject> еlementsForBoost = new HashSet<GameObject>();
        for (int i = 0; i < board.countOfColumns; i++)
        {
            for (int j = 0; j < board.countOfRows; j++)
            {
                if( (i==column && j== row) || (Mathf.Abs(column-i)==1 && Mathf.Abs(row-j)==1) 
                    ||(i==column && Mathf.Abs(row - j) == 1) || (j==row && Mathf.Abs(column-i)==1) )
                {
                    if (board.AllCrystals[i, j] != null || !board.IsEmptySpaces[i,j])
                    {
                        еlementsForBoost.Add(board.AllCrystals[i, j]);
                    }
                }
            }
        }

        foreach (var item in еlementsForBoost)
        {
            item.GetComponent<Crystal>().IsMatched = true;
        }
    }

    

    public void FindPotencialBombIn(List<List<GameObject>> Match, string bombTag)
    {
        if (PotencialBomb1 != null && PotencialBomb2 != null)
        {
            for (int i = 0; i < Match.Count; i++)
            {
                for (int j = 0; j < Match[i].Count; j++)
                {
                    if (Match[i][j] == PotencialBomb1 ^ Match[i][j] == PotencialBomb2)
                    {
                        gameScore.ChangeRemainingCounts(Match[i][j].GetComponent<Crystal>().tag);
                        Match[i][j].GetComponent<Crystal>().tag = bombTag;

                        if (bombTag == "RadialBomb")
                        {
                            Match[i][j].GetComponent<Crystal>().gameObject.GetComponent<SpriteRenderer>().sprite = radialBombSprite;
                        }
                        if (bombTag == "RowBomb")
                        {
                            Match[i][j].GetComponent<Crystal>().gameObject.GetComponent<SpriteRenderer>().sprite = rowBombSprite;
                        }
                        if (bombTag == "ColumnBomb")
                        {
                            Match[i][j].GetComponent<Crystal>().gameObject.GetComponent<SpriteRenderer>().sprite = columnBombSprite;
                        }
                        PotencialBomb1 = null;
                        PotencialBomb2 = null;
                        return;
                    }
                }
            }
        }
    }

}
