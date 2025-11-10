using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardController : MonoBehaviour
{
    public event Action OnScoreEvent = delegate { };

    public bool IsBusy { get; private set; }
    public void SetIsBusy() => IsBusy = true;

    private Board m_board;

    public Board GetBoard() => m_board;

    private GameManager m_gameManager;

    private Camera m_cam;

    private GameSettings m_gameSettings;

    private bool m_gameOver;

    public int GetBoardSize() => m_board.GetBoardSize;

    public void StartGame(GameManager gameManager, GameSettings gameSettings)
    {
        m_gameManager = gameManager;

        m_gameSettings = gameSettings;

        m_gameManager.StateChangedAction += OnGameStateChange;

        m_cam = Camera.main;

        m_board = new Board(this.transform, gameSettings);

        Fill(m_gameManager.LevelMode == GameManager.eLevelMode.TIMER);
    }

    private void Fill(bool isTimerMode)
    {
        m_board.Fill(isTimerMode);
    }

    private void OnGameStateChange(GameManager.eStateGame state)
    {
        switch (state)
        {
            case GameManager.eStateGame.GAME_STARTED:
                IsBusy = false;
                if(this.GetComponent<AutoPlay>().isPlayingAuto == true) { IsBusy = true; }
                break;
            case GameManager.eStateGame.PAUSE:
                IsBusy = true;
                break;
            case GameManager.eStateGame.GAME_OVER:
                m_gameOver = true;
                break;
        }
    }


    public void Update() 
    {
        if (m_gameOver) return;
        if (IsBusy) return;
        if (m_gameManager.State == GameManager.eStateGame.AUTO_PLAYING) return;

        if (Input.GetMouseButtonDown(0))
        {
            var hit = Physics2D.Raycast(m_cam.ScreenToWorldPoint(Input.mousePosition), Vector2.zero);
            if (!hit.collider) return;

            IsBusy = true;

            if (hit.collider.TryGetComponent<Cell>(out var cell))
            {
                m_board.MoveCellToBottomCell(cell, () =>
                {
                    CheckBottomCellMatch(m_board);
                });
            }
            else if (IsTimerMode() && hit.collider.TryGetComponent<CellBottom>(out var bottomCell))
            {
                if (!bottomCell.IsEmpty)
                {
                    m_board.MoveBottomCellToCell(bottomCell, () => { IsBusy = false; });
                }
            }
        }
    }

    private bool IsTimerMode()
    {
        return m_gameManager != null &&
               m_gameManager.GetComponent<LevelTime>() != null;
    }

    public void CheckBottomCellMatch(Board board) 
    {
        List<CellBottom> matches = new List<CellBottom>();
        CellBottom[] cellsBottom = board.GetBottomCells();

        if (cellsBottom == null || cellsBottom.Length < 3) return;

        int count = 1;
        for (int i = 1; i < cellsBottom.Length; i++)
        {
            if (cellsBottom[i].IsEmpty || cellsBottom[i - 1].IsEmpty)
            {
                count = 1;
                continue;
            }

            if (cellsBottom[i].IsSameType(cellsBottom[i - 1]))
            {
                count++;
                if (count == 3)
                {
                    matches.Add(cellsBottom[i - 2]);
                    matches.Add(cellsBottom[i - 1]);
                    matches.Add(cellsBottom[i]);
                }
            }
            else
            {
                count = 1;
            }
        }

        if (matches != null && matches.Count > 0)
        {
            for (int i = 0; i < matches.Count; i++)
            {
                matches[i].ExplodeItem();
            }

            ShiftBottomCells(m_board);

            OnScoreEvent();
            IsBusy = false;
        }
        else
        {
            CheckLoseCondition(board);
            IsBusy = false;
        }

        return;
    }

    private void CheckLoseCondition(Board board)
    {
        CellBottom[] cellsBottom = board.GetBottomCells();
        int filledCount = 0;

        foreach (var cell in cellsBottom)
        {
            if (!cell.IsEmpty)
                filledCount++;
        }

        if (filledCount >= 5 && !IsTimerMode())
        {
            m_gameManager.GameOver();
        }
    }

    private void ShiftBottomCells(Board board) 
    {
        CellBottom[] cellsBottom = board.GetBottomCells();
        int targetIndex = 0;

        for (int i = 0; i < cellsBottom.Length; i++)
        {
            if (!cellsBottom[i].IsEmpty)
            {
                if (i != targetIndex)
                {
                    Item item = cellsBottom[i].Item;
                    cellsBottom[targetIndex].Assign(item, cellsBottom[i].BoardX, cellsBottom[i].BoardY);
                    cellsBottom[i].Free();

                    item.View.DOMove(cellsBottom[targetIndex].transform.position, 0.3f);
                }
                targetIndex++;
            }
        }
    }


    internal void Clear()
    {
        m_board.Clear();
    }

    private void OnDestroy()
    {
        if (m_gameManager != null)
        {
            m_gameManager.StateChangedAction -= OnGameStateChange;
        }
    }
}
