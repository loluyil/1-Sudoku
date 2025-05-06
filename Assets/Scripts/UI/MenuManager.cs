using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button difficultyButton;
    [SerializeField] private Button settingsButton;


    void Start()
    {
        difficultyButton.GetComponent<Button>().onClick.AddListener(DifficultyButtonOnClick);
        startButton.GetComponent<Button>().onClick.AddListener(StartButtonOnClick);
    }

    void StartButtonOnClick()
    {
        SceneManager.LoadScene("SudokuScene");
    }

    void DifficultyButtonOnClick()
    {
        // Logic to change difficulty
        // For example, toggle between Easy, Medium, Hard
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


