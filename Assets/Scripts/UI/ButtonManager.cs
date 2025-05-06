using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class ButtonManager : MonoBehaviour
{
    [SerializeField] private Button solveButton;
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button newGameButton;
    [SerializeField] private Button difficultyButton;

    public SudokuLogic sudokuLogic;
    void Start()
    {
        solveButton.onClick.AddListener(SolveOnClick);
        confirmButton.onClick.AddListener(ConfirmOnClick);
        newGameButton.onClick.AddListener(NewGameOnClick);
        difficultyButton.onClick.AddListener(DifficultyButtonOnClick);
    }

    void Update()
    {
        sudokuLogic.currentDifficulty = GameSettings._difficulty;
        difficultyButton.GetComponentInChildren<TMP_Text>().text = GameSettings._difficulty.ToString();   
    }
    
    void SolveOnClick()
    {
        sudokuLogic.SolveBoard();
    }

    void ConfirmOnClick()
    {
        sudokuLogic.IsCorrect();
    }

    void NewGameOnClick()
    {
        sudokuLogic.GenerateNewBoard();
    }

    void DifficultyButtonOnClick()
    {
        if (GameSettings._difficulty == SudokuLogic.DifficultyLevel.Easy)
        {
            GameSettings._difficulty = SudokuLogic.DifficultyLevel.Medium;
        }
        else if (GameSettings._difficulty == SudokuLogic.DifficultyLevel.Medium)
        {
            GameSettings._difficulty = SudokuLogic.DifficultyLevel.Hard;
        }
        else if (GameSettings._difficulty == SudokuLogic.DifficultyLevel.Hard)
        {
            GameSettings._difficulty = SudokuLogic.DifficultyLevel.Expert;
        }
        else
        {
            GameSettings._difficulty = SudokuLogic.DifficultyLevel.Easy;
        }

        difficultyButton.GetComponentInChildren<TMP_Text>().text = GameSettings._difficulty.ToString();
    }
}
