using System;
using System.Collections.Generic;
using Jerry;
using UnityEngine;

public class InfinitelyLinearGridLayoutGroup<T, F> : MonoBehaviour
    where T : LayoutItem
    where F : ILayoutItemData
{
    /// <summary>
    /// 视野内的元素开始的编号
    /// </summary>
    private int curFirstIdx;
    private RectTransform rectTran;
    /// <summary>
    /// 创建的对象
    /// </summary>
    private List<T> itemList = new List<T>();

    private bool awaked = false;
    private bool inited = false;
    private bool ready = false;

    private ConfigData config = new ConfigData();

    /// <summary>
    /// 数据
    /// </summary>
    private List<F> datas = new List<F>();
    private int TotalAmt
    {
        get
        {
            return (datas == null) ? 0 : datas.Count;
        }
    }

    public virtual void Awake()
    {
        awaked = true;
        TryWork();
    }

    public virtual void Update()
    {
        CheckUpdate();
    }

    public void DoInit(ConfigData tconfig, List<F> tdatas)
    {
        config = tconfig;
        datas = tdatas;
        inited = true;
        TryWork();
    }

    private void TryWork()
    {
        if (!awaked
            || !inited)
        {
            return;
        }

        rectTran = this.transform as RectTransform;
        switch (config.dir)
        {
            case Dir.Horizontal:
                {
                    rectTran.pivot = new Vector2(0, 0.5f);
                    (rectTran.parent as RectTransform).pivot = new Vector2(0, 0.5f);
                    rectTran.anchorMin = new Vector2(0, 0.5f);
                    rectTran.anchorMax = new Vector2(0, 0.5f);
                }
                break;
            case Dir.Vertical:
                {
                    rectTran.pivot = new Vector2(0.5f, 1.0f);
                    (rectTran.parent as RectTransform).pivot = new Vector2(0.5f, 1.0f);
                    rectTran.anchorMin = new Vector2(0.5f, 1.0f);
                    rectTran.anchorMax = new Vector2(0.5f, 1.0f);
                }
                break;
        }

        itemList.Clear();
        JerryUtil.DestroyAllChildren(this.transform);

        curFirstIdx = -1;

        if (config.startIdx >= TotalAmt)
        {
            config.startIdx = TotalAmt - 1;
        }
        if (config.startIdx < 0)
        {
            config.startIdx = 0;
        }
        ResetPos();
        ResetDelta();

        ready = true;
    }

    private float calFirstIdxPos, calFirstIdxPosSize;
    private int calFirstIdxIdx;
    private void CalFirstIdx()
    {
        calFirstIdxPos = config.dir == Dir.Horizontal ? -this.transform.localPosition.x : this.transform.localPosition.y;
        calFirstIdxPosSize = config.dir == Dir.Horizontal ? config.cellSize.x : config.cellSize.y;
        calFirstIdxIdx = (int)(calFirstIdxPos / (calFirstIdxPosSize + config.spacing));//一个元素的位置:[i*(size+spacing),i*(size+spacing)+size]
        calFirstIdxIdx -= config.bufferHalfAmt;
        if (calFirstIdxIdx < 0)
        {
            calFirstIdxIdx = 0;
        }
    }

    private void CheckUpdate()
    {
        if (!awaked
            || !inited
            || !ready)
        {
            return;
        }

        CalFirstIdx();
        if (curFirstIdx != calFirstIdxIdx)
        {
            curFirstIdx = calFirstIdxIdx;
            RefreshData();
        }
    }

    private int curLastIdx;
    private T tmpLayoutItem;
    private void RefreshData()
    {
        curLastIdx = curFirstIdx + config.oneScreenAmt + config.bufferHalfAmt * 2;
        if (curLastIdx >= TotalAmt)
        {
            curLastIdx = TotalAmt - 1;
        }

        foreach (T item in itemList)
        {
            if (item.GetGridIdx() >= curFirstIdx
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
                tmpLayoutItem = JerryUtil.CloneGo<T>(new JerryUtil.CloneGoData()
                {
                    name = i.ToString(),
                    parant = this.transform,
                    prefab = config.prefab.gameObject,
                    active = true,
                });
                switch (config.dir)
                {
                    case Dir.Horizontal:
                        {
                            (tmpLayoutItem.transform as RectTransform).pivot = new Vector2(0, 0.5f);
                            (tmpLayoutItem.transform as RectTransform).anchorMin = new Vector2(0.5f, 0.5f);
                            (tmpLayoutItem.transform as RectTransform).anchorMax = new Vector2(0.5f, 0.5f);
                        }
                        break;
                    case Dir.Vertical:
                        {
                            (tmpLayoutItem.transform as RectTransform).pivot = new Vector2(0.5f, 1.0f);
                            (tmpLayoutItem.transform as RectTransform).anchorMin = new Vector2(0.5f, 0.5f);
                            (tmpLayoutItem.transform as RectTransform).anchorMax = new Vector2(0.5f, 0.5f);
                        }
                        break;
                }
                itemList.Add(tmpLayoutItem);
            }
            tmpLayoutItem.SetGridState(true);
            tmpLayoutItem.SetGridIdx(i, CalGridPos(i), datas[i]);
        }
    }

    private Vector3 CalGridPos(int idx)
    {
        Vector3 ret = Vector3.zero;
        switch (config.dir)
        {
            case Dir.Horizontal:
                {
                    ret.x += idx * (config.spacing + config.cellSize.x);
                }
                break;
            case Dir.Vertical:
                {
                    ret.y -= idx * (config.spacing + config.cellSize.y);
                }
                break;
        }
        return ret;
    }

    private void ResetPos()
    {
        Vector3 pos = Vector3.zero;
        switch (config.dir)
        {
            case Dir.Horizontal:
                {
                    pos.x -= config.startIdx * (config.spacing + config.cellSize.x);
                }
                break;
            case Dir.Vertical:
                {
                    pos.y += config.startIdx * (config.spacing + config.cellSize.x);
                }
                break;
        }
        this.transform.localPosition = pos;
    }

    private void ResetDelta()
    {
        Vector2 size = config.cellSize;
        switch (config.dir)
        {
            case Dir.Horizontal:
                {
                    size.x = TotalAmt * config.cellSize.x;
                    if (TotalAmt > 0)
                    {
                        size.x += (TotalAmt - 1) * config.spacing;
                    }
                }
                break;
            case Dir.Vertical:
                {
                    size.y = TotalAmt * config.cellSize.y;
                    if (TotalAmt > 0)
                    {
                        size.y += (TotalAmt - 1) * config.spacing;
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

    public class ConfigData
    {
        public Dir dir = Dir.Horizontal;
        public Vector2 cellSize = new Vector2(100f, 100f);
        public float spacing = 10f;
        public Transform prefab;

        /// <summary>
        /// 一屏数量，取上整
        /// </summary>
        public int oneScreenAmt = 1;

        /// <summary>
        /// 额外缓存数量
        /// </summary>
        public int bufferHalfAmt = 0;

        /// <summary>
        /// 开始下标
        /// </summary>
        public int startIdx = 0;
    }
}