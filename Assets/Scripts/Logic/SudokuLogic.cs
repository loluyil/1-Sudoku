using System.Collections;
using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine.U2D.IK;

public class SudokuLogic : MonoBehaviour
{
    public UIGridRenderer gridRenderer;
    public GameObject numPrefab;
    public GameObject numInputPrefab;
    public Transform canvasTransform;
    public int[,] sudokuBoard = new int[9, 9];
    private int[,] solvedBoard;

    public List<int>[] rows = new List<int>[9];
    public List<int>[] columns = new List<int>[9];
    public List<int>[] blocks = new List<int>[9];
    
    public enum DifficultyLevel
    {
        Test,
        Easy,
        Medium,
        Hard,
        Expert
    }
    
    public DifficultyLevel currentDifficulty = DifficultyLevel.Expert;
    public bool useSymmetry = true;


    void Start()
    {   
        Debug.ClearDeveloperConsole();
        currentDifficulty = GameSettings._difficulty;
        StartCoroutine(InitializeGame());
    }

    private IEnumerator InitializeGame()
    {
        //Wait until the renderer has initialized its row/column data
        yield return new WaitUntil(() =>
            gridRenderer.AllRows != null &&
            gridRenderer.AllColumns != null &&
            gridRenderer.AllRows[0] != null &&
            gridRenderer.AllColumns[0] != null);

        for(int i = 0; i < 9; i++)
        {
            rows[i] = new List<int>();
            columns[i] = new List<int>();
            blocks[i] = new List<int>();
        }

        FillDiagonal();
        FillBoard();

        //Store the full solution
        solvedBoard = new int[9, 9];
        for(int x = 0; x < 9; x++)
        {
            for(int y = 0; y < 9; y++)
            {
                solvedBoard[x, y] = sudokuBoard[x, y];
            }
        }
        GeneratePuzzle(currentDifficulty);
        PrintBoard(solvedBoard, "Solved Board");
    }

    //Fills the 3x3 blocks diagonally with unique numbers
    //Okay.. this honestly isn't needed.. DLX can generate from an empty board, but it does make it a little faster :D
    private void FillDiagonal()
    {
        for(int i = 0; i < 3; i++)
        {
            List<int> nums = new() { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            ShuffleList(nums); 

            int xStart = i * 3;
            int yStart = i * 3;

            int numIndex = 0;
            for(int dx = 0; dx < 3; dx++)
            {
                for(int dy = 0; dy < 3; dy++)
                {
                    int x = xStart + dx;
                    int y = yStart + dy;
                    sudokuBoard[x, y] = nums[numIndex++];
                }
            }
        }
    }



    //DLX Algorithm but i have no idea how it works --- learn it later
    public void FillBoard()
    {
        DLX solver = new DLX(sudokuBoard);
        bool solved = solver.Solve();
        
        if(solved)
        {
            //Get the solved board
            solvedBoard = solver.GetSolution();

            for(int x = 0; x < 9; x++)
            {
                for(int y = 0; y < 9; y++)
                {
                    sudokuBoard[x, y] = solvedBoard[x, y];
                    rows[y].Add(solvedBoard[x, y]);
                    columns[x].Add(solvedBoard[x, y]);
                    blocks[(x / 3) + (y / 3) * 3].Add(solvedBoard[x, y]);
                }
            }
        }
    }

    //Stores numbers in the rows, columns, and blocks lists and instantiates the number prefab
    private void GenerateNumbers(int x, int y)
    {
            rows[y].Add(sudokuBoard[x, y]);
            columns[x].Add(sudokuBoard[x, y]);
            blocks[(x / 3) + (y / 3) * 3].Add(sudokuBoard[x, y]);
            
            //Instantiates number prefab
            Vector2 position = new Vector2(gridRenderer.AllColumns[0][x], gridRenderer.AllRows[0][y]);
            
            GameObject numObject = Instantiate(numPrefab, canvasTransform);
            TextMeshProUGUI textComponent = numObject.GetComponent<TextMeshProUGUI>();
            textComponent.text = sudokuBoard[x, y].ToString();
            
            RectTransform canvasRectTransform = canvasTransform.GetComponent<RectTransform>();
            RectTransform numRectTransform = numObject.GetComponent<RectTransform>();
            numRectTransform.SetParent(canvasRectTransform, false);
            numRectTransform.anchoredPosition = position;
    }

    private void GenerateInputs(int x, int y)
    {
            //Instantiates number prefab
            Vector2 position = new Vector2(gridRenderer.AllColumns[0][x], gridRenderer.AllRows[0][y]);
            
            GameObject inputObject = Instantiate(numInputPrefab, canvasTransform);
            
            RectTransform canvasRectTransform = canvasTransform.GetComponent<RectTransform>();
            RectTransform numRectTransform = inputObject.GetComponent<RectTransform>();
            numRectTransform.SetParent(canvasRectTransform, false);
            numRectTransform.anchoredPosition = position;
    }

    //Method to generate a Sudoku puzzle based on the selected difficulty level
    public void GeneratePuzzle(DifficultyLevel difficulty)
    {
        currentDifficulty = difficulty;
        
        ClearChildren();
        
        //How many cells to dig based on difficulty
        int cellsToDig = GetCellsToDigForDifficulty(difficulty);
        //Debug.Log($"Generating {difficulty} puzzle by removing {cellsToDig} cells");
        
        //Reset sudokuBoard and tracking lists
        for(int i = 0; i < 9; i++)
        {
            rows[i].Clear();
            columns[i].Clear();
            blocks[i].Clear();
        }
        
        //Create a working copy of the board
        for(int i = 0; i < 9; i++)
        {
            for(int j = 0; j < 9; j++)
            {
                sudokuBoard[i, j] = solvedBoard[i, j];
            }
        }
        
        //Create a list of all positions to potentially remove
        List<Vector2Int> positions = new List<Vector2Int>();
        for(int i = 0; i < 9; i++)
        {
            for(int j = 0; j < 9; j++)
            {
                positions.Add(new Vector2Int(i, j));
            }
        }
        
        //Random removal
        ShuffleList(positions);
        
        int cellsDug = 0;
        int posIndex = 0;
        
        //Keep removing numbers until we reach the desired difficulty
        while (cellsDug < cellsToDig && posIndex < positions.Count)
        {
            Vector2Int pos = positions[posIndex++];
            int i = pos.x;
            int j = pos.y;
            
            if(sudokuBoard[i, j] == 0)
                continue;
            
            //Remember the value in case we need to restore it
            int temp = sudokuBoard[i, j];
            sudokuBoard[i, j] = 0;
            cellsDug++;
            
            //If symmetry is enabled, also remove the symmetrical cell
            if(useSymmetry)
            {
                int symI = 8 - i;
                int symJ = 8 - j;
                
                //Skip if already dug or if it's the same position (center cell)
                if(sudokuBoard[symI, symJ] != 0 && (symI != i || symJ != j))
                {
                    int symTemp = sudokuBoard[symI, symJ];
                    sudokuBoard[symI, symJ] = 0;
                    cellsDug++;
                    
                    //If removing both cells makes the puzzle unsolvable or non-unique,
                    //put both back
                    if(!HasUniqueSolution(sudokuBoard))
                    {
                        sudokuBoard[i, j] = temp;
                        sudokuBoard[symI, symJ] = symTemp;
                        cellsDug -= 2;
                    }
                }
                else if(!HasUniqueSolution(sudokuBoard))
                {
                    sudokuBoard[i, j] = temp;
                    cellsDug--;
                }
            }
            else
            {
                if(!HasUniqueSolution(sudokuBoard))
                {
                    sudokuBoard[i, j] = temp;
                    cellsDug--;
                }
            }
        }
        
        //Add remaining numbers from the puzzle to tracking lists and display them
        for(int x = 0; x < 9; x++)
        {
            for(int y = 0; y < 9; y++)
            {
                if(sudokuBoard[x, y] != 0)
                {
                    GenerateNumbers(x, y);
                }
            }
        }

        for(int x = 0; x < 9; x++)
        {
            for(int y = 0; y < 9; y++)
            {
                if(sudokuBoard[x, y] == 0)
                {
                    GenerateInputs(x, y);
                }
            }
        }
        
        Debug.Log($"Generated a {difficulty} puzzle with {81 - cellsDug} clues remaining");
    }

    private int GetCellsToDigForDifficulty(DifficultyLevel difficulty)
    {
        switch (difficulty)
        {
            case DifficultyLevel.Test:
                return Random.Range(1, 5);
            case DifficultyLevel.Easy:
                return Random.Range(41, 50);
            case DifficultyLevel.Medium:
                return Random.Range(50, 55);
            case DifficultyLevel.Hard:
                return Random.Range(55, 60);
            case DifficultyLevel.Expert:
                return Random.Range(60, 64);
            default:
                return 45;
        }
    }

    //Helper method to check ifthe board has a unique solution
    private bool HasUniqueSolution(int[,] board)
    {
        //Check if the board is solvable with DLX
        DLX solver = new DLX(board);
        if(!solver.Solve())
            return false;
            
        int[,] solution = solver.GetSolution();
        
        //Try to find a different solution by adding constraints
        for(int i = 0; i < 9; i++)
        {
            for(int j = 0; j < 9; j++)
            {
                //Only check cells that were empty in the original board
                if(board[i, j] == 0)
                {
                    //Try a different value forthis cell
                    int originalValue = solution[i, j];
                    
                    //Test if we can find an alternative solution
                    for(int num = 1; num <= 9; num++)
                    {
                        if(num != originalValue)
                        {
                            //Create a new board with this constraint
                            int[,] testBoard = new int[9, 9];
                            for(int x = 0; x < 9; x++)
                            {
                                for(int y = 0; y < 9; y++)
                                {
                                    testBoard[x, y] = board[x, y];
                                }
                            }
                            
                            //Set the constraint
                            testBoard[i, j] = num;
                            
                            DLX testSolver = new DLX(testBoard);
                            if(testSolver.Solve())
                            {
                                //Found another valid solution, so this puzzle doesn't have a unique solution
                                return false;
                            }
                        }
                    }
                    return true;
                }
            }
        }
        return true;
    }

    //For the solve button
    public void SolveBoard()
    {
        foreach(Transform child in canvasTransform)
        {
            if(child.GetComponent<TMP_InputField>() != null)
            {
                Destroy(child.gameObject);
            }
        }
        
        for(int x = 0; x < 9; x++)
        {
            for(int y = 0; y < 9; y++)
            {
                if(sudokuBoard[x, y] == 0)
                {
                    sudokuBoard[x, y] = solvedBoard[x, y];
                    GenerateNumbers(x, y);
                }
            }
        }
    }

    public void IsCorrect()
    {
        int[,] playerBoard = new int[9, 9];

        //Initialize the player board with zeros
        for(int x = 0; x < 9; x++)
        {
            for(int y = 0; y < 9; y++)
            {
                playerBoard[x, y] = 0;
            }
        }

        //First store the pre-filled numbers from sudokuBoard
        for(int x = 0; x < 9; x++)
        {
            for(int y = 0; y < 9; y++)
            {
                if(sudokuBoard[x, y] != 0)
                {
                    playerBoard[x, y] = sudokuBoard[x, y];
                }
            }
        }

        foreach(Transform child in canvasTransform)
        {
            TMP_InputField inputField = child.GetComponent<TMP_InputField>();
            if(inputField != null)
            {
                //Get the position
                RectTransform rectTransform = child.GetComponent<RectTransform>();
                Vector2 position = rectTransform.anchoredPosition;
                
                //Find the corresponding x and y indices in the grid
                int x = -1;
                int y = -1;
                
                //Find which column this position belongs to
                for(int i = 0; i < 9; i++)
                {
                    if(Mathf.Approximately(position.x, gridRenderer.AllColumns[0][i]))
                    {
                        x = i;
                        break;
                    }
                }
                
                //Find which row this position belongs to
                for(int i = 0; i < 9; i++)
                {
                    if(Mathf.Approximately(position.y, gridRenderer.AllRows[0][i]))
                    {
                        y = i;
                        break;
                    }
                }
                
                if(x != -1 && y != -1)
                {
                    string inputText = inputField.text;
                    
                    // Make sure the input is not empty and is a valid number
                    if(!string.IsNullOrEmpty(inputText) && int.TryParse(inputText, out int inputValue))
                    {
                        playerBoard[x, y] = inputValue;
                    }
                }
            }
        }   
        
        //If playerboard = solvedboard
        bool isCorrect = true;
        for(int x = 0; x < 9; x++)
        {
            for(int y = 0; y < 9; y++)
            {
                if(playerBoard[x, y] != solvedBoard[x, y])
                {
                    isCorrect = false;
                    Debug.Log($"Incorrect value at ({x}, {y}): Expected {solvedBoard[x, y]}, Got {playerBoard[x, y]}");
                }
            }
        }
        
        if(isCorrect)
        {
            Debug.Log("Congratulations! The puzzle is solved correctly!");
            ClearChildren();
        }
        else
        {
            Debug.Log("The solution is not correct. Keep trying!");
        }
        
        PrintBoard(playerBoard, "Player's Board");
    }

    private void ClearChildren() 
    {
        foreach(Transform child in canvasTransform)
        {
            if(child.GetComponent<TextMeshProUGUI>() != null)
            {
                Destroy(child.gameObject);
            }

            if(child.GetComponent<TMP_InputField>() != null)
            {
                Destroy(child.gameObject);
            }
        }
    }

public void GenerateNewBoard()
{
    ClearChildren();

    for(int i = 0; i < 9; i++)
    {
        rows[i].Clear();
        columns[i].Clear();
        blocks[i].Clear();
    }

    for(int x = 0; x < 9; x++)
    {
        for(int y = 0; y < 9; y++)
        {
            sudokuBoard[x, y] = 0;
        }
    }

    FillDiagonal();
    FillBoard();

    solvedBoard = new int[9, 9];
    for(int x = 0; x < 9; x++)
    {
        for(int y = 0; y < 9; y++)
        {
            solvedBoard[x, y] = sudokuBoard[x, y];
        }
    }
    
    GeneratePuzzle(currentDifficulty);
    PrintBoard(solvedBoard, "Solved Board");
}

    //Shuffle function
    private void ShuffleList<T>(List<T> list)
    {
        int n = list.Count;
        for(int i = n - 1; i > 0; i--)
        {
            int j = Random.Range(0, i + 1);
            T temp = list[i];
            list[i] = list[j];
            list[j] = temp;
        }
    }

    private void PrintBoard(int[,] boardToPrint, string label = "Board")
    {
        System.Text.StringBuilder sb = new();
        sb.AppendLine($"=== {label} ===");
        for(int y = 8; y >= 0; y--)
        {
            for(int x = 0; x < 9; x++)
            {
                sb.Append(boardToPrint[x, y]);
                sb.Append(' ');
            }
            sb.AppendLine();
        }
        Debug.Log(sb.ToString());
    }
}