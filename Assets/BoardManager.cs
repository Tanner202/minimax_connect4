using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoardManager : MonoBehaviour
{   
    [SerializeField] private GameObject winText;
    [SerializeField] private GameObject loseText;

    [SerializeField] private Material aIMaterial;
    [SerializeField] private Material playerMaterial;
    [SerializeField] private Transform gridLayout;

    private Board mainBoard;

    void Start()
    {
        mainBoard = new Board();

        mainBoard.OnAddPiece += UpdateColor;
    }

    public void AddPiece(int column)
    {
        mainBoard.AddPiece(column, Board.Owner.Player);
        (float, int) data = Minimax(mainBoard, 5, Board.Owner.AI, -Mathf.Infinity, Mathf.Infinity);
        mainBoard.AddPiece(data.Item2, Board.Owner.AI);

        if (mainBoard.IsWinner(Board.Owner.Player))
        {
            winText.SetActive(true);
        }
        else if (mainBoard.IsWinner(Board.Owner.AI))
        {
            loseText.SetActive(true);
        }
    }

    private void UpdateColor(object sender, Board.OnAddPieceEventArgs e)
    {
        int count = 0;
        foreach (Transform child in gridLayout)
        {
            if (count == (e.row * 7) + e.column)
            {
                if (e.owner == Board.Owner.Player)
                {
                    child.GetComponent<Image>().material = playerMaterial;
                }
                else if (e.owner == Board.Owner.AI)
                {
                    child.GetComponent<Image>().material = aIMaterial;
                }
            }
            count += 1;
        }
    }

    private (float, int) Minimax(Board board, int depth, Board.Owner owner, float alpha, float beta)
    {
        if (board.IsWinner(owner))
        {
            if (owner == Board.Owner.Player)
            {
                return (-999999999, -1);
            }
            else if (owner == Board.Owner.AI)
            {
                return (999999999, -1);
            }
            else
            {
                return (0, -1);
            }
        }
        else if (depth == 0)
        {
            int score = board.CalculateScore();
            return (score, -1);
        }

        if (owner == Board.Owner.AI)
        {
            return GetBestMaximizerScore(board, depth, alpha, beta);
        }
        else
        {
            return GetBestMinimizerScore(board, depth, alpha, beta);
        }
    }

    private (float, int) GetBestMaximizerScore(Board board, int depth, float alpha, float beta)
    {
        float bestScore = -Mathf.Infinity;

        int bestColumn = 3;
        for (int column = 0; column < 7; column++)
        {
            Board boardCopy = new Board();
            boardCopy.SetBoard(board);
            float score;

            if (boardCopy.IsValidLocation(column))
            {
                boardCopy.AddPiece(column, Board.Owner.AI);
            }
            else
            {
                continue;
            }
            score = Minimax(boardCopy, depth - 1, Board.Owner.Player, alpha, beta).Item1;

            if (score > bestScore)
            {
                bestScore = score;
                bestColumn = column;
            }

            if (score > alpha)
            {
                alpha = score;
            }
            if (beta <= alpha) break;
        }
        return (bestScore, bestColumn);
    }

    private (float, int) GetBestMinimizerScore(Board board, int depth, float alpha, float beta)
    {
        float bestScore = Mathf.Infinity;

        int bestColumn = 3;
        for (int column = 0; column < 7; column++)
        {
            Board boardCopy = new Board();
            boardCopy.SetBoard(board);
            float score;
            
            if (boardCopy.IsValidLocation(column))
            {
                boardCopy.AddPiece(column, Board.Owner.Player);
            }
            else
            {
                continue;
            }
            score = Minimax(boardCopy, depth - 1, Board.Owner.AI, alpha, beta).Item1;

            if (score < bestScore)
            {
                bestScore = score;
                bestColumn = column;              
            }

            if (score < beta)
            {
                beta = score;
            }
            if (beta <= alpha) break;
        }
        return (bestScore, bestColumn);
    }
}
