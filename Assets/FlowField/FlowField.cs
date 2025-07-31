using UnityEngine;

/// This class takes in a Dijkstra grid and returns a
/// 2D array of Vector3's representing the FlowField
public class FlowField
{
    private readonly int col;
    private readonly int row;
    private readonly Vector3[,] flowfieldArr;

    public FlowField(int col, int row)
    {
        this.col = col;
        this.row = row;
        flowfieldArr = new Vector3[row, col];
    }

    public Vector3[,] Generate(int[,] dGrid)
    {
        for (int j = 0; j < col; j++)
        {
            for (int i = 0; i < row; i++)
            {
                int min = int.MaxValue;
                int j_dest = -1; //the indices of the cell with the smallest cost value
                int i_dest = -1; //set these to -1 so we know if they are not set

                //(i,j+1) EAST
                if (j + 1 < col)
                {
                    if ((dGrid[i, j + 1] < min) && (dGrid[i, j + 1] != -1))
                    {
                        min = dGrid[i, j + 1];
                        i_dest = i;
                        j_dest = j + 1;
                    }
                }

                //(i-1,j+1) SOUTH-EAST
                if ((j + 1 < col) && (i > 0))
                {
                    if ((dGrid[i - 1, j + 1] < min) && (dGrid[i - 1, j + 1] != -1))
                    {
                        min = dGrid[i - 1, j + 1];
                        i_dest = i - 1;
                        j_dest = j + 1;
                    }
                }

                //(i,j+1) SOUTH
                if (i > 0)
                {
                    if ((dGrid[i - 1, j] < min) && (dGrid[i - 1, j] != -1))
                    {
                        min = dGrid[i - 1, j];
                        i_dest = i - 1;
                        j_dest = j;
                    }
                }

                //(i-1,j-1) SOUTH-WEST
                if ((j > 0) && (i > 0))
                {
                    if ((dGrid[i - 1, j - 1] < min) && (dGrid[i - 1, j - 1] != -1))
                    {
                        min = dGrid[i - 1, j - 1];
                        i_dest = i - 1;
                        j_dest = j - 1;
                    }
                }

                //(i,j+1) WEST
                if (j > 0)
                {
                    if ((dGrid[i, j - 1] < min) && (dGrid[i, j - 1] != -1))
                    {
                        min = dGrid[i, j - 1];
                        i_dest = i;
                        j_dest = j - 1;
                    }
                }

                //(i+1,j-1) NORTH-WEST
                if ((j > 0) && (i + 1 < row))
                {
                    if ((dGrid[i + 1, j - 1] < min) && (dGrid[i + 1, j - 1] != -1))
                    {
                        min = dGrid[i + 1, j - 1];
                        i_dest = i + 1;
                        j_dest = j - 1;
                    }
                }

                //(i,j+1) NORTH
                if (i + 1 < row)
                {
                    if ((dGrid[i + 1, j] < min) && (dGrid[i + 1, j] != -1))
                    {
                        min = dGrid[i + 1, j];
                        i_dest = i + 1;
                        j_dest = j;
                    }
                }

                //(i+1,j-1) NORTH-EAST
                if ((j + 1 < col) && (i + 1 < row))
                {
                    if ((dGrid[i + 1, j + 1] < min) && (dGrid[i + 1, j + 1] != -1))
                    {
                        i_dest = i + 1;
                        j_dest = j + 1;
                    }
                }

                Vector3 field = new()
                {
                    y = 0.0f,
                    x = i_dest - i,
                    z = j_dest - j
                };

                if ((i_dest == -1) || (j_dest == -1))
                {
                    field = new Vector3(0, 0, 0);
                }

                flowfieldArr[i, j] = field / field.magnitude; //normalize vector

            }//end for j
        }//end for i

        return flowfieldArr;

    }//end function
}