using UnityEngine;
using TMPro;
using UnityEngine.UI;


public class ButtonManager : MonoBehaviour
{
    [SerializeField] private Button solveButton;
    [SerializeField] private Button confirmButton;

    public SudokuLogic sudokuLogic;
    void Start()
    {
        solveButton.onClick.AddListener(SolveOnClick);
        confirmButton.onClick.AddListener(ConfirmOnClick);
    }

    void SolveOnClick()
    {
        sudokuLogic.SolveBoard();
    }

    void ConfirmOnClick()
    {
        sudokuLogic.IsCorrect();
    }


}
