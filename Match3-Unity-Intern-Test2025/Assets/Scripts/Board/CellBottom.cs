using System;
using UnityEngine;

public class CellBottom : MonoBehaviour
{
    public int BoardX { get; private set; }

    public int BoardY { get; private set; }

    public int BottomBoardX { get; private set; }

    public Item Item { get; private set; }

    public bool IsEmpty => Item == null;

    public void Setup(int bottomBoardX)
    {
        this.BottomBoardX = bottomBoardX;
    }

    public void Free()
    {
        Item = null;
        BoardX = -1;
        BoardY = -1;
    }

    public void Assign(Item item, int x, int y)
    {
        if (Item != null) Free();
        Item = item;
        Item.SetCellBottom(this);
        BoardX = x;
        BoardY = y;
    }

    public void ApplyItemPosition(bool withAppearAnimation)
    {
        Item.SetViewPosition(this.transform.position);

        if (withAppearAnimation)
        {
            Item.ShowAppearAnimation();
        }
    }

    internal void Clear()
    {
        if (Item != null)
        {
            Item.Clear();
            Item = null;
            BoardX = -1;
            BoardY = -1;
        }
    }

    internal bool IsSameType(Cell other)
    {
        return Item != null && other.Item != null && Item.IsSameType(other.Item);
    }

    internal bool IsSameType(CellBottom other)
    {
        return Item != null && other.Item != null && Item.IsSameType(other.Item);
    }

    internal void ExplodeItem()
    {
        if (Item == null) return;

        Item.ExplodeView();
        Item = null;
        BoardX = -1;
        BoardY = -1;
    }

    internal void AnimateItemForHint()
    {
        Item.AnimateForHint();
    }

    internal void StopHintAnimation()
    {
        Item.StopAnimateForHint();
    }

    internal void ApplyItemMoveToPosition()
    {
        Item.AnimationMoveToPosition();
    }
}
