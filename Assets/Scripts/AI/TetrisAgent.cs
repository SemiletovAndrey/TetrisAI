using System;
using System.Collections;
using System.Linq;
using System.Numerics;
using UnityEngine;

public class TetrisAgent : MonoBehaviour
{
    private QTable qTable;
    private ActionT currentAction;

    public Piece piece;
    public State currentState;

    [SerializeField] private Board board;
    [SerializeField] private double learningRate = 0.1;
    [SerializeField] private double discountFactor = 0.9;
    [SerializeField] private double lineClearReward = 10;
    [Space(10)]
    [SerializeField] private bool isFirst = true;
    [SerializeField] private string fileName = "QTable";

    public Vector2Int[] pos;

    private int width;
    private int height;
    private int maxSections;

    private void Start()
    {
        width = board.boardSize.x;
        height = board.boardSize.y;
        currentState = new State(width, height, 2, 2);
        maxSections = currentState.sectionsMatrix.CalculateMaxCountSectionsOnTheFloor();
        Debug.Log($"Max count sections {maxSections}");
        qTable = new QTable(width, height, 3, Enum.GetNames(typeof(ActionT)).Length, board.tetrominos.Length, maxSections);
        currentAction = ActionT.None;

        isFirst = PlayerPrefs.GetInt("ToggleValue") == 1 ? true : false;

        if (isFirst == true)
        {
            qTable.InitializeRandom();
        }
        else
        {
            qTable.Load(fileName);
        }


        StartCoroutine(ApplyAction());
    }

    void LateUpdate()
    {

    }

    private ActionT ChooseAction(State currentState, QTable qTable)
    {
        int x = currentState.position[0].x;
        int y = currentState.position[0].y;
        int rotation = currentState.rotation;
        int currentTetrominoIndex = (int)currentState.currentTetromino.tetromino;
        int indexSection = currentState.sectionsMatrix.encodeSection;           //TO DO

        Debug.Log($"x {x}; y {y}; sectoin {indexSection}; rotation {rotation}; currentTetromino {currentTetrominoIndex}");
        double maxQValue = double.MinValue;
        ActionT chosenAction = ActionT.Right;

        for (int actionIndex = 0; actionIndex < qTable.ActionCount; actionIndex++)
        {
            double qValue = qTable.Values[x, y, rotation, actionIndex, currentTetrominoIndex, indexSection];
            if (qValue > maxQValue)
            {
                maxQValue = qValue;
                chosenAction = (ActionT)actionIndex;
            }
        }
        return chosenAction;
    }

    [ContextMenu("Move")]
    public void ApplyActionToTetromino()
    {
        // Применение действия к тетромино
        switch (currentAction)
        {
            case ActionT.None:
                //Debug.Log("None");
                break;
            case ActionT.Left:
                piece.MoveLeft();
                //Debug.Log("Left");
                break;
            case ActionT.Right:
                piece.MoveRight();
                //Debug.Log("Right");
                break;
        }

    }


    [ContextMenu("PrintArrayGrid")]
    public void PrintArrayGrid()
    {
        //for (int i = 0; i < currentState.grid.GetLength(0); i++)
        //{
        //    for (int j = 0; j < currentState.grid.GetLength(1); j++)
        //    {
        //        Debug.Log($"myArray[{i},{j}] = {currentState.grid[i, j]}");
        //    }
        //}
        //int encodeValue = currentState.sectionsMatrix.Encode(currentState.grid);
        //currentState.UpdateSections();
        Debug.Log($"EncodeValue {currentState.sectionsMatrix.encodeSection}");
    }

    [ContextMenu("PrintArrayPosition")]
    public void PrintPosition()
    {
        Debug.Log($"X: {currentState.position[0].x} Y: {currentState.position[0].y}");
    }

    private IEnumerator ApplyAction()
    {
        while (true)
        {
            yield return null;
            State nextState = UpdateStateAfterAction(currentState, currentAction); //
            double reward = CalculateReward(currentState, currentAction, nextState);//
            UpdateQValue(currentState, currentAction, reward, nextState);//
            currentAction = ChooseAction(currentState, qTable);
            //Debug.Log($"CurrentValue {qTable.Values[currentState.position[0].x, currentState.position[0].y,currentState.rotation,(int)currentAction,(int)currentState.currentTetromino.tetromino]}");

            yield return new WaitForSeconds(0.1f);

        }
    }


    public void UpdateQValue(State currentState, ActionT action, double reward, State nextState)
    {
        double currentQValue = qTable.Get(currentState, action);
        double maxNextQValue = GetMaxQValue(nextState);
        double newQValue = currentQValue + learningRate * (reward + discountFactor * maxNextQValue - currentQValue);
        qTable.Set(currentState, action, newQValue);
    }

    private double GetMaxQValue(State state)
    {
        int x = state.position[0].x;
        int y = state.position[0].y;
        int rotation = state.rotation;
        int currentTetrominoIndex = (int)state.currentTetromino.tetromino;
        int indexSection = state.sectionsMatrix.encodeSection;

        double maxQValue = double.MinValue;

        for (int actionIndex = 0; actionIndex < qTable.ActionCount; actionIndex++)
        {
            double qValue = qTable.Values[x, y, rotation, actionIndex, currentTetrominoIndex, indexSection];
            if (qValue > maxQValue)
            {
                maxQValue = qValue;
            }
        }

        return maxQValue;
    }

    private State CloneCurrentState(State currentState)
    {
        State clone = new State(width, height, 2, 2);
        clone.position = currentState.position;
        clone.rotation = currentState.rotation;
        clone.grid = currentState.grid;
        clone.currentTetromino = currentState.currentTetromino;
        return clone;
    }

    private State UpdateStateAfterAction(State state, ActionT action)
    {
        State nextState = CloneCurrentState(state);
        //nextState.ClearGrid(nextState.position);
        switch (action)
        {
            case ActionT.Left:
                if (nextState.position.All(p => p.x - 1 >= 0))
                {
                    //nextState.UpdateState(nextState.position.Select(p => new Vector2Int(p.x - 1, p.y)).ToArray(), nextState.currentTetromino);
                }
                else
                {
                    qTable.Set(state, action, -100);
                }
                break;
            case ActionT.Right:
                if (nextState.position.All(p => p.x + 1 < nextState.grid.GetLength(0)))
                {
                    //nextState.UpdateState(nextState.position.Select(p => new Vector2Int(p.x + 1, p.y)).ToArray(), nextState.currentTetromino);
                }
                else
                {
                    qTable.Set(state, action, -100);
                }
                break;

        }
        return nextState;
    }



    public double CalculateReward(State currentState, ActionT action, State nextState)
    {
        // Пример: назначаем положительное вознаграждение, если после действия тетромино
        // упало на новый уровень (был заполнен ряд), и отрицательное вознаграждение в противном случае.

        int currentLinesCleared = CountClearedLines(currentState);
        int nextLinesCleared = CountClearedLines(nextState);

        int linesCleared = nextLinesCleared - currentLinesCleared;

        // Назначаем вознаграждение в зависимости от количества очищенных линий
        double reward = linesCleared * lineClearReward; // Пример: 10 очков за каждую очищенную линию
        //Debug.Log($"Reward: {reward}");
        // Вы также можете добавить другие условия и параметры для расчета вознаграждения в вашей конкретной ситуации.

        return reward;
    }




    private int CountClearedLines(State state)
    {
        int linesCleared = 0;

        for (int y = 0; y < state.grid.GetLength(1); y++)
        {
            bool isLineCleared = true;

            for (int x = 0; x < state.grid.GetLength(0); x++)
            {
                if (state.grid[x, y] == 0)
                {
                    isLineCleared = false;
                    break;
                }
            }

            if (isLineCleared)
            {
                linesCleared++;
            }
        }

        return linesCleared;
    }   //готовый


    public void SaveQTable()
    {
        qTable.Save(fileName);
    }       //готовый

}
