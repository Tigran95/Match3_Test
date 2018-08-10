using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraAdapter : MonoBehaviour
{

    private Board board;

    private IEnumerator Start()
    {
        yield return new WaitForSeconds(0.5f);
        board = FindObjectOfType<Board>();
        
        transform.position = new Vector3(GetTopTilePositionX(), GetTopTilePositionY(), transform.position.z);
        
    }

    private float GetTopTilePositionY()
    {
        GameObject topElement;
        int topTileRow = board.countOfRows;
        do
        {
            topTileRow--;
            topElement = board.AllTiles[board.countOfColumns - 1, topTileRow];
        }
        while (board.IsEmptySpaces[board.countOfColumns - 1, topTileRow]);

        return topElement.GetComponent<SpriteRenderer>().bounds.max.y;
    }
    private float GetTopTilePositionX()
    {
        GameObject topElement;
        int topTileRow = board.countOfRows;
        do
        {
            topTileRow--;
            topElement = board.AllTiles[board.countOfColumns / 2, topTileRow];
        }
        while (board.IsEmptySpaces[board.countOfColumns / 2, topTileRow]);

        if (board.countOfColumns % 2 == 0)
        {
            return topElement.GetComponent<SpriteRenderer>().bounds.min.x;
        }
        else
        {
            return topElement.transform.position.x;
        }
    }
    private float GetLeftTilePositionX()
    {
        GameObject leftElement;
        int leftTileRow = board.countOfRows;
        do
        {
            leftTileRow--;
            leftElement = board.AllTiles[0, leftTileRow];
        }
        while (board.IsEmptySpaces[0, leftTileRow]);
        return leftElement.GetComponent<SpriteRenderer>().bounds.min.x;
    }
    private float GetBottomTilePositionY()
    {
        GameObject bottomElement;
        int bottomTileColumn = -1;
        do
        {
            bottomTileColumn++;
            bottomElement = board.AllTiles[bottomTileColumn,0];
        }
        while (board.IsEmptySpaces[bottomTileColumn,0]);

        return bottomElement.GetComponent<SpriteRenderer>().bounds.min.y;
     
    }
    private void Update()
    {
        if (board != null && CameraAnglePoints.cameraAngels != null)
        {

            if (GetLeftTilePositionX() < CameraAnglePoints.cameraAngels[0].transform.position.x
                ||GetBottomTilePositionY()< CameraAnglePoints.cameraAngels[0].transform.position.y)
            {
                Camera.main.orthographicSize += 0.5f;
            }
        }
    }
}
