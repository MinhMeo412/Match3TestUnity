using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SocialPlatforms;

public class AutoPlay : MonoBehaviour
{
    private GameManager m_gameManager;
    private BoardController m_boardController;
    private Board m_board;

    public bool isPlayingAuto = false;
    private List<Cell> autoPlayCells;

    private void Awake()
    {
        m_boardController = GetComponent<BoardController>();
        m_board = m_boardController.GetBoard();
    }

    public void SetGameManager(GameManager gameManager)
        { m_gameManager = gameManager; }

    public void StartAutoWin() => StartCoroutine(AutoWin());
    public void StartAutoLose() => StartCoroutine(AutoLose());




    private IEnumerator AutoWin()
    {
        isPlayingAuto = true;
        m_gameManager.SetState(GameManager.eStateGame.AUTO_PLAYING);

        if (CanStillWin())
        {
            for (int i = 0; i < autoPlayCells.Count; i++)
            {
                bool isDone = false;

                m_board.MoveCellToBottomCell(autoPlayCells[i], () => 
                { 
                    m_boardController.CheckBottomCellMatch(m_board);
                    isDone = true;
                });
                while (!isDone)
                    yield return null;

                float timer = 0f;
                while (timer < 0.2f)
                {
                    if (m_gameManager.State != GameManager.eStateGame.PAUSE)
                        timer += Time.deltaTime;

                    yield return null;
                }

                yield return new WaitForSeconds(0.5f);
            }    
        }
    }

    private bool CanStillWin()
    {
        CellBottom[] bottoms = m_board.GetBottomCells();
        int emptySlots = 0;
        for (int i = 0; i < bottoms.Length; i++)
        {
            if(bottoms[i].IsEmpty == true)
            {
                emptySlots++;
                if (emptySlots >= 3)
                {
                    autoPlayCells = GetSortedWinCells();
                    return true;
                }
            }
        }    

        for (int i = 0;i < bottoms.Length;i++)
        {
            for (int j = i + 1;j < bottoms.Length; j++)
            {
                if (bottoms[i].IsSameType(bottoms[j]))
                {
                    autoPlayCells = GetSortedWinCells(true, bottoms[i]);
                    return true;
                } 
                    
            }    
        }
        return false;
    }

    private List<Cell> GetSortedWinCells(bool hasBottomItemDuplicate = false, CellBottom cellBottom = null)
    {
        Cell[,] cells = m_board.GetCells();

        List<Cell> allCellHasItem = new List<Cell>();
        for (int x = 0; x < cells.GetLength(0); x++)
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                if (!cells[x, y].IsEmpty)
                    allCellHasItem.Add(cells[x, y]);
            }

        List<List<Cell>> groups = new List<List<Cell>>();

        foreach (var cell in allCellHasItem)
        {
            bool foundGroup = false;

            foreach (var group in groups)
            {
                if (group[0].IsSameType(cell))
                {
                    group.Add(cell);
                    foundGroup = true;
                    break;
                }
            }

            if (!foundGroup)
            {
                groups.Add(new List<Cell>() { cell });
            }
        }

        if (hasBottomItemDuplicate && cellBottom != null)
        {
            int index = groups.FindIndex(g => g[0].IsSameType(cellBottom));
            if (index > 0)
            {
                var matchGroup = groups[index];
                groups.RemoveAt(index);
                groups.Insert(0, matchGroup);
            }
        }

        List<Cell> sorted = new List<Cell>();
        foreach (var group in groups)
        {
            sorted.AddRange(group);
        }

        return sorted;
    }

    private IEnumerator AutoLose()
    {
        isPlayingAuto = true;
        m_gameManager.SetState(GameManager.eStateGame.AUTO_PLAYING);

        if (CanStillLose())
        {
            for (int i = 0; i < autoPlayCells.Count; i++)
            {
                if (m_gameManager.State == GameManager.eStateGame.GAME_OVER)
                {
                    break;
                }

                bool isDone = false;

                m_board.MoveCellToBottomCell(autoPlayCells[i], () =>
                {
                    m_boardController.CheckBottomCellMatch(m_board);
                    isDone = true;
                });
                while (!isDone)
                    yield return null;

                float timer = 0f;
                while (timer < 0.2f)
                {
                    if (m_gameManager.State != GameManager.eStateGame.PAUSE)
                        timer += Time.deltaTime;

                    yield return null;
                }

                yield return new WaitForSeconds(0.5f);
            }
        }
    }

    private bool CanStillLose()
    {
        CellBottom[] bottoms = m_board.GetBottomCells();

        Cell[,] cells = m_board.GetCells();

        List<Cell> allCellHasItem = new List<Cell>();
        for (int x = 0; x < cells.GetLength(0); x++)
        {
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                if (!cells[x, y].IsEmpty)
                    allCellHasItem.Add(cells[x, y]);
            }
        }

        int totalCount = bottoms.Count(b => !b.IsEmpty) + allCellHasItem.Count;
        if (totalCount < 5)
            return false;

        List<Cell> typeGroups = new List<Cell>();
        foreach (var cell in allCellHasItem)
        {
            bool foundGroup = false;
            foreach (var group in typeGroups)
            {
                if (group.IsSameType(cell))
                {
                    foundGroup = true;
                    break;
                }
            }

            if (!foundGroup)
                typeGroups.Add(cell);
        }

        if (typeGroups.Count < 3)
            return false;

        autoPlayCells = GetLoseCells();
        return true;
    }

    private List<Cell> GetLoseCells()
    {
        Cell[,] cells = m_board.GetCells();

        List<Cell> allCellHasItem = new List<Cell>();
        for (int x = 0; x < cells.GetLength(0); x++)
        {
            for (int y = 0; y < cells.GetLength(1); y++)
            {
                if (!cells[x, y].IsEmpty)
                    allCellHasItem.Add(cells[x, y]);
            }
        }

        List<List<Cell>> groups = new List<List<Cell>>();
        foreach (var cell in allCellHasItem)
        {
            bool foundGroup = false;
            foreach (var group in groups)
            {
                if (group[0].IsSameType(cell))
                {
                    group.Add(cell);
                    foundGroup = true;
                    break;
                }
            }

            if (!foundGroup)
            {
                groups.Add(new List<Cell>() { cell });
            }
        }


        List<Cell> loseCells = new List<Cell>();
        for (int repeat = 0; repeat < 2; repeat++)
        {
            foreach (var group in groups)
            {
                if (group.Count > 0)
                {
                    loseCells.Add(group[0]);
                }
            }
        }

        return loseCells;
    }

}
