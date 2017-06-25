using System.Collections.Generic;
using Jerry;
using UnityEngine;

public class InfinitelyLinearGridLayoutGroup : MonoBehaviour
{
    [Header("Settings")]

    public Dir dir = Dir.Horizontal;
    public Vector2 cellSize = new Vector2(100f, 100f);
    public float spacing = 10f;
    public Transform prefab;

    [Header("Init")]

    public int oneScreenAmt = 1;
    public int bufferHalfAmt = 1;
    public int startIdx = 0;
    public int totalAmt;

    /// <summary>
    /// 视野内的元素开始的编号
    /// </summary>
    private int curFirstIdx;
    private RectTransform rectTran;
    private List<LayoutItem> itemList = new List<LayoutItem>();
    
    void Awake()
    {
        DoInit();
    }

    void Update()
    {
        CheckUpdate();
    }

    private void DoInit()
    {
        rectTran = this.transform as RectTransform;
        itemList.Clear();
        JerryUtil.DestroyAllChildren(this.transform);
        curFirstIdx = -1;

        if (startIdx >= totalAmt)
        {
            startIdx = totalAmt - 1;
        }
        if (startIdx < 0)
        {
            startIdx = 0;
        }
        ResetPos();
        ResetDelta();
    }

    private float calFirstIdxPos, calFirstIdxPosSize;
    private int calFirstIdxIdx;
    private void CalFirstIdx()
    {
        calFirstIdxPos = dir == Dir.Horizontal ? -this.transform.localPosition.x : this.transform.localPosition.y;
        calFirstIdxPosSize = dir == Dir.Horizontal ? cellSize.x : cellSize.y;
        calFirstIdxIdx = (int)(calFirstIdxPos / (calFirstIdxPosSize + spacing));//一个元素的位置:[i*(size+spacing),i*(size+spacing)+size]
        calFirstIdxIdx -= bufferHalfAmt;
        if (calFirstIdxIdx < 0)
        {
            calFirstIdxIdx = 0;
        }
    }

    private void CheckUpdate()
    {
        CalFirstIdx();
        if (curFirstIdx != calFirstIdxIdx)
        {
            curFirstIdx = calFirstIdxIdx;
            RefreshData();
        }
    }

    private int curLastIdx;
    private LayoutItem tmpLayoutItem;
    private void RefreshData()
    {
        curLastIdx = curFirstIdx + oneScreenAmt + bufferHalfAmt * 2;
        if (curLastIdx >= totalAmt)
        {
            curLastIdx = totalAmt - 1;
        }

        foreach (LayoutItem item in itemList)
        {
            if(item.GetGridIdx() >= curFirstIdx
                && item.GetGridIdx() <= curLastIdx)
            {
                item.SetGridState(true);
            }
            else
            {
                item.SetGridState(false);
            }
        }

        for (int i = curFirstIdx; i <= curLastIdx; i++)
        {
            if (itemList.Exists((x) => x.GetGridIdx() == i))
            {
                continue;
            }

            tmpLayoutItem = itemList.Find((x) => x.GetGridState() == false);
            if (tmpLayoutItem == null)
            {
                tmpLayoutItem = JerryUtil.CloneGo<Item>(new JerryUtil.CloneGoData()
                {
                    name = i.ToString(),
                    parant = this.transform,
                    prefab = prefab.gameObject,
                    active = true,
                });
                itemList.Add(tmpLayoutItem);
            }
            tmpLayoutItem.SetGridState(true);
            tmpLayoutItem.SetGridIdx(i, CalGridPos(i));
        }
    }

    private Vector3 CalGridPos(int idx)
    {
        Vector3 ret = Vector3.zero;
        switch (dir)
        {
            case Dir.Horizontal:
                {
                    ret.x += idx * (spacing + cellSize.x);
                }
                break;
            case Dir.Vertical:
                {
                    ret.x -= idx * (spacing + cellSize.x);
                }
                break;
        }
        return ret;
    }

    private void ResetPos()
    {
        Vector3 pos = Vector3.zero;
        switch (dir)
        {
            case Dir.Horizontal:
                {
                    pos.x -= startIdx * (spacing + cellSize.x);
                }
                break;
            case Dir.Vertical:
                {
                    pos.y += startIdx * (spacing + cellSize.x);
                }
                break;
        }
        this.transform.localPosition = pos;
    }

    private void ResetDelta()
    {
        Vector2 size = cellSize;
        switch (dir)
        {
            case Dir.Horizontal:
                {
                    size.x = totalAmt * cellSize.x;
                    if (totalAmt > 0)
                    {
                        size.x += (totalAmt - 1) * spacing;
                    }
                }
                break;
            case Dir.Vertical:
                {
                    size.y = totalAmt * cellSize.y;
                    if (totalAmt > 0)
                    {
                        size.y += (totalAmt - 1) * spacing;
                    }
                }
                break;
        }
        rectTran.sizeDelta = size;
    }

    public enum Dir
    {
        Horizontal = 0,
        Vertical,
    }
}