using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoardTester : MonoBehaviour
{
    void Start()
    {
        CheckColumnScoring();
        CheckOpponentVerticalScoring();
    }

    private void Assert(int number, int actualNumber)
    {
        if (number != actualNumber)
        {
            Debug.Log("ERROR IN TESTING: Score was " + number + " when it was supposed to be " + actualNumber);
        }
    }

    private void CheckColumnScoring()
    {
        Board board = new Board();
        board.AddPiece(3, Board.Owner.AI);
        Debug.Log("Score 1: " + board.CalculateScore());
        Assert(board.CalculateScore(), 5);
    } 

    private void CheckOpponentVerticalScoring()
    {
        Board board = new Board();
        board.AddPiece(3, Board.Owner.Player);
        board.AddPiece(3, Board.Owner.Player);
        Assert(board.CalculateScore(), -2);

        board.AddPiece(3, Board.Owner.Player);
        Assert(board.CalculateScore(), -102);
    }

    private void CheckOpponentAndAIDiagonalScoring()
    {
        Board board = new Board();
        board.AddPiece(3, Board.Owner.AI);
        Assert(board.CalculateScore(), 5);
        board.AddPiece(3, Board.Owner.Player);
        Assert(board.CalculateScore(), 5);

        board.AddPiece(2, Board.Owner.AI);
        Assert(board.CalculateScore(), 15);

        board.AddPiece(4, Board.Owner.AI);
        Assert(board.CalculateScore(), 15);
    }

    /*private void CheckCopying()
    {
        Board board = new Board();
        board.AddPiece(3, Board.Owner.Player);
        Board boardCopy = new Board();
        boardCopy.SetBoard(board);
        Debug.Log("-----");
        boardCopy.DebugTakenSpots();
        boardCopy.AddPiece(6, Board.Owner.AI);
        Debug.Log("-----");
        board.DebugTakenSpots();
        Debug.Log("-----");
        board.AddPiece(4, Board.Owner.Player);
        boardCopy.DebugTakenSpots();
        Debug.Log("-----");
        board.DebugTakenSpots();
    }*/
}
