using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board
{
    public event EventHandler<OnAddPieceEventArgs> OnAddPiece;

    public enum Owner
    {
        None,
        Player,
        AI
    }

    public class OnAddPieceEventArgs : EventArgs
    {
        public Owner owner;
        public int row;
        public int column;
    }

    // key: Coords, value: Owner
    private Dictionary<(int, int), Owner> board = new();
    private int boardWidth = 7;
    private int boardHeight = 6;

    public Board()
    {
        for (int column = 0; column < boardWidth; column++)
        {
            for (int row = 0; row < boardHeight; row++)
            {
                board[(column, row)] = Owner.None;
            }
        }
    }

    public void SetBoard(Board newBoard)
    {
        foreach (KeyValuePair<(int, int), Owner> data in newBoard.GetBoard())
        {
            board[data.Key] = data.Value;
        }
    }

    public void AddPiece(int column, Owner owner)
    {
        for (int row = 0; row < boardHeight; row++)
        {
            if (!board.ContainsKey((column, row))) return;
            if (board[(column, row)] == Owner.None)
            {
                board[(column, row)] = owner;
                OnAddPiece?.Invoke(this, new OnAddPieceEventArgs {
                    owner = owner,
                    row = row,
                    column = column
                });
                return;
            }
        }
    }

    public void GetTakenSpots()
    {
        for (int debugColumn = 0; debugColumn < boardWidth; debugColumn++)
        {       
            for (int debugRow = 0; debugRow < boardHeight; debugRow++)
            {
                if (board[(debugColumn, debugRow)] != Owner.None)
                {
                    Debug.Log($"{(debugColumn, debugRow)}: {board[(debugColumn, debugRow)]}");
                }
            }
        }
    }

    public bool IsWinner(Owner owner)
    {
        if (HorizontalWin(owner))
        {
            Debug.Log("Won Horizontally");
            return true;
        }
        else if (VerticalWin(owner))
        {
            Debug.Log("Won Vertically");
            return true;  
        }
        else if (DiagonalNegativeWin(owner))
        {
            Debug.Log("Won Negative Diagonally");
            return true;
        }
        else if (DiagonalPositiveWin(owner))
        {
            Debug.Log("Won Positive Diagonally");
            return true;
        }
        return false;
    }

    public int CalculateScore()
    {
        int score = 0;
        score += ScoreColumns();
        score += ScoreHorizontal();
        score += ScoreVertical();
        score += ScoreDiagonals();

        return score;
    }

    private int ScoreDiagonals()
    {
        int totalScore = 0;
        for (int row = 0; row < boardHeight; row++)
        {
            for (int column = 0; column < boardWidth; column++)
            {
                List<(int, int)> coords = new();
                AddNegativeDiagonalCoords(coords, column, row);
                AddPositiveDiagonalCoords(coords, column, row);

                totalScore += ScoreCoords(coords);
            }
        }
        return totalScore;
    }

    private void AddNegativeDiagonalCoords(List<(int, int)> coords, int column, int row)
    {
        bool isMissingSlot = false;
        for (int i = 0; i < 4; i++)
        {
            if (!board.ContainsKey((column + i, row - i))) 
            {
                isMissingSlot = true;
                break;
            }
            coords.Add((column + i, row - i));
        }
    }

    private bool DiagonalNegativeWin(Owner owner)
    {
        for (int row = 0; row < boardHeight; row++)
        {
            for (int column = 0; column < boardWidth; column++)
            {
                bool isMissingSlot = false;
                List<(int, int)> coords = new();
                for (int i = 0; i < 4; i++)
                {
                    if (!board.ContainsKey((column + i, row - i))) 
                    {
                        isMissingSlot = true;
                        break;
                    }
                    coords.Add((column + i, row - i));
                }
                if (GetPieceCount(coords, owner) == 4)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private void AddPositiveDiagonalCoords(List<(int, int)> coords, int column, int row)
    {
        bool isMissingSlot = false;
        for (int i = 0; i < 4; i++)
        {
            if (!board.ContainsKey((column + i, row + i))) 
            {
                isMissingSlot = true;
                break;
            }
            coords.Add((column + i, row + i));
        }
    }

    private bool DiagonalPositiveWin(Owner owner)
    {
        for (int row = 0; row < boardHeight; row++)
        {
            for (int column = 0; column < boardWidth; column++)
            {
                bool isMissingSlot = false;
                List<(int, int)> coords = new();
                for (int i = 0; i < 4; i++)
                {
                    if (!board.ContainsKey((column + i, row + i))) 
                    {
                        isMissingSlot = true;
                        break;
                    }
                    coords.Add((column + i, row + i));
                }
                if (GetPieceCount(coords, owner) == 4)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private int ScoreHorizontal()
    {
        int totalScore = 0;
        for (int row = 0; row < boardHeight; row++)
        {
            for (int column = 0; column < boardWidth - 3; column++)
            {
                List<(int, int)> coords = new();
                for (int i = 0; i < 4; i++)
                {
                    coords.Add((column + i, row));
                }
                totalScore += ScoreCoords(coords);
            }
        }
        return totalScore;
    }

    private bool HorizontalWin(Owner owner)
    {
        for (int row = 0; row < boardHeight; row++)
        {
            for (int column = 0; column < boardWidth - 3; column++)
            {
                List<(int, int)> coords = new();
                for (int i = 0; i < 4; i++)
                {
                    coords.Add((column + i, row));
                }
                if (GetPieceCount(coords, owner) == 4)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private int ScoreVertical()
    {
        int totalScore = 0;
        for (int column = 0; column < boardWidth; column++)
        {
            for (int row = 0; row < boardHeight - 3; row++)
            {
                List<(int, int)> coords = new();
                for (int i = 0; i < 4; i++)
                {
                    coords.Add((column, row + i));
                }
                totalScore += ScoreCoords(coords);
            }
        }
        return totalScore;
    }

    private bool VerticalWin(Owner owner)
    {
        for (int column = 0; column < boardWidth; column++)
        {
            for (int row = 0; row < boardHeight - 3; row++)
            {
                List<(int, int)> coords = new();
                for (int i = 0; i < 4; i++)
                {
                    coords.Add((column, row + i));
                }
                if (GetPieceCount(coords, owner) == 4)
                {
                    return true;
                }
            }
        }
        return false;
    }

    private int ScoreCoords(List<(int, int)> coords)
    {
        int score = 0;
        int aIPieceCount = GetPieceCount(coords, Owner.AI);
        int emptyPieceCount = GetPieceCount(coords, Owner.None);
        int playerPieceCount = GetPieceCount(coords, Owner.Player);

        if (aIPieceCount == 2 && emptyPieceCount == 2)
        {
            score += 2;
        }
        else if (aIPieceCount == 3 && emptyPieceCount == 1)
        {
            score += 10;
        }
        else if (playerPieceCount == 2 && emptyPieceCount == 2)
        {
            score -= 2;
        }
        else if (playerPieceCount == 3 && emptyPieceCount == 1)
        {
            score -= 100;
        }
        return score;
    }

    private int GetPieceCount(List<(int, int)> coords, Owner owner)
    {
        int pieceCount = 0;
        foreach ((int, int) coord in coords)
        {
            if (IsOwner(coord, owner))
            {
                pieceCount += 1;
            }
        }
        return pieceCount;
    }

    public bool IsValidLocation(int column)
    {
        if (board[(column, boardHeight - 1)] == Owner.None)
        {
            return true;
        } 
        return false;
    }

    private int ScoreColumns()
    {
        int totalScore = 0;
        for (int row = 0; row < boardHeight; row++)
        {
            for (int column = 0; column < boardWidth; column++)
            {
                if (IsOwner((column, row), Owner.AI))
                {
                    if (column == 1 || column == 5)
                    {
                        totalScore += 1;
                    }
                    else if (column == 2 || column == 4)
                    {
                        totalScore += 2;
                    }
                    else if (column == 3)
                    {
                        totalScore += 5;
                    }
                }
            }
        }
        return totalScore;
    }

    private bool IsOwner((int, int) coord, Owner owner)
    {
        if (board[coord] == owner)
        {
            return true;
        }
        return false;
    }

    public Dictionary<(int, int), Owner> GetBoard()
    {
        return board;
    }
}
