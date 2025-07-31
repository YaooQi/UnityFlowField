using System;
using System.Collections.Generic;

/// Class for generating Dijkstra Grids
public class DijkstraGrid
{
    //Row and Column depend on grid size
    public int col;
    public int row;
    private Cell[,] cellDetails;
    private readonly Queue<Cell> toVisit;
    private readonly int[,] dijkstraGridArr;

    public DijkstraGrid(int col, int row)
    {
        this.col = col;
        this.row = row;
        cellDetails = new Cell[this.row, this.col];
        toVisit = new Queue<Cell>(col * row);
        dijkstraGridArr = new int[row, col];
    }

    struct Cell
    {
        //Row and Column index of its parent
        //Note that 0 <= i <= ROW - 1 & 0 <= j <= COL - 1
        public int parent_i, parent_j; //indices of parent cell
        public int row, col; //indices in cell array
        public int distance; //distance from the source
    }

    //Utility function to check whether a given cell is valid or not
    bool IsValid(int row, int col)
    {
        //returns true if row# and col# is within range
        return (row >= 0) && (row < this.row) &&
               (col >= 0) && (col < this.col);
    }

    //utility function to check whether the given cell is blocked or not
    bool IsUnBlocked(int[,] grid, int row, int col)
    {
        //returns true if the cell is not clocked else false
        return grid[row, col] != int.MaxValue;
    }

    //Utility function to check whether destination cell has been reached or not
    public bool IsDestination(int row, int col, Vector2Int dest)
    {
        return row == dest.c && col == dest.r;
    }

    //generate djikstra grid on a given obstacle grid
    public int[,] GenerateGrid(int col, int row, Vector2Int b1, Dictionary<Vector2Int, int> obstacleGrid, Vector2Int destination)
    {
        int rOff = b1.r;
        int cOff = b1.c;

        Vector2Int dest = new(destination.c - rOff, destination.r - cOff, row);

        int i;
        int j;

        // int[,] dijkstraGridArr = new int[row, col]; //give the djikstra grid blocked spaces
        Array.Clear(dijkstraGridArr, 0, dijkstraGridArr.Length);

        foreach (KeyValuePair<Vector2Int, int> pair in obstacleGrid)
        {
            int r = pair.Key.c;
            int c = pair.Key.r;
            if (r - rOff < row && c - cOff < col)
            {
                dijkstraGridArr[r - rOff, c - cOff] = pair.Value;
            }
        }

        //check if the destination is out of range
        if (IsValid(dest.c, dest.r) == false)
        {
            LogWrapper.Log("Dijkstra destination is invalid");
            return null;
        }

        //Destination is blocked
        if (IsUnBlocked(dijkstraGridArr, dest.c, dest.r) == false)
        {
            LogWrapper.Log("Dijkstra destination is blocked");
            LogWrapper.Log("Finding a new destination...");

            dest = FindNearestUnblocked(dijkstraGridArr, dest, col, row);

            if (IsValid(dest.c, dest.r) && IsUnBlocked(dijkstraGridArr, dest.c, dest.r))
            {
                LogWrapper.Log("New destination found!");
            }
            else
            {
                LogWrapper.Log("No new destination could be found");
                return null;
            }
        }

        //FLOOD FILL FROM THE END POINT

        //declare a 2D array of cell structures to hold the details of that cell

        //initialize with default values for cellDetails
        for (i = 0; i < row; i++)
        {
            for (j = 0; j < col; j++)
            {
                cellDetails[i, j].parent_i = -1;
                cellDetails[i, j].parent_j = -1;
                cellDetails[i, j].distance = -1;
                cellDetails[i, j].row = i;
                cellDetails[i, j].col = j;
            }
        }

        //initialize destination cell
        cellDetails[dest.c, dest.r].distance = 0;

        i = dest.c; //row
        j = dest.r; //col

        //SET PARENTS AND ADD Item1 FOUR NEIGHBOURS TO QUEUE
        toVisit.Clear();
        //WORKING
        if (j + 1 < col) //check for within bounds
        {

            cellDetails[dest.c, dest.r + 1].parent_i = i;
            cellDetails[dest.c, dest.r + 1].parent_j = j;
            toVisit.Enqueue(cellDetails[dest.c, dest.r + 1]); //right
        }

        if (i > 0)
        {

            cellDetails[dest.c - 1, dest.r].parent_i = i;
            cellDetails[dest.c - 1, dest.r].parent_j = j;
            toVisit.Enqueue(cellDetails[dest.c - 1, dest.r]); //down\
        }

        if (j > 0)
        {
            cellDetails[dest.c, dest.r - 1].parent_i = i;
            cellDetails[dest.c, dest.r - 1].parent_j = j;
            toVisit.Enqueue(cellDetails[dest.c, dest.r - 1]); //left
        }

        if (i + 1 < row)
        {
            cellDetails[dest.c + 1, dest.r].parent_i = i;
            cellDetails[dest.c + 1, dest.r].parent_j = j;
            toVisit.Enqueue(cellDetails[dest.c + 1, dest.r]); //up
        }

        while (toVisit.Count > 0)
        {
            Cell tempCell = toVisit.Dequeue(); //pull cell of the QUEUE

            //check if the destination is out of range
            if (IsValid(tempCell.row, tempCell.col))
            {
                if (cellDetails[tempCell.row, tempCell.col].distance == -1)
                {

                    //Check if destination is blocked
                    if (IsUnBlocked(dijkstraGridArr, tempCell.row, tempCell.col))
                    {
                        //grab the parent's indices
                        int parent_i = cellDetails[tempCell.row, tempCell.col].parent_i;
                        int parent_j = cellDetails[tempCell.row, tempCell.col].parent_j;

                        //calculate distance from parents
                        try
                        {
                            cellDetails[tempCell.row, tempCell.col].distance = cellDetails[parent_i, parent_j].distance + 1;
                        }
                        catch (IndexOutOfRangeException e)
                        {
                            cellDetails[tempCell.row, tempCell.col].distance = cellDetails[parent_i, parent_j].distance + 1;
                            LogWrapper.Log(e.Message);
                        }

                        //change to the indices of the currently selected cell
                        i = tempCell.row;
                        j = tempCell.col;


                        if (j + 1 < col) //check for within bounds
                        {
                            //check if it has been unvisited (parent set to -1,-1)
                            if ((cellDetails[i, j + 1].parent_i == -1) && (cellDetails[i, j + 1].parent_j == -1))
                            {
                                cellDetails[i, j + 1].parent_i = i;
                                cellDetails[i, j + 1].parent_j = j;
                                toVisit.Enqueue(cellDetails[i, j + 1]); //right
                            }
                        }

                        if (i > 0)
                        {
                            if ((cellDetails[i - 1, j].parent_i == -1) && (cellDetails[i - 1, j].parent_j == -1))
                            {
                                cellDetails[i - 1, j].parent_i = i;
                                cellDetails[i - 1, j].parent_j = j;
                                toVisit.Enqueue(cellDetails[i - 1, j]); //down
                            }
                        }

                        if (j > 0)
                        {
                            if ((cellDetails[i, j - 1].parent_i == -1) && (cellDetails[i, j - 1].parent_j == -1))
                            {
                                cellDetails[i, j - 1].parent_i = i;
                                cellDetails[i, j - 1].parent_j = j;
                                toVisit.Enqueue(cellDetails[i, j - 1]); //left
                            }
                        }

                        if (i + 1 < row)
                        {
                            if ((cellDetails[i + 1, j].parent_i == -1) && (cellDetails[i + 1, j].parent_j == -1))
                            {
                                cellDetails[i + 1, j].parent_i = i;
                                cellDetails[i + 1, j].parent_j = j;
                                toVisit.Enqueue(cellDetails[i + 1, j]); //up
                            }
                        }

                    }
                    else
                    {
                        //calculate distance from parents
                        cellDetails[tempCell.row, tempCell.col].distance = int.MaxValue;
                    }
                }//end if uninitialized
            }//end if isValid
        }//end while

        //transfer distance values to the Dijkstra grid
        for (i = 0; i < row; i++)
        {
            //String s = "";
            for (j = 0; j < col; j++)
            {
                if (dijkstraGridArr[i, j] != int.MaxValue)
                {
                    dijkstraGridArr[i, j] = cellDetails[i, j].distance;
                }
            }
        }

        return dijkstraGridArr;

    }

    //PERFORM BREADTH-FIRST SEARCH TO FIND NEAREST UNBLOCKED CELL

    public Vector2Int FindNearestUnblocked(int[,] obstacleGrid, Vector2Int dest, int rowLength, int colLength)
    {
        int dest_i = dest.c;
        int dest_j = dest.r;

        if (obstacleGrid[dest_i, dest_j] == int.MaxValue)
        {
            //declare a 2D array of cell structures to hold the details of that cell
            if (cellDetails == null || cellDetails.GetLength(0) != rowLength || cellDetails.GetLength(1) != colLength)
            {
                cellDetails = new Cell[rowLength, colLength];
            }
            int i;
            int j;

            //initialize with default values for cellDetails
            for (i = 0; i < rowLength; i++)
            {
                for (j = 0; j < colLength; j++)
                {
                    cellDetails[i, j].parent_i = -1;
                    cellDetails[i, j].parent_j = -1;
                    cellDetails[i, j].row = i;
                    cellDetails[i, j].col = j;
                    cellDetails[i, j].distance = -1;
                }
            }

            //FIFO STRUCTURE
            toVisit.Clear();

            //SET PARENTS AND ADD FIRST FOUR NEIGHBOURS TO QUEUE
            //WORKING
            if (dest_j + 1 < rowLength) //check for within bounds
            {
                cellDetails[dest.c, dest.r + 1].parent_i = dest_i;
                cellDetails[dest.c, dest.r + 1].parent_j = dest_j;
                toVisit.Enqueue(cellDetails[dest.c, dest.r + 1]); //right
            }

            if (dest_i > 0)
            {
                cellDetails[dest.c - 1, dest.r].parent_i = dest_i;
                cellDetails[dest.c - 1, dest.r].parent_j = dest_j;
                toVisit.Enqueue(cellDetails[dest.c - 1, dest.r]); //down
            }

            if (dest_j > 0)
            {
                cellDetails[dest.c, dest.r - 1].parent_i = dest_i;
                cellDetails[dest.c, dest.r - 1].parent_j = dest_j;
                toVisit.Enqueue(cellDetails[dest.c, dest.r - 1]); //left
            }

            if (dest_i + 1 < colLength)
            {
                cellDetails[dest.c + 1, dest.r].parent_i = dest_i;
                cellDetails[dest.c + 1, dest.r].parent_j = dest_j;
                toVisit.Enqueue(cellDetails[dest.c + 1, dest.r]); //up
            }

            while (toVisit.Count > 0)
            {
                Cell tempCell = toVisit.Dequeue(); //pull cell of the QUEUE

                if (cellDetails[tempCell.row, tempCell.col].distance == -1)
                {

                    //Destination is blocked
                    if (obstacleGrid[tempCell.row, tempCell.col] == int.MaxValue)
                    {

                        //change to the indices of the currently selected cell
                        i = tempCell.row;
                        j = tempCell.col;


                        if (j + 1 < colLength) //check for within bounds
                        {
                            //check if it has been unvisited (parent set to -1,-1)
                            if ((cellDetails[i, j + 1].parent_i == -1) && (cellDetails[i, j + 1].parent_j == -1))
                            {
                                cellDetails[i, j + 1].parent_i = i;
                                cellDetails[i, j + 1].parent_j = j;
                                toVisit.Enqueue(cellDetails[i, j + 1]); //right
                            }
                        }

                        if (i > 0)
                        {
                            if ((cellDetails[i - 1, j].parent_i == -1) && (cellDetails[i - 1, j].parent_j == -1))
                            {
                                cellDetails[i - 1, j].parent_i = i;
                                cellDetails[i - 1, j].parent_j = j;
                                toVisit.Enqueue(cellDetails[i - 1, j]); //down
                            }
                        }

                        if (j > 0)
                        {
                            if ((cellDetails[i, j - 1].parent_i == -1) && (cellDetails[i, j - 1].parent_j == -1))
                            {
                                cellDetails[i, j - 1].parent_i = i;
                                cellDetails[i, j - 1].parent_j = j;
                                toVisit.Enqueue(cellDetails[i, j - 1]); //left
                            }
                        }

                        if (i + 1 < rowLength)
                        {
                            if ((cellDetails[i + 1, j].parent_i == -1) && (cellDetails[i + 1, j].parent_j == -1))
                            {
                                cellDetails[i + 1, j].parent_i = i;
                                cellDetails[i + 1, j].parent_j = j;
                                toVisit.Enqueue(cellDetails[i + 1, j]); //up
                            }
                        }

                    }
                    else
                    {
                        //calculate distance from parents
                        return new Vector2Int(tempCell.row, tempCell.col, row);
                    }
                }//end if uninitialized
            }//end while
        }

        return new Vector2Int(dest_i, dest_j, row);
    }
}