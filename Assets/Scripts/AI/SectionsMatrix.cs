using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

public class SectionsMatrix
{
    private int widthSection;
    private int heightSection;
    private int width;
    private int height;
    //private int sectionWidth = 2;

    private int[,] sections;

    public int encodeSection;


    private Dictionary<string, int> configurationsMapping = new Dictionary<string, int>{
        //1 square
        { "0000000000", 0 },
        { "1100000000",1 },
        { "0110000000",2 },
        { "0011000000",3 },
        { "0001100000",4 },
        { "0000110000",5 },
        { "0000011000",6 },
        { "0000001100",7 },
        { "0000000110",8 },
        { "0000000011",9 },
        //2 square
        { "1111000000",10 },
        { "1101100000",11 },
        { "1100110000",12 },
        { "1100011000",13 },
        { "1100001100",14 },
        { "1100000110",15 },
        { "1100000011",16 },
        { "0111100000",17 },
        { "0110110000",18 },
        { "0110011000",19 },
        { "0110001100",20 },
        { "0110000110",21 },
        { "0110000011",22 },
        { "0011110000",23 },
        { "0011011000",24 },
        { "0011001100",25 },
        { "0011000110",26 },
        { "0011000011",27 },
        { "0001111000",28 },
        { "0001101100",29 },
        { "0001100110",30 },
        { "0001100011",31 },
        { "0000111100",32 },
        { "0000110110",33 },
        { "0000110011",34 },
        { "0000011110",35 },
        { "0000011011",36 },
        { "0000001111",37 },
        //3 squares
        { "1111110000",38 },
        { "1111011000",39 },
        { "1111001100",40 },
        { "1111000110",41 },
        { "1111000011",42 },
        { "1101111000",43 },
        { "1101101100",44 },
        { "1101100110",45 },
        { "1101100011",46 },
        { "1100111100",47 },
        { "1100110110",48 },
        { "1100110011",49 },
        { "1100011110",50 },
        { "1100011011",51 },
        { "1100001111",52 },
        { "0111111000",53 },
        { "0111101100",54 },
        { "0111100110",55 },
        { "0111100011",56 },
        { "0110111100",57 },
        { "0110110110",58 },
        { "0110110011",59 },
        { "0110011110",60 },
        { "0110011011",61 },
        { "0110001111",62 },
        { "0011111100",63 },
        { "0011110110",64 },
        { "0011110011",65 },
        { "0011011110",66 },
        { "0011011011",67 },
        { "0011001111",68 },
        { "0001111110",69 },
        { "0001111011",70 },
        { "0001101111",71 },
        { "0000111111",72 },
        //4 square 
        { "1111111100",73 },
        { "1111110110",74 },
        { "1111110011",75 },
        { "1111011110",76 },
        { "1111011011",77 },
        { "1111001111",78 },
        { "1101111110",79 },
        { "1101111011",80 },
        { "1101101111",81 },
        { "1100111111",82 },
        { "0111111110",83 },
        { "0111111011",84 },
        { "0111101111",85 },
        { "0110111111",86 },
        { "0011111111",87 },
        //5 sqaure
        { "1111111111",88 },


    };


    public SectionsMatrix(int widthSection, int heightSection, int width, int height)
    {
        this.widthSection = widthSection;
        this.heightSection = heightSection;
        this.width = width;
        this.height = height;
        sections = new int[width, heightSection];
    }

    public int CalculateMaxCountSectionsOnTheFloor()
    {
        //int maxSections = width / 2; // ћаксимальное количество секций, которые могут поместитьс€ на полу
        //int totalWays = 1; // Ќачальное количество способов
        //int res = 1;

        //for (int i = 0; i < maxSections; i++)
        //{
        //    totalWays *= (width - 2 * i); // ѕримен€ем формулу дл€ каждой секции
        //    res += totalWays;
        //}

        //return res;

        int[] dp = new int[11]; // ƒинамический массив, где dp[i] - количество способов заполнить колонки до i-й
        dp[0] = 1; // »нициализируем базовый случай: один способ заполнить 0 колонок (ничего не делать)

        for (int i = 1; i <= 10; i++)
        {
            dp[i] = dp[i - 1]; // —пособ заполнени€, когда последн€€ колонка не зан€та
            if (i >= 2)
            {
                dp[i] += dp[i - 2]; // ƒобавл€ем способы, когда последние две колонки зан€ты одним квадратом
            }
        }

        return dp[10];
    } //89

    public double[,] CalculateFillingSection(int[,] grid)
    {
        int rows = grid.GetLength(0);
        int cols = grid.GetLength(1);

        // —оздаем новую матрицу такого же размера
        double[,] filledGrid = new double[rows, cols];

        //  опируем все значени€ из исходной матрицы в новую (незаполненные €чейки оставл€ем как 0)
        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                filledGrid[i, j] = grid[i, j];
            }
        }

        // ѕровер€ем только два последних р€да
        for (int i = rows - 2; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                if (grid[i, j] == 1) // ≈сли найдена зан€та€ €чейка
                {
                    // «аполн€ем блок 2x2, убежда€сь, что мы не выходим за пределы матрицы
                    for (int k = 0; k < 2; k++)
                    {
                        for (int l = 0; l < 2; l++)
                        {
                            if (i + k < rows && j + l < cols)
                            {
                                filledGrid[i + k, j + l] = 1;
                            }
                        }
                    }
                }
            }
        }

        for (int i = 0; i < filledGrid.GetLength(0); i++)
        {
            for (int j = 0; j < filledGrid.GetLength(1); j++)
            {
                Debug.Log($"myArray[{i},{j}] = {filledGrid[i, j]}");
            }
        }
        return filledGrid;
    }


    public int Encode(int[,] grid)
    {

        sections = FillSections(grid);

        char[] code = new char[10];
        Array.Fill(code, '0');

        for (int col = 0; col <= width - widthSection; col++)
        {
            if (sections[col, 0] == 1 && sections[col, 1] == 1 && sections[col + 1, 0] == 1 && sections[col + 1, 1] == 1)
            {
                code[col] = '1';
                code[col + 1] = '1';
            }
        }

        string configurationCode = new string(code);
        if (configurationsMapping.TryGetValue(configurationCode, out int index))
        {
            encodeSection = index;
            return index;
        }
        encodeSection = -1;
        return -1; // ≈сли конфигураци€ не найдена
    }


    public double[,] DecodeSection()
    {
        return new double[,] { { 0 } };
    }

    public int[,] FillSections(int[,] grid)
    {
        int[,] res = new int[grid.GetLength(0), heightSection];
        int countY = 0;

        for (int i = grid.GetLength(1) - heightSection; i < grid.GetLength(1); i++)
        {
            for (int j = 0; j < grid.GetLength(0); j++)
            {
                res[j, countY] = grid[j, i];
            }
            countY++;
        }
        return res;
    }



}
