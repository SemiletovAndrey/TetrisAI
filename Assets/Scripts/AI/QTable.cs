using System.IO;
using UnityEngine;
using System.Runtime.Serialization.Formatters.Binary;
using UnityEngine.Tilemaps;
using System;


public enum ActionT
{
    None = 0,
    Left = 1,
    Right = 2
}



public class QTable
{
    private int width;
    private int height;
    private int depth;
    private int actionCount;
    private int countTetromino;
    private int maxCountSection;
    private double[,,,,,] _values;

    public int Width { get { return width; } }
    public int Height { get { return height; } }
    public int Depth { get { return depth; } }
    public int ActionCount { get { return actionCount; } }
    public int MaxCountSection { get { return maxCountSection; } }
    public double[,,,,,] Values { get { return _values; } }


    public QTable(int width, int height, int depth, int actionCount, int countTetromino, int maxCountSection)
    {
        this.width = width;
        this.height = height;
        this.depth = depth;
        this.actionCount = actionCount;
        this.countTetromino = countTetromino;
        this.maxCountSection = maxCountSection;
        _values = new double[width, height, depth, actionCount, countTetromino, maxCountSection];
    }

    public void InitializeRandom()   
    {
        Debug.Log("Init");
        for (int x = 0; x < width; x++)
        {
            for (int y = 0; y < height; y++)
            {
                for (int rotation = 0; rotation < depth; rotation++)
                {
                    for (int actionIndex = 0; actionIndex < actionCount; actionIndex++)
                    {
                        for (int countT = 0; countT < countTetromino; countT++)
                        {
                            for(int maxCountS = 0; maxCountS < maxCountSection; maxCountS++)
                            {
                                _values[x, y, rotation, actionIndex, countT, maxCountS] = UnityEngine.Random.Range(0, 100);
                            }
                        }
                    }
                }
            }
        }
    }

    public double Get(State state, ActionT action)
    {
        int x = (int)state.position[0].x;
        int y = (int)state.position[0].y;
        int rotation = state.rotation;
        int actionIndex = (int)action;
        int indexTetromino = (int)state.currentTetromino.tetromino;
        int indexSection = state.sectionsMatrix.encodeSection;                                      
        return _values[x, y, rotation, actionIndex, indexTetromino, indexSection];
    }

    public void Set(State state, ActionT action, double value)
    {
        int x = state.position[0].x;
        int y = state.position[0].y;
        int rotation = state.rotation;
        int actionIndex = (int)action;
        int indexTetromino = (int)state.currentTetromino.tetromino;
        int indexSection = state.sectionsMatrix.encodeSection;                                               
        _values[x, y, rotation, actionIndex, indexTetromino,indexSection] = value;
    }

    public QTableData GetQTableData()
    {
        return new QTableData
        {
            width = width,
            height = height,
            depth = depth,
            actionCount = actionCount,
            countTetromino = countTetromino,
            maxCountSection = maxCountSection,
            values = _values
        };
    }

    public void SetQTableData(QTableData data)
    {
        width = data.width;
        height = data.height;
        depth = data.depth;
        actionCount = data.actionCount;
        countTetromino = data.countTetromino;
        maxCountSection = data.maxCountSection;
        _values = data.values;
    }

    public void Save(string fileName)
    {
        BinaryFormatter formatter = new BinaryFormatter();
        QTableData data = GetQTableData();
        using (FileStream stream = new FileStream(fileName, FileMode.Create))
        {
            formatter.Serialize(stream, data);
            Debug.Log($"Saved data: {data}");
        }
    }

    public void Load(string fileName)
    {
        if (File.Exists(fileName))
        {
            BinaryFormatter formatter = new BinaryFormatter();
            using (FileStream stream = new FileStream(fileName, FileMode.Open))
            {
                try
                {
                    QTableData data = formatter.Deserialize(stream) as QTableData;
                    if (data != null)
                    {
                        SetQTableData(data);
                        Debug.Log($"Loaded data: {data}");
                    }
                    else
                    {
                        Debug.LogError("Failed to deserialize the file or the data is not of the expected type.");
                    }
                }
                catch (Exception ex)
                {
                    Debug.LogError($"Error during deserialization: {ex.Message}");
                }
            }
        }
        else
        {
            Debug.LogError($"File not found: {fileName}");
        }
    }
}




