using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Unity.MLAgents;
using Unity.MLAgents.Sensors;
using Unity.MLAgents.Actuators;
using Zenject;

public class TetrisAgentML : Agent
{
    private Board gameBoard; 
    private Piece piece;

    [Inject]
    public void Construct(Board board, Piece piece)
    {
        gameBoard = board;
        this.piece = piece;
    }

    public override void OnEpisodeBegin()
    {
        gameBoard.SpawnPiece();
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        for (int x = 0; x < gameBoard.boardSize.x; x++)
        {
            for (int y = 0; y < gameBoard.boardSize.y; y++)
            {
                sensor.AddObservation(gameBoard.currentState.grid[x, y]); 
            }
        }
        foreach (Vector2Int pos in gameBoard.currentState.position)
        {
            sensor.AddObservation(pos.x);
            sensor.AddObservation(pos.y);
        }
        int currentTetrominoIndex = (int)gameBoard.currentState.currentTetromino.tetromino; 
        sensor.AddObservation(currentTetrominoIndex);
    }


    public override void OnActionReceived(ActionBuffers actions)
    {
        if (piece.actionTimer <= 0)
        {
            int action = actions.DiscreteActions[0];
            Vector2Int moveDirection = Vector2Int.zero;

            switch (action)
            {
                case 0:
                    //moveDirection = Vector2Int.down;
                    break;
                case 1:
                    moveDirection = Vector2Int.left;
                    break;
                case 2:
                    moveDirection = Vector2Int.right;
                    break;
            }

            piece.SetAgentAction(moveDirection);
            piece.actionTimer = piece.actionCooldown;
        }
    }


    public override void Heuristic(in ActionBuffers actionsOut)
    {
        Vector2Int moveDirection = Vector2Int.zero;
        var discreteActionsOut = actionsOut.DiscreteActions;
        if (Input.GetKey(KeyCode.LeftArrow))
            moveDirection = Vector2Int.left;
        if (Input.GetKey(KeyCode.RightArrow))
            moveDirection = Vector2Int.right;

        piece.SetAgentAction(moveDirection);

    }


    public void CalculateReward(float value)
    {
        Debug.Log($"Value {value}");
        AddReward(value);
    }

    public void EndEpisodeTetris()
    {
        EndEpisode();
    }
}
