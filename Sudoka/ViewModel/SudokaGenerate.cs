using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

/*if (solveSudoku(false, tableSolution1))
                         {
                             tableSolution2 = (int[,])tableSolution1.Clone();
                             baseTable[i, j] = 0;
                             if (solveSudoku(true, tableSolution1))
                                 if(!CompareMas(tableSolution1, tableSolution2)) baseTable[i, j] = temp;
                         }*/


/*if (SolveSudokuForGenerate(tableSolution2, tableSolution1))
                    {
                        if (!CompareMas(tableSolution1, tableSolution2))
                        {
                            baseTable[i, j] = temp;

                        }
                    }*/

namespace WpfApp2
{
class SudokaGenerate
{
    public readonly int N = 9;
    public readonly int[,] TABLE =   {
        {1, 2, 3, 4, 5, 6, 7, 8, 9 },
        {4, 5, 6, 7, 8, 9, 1, 2, 3 },
        {7, 8, 9, 1, 2, 3, 4, 5, 6 },
        {2, 3, 4, 5, 6, 7, 8, 9, 1 },
        {5, 6, 7, 8, 9, 1, 2, 3, 4 },
        {8, 9, 1, 2, 3, 4, 5, 6, 7 },
        {3, 4, 5, 6, 7, 8, 9, 1, 2 },
        {6, 7, 8, 9, 1, 2, 3, 4, 5 },
        {9, 1, 2, 3, 4, 5, 6, 7, 8 }
    };

    Random random;

    delegate void Stir();

    public int[,] baseTable { get; private set; } =
    {
        {1, 2, 3, 4, 5, 6, 7, 8, 9 },
        {4, 5, 6, 7, 8, 9, 1, 2, 3 },
        {7, 8, 9, 1, 2, 3, 4, 5, 6 },
        {2, 3, 4, 5, 6, 7, 8, 9, 1 },
        {5, 6, 7, 8, 9, 1, 2, 3, 4 },
        {8, 9, 1, 2, 3, 4, 5, 6, 7 },
        {3, 4, 5, 6, 7, 8, 9, 1, 2 },
        {6, 7, 8, 9, 1, 2, 3, 4, 5 },
        {9, 1, 2, 3, 4, 5, 6, 7, 8 }
    };

    public SudokaGenerate()
    {
        random = new Random();
    }

    private void RemoveNum(object difficult)
    {
        int[,] flook = (int[,])baseTable.Clone();
        int[,] FirstTable = (int[,])baseTable.Clone();
        int iterator = 0;
        int i, j;
        int _difficult = (int)difficult;
        int[,] tableSolution1 = (int[,])baseTable.Clone();
        int[,] tableSolution2 = (int[,])baseTable.Clone();

//            int kek = 0;

        while (true)
        {
            while (iterator <= _difficult)
            {

                i = random.Next(0, 9);
                j = random.Next(0, 9);

                if (flook[i, j] != 0)
                {
                    flook[i, j] = 0;
                    iterator++;

                   // int temp = baseTable[i, j];
                    baseTable[i, j] = 0;

                        /*  tableSolution1 = (int[,])baseTable.Clone();

                          if (!solveSudoku(tableSolution1))
                          {
                              baseTable[i, j] = temp;
                          }*/
                }
 //               if (kek == 10000)
 //               {
 //                   break;
 //               }
 //               kek++;
            }

//                if (kek == 10000)
//                {
//                    baseTable = (int[,])FirstTable.Clone();
//                   flook = (int[,])baseTable.Clone();
//                    iterator = 0;
//                    kek = 0;
 //                   continue;
 //               }
 //           kek = 0;
            tableSolution1 = (int[,])baseTable.Clone();
            if (!solveSudoku(tableSolution1)) continue;

            tableSolution2 = (int[,])baseTable.Clone();
            if (SolveSudokuForGenerate(tableSolution2, tableSolution1))
            {
                if (CompareMas(tableSolution1, tableSolution2)) break;
            }
            baseTable = (int[,])FirstTable.Clone();
            flook = (int[,])baseTable.Clone();
            iterator = 0;
        }
    }



    private bool CompareMas(int[,] tabletableSolution1, int[,] tabletableSolution2)
    {
        for (int i = 0; i < N; i++)
        {
            for (int j = 0; j < N; j++)
            {
                if (tabletableSolution1[i, j] != tabletableSolution2[i, j]) return false;
            }
        }

        return true;
    }

        public void GenerateSudoka(object difficult)
        {

            baseTable = (int[,])TABLE.Clone();
            Stir stir;
            for (int i = 0; i < 13; i++)
            {
                switch (random.Next(0, 5))
                {
                    case 0:
                        {
                            stir = TransTable;
                            break;
                        }
                    case 1:
                        {
                            stir = SwapRowsSmall;
                            break;
                        }
                    case 2:
                        {
                            stir = SwapColumnsSmall;
                            break;
                        }
                    case 3:
                        {
                            stir = SwapRowsArea;
                            break;
                        }
                    case 4:
                        {
                            stir = SwapColumnsArea;
                            break;
                        }
                    default:
                        {
                            stir = TransTable;
                            break;
                        }
                }
                stir();
            }
            RemoveNum(difficult);
        }

    private void TransTable()
    {
        int[,] NewTable = new int [N, N];

        for(int i = 0; i<N; i++)
        {
            for(int j = 0; j<N; j++)
            {
                NewTable[j, i] = baseTable[i, j];
            }
        }

        baseTable = (int[,])NewTable.Clone();
    }

    private void SwapRowsSmall()
    {
        int rowHalf = N / 3;

        int area = random.Next(0, rowHalf - 1); // случайный район
        int row1 = random.Next(0, rowHalf - 1); // случайная строка 1
        int rowNum1 = area * rowHalf + row1;
        int row2;

        do
        {
            row2 = random.Next(0, rowHalf - 1); // случайная строка 2
        }
        while (row1 == row2);

        int rowNum2 = area * rowHalf + row2;

        int Buffer;
        for(int j = 0; j< N; j++)
        {
            Buffer = baseTable[rowNum1, j];
            baseTable[rowNum1, j] = baseTable[rowNum2, j];
            baseTable[rowNum2, j] = Buffer;
        }

    }

    private void SwapColumnsSmall()
    {
        SwapRowsSmall();
        TransTable();
    }

    private void SwapRowsArea()
    {
        int rowHalf = N / 3;

        int area1 = random.Next(0, rowHalf - 1); // случайный район 1
        int area2;

        do
        {
            area2 = random.Next(0, rowHalf - 1); // случайный район 2
        }
        while (area1 == area2);

        int startArea1 = area1 * rowHalf;
        int startArea2 = area2 * rowHalf;
        int Buffer;

        for(int i = 0; i<3; i++)
        {
            for(int j = 0; j<N; j++)
            {
                Buffer = baseTable[startArea1 + i, j];
                baseTable[startArea1 + i, j] = baseTable[startArea2 + i, j];
                baseTable[startArea2 + i, j] = Buffer;
            }
        }
    }

    private void SwapColumnsArea()
    {
        SwapRowsArea();
        TransTable();
    }

    public bool isSafe(int[,] tableSolution, int row, int col, int num)
    {
        for (int i = 0; i < tableSolution.GetLength(0); i++)
        {
            // проверка строки и столбца на уникальные числа
            if (tableSolution[row, i] == num || tableSolution[i, col] == num)
            {
                return false;
            }
        }

        // проверка квадрата на уникальное число
        int sqrt = (int)Math.Sqrt(tableSolution.GetLength(0));
        int boxRowStart = row - row % sqrt;
        int boxColStart = col - col % sqrt;

        for (int r = boxRowStart; r < boxRowStart + sqrt; r++)
        {
            for (int d = boxColStart; d < boxColStart + sqrt; d++)
            {
                if (tableSolution[r, d] == num)
                        return false;
            }
        }
        return true; // если число подходит
    }

    public bool solveSudokySafe(bool start, int[,] tableSolution)
    {
       // countSolve = 0;
        if (solveSudoku(tableSolution)) return true;
        else return false;
    }


    private bool SolveSudokuForGenerate(int[,] tableSolution, int[,] tableSolutionOld)
    {
        int row = -1;
        int col = -1;
        bool isEmpty = true;

        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (tableSolution[i, j] == 0) // пропущенные значения судоку
                {
                    row = i;
                    col = j;

                    isEmpty = false;
                    break;
                }
            }
            if (!isEmpty) break;
        }

        if (isEmpty) return true; // пустого места не осталось

        for (int num = 9; num >= 1; num--) // для возврата каждой строки
        {
            if (isSafe(tableSolution, row, col, num))
            {
                    //if (tableSolutionOld[row, col] == num) continue;
                    //else 
                    tableSolution[row, col] = num;

                if (SolveSudokuForGenerate(tableSolution, tableSolutionOld))
                {
                    return true;
                }
                else tableSolution[row, col] = 0; // замена

            }
        }
        return false;
    }

    private bool solveSudoku(int[,] tableSolution)
    {
        int row = -1;
        int col = -1;
        bool isEmpty = true;

        for (int i = 0; i < 9; i++)
        {
            for (int j = 0; j < 9; j++)
            {
                if (tableSolution[i, j] == 0) // пропущенные значения судоку
                {
                    row = i;
                    col = j;

                    isEmpty = false;
                    break;
                }
            }
            if (!isEmpty) break;
        }

        if (isEmpty) return true; // пустого места не осталось

        for (int num = 1; num <= 9; num++) // для возврата каждой строки
        {
            if (isSafe(tableSolution, row, col, num))
            {
                tableSolution[row, col] = num;

                if (solveSudoku(tableSolution))
                {
                    return true;
                }
                else tableSolution[row, col] = 0; // замена
            }
        }

        return false;
    }
}
}
