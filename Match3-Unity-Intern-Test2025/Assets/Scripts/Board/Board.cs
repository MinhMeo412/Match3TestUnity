using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using UnityEngine;

public class Board
{
    public enum eMatchDirection
    {
        NONE,
        HORIZONTAL,
        VERTICAL,
        ALL
    }

    private int boardSizeX;

    private int boardSizeY;

    private Cell[,] m_cells;
    public Cell[,] GetCells() { return m_cells; }

    private Transform m_root;

    private CellBottom[] m_cellsBottom ;

    public CellBottom[] GetBottomCells() => m_cellsBottom;
    public int GetBoardSize => (boardSizeX * boardSizeY);

    public Board(Transform transform, GameSettings gameSettings)
    {
        m_root = transform;

        this.boardSizeX = gameSettings.BoardSizeX;
        this.boardSizeY = gameSettings.BoardSizeY;

        m_cells = new Cell[boardSizeX, boardSizeY];
        m_cellsBottom = new CellBottom[5];

        CreateBoard();

        CreateBottomCells();
    }

    private void CreateBoard()
    {
        Vector3 origin = new Vector3(-boardSizeX * 0.5f + 0.5f, -boardSizeY * 0.5f + 0.5f, 0f);
        GameObject prefabBG = Resources.Load<GameObject>(Constants.PREFAB_CELL_BACKGROUND);
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                GameObject go = GameObject.Instantiate(prefabBG);
                go.transform.position = origin + new Vector3(x, y, 0f);
                go.transform.SetParent(m_root);

                Cell cell = go.GetComponent<Cell>();
                cell.Setup(x, y);

                m_cells[x, y] = cell;
            }
        }

        //set neighbours
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                if (y + 1 < boardSizeY) m_cells[x, y].NeighbourUp = m_cells[x, y + 1];
                if (x + 1 < boardSizeX) m_cells[x, y].NeighbourRight = m_cells[x + 1, y];
                if (y > 0) m_cells[x, y].NeighbourBottom = m_cells[x, y - 1];
                if (x > 0) m_cells[x, y].NeighbourLeft = m_cells[x - 1, y];
            }
        }

    }

    private void CreateBottomCells() 
    {
        int bottomCellCount = 5;

        Vector3 origin = new Vector3(-boardSizeX * 0.5f + 0.5f, -boardSizeY * 0.5f + 0.5f, 0f);

        float bottomY = origin.y - 1.5f;

        float startX = -bottomCellCount * 0.5f + 0.5f;

        GameObject prefabBG = Resources.Load<GameObject>(Constants.PREFAB_CELL_BACKGROUND_BOTTOM);

        for (int i = 0; i < bottomCellCount; i++)
        {
            GameObject go = GameObject.Instantiate(prefabBG);
            go.transform.position = new Vector3(startX + i, bottomY, 0f);
            go.transform.SetParent(m_root);

            CellBottom cellBottom = go.GetComponent<CellBottom>();
            cellBottom.Setup(i);

            m_cellsBottom[i] = cellBottom;
        }
    }


    internal void Fill(bool isTimerMode) 
    {
        int totalCells = boardSizeX * boardSizeY;

        if (totalCells % 3 != 0)
        {
            Debug.LogError($"Số lượng ô không phù hợp cho gameplay");
            return;
        }

        NormalItem.eNormalType[] allTypes = (NormalItem.eNormalType[])Enum.GetValues(typeof(NormalItem.eNormalType));

        List<NormalItem.eNormalType> pool = new List<NormalItem.eNormalType>();

        int groupCount = totalCells / 3;

        int minTypes = isTimerMode ? allTypes.Length : Mathf.Min(5, allTypes.Length);

        NormalItem.eNormalType[] selectedTypes = allTypes.OrderBy(x => UnityEngine.Random.value).Take(minTypes).ToArray();

        for (int i = 0; i < selectedTypes.Length; i++)
        {
            pool.Add(selectedTypes[i]);
            pool.Add(selectedTypes[i]);
            pool.Add(selectedTypes[i]);
            groupCount--;
        }

        for (int i = 0; i < groupCount; i++)
        {
            NormalItem.eNormalType type = allTypes[UnityEngine.Random.Range(0, allTypes.Length)];
            pool.Add(type);
            pool.Add(type);
            pool.Add(type);
        }

        for (int i = 0; i < pool.Count; i++)
        {
            int rand = UnityEngine.Random.Range(i, pool.Count);
            (pool[i], pool[rand]) = (pool[rand], pool[i]);
        }

        int index = 0;
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                Cell cell = m_cells[x, y];
                NormalItem item = new NormalItem();

                var type = pool[index++];
                item.SetType(type);
                item.SetView();
                item.SetViewRoot(m_root);

                cell.Assign(item);
                cell.ApplyItemPosition(false);
            }
        }
    }

    public void MoveCellToBottomCell(Cell cell, Action callback) 
    {
        if (cell == null || cell.IsEmpty || cell.Item == null)
        {
            callback?.Invoke();
            return;
        }

        int filledCount = 0;
        for (int i = 0; i < m_cellsBottom.Length; i++)
        {
            if (!m_cellsBottom[i].IsEmpty) filledCount++;
        }

        if (filledCount >= 5)
        {
            callback?.Invoke();
            return;
        }

        Item item = cell.Item;

        int insertIndex = -1;
        bool foundSameType = false;

        for (int i = 0; i < m_cellsBottom.Length; i++)
        {
            if (!m_cellsBottom[i].IsEmpty && m_cellsBottom[i].IsSameType(cell))
            {
                insertIndex = i + 1;
                foundSameType = true;
            }
            else if (m_cellsBottom[i].IsEmpty && !foundSameType)
            {
                insertIndex = i;
                break;
            }
        }

        List<Tweener> tweens = new List<Tweener>();

        for (int j = m_cellsBottom.Length - 1; j > insertIndex; j--)
        {
            if (!m_cellsBottom[j - 1].IsEmpty)
            {
                Item shiftItem = m_cellsBottom[j - 1].Item;
                m_cellsBottom[j].Assign(shiftItem, m_cellsBottom[j - 1].BoardX, m_cellsBottom[j - 1].BoardY);
                Tweener t = shiftItem.View.DOMove(m_cellsBottom[j].transform.position, 0.3f);
                tweens.Add(t);
            }
        }

        m_cellsBottom[insertIndex].Assign(item,cell.BoardX,cell.BoardY);
        Tweener tNew = item.View.DOMove(m_cellsBottom[insertIndex].transform.position, 0.3f);
        tweens.Add(tNew);

        cell.Free();

        DOTween.Sequence()
               .AppendInterval(0)
               .OnStart(() =>
               {
                   foreach (var tw in tweens)
                   {
                       DOTween.Sequence().Join(tw);
                   }
               })
               .OnComplete(() => { callback?.Invoke(); });
    }

    public void MoveBottomCellToCell(CellBottom cellBottom, Action callback)
    {
        if (cellBottom == null || cellBottom.IsEmpty || cellBottom.Item == null)
        {
            callback?.Invoke();
            return;
        }

        Item item = cellBottom.Item;
        int originX = cellBottom.BoardX;
        int originY = cellBottom.BoardY;

        Cell originCell = m_cells[originX, originY];
        originCell.Assign(item);
        cellBottom.Free();

        int removedIndex = cellBottom.BottomBoardX;

        List<Tweener> tweens = new List<Tweener>();

        for (int j = removedIndex + 1; j < m_cellsBottom.Length; j++)
        {
            if (!m_cellsBottom[j].IsEmpty)
            {
                Item shiftItem = m_cellsBottom[j].Item;

                m_cellsBottom[j - 1].Assign(shiftItem, m_cellsBottom[j].BoardX, m_cellsBottom[j].BoardY);

                Tweener t = shiftItem.View.DOMove(m_cellsBottom[j - 1].transform.position, 0.3f);
                tweens.Add(t);

                m_cellsBottom[j].Free();
            }
        }

        Tweener tNew = item.View.DOMove(originCell.transform.position, 0.3f);
        tweens.Add(tNew);

        DOTween.Sequence()
               .AppendInterval(0)
               .OnStart(() =>
               {
                   foreach (var tw in tweens)
                   {
                       DOTween.Sequence().Join(tw);
                   }
               })
               .OnComplete(() => { callback?.Invoke(); });
    }

    public void Clear()
    {
        for (int x = 0; x < boardSizeX; x++)
        {
            for (int y = 0; y < boardSizeY; y++)
            {
                Cell cell = m_cells[x, y];
                cell.Clear();

                GameObject.Destroy(cell.gameObject);
                m_cells[x, y] = null;
            }
        }
    }
}
