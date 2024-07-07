using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class QTableData
{
    public int width;
    public int height;
    public int depth;
    public int actionCount;
    public int countTetromino;
    public int maxCountSection;
    public double[,,,,,] values;
}
