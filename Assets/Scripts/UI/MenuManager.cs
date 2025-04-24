using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MenuManager : MonoBehaviour
{
    [SerializeField] private Button startButton;
    [SerializeField] private Button difficultyButton;
    [SerializeField] private Button settingsButton;

    public SudokuLogic sudokuLogic;

    void Start()
    {
        startButton.GetComponent<Button>().onClick.AddListener(StartButtonOnClick);
    }

    void StartButtonOnClick()
    {
        SceneManager.LoadScene("SudokuScene");
    }

    void ChangeDifficultyButtonOnClick()
    {
        
    }
}
