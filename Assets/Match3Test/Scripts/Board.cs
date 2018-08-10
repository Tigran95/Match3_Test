using System.Collections;
using System.Collections.Generic;
using UnityEngine;




public class Board : MonoBehaviour
{

    public GameObject[,] AllCrystals { get; set; }
    public GameObject[,] AllTiles { get; set; }
    public bool[,] IsEmptySpaces { get; set; }

    public int countOfColumns;
    public int countOfRows;
    public int offset;
    public int emptySpacesCount;
    public bool isInProcess;
   
    public GameObject tilePrefab;
    public GameObject[] crystals;
    public Transform crystalsPlace;

    private GameScore gameScore;
   

    
    
    private void Start()
    {
        gameScore = FindObjectOfType<GameScore>();
        IsEmptySpaces = new bool[countOfColumns, countOfRows];
         AllCrystals = new GameObject[countOfColumns, countOfRows];
        AllTiles = new GameObject[countOfColumns, countOfRows];
        FirstFill();
    }
   
    private void Update()
    {
         isInProcess = CheckGameStatus();
    }
    private void GenerateEmptySpaces()
    {
        for (int i = 0; i < emptySpacesCount; i++)
        {
            int randomRow;
            int randomColumn;
            do
            {
                randomRow = Random.Range(0, countOfRows);
                randomColumn = Random.Range(0, countOfColumns);
            }
            while (IsEmptySpaces[randomColumn, randomRow]);
            IsEmptySpaces[randomColumn, randomRow] = true;
        }
    }

    private void FirstFill()
    {
        GenerateEmptySpaces();
        for (int i = 0; i < countOfColumns; i++)
        {
            for(int j =0; j < countOfRows; j++)
            {
                if (!IsEmptySpaces[i,j])
                {

                Vector2 tempPosition = new Vector2(i, j);
                GameObject tile=Instantiate(tilePrefab,tempPosition, Quaternion.identity);
                tile.transform.parent = this.transform;
                tile.name = i + "" + j;
                    AllTiles[i, j] = tile;
                int crystalToUse;
                do
                {
                 crystalToUse = Random.Range(0, crystals.Length);
                }
                while (ExcludeMatches(i, j, crystals[crystalToUse])) ;
                GameObject Crystal = Instantiate(crystals[crystalToUse], tempPosition, Quaternion.identity);
                Crystal.GetComponent<Crystal>().Column =i;
                Crystal.GetComponent<Crystal>().Row = j;
                Crystal.transform.parent = crystalsPlace;
                Crystal.name = i + "" + j + "Crystal";
                AllCrystals[i, j] = Crystal;
                }
            }
        }
       
    }

   

    private bool ExcludeMatches(int column,int row, GameObject Piece)
    {
        #region //Do not allow the presence of a combination//
        if (column>1 && row > 1)
        { 
            if(AllCrystals[column-1,row]!=null && AllCrystals[column - 2, row] != null)
            {
                if (AllCrystals[column-1,row].tag==Piece.tag && AllCrystals[column - 2, row].tag == Piece.tag)
                {
                    return true;
                }
            }
            if (AllCrystals[column, row - 1] != null && AllCrystals[column, row - 2] != null)
            {
                if (AllCrystals[column, row - 1].tag == Piece.tag && AllCrystals[column, row - 2].tag == Piece.tag)
                {
                    return true;
                }
            }
            if (AllCrystals[column, row - 1] != null && AllCrystals[column - 1, row] != null && AllCrystals[column - 1, row - 1] != null)
            {
                if (AllCrystals[column, row - 1].tag == Piece.tag && AllCrystals[column - 1, row].tag == Piece.tag &&
                    AllCrystals[column - 1, row - 1].tag == Piece.tag)
                {
                    return true;
                }
            }
        }

        else if(column<=1 || row <= 1 )
        {
            if (row > 1)
            {
                if (AllCrystals[column, row - 1] != null && AllCrystals[column, row - 2] != null)
                {
                    if (AllCrystals[column, row - 1].tag == Piece.tag && AllCrystals[column, row - 2].tag == Piece.tag)
                    {
                        return true;
                    }
                }
                if (AllCrystals[column, row - 1] != null)
                {
                    if (AllCrystals[column, row - 1].tag == Piece.tag)
                    {
                        return true;
                    }
                }
            }
            if (column > 1)
            {
                if (AllCrystals[column - 1, row] != null && AllCrystals[column - 2, row] != null)
                {
                    if (AllCrystals[column - 1, row].tag == Piece.tag && AllCrystals[column - 2, row].tag == Piece.tag)
                    {
                        return true;
                    }
                }
                if (AllCrystals[column - 1, row] != null)
                {
                    if (AllCrystals[column - 1, row].tag == Piece.tag)
                    {
                        return true;
                    }
                }
            }
        }
        else if(column<=1 && row <= 1)
        {
            if(AllCrystals[column - 1, row - 1].tag == Piece.tag)
            {
                return true;
            }
        }
        #endregion
        return false;

    }

    private void DestroyMatchesAt(int column, int row)
    {
        if (AllCrystals[column, row].GetComponent<Crystal>().IsMatched)
        {
            gameScore.ChangeRemainingCounts(AllCrystals[column, row].tag);
            Destroy(AllCrystals[column, row]);
            AllCrystals[column, row] = null;
        }    
    }
    public void DestroyMatches()
    {
        for (int i = 0; i < countOfColumns; i++)
        {
            for(int j = 0; j < countOfRows; j++)
            {
                if (AllCrystals[i, j] != null)
                {
                    DestroyMatchesAt(i, j);
                }
            }
        }
        StartCoroutine(DecreaseRow());
    }

    private IEnumerator DecreaseRow()
    {
        for(int i = 0; i < countOfColumns; i++)
        {
            for(int j = 0; j < countOfRows; j++)
            {
                if (!IsEmptySpaces[i, j] && AllCrystals[i, j] == null)
                {
                    for (int k = j + 1; k < countOfRows; k++)
                    {
                        if (AllCrystals[i, k] != null )
                        {
                            AllCrystals[i, k].GetComponent<Crystal>().Row = j;
                            AllCrystals[i, k] = null;
                            break;
                        }
                    }
                }
            }
        }
        yield return new WaitForSeconds(0.4f);
        StartCoroutine(FillBoard());
    }

    private IEnumerator FillBoard()
    {
        ReffilBoard();
        yield return new WaitForSeconds(0.5f);

        while (IsMatchesOnBoard())
        {
            yield return new WaitForSeconds(0.5f);
            DestroyMatches();
        }
        yield return new WaitForSeconds(0.5f);
    }

    private void ReffilBoard()
    {
        for (int i = 0; i < countOfColumns; i++)
        {
            for (int j = 0; j < countOfRows; j++)
            {
                if (AllCrystals[i, j] == null && !IsEmptySpaces[i,j])
                {
                    Vector2 tempPosition = new Vector2(i, j+offset);
                   int  crystalToUse = Random.Range(0, crystals.Length);
                    GameObject crystal = Instantiate(crystals[crystalToUse], tempPosition, Quaternion.identity);
                    crystal.transform.parent = crystalsPlace;
                    crystal.name = i + "" + j + "Crystal";
                    AllCrystals[i, j] = crystal;
                    crystal.GetComponent<Crystal>().Column = i;
                    crystal.GetComponent<Crystal>().Row = j;
                }
            }
        }
    }

    private bool IsMatchesOnBoard()
    {
        for (int i = 0; i < countOfColumns; i++)
        {
            for (int j = 0; j < countOfRows; j++)
            {
                if (AllCrystals[i, j] != null)
                {
                    if (AllCrystals[i, j].GetComponent<Crystal>().IsMatched)
                    {
                        return true;
                    }
                }
            }
        }
        return false;
    }

    
    /// <summary>
    /// Can you make a move or not
    /// </summary>
    private bool CheckGameStatus()
    {
        for(int i = 0; i < countOfColumns; i++)
        {
            for(int j = 0; j < countOfRows; j++)
            {
                if (!IsEmptySpaces[i, j])
                {
                    if(AllCrystals[i,j]==null || AllCrystals[i, j].GetComponent<Crystal>().IsMatched ||gameScore.TheResultIsKnown)
                    {
                        return true;
                    }
                }
            }
        }
       StartCoroutine (gameScore.FindOutTheResult());
        
        return false;
    }
}
