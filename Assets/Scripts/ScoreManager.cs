using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ScoreManager : MonoBehaviour
{
    public IntSetSO scoreList;

    public Text scoreText;
    public Text scoreModifyText;
    public int maxWidth;
    public IntVariableSO currentScore;
    public IntVariableSO highestScore;

    [Space]
    [SerializeField]
    private float scoreModifyLastDuration;
    private bool isShowingScoreModify;
    private float timer;

    // Start is called before the first frame update
    void Start()
    {
        Initialize();
    }

    private void Initialize()
    {
        currentScore.SetDefaultValue();
        isShowingScoreModify = false;
        timer = 0f;
        ClearScoreModifyText();
    }

    // Update is called once per frame
    void Update()
    {
        scoreText.text = string.Format("{0:D8}",currentScore.Value);

        if(isShowingScoreModify)
        {
            timer += Time.deltaTime;
            if (timer > scoreModifyLastDuration)
            {
                isShowingScoreModify = false;
                ClearScoreModifyText();
            }

        }

    }

    //public void ShipScoreAdd()
    //{

    //    ScoreModify(scoreList.Items[0].Value);
    //}

    //public void ShipScoreMinus()
    //{
    //    ScoreModify(-scoreList.Items[0].Value);
    //}

    //public void TargetScoreAdd()
    //{
    //    ScoreModify(scoreList.Items[1].Value);
    //}

    public void ShowScoreModify(int scoreModify)
    {
        bool isPositive = scoreModify > 0;

        scoreModifyText.text = (isPositive ? "+  " : "-  ") + scoreModify.ToString();
    }

    //public void UpdateScore()
    //{
    //    scoreText.text = currentScore.Value.ToString();
    //}

    public void ScoreModify(int scoreModify)
    {
        currentScore.Value += scoreModify;

        ShowScoreModify(scoreModify);
    }

    private void UpdateHighestScore()
    {
        highestScore.SetValue((currentScore.Value > highestScore.Value) ? currentScore.Value : highestScore.Value);
    }

    private void ClearScoreModifyText()
    {
        scoreModifyText.text = "";
    }

    public void ShowSocreModifyShip()
    {
        ResetTimer();
        isShowingScoreModify = true;
        scoreModifyText.text = "+  " + scoreList.Items[0].Value.ToString();
    }

    public void ShowScoreModifyStation()
    {
        ResetTimer();
        isShowingScoreModify = true;
        scoreText.text = "+  " + scoreList.Items[1].Value.ToString();
    }

    public void ResetTimer()
    {
        timer = 0f;
    }
}
