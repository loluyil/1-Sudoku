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
        Button solveBtn = solveButton.GetComponent<Button>();
        solveBtn.onClick.AddListener(SolveOnClick);

        Button confirmBtn = confirmButton.GetComponent<Button>();
        confirmBtn.onClick.AddListener(ConfirmOnClick);
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
