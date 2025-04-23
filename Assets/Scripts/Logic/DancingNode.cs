using System.Collections.Generic;

public class DancingNode
{
    public DancingNode left, right, up, down;
    public ColumnNode column;
    public int rowID;  //For Sudoku: encodes the cell and value

    public DancingNode()
    {
        left = right = up = down = this;
    }

    public DancingNode(ColumnNode c)
    {
        left = right = up = down = this;
        column = c;
    }

    public void RemoveHorizontal()
    {
        left.right = right;
        right.left = left;
    }

    public void RestoreHorizontal()
    {
        left.right = this;
        right.left = this;
    }

    public void RemoveVertical()
    {
        up.down = down;
        down.up = up;
        column.size--;
    }

    public void RestoreVertical()
    {
        up.down = this;
        down.up = this;
        column.size++;
    }
}

public class ColumnNode : DancingNode
{
    public int size;
    public string name;

    public ColumnNode(string n) : base()
    {
        size = 0;
        name = n;
        column = this;
    }

    public void Cover()
    {
        RemoveHorizontal();
        
        for(DancingNode i = down; i != this; i = i.down)
        {
            for(DancingNode j = i.right; j != i; j = j.right)
            {
                j.RemoveVertical();
            }
        }
    }

    public void Uncover()
    {
        for(DancingNode i = up; i != this; i = i.up)
        {
            for(DancingNode j = i.left; j != i; j = j.left)
            {
                j.RestoreVertical();
            }
        }
        
        RestoreHorizontal();
    }
}

public class DLX
{
    private ColumnNode header;
    private List<DancingNode> solution;
    private int[,] grid;
    
    public DLX(int[,] initialGrid)
    {
        grid = new int[9, 9];
        for(int i = 0; i < 9; i++)
        {
            for(int j = 0; j < 9; j++)
            {
                grid[i, j] = initialGrid[i, j];
            }
        }
        
        header = CreateDLXGrid();
        solution = new List<DancingNode>();
    }
    
    private ColumnNode CreateDLXGrid()
    {
        //Create the header node
        ColumnNode headerNode = new ColumnNode("header");
        List<ColumnNode> columnNodes = new List<ColumnNode>();
        
        //Create column nodes for each constraint
        //9x9 cells, each containing one value: 81 constraints
        for(int i = 0; i < 9; i++)
        {
            for(int j = 0; j < 9; j++)
            {
                columnNodes.Add(new ColumnNode($"Cell_{i}{j}"));
            }
        }
        
        //Each row must contain each digit 1-9: 81 constraints
        for(int i = 0; i < 9; i++)
        {
            for(int num = 1; num <= 9; num++)
            {
                columnNodes.Add(new ColumnNode($"Row_{i}#{num}"));
            }
        }
        
        //Each column must contain each digit 1-9: 81 constraints
        for(int j = 0; j < 9; j++)
        {
            for(int num = 1; num <= 9; num++)
            {
                columnNodes.Add(new ColumnNode($"Col_{j}#{num}"));
            }
        }
        
        //Each 3x3 block must contain each digit 1-9: 81 constraints
        for(int block = 0; block < 9; block++)
        {
            for(int num = 1; num <= 9; num++)
            {
                columnNodes.Add(new ColumnNode($"Block_{block}#{num}"));
            }
        }
        
        //Link the column nodes horizontally
        headerNode.right = columnNodes[0];
        columnNodes[0].left = headerNode;
        
        for(int i = 0; i < columnNodes.Count - 1; i++)
        {
            columnNodes[i].right = columnNodes[i + 1];
            columnNodes[i + 1].left = columnNodes[i];
        }
        
        columnNodes[columnNodes.Count - 1].right = headerNode;
        headerNode.left = columnNodes[columnNodes.Count - 1];
        
        //Create the DLX grid
        //For each possible cell value (row, col, num)
        for(int row = 0; row < 9; row++)
        {
            for(int col = 0; col < 9; col++)
            {
                for(int num = 1; num <= 9; num++)
                {
                    //Skip if the cell is already filled with a different number
                    if(grid[col, row] != 0 && grid[col, row] != num)
                        continue;
                    
                    DancingNode rowNode = null;
                    
                    //Calculate block index (0-8)
                    int block = (row / 3) * 3 + (col / 3);
                    
                    //Cell constraint (which cell is being filled)
                    int cellConstraintIndex = row * 9 + col;
                    DancingNode cellNode = new DancingNode(columnNodes[cellConstraintIndex]);
                    cellNode.rowID = (row * 9 + col) * 9 + (num - 1);
                    
                    if(rowNode == null)
                        rowNode = cellNode;
                    
                    //Add to column
                    cellNode.up = columnNodes[cellConstraintIndex].up;
                    cellNode.down = columnNodes[cellConstraintIndex];
                    cellNode.up.down = cellNode;
                    columnNodes[cellConstraintIndex].up = cellNode;
                    columnNodes[cellConstraintIndex].size++;
                    
                    //Row constraint (row r must contain number n)
                    int rowConstraintIndex = 81 + row * 9 + (num - 1);
                    DancingNode rowConstraintNode = new DancingNode(columnNodes[rowConstraintIndex]);
                    rowConstraintNode.rowID = cellNode.rowID;
                    
                    //Link horizontally
                    rowConstraintNode.left = rowNode.left;
                    rowConstraintNode.right = rowNode;
                    rowNode.left.right = rowConstraintNode;
                    rowNode.left = rowConstraintNode;
                    
                    //Add to column
                    rowConstraintNode.up = columnNodes[rowConstraintIndex].up;
                    rowConstraintNode.down = columnNodes[rowConstraintIndex];
                    rowConstraintNode.up.down = rowConstraintNode;
                    columnNodes[rowConstraintIndex].up = rowConstraintNode;
                    columnNodes[rowConstraintIndex].size++;
                    
                    //Column constraint (column c must contain number n)
                    int colConstraintIndex = 162 + col * 9 + (num - 1);
                    DancingNode colConstraintNode = new DancingNode(columnNodes[colConstraintIndex]);
                    colConstraintNode.rowID = cellNode.rowID;
                    
                    //Link horizontally
                    colConstraintNode.left = rowNode.left;
                    colConstraintNode.right = rowNode;
                    rowNode.left.right = colConstraintNode;
                    rowNode.left = colConstraintNode;
                    
                    //Add to column
                    colConstraintNode.up = columnNodes[colConstraintIndex].up;
                    colConstraintNode.down = columnNodes[colConstraintIndex];
                    colConstraintNode.up.down = colConstraintNode;
                    columnNodes[colConstraintIndex].up = colConstraintNode;
                    columnNodes[colConstraintIndex].size++;
                    
                    //Block constraint (block b must contain number n)
                    int blockConstraintIndex = 243 + block * 9 + (num - 1);
                    DancingNode blockConstraintNode = new DancingNode(columnNodes[blockConstraintIndex]);
                    blockConstraintNode.rowID = cellNode.rowID;
                    
                    //Link horizontally
                    blockConstraintNode.left = rowNode.left;
                    blockConstraintNode.right = rowNode;
                    rowNode.left.right = blockConstraintNode;
                    rowNode.left = blockConstraintNode;
                    
                    //Add to column
                    blockConstraintNode.up = columnNodes[blockConstraintIndex].up;
                    blockConstraintNode.down = columnNodes[blockConstraintIndex];
                    blockConstraintNode.up.down = blockConstraintNode;
                    columnNodes[blockConstraintIndex].up = blockConstraintNode;
                    columnNodes[blockConstraintIndex].size++;
                }
            }
        }
        
        return headerNode;
    }

    public bool Solve()
    {
        if(header.right == header)
            return true; //All constraints satisfied
        
        //Choose column with fewest possibilities (MRV heuristic)
        ColumnNode column = (ColumnNode)header.right;
        for(ColumnNode j = (ColumnNode)header.right; j != header; j = (ColumnNode)j.right)
        {
            if(j.size < column.size)
                column = j;
        }
        
        column.Cover();
        
        for(DancingNode r = column.down; r != column; r = r.down)
        {
            solution.Add(r);
            
            for(DancingNode j = r.right; j != r; j = j.right)
                j.column.Cover();
            
            if(Solve())
                return true;
            
            //If we get here, we need to backtrack
            r = solution[solution.Count - 1];
            solution.RemoveAt(solution.Count - 1);
            
            column = r.column;
            for(DancingNode j = r.left; j != r; j = j.left)
                j.column.Uncover();
        }
        
        column.Uncover();
        return false;
    }

    public int[,] GetSolution()
    {
        int[,] result = new int[9, 9];
        
        //Initialize with the original values
        for(int i = 0; i < 9; i++)
            for(int j = 0; j < 9; j++)
                result[i, j] = grid[i, j];
        
        //Apply the solution
        foreach(DancingNode node in solution)
        {
            int id = node.rowID;
            int row = id / 81;
            int col = (id % 81) / 9;
            int num = (id % 9) + 1;
            result[col, row] = num;
        }
        
        return result;
    }
}