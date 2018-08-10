using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameScore : MonoBehaviour
{

    public int NumberOfPerformedSteps { get; set; }
    public bool TheResultIsKnown { get; set; } //when you win or lost

    private int _objectivesCount;
    private int _stepsMaxCount;

    private List<GameObject> _objectives= new List<GameObject>();

    public List<GameObject> objectivePrefabs = new List<GameObject>();
    public Transform _objectivesPlace;

    public GameObject winPanel, lostPanel;
    public Text stepsText;

    private Board _board;
   
    private void Start()
    {
        _board = FindObjectOfType<Board>();
        _objectivesCount = Random.Range(1, 4);
        _stepsMaxCount = (_board.countOfColumns + _board.countOfRows)*1/3*_objectivesCount;
        GenerateObjectives();
        SetObjectivesCountValues();
    }
    private void Update()
    {
       stepsText.text= GetRemainingSteps().ToString();
    }
    private int GetRemainingSteps()
    {
        if (_stepsMaxCount <= NumberOfPerformedSteps)
        {
            return 0;
        }
        else
        {
            return _stepsMaxCount - NumberOfPerformedSteps;
        }
    }
    private void GenerateObjectives()
    {

        int objectivesCount = objectivePrefabs.Count;
        for(int i = 0; i < _objectivesCount; i++)
        {
            int randomObjectiveIndex = Random.Range(0,objectivesCount);

            GameObject randomObjective = Instantiate(objectivePrefabs[randomObjectiveIndex], _objectivesPlace);
            randomObjective.name = objectivePrefabs[randomObjectiveIndex].name;
            _objectives.Add(randomObjective);
            #region // Avoid duplications//
            GameObject temporalObject = objectivePrefabs[randomObjectiveIndex];
            objectivePrefabs.RemoveAt(randomObjectiveIndex);
            objectivePrefabs.Add(temporalObject);
            objectivesCount--;
            #endregion
        }
    }
    private void SetObjectivesCountValues()
    {
        for(int i = 0; i < _objectives.Count; i++)
        {
            int ranomDestination = Random.Range(_stepsMaxCount, _stepsMaxCount*3/2);
            _objectives[i].GetComponentInChildren<Text>().text = ranomDestination.ToString();
        }
    }

   
  
    public void ChangeRemainingCounts(string crystalsTag)
    {
        for(int i = 0; i < _objectives.Count; i++)
        {
            if (_objectives[i].tag == crystalsTag)
            {
                if(System.Convert.ToInt32(_objectives[i].GetComponentInChildren<Text>().text) - 1 <= 0)
                {
                    _objectives[i].GetComponentInChildren<Text>().text = "0";
                }
                else
                {
                    _objectives[i].GetComponentInChildren<Text>().text = 
                (System.Convert.ToInt32(_objectives[i].GetComponentInChildren<Text>().text) - 1).ToString();
                }

            }
        }
     
    }
    public IEnumerator FindOutTheResult()
    {
        yield return new WaitForEndOfFrame();
        int optimalCount = 0;
        for (int i = 0; i < _objectives.Count; i++)
        {

            if (System.Convert.ToInt32(_objectives[i].GetComponentInChildren<Text>().text) <= 0)
            {
                _objectives[i].GetComponentInChildren<Text>().text = "0";
                optimalCount++;
            }
        }
        if (optimalCount == _objectives.Count)
        {
            Debug.Log("win");
            winPanel.SetActive(true);
            TheResultIsKnown = true;
            
           
        }
        if (optimalCount != _objectives.Count & System.Convert.ToInt32(stepsText.text) <= 0)
        {
            Debug.Log("Lost");
            lostPanel.SetActive(true);
            TheResultIsKnown = true;
            
        }
    }

    public void RestartScene()
    {
        SceneManager.LoadScene("GameScene");
    }
}
