using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public class Crystal : MonoBehaviour
{
    public int Column { get; set; }
    public int Row { get; set; }
    public bool IsMatched { get; set; }

    private int _previousColumn;
    private int _previousRow;
    private int _targetX;
    private int _targetY;
    private float _swipeAngle;
    private float _swipeResist;

    private GameObject _otherCrystal;
    private Vector2 _firstClickPosition;
    private Vector2 _finalClickPosition;
    private Vector2 _tempPosition;
  
    private Board board;
    private MatchFinder matchFinder;
    private Boosters booster;
    private GameScore gameScore;
    
    
    private void Start()
    {

        _swipeResist = 1;

        booster = FindObjectOfType<Boosters>();
        board = FindObjectOfType<Board>();
        matchFinder = FindObjectOfType<MatchFinder>();
        gameScore = FindObjectOfType<GameScore>();
    }

    private void Update()
    {
        if (IsMatched)
        {
        SpriteRenderer matched = GetComponent<SpriteRenderer>();
        matched.color = new Color(2, 2, 2, 0.2f);
        }

        _targetX = Column;
        _targetY = Row;

        if (Mathf.Abs(_targetX - transform.position.x) > 0.1f)
        {
            _tempPosition = new Vector2(_targetX, transform.position.y);
            transform.position = Vector2.Lerp(transform.position, _tempPosition, Time.deltaTime * 10f);
            board.AllCrystals[Column, Row] = this.gameObject;
            matchFinder.FindAllMatches();
        }
        else
        {
            _tempPosition = new Vector2(_targetX, transform.position.y);
            transform.position = _tempPosition;
        }

        if (Mathf.Abs(_targetY - transform.position.y) > 0.1f )
        {
            _tempPosition = new Vector2(transform.position.x,_targetY);
            transform.position = Vector2.Lerp(transform.position, _tempPosition, Time.deltaTime* 10f);
            
                board.AllCrystals[Column, Row] = this.gameObject;
            
            matchFinder.FindAllMatches();
        }
        else
        {
            _tempPosition = new Vector2(transform.position.x, _targetY);
            transform.position = _tempPosition;
            
        }
    }

    private void OnMouseUpAsButton()
    {
        if (!board.isInProcess)
        {
            if (tag == "RowBomb")
            {
                gameScore.NumberOfPerformedSteps++;
                booster.UseRowBomb(Row);
                board.DestroyMatches();
            }
            if (tag == "ColumnBomb")
            {
                gameScore.NumberOfPerformedSteps++;
                booster.UseColumnBomb(Column);
                board.DestroyMatches();
            }
            if (tag == "RadialBomb")
            {
                gameScore.NumberOfPerformedSteps++;
                booster.UseRadialBomb(Column, Row);
                board.DestroyMatches();
            }
        }
    }

    private void OnMouseDown()
    {
        if (!board.isInProcess)
        {
        _firstClickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        }
       
    }
    private void OnMouseUp()
    {
        if (!board.isInProcess)
        {
        _finalClickPosition = Camera.main.ScreenToWorldPoint(Input.mousePosition);
         CalculateAngle();
        }
    }

    private void CalculateAngle()
    {
        Vector2 vector = _finalClickPosition - _firstClickPosition;
        if (Mathf.Abs(vector.x) > _swipeResist || Mathf.Abs(vector.y) > _swipeResist)
        {
            _swipeAngle = Mathf.Atan2(vector.y, vector.x) * 180 / Mathf.PI;
            StartCoroutine(MovePieces());
        }
    }

    private void SwapCrystalsTo(Vector2 direction)
    {
        _otherCrystal = board.AllCrystals[Column+(int)direction.x, Row+(int)direction.y];
        _previousRow = Row;
        _previousColumn = Column;
        if (_otherCrystal != null)
        {

        _otherCrystal.GetComponent<Crystal>().Column += -1*(int)direction.x;
        _otherCrystal.GetComponent<Crystal>().Row += -1 * (int)direction.y;
        Column += (int)direction.x;
        Row += (int)direction.y;
        }
        
    }
    private IEnumerator MovePieces()
    {
        yield return new WaitForEndOfFrame();
        if (_swipeAngle>-45 && _swipeAngle <= 45 && Column<board.countOfColumns-1)
        {
            //SwapRight
            matchFinder.PotencialBombsTag = "RowBomb";
            SwapCrystalsTo(Vector2.right);
        }
        else if (_swipeAngle > 45 && _swipeAngle <= 135 && Row<board.countOfRows-1)
        {
            //SwapUp
            matchFinder.PotencialBombsTag = "ColumnBomb";
            SwapCrystalsTo(Vector2.up);
        }
        else if ((_swipeAngle > 135 || _swipeAngle <= -135) && Column>0)
        {
            //SwapLeft
            matchFinder.PotencialBombsTag = "RowBomb";
            SwapCrystalsTo(Vector2.left);
        }

        else if (_swipeAngle < -45 && _swipeAngle >= -135 && Row>0)
        {
            //SwapDown
            matchFinder.PotencialBombsTag = "ColumnBomb";
            SwapCrystalsTo(Vector2.down);
        }
         
        booster.PotencialBomb1 = this.gameObject;
        booster.PotencialBomb2 = _otherCrystal;
        #region Checking the possibility of swap
        if (_otherCrystal != null)
        {
            if (this.gameObject.tag == "RadialBomb" || _otherCrystal.tag == "RadialBomb"
                  || this.gameObject.tag == "ColumnBomb" || _otherCrystal.tag == "ColumnBomb"
                  || this.gameObject.tag == "RowBomb" || _otherCrystal.tag == "RowBomb"
                  )
            {
                _otherCrystal.GetComponent<Crystal>().Column = Column;
                _otherCrystal.GetComponent<Crystal>().Row = Row;
                Column = _previousColumn;
                Row = _previousRow;
            }
            else
            {
                gameScore.NumberOfPerformedSteps++;
                StartCoroutine(ChackMove());
            }
        }
        #endregion
    }
    private IEnumerator ChackMove()
    {
        yield return new WaitForSeconds(0.5f);
        if (_otherCrystal != null)
        {
            if ((!IsMatched && !_otherCrystal.GetComponent<Crystal>().IsMatched)) 
                
            {
                if(this.gameObject.tag=="RadialBomb" || _otherCrystal.tag == "RadialBomb" 
                   || this.gameObject.tag == "ColumnBomb" || _otherCrystal.tag == "ColumnBomb"
                   || this.gameObject.tag == "RowBomb" || _otherCrystal.tag == "RowBomb" )
                {
                    board.DestroyMatches();
                }
                else
                {

                    # region If there is no match,returns everything to their former places
                    _otherCrystal.GetComponent<Crystal>().Row = Row;
                _otherCrystal.GetComponent<Crystal>().Column = Column;
                Row = _previousRow;
                Column = _previousColumn;
                    #endregion
                }
            }
            else
            {
                board.DestroyMatches();
            }
            _otherCrystal = null;
        }
    }
}
