using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class MatchFinder : MonoBehaviour
{
    //gets the value in the script "Crystal"
    public string PotencialBombsTag { get; set; }

    private int _minType;
    private int _middleType;
    private int _maxType;

    private List<List<GameObject>> _matches_5InRows = new List<List<GameObject>>();
    private List<List<GameObject>> _matches_4InRows = new List<List<GameObject>>();
    private List<List<GameObject>> _matches_3InRows = new List<List<GameObject>>();
    private List<List<GameObject>> _matches_5InColumns = new List<List<GameObject>>();
    private List<List<GameObject>> _matches_4InColumns = new List<List<GameObject>>();
    private List<List<GameObject>> _matches_3InColumns = new List<List<GameObject>>();
    private List<List<GameObject>> _squareMatches = new List<List<GameObject>>();

    private Board board;
    private Boosters booster;
    
    private void Start()
    {
        _minType = 3;
        _middleType = 4;
        _maxType = 5;
        
        board = FindObjectOfType<Board>();
        booster = FindObjectOfType<Boosters>();
     

    }

    public void FindAllMatches()
    {
        ClearAllMatchContainers();

        FindMatchesOfSpecialTypeAndSave();

        RemoveSub_Matches();

        TakingOneOfThe_IntersectingMatches();

        FindPotencialBombs();

        EnableMatches();
    }


    private void ClearAllMatchContainers()
    {
        _matches_5InRows.Clear();
        _matches_4InRows.Clear();
        _matches_3InRows.Clear();
        _matches_5InColumns.Clear();
        _matches_4InColumns.Clear();
        _matches_3InColumns.Clear();
        _squareMatches.Clear();
    }

    private void FindMatchesOfSpecialTypeAndSave()
    {
        FindMatchesOfTypeIn_Columns_AndSaveTo(_matches_3InColumns, _minType);
        FindMatchesOfTypeIn_Rows_AndSaveTo(_matches_3InRows, _minType);

        FindSquareMatches();

        FindMatchesOfTypeIn_Columns_AndSaveTo(_matches_4InColumns, _middleType);
        FindMatchesOfTypeIn_Rows_AndSaveTo(_matches_4InRows, _middleType);

        FindMatchesOfTypeIn_Columns_AndSaveTo(_matches_5InColumns, _maxType);
        FindMatchesOfTypeIn_Rows_AndSaveTo(_matches_5InRows, _maxType);
    }

    private void RemoveSub_Matches()
    {
        ClearFirstMatchFromSecond(_matches_5InColumns, _matches_4InColumns);
        ClearFirstMatchFromSecond(_matches_5InRows, _matches_4InRows);

        ClearFirstMatchFromSecond(_matches_5InColumns, _matches_3InColumns);
        ClearFirstMatchFromSecond(_matches_5InRows, _matches_3InRows);

        ClearFirstMatchFromSecond(_matches_4InColumns, _matches_3InColumns);
        ClearFirstMatchFromSecond(_matches_4InRows, _matches_3InRows);
    }

    private void TakingOneOfThe_IntersectingMatches()
    {
        ClearFirstMatchFromSecond(_matches_5InColumns, _matches_5InRows);
        ClearFirstMatchFromSecond(_matches_5InColumns, _matches_4InRows);
        ClearFirstMatchFromSecond(_matches_5InColumns, _matches_3InRows);

        ClearFirstMatchFromSecond(_matches_4InColumns, _matches_4InRows);
        ClearFirstMatchFromSecond(_matches_4InColumns, _matches_3InRows);
        ClearFirstMatchFromSecond(_matches_3InColumns, _matches_3InRows);

        ClearFirstMatchFromSecond(_matches_5InRows, _matches_4InColumns);
        ClearFirstMatchFromSecond(_matches_5InRows, _matches_3InColumns);
        ClearFirstMatchFromSecond(_matches_4InRows, _matches_3InColumns);
      
        ClearFirstMatchFromSecond(_matches_5InColumns, _squareMatches);
        ClearFirstMatchFromSecond(_matches_5InRows, _squareMatches);

        ClearFirstMatchFromSecond(_matches_4InColumns, _squareMatches);
        ClearFirstMatchFromSecond(_matches_4InRows, _squareMatches);

        ClearFirstMatchFromSecond(_squareMatches, _matches_3InColumns);
        ClearFirstMatchFromSecond(_squareMatches, _matches_3InRows);
    }

    private void FindMatchesOfTypeIn_Rows_AndSaveTo(List<List<GameObject>> ContainerForMatches, int matchElementsType)
    {
        for (int j = 0; j < board.countOfRows; j++)
        {
            HashSet<GameObject> CurrentMatches = new HashSet<GameObject>();
            for (int i = 0; i < board.countOfColumns; i++)
            {
                GameObject CurrentCrystal = board.AllCrystals[i, j];
                if (CurrentCrystal != null)
                {
                    if (i > 0 && i < board.countOfColumns - 1)
                    {
                        GameObject PreviousCrystal = board.AllCrystals[i - 1, j];
                        GameObject NextCrystal = board.AllCrystals[i + 1, j];

                        if (PreviousCrystal != null && NextCrystal != null)
                        {
                            if (PreviousCrystal.tag == CurrentCrystal.tag && NextCrystal.tag == CurrentCrystal.tag)
                            {
                                if (PreviousCrystal.GetComponent<Crystal>().IsMatched
                                    || CurrentCrystal.GetComponent<Crystal>().IsMatched
                                    || NextCrystal.GetComponent<Crystal>().IsMatched )
                                {
                                    continue;
                                }
                                CurrentMatches.Add(NextCrystal);
                                CurrentMatches.Add(PreviousCrystal);
                                CurrentMatches.Add(CurrentCrystal);
                                
                                if (CurrentMatches.Count == matchElementsType)
                                {
                                    ContainerForMatches.Add(CurrentMatches.ToList());
                                    CurrentMatches.Clear();
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void FindMatchesOfTypeIn_Columns_AndSaveTo(List<List<GameObject>> ContainerForMatches, int matchElementsType)
    {
        for (int i = 0; i < board.countOfColumns; i++)
        {
            HashSet<GameObject> CurrentMatches = new HashSet<GameObject>();
            for (int j = 0; j < board.countOfRows; j++)
            {
                GameObject CurrentCrystal = board.AllCrystals[i, j];
                if (CurrentCrystal != null)
                {
                    if (j > 0 && j < board.countOfRows - 1)
                    {
                        GameObject PreviousCrystal = board.AllCrystals[i, j - 1];
                        GameObject NextCrystal = board.AllCrystals[i, j + 1];

                        if (PreviousCrystal != null && NextCrystal != null)
                        {
                            if (PreviousCrystal.tag == CurrentCrystal.tag && NextCrystal.tag == CurrentCrystal.tag)
                            {
                                if (PreviousCrystal.GetComponent<Crystal>().IsMatched 
                                    || CurrentCrystal.GetComponent<Crystal>().IsMatched 
                                    || NextCrystal.GetComponent<Crystal>().IsMatched )
                                {
                                    continue;
                                }
                                CurrentMatches.Add(NextCrystal);
                                CurrentMatches.Add(PreviousCrystal);
                                CurrentMatches.Add(CurrentCrystal);

                                if (CurrentMatches.Count == matchElementsType)
                                {
                                    ContainerForMatches.Add(CurrentMatches.ToList());
                                    CurrentMatches.Clear();
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    private void FindSquareMatches()
    {
        for (int i = 0; i < board.countOfColumns; i++)
        {
            for (int j = 0; j < board.countOfRows; j++)
            {
                GameObject CurrentCrystal = board.AllCrystals[i, j];
                if (CurrentCrystal != null)
                {
                    if (i > 0 && j > 0)
                    {
                        GameObject BottomCrystal = board.AllCrystals[i, j - 1];
                        GameObject LeftCrystal = board.AllCrystals[i - 1, j];
                        GameObject AngularCrystal = board.AllCrystals[i - 1, j - 1];

                        if (BottomCrystal != null && LeftCrystal != null && AngularCrystal != null)
                        {
                            if (BottomCrystal.tag == CurrentCrystal.tag && LeftCrystal.tag == CurrentCrystal.tag &&
                                AngularCrystal.tag == CurrentCrystal.tag)
                            {
                                if (BottomCrystal.GetComponent<Crystal>().IsMatched 
                                    || LeftCrystal.GetComponent<Crystal>().IsMatched 
                                    || CurrentCrystal.GetComponent<Crystal>().IsMatched 
                                    || AngularCrystal.GetComponent<Crystal>().IsMatched)
                                {
                                    continue;
                                }
                                HashSet<GameObject> CurrentSquareMatch = new HashSet<GameObject>();
                                CurrentSquareMatch.Add(BottomCrystal);
                                CurrentSquareMatch.Add(LeftCrystal);
                                CurrentSquareMatch.Add(AngularCrystal);
                                CurrentSquareMatch.Add(CurrentCrystal);

                                if (CurrentSquareMatch.Count == 4)
                                {
                                    _squareMatches.Add(CurrentSquareMatch.ToList());
                                }
                            }
                        }
                    }
                }
            }
        }
    }
  
    private void ClearFirstMatchFromSecond(List<List<GameObject>> BigMatchContainer, List<List<GameObject>> SmallMatchContainer)
    {
        for (int i = 0; i < BigMatchContainer.Count; i++)
        {
            for (int j = 0; j < BigMatchContainer[i].Count; j++)
            {
                for (int m = 0; m < SmallMatchContainer.Count; m++)
                {
                    for (int n = 0; n < SmallMatchContainer[m].Count; n++)
                    {
                        if (BigMatchContainer[i][j] == SmallMatchContainer[m][n])
                        {
                            SmallMatchContainer.RemoveAt(m);
                            break;
                        }
                    }
                }
            }
        }
    }
    private void FindPotencialBombs()
    {
        booster.FindPotencialBombIn(_matches_5InColumns, PotencialBombsTag);
        booster.FindPotencialBombIn(_matches_5InRows, PotencialBombsTag);
        booster.FindPotencialBombIn(_matches_4InColumns, PotencialBombsTag);
        booster.FindPotencialBombIn(_matches_4InRows, PotencialBombsTag);
        booster.FindPotencialBombIn(_squareMatches, "RadialBomb");
    }

    private void EnableMatches()
    {
        UseThisMatch(_matches_5InColumns);
        UseThisMatch(_matches_4InColumns);
        UseThisMatch(_matches_3InColumns);
        UseThisMatch(_matches_5InRows);
        UseThisMatch(_matches_4InRows);
        UseThisMatch(_matches_3InRows);
        UseThisMatch(_squareMatches);
    }

    private void UseThisMatch(List<List<GameObject>> Match)
    {
        for (int i = 0; i < Match.Count; i++)
        {
            for (int j = 0; j < Match[i].Count; j++)
            {
                if (Match[i][j].tag == "RadialBomb" || Match[i][j].tag == "RowBomb" || Match[i][j].tag == "ColumnBomb")
                {
                    continue;
                }
                Match[i][j].GetComponent<Crystal>().IsMatched = true;
            }

        }
    }
}
