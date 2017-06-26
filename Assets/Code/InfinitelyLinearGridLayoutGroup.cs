using System.Collections.Generic;
using Jerry;
using UnityEngine;

public class InfinitelyLinearGridLayoutGroup : MonoBehaviour
{
    [Header("Settings")]

    /// <summary>
    /// 视野内的元素开始的编号
    /// </summary>
    private int curFirstIdx;
    private RectTransform rectTran;
    /// <summary>
    /// 创建的对象
    /// </summary>
    private List<ILayoutItem> itemList = new List<ILayoutItem>();

    private bool awaked = false;
    private bool inited = false;
    private bool ready = false;
    private InitData<ILayoutItemData, LayoutItem> data;

    void Awake()
    {
        awaked = true;
        TryWork();
    }

    void Update()
    {
        CheckUpdate();
    }

    public void DoInit<T, F>(InitData<T, F> tdata)
        where T : ILayoutItemData
        where F : LayoutItem
    {
        data = tdata as InitData<ILayoutItemData, LayoutItem>;
        Debug.LogWarning((data == null) + " ");
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

        itemList.Clear();
        JerryUtil.DestroyAllChildren(this.transform);

        curFirstIdx = -1;

        if (data.startIdx >= data.TotalAmt)
        {
            data.startIdx = data.TotalAmt - 1;
        }
        if (data.startIdx < 0)
        {
            data.startIdx = 0;
        }

        ResetPos();
        ResetDelta();

        ready = true;
    }

    private float calFirstIdxPos, calFirstIdxPosSize;
    private int calFirstIdxIdx;
    private void CalFirstIdx()
    {
        calFirstIdxPos = data.dir == Dir.Horizontal ? -this.transform.localPosition.x : this.transform.localPosition.y;
        calFirstIdxPosSize = data.dir == Dir.Horizontal ? data.cellSize.x : data.cellSize.y;
        calFirstIdxIdx = (int)(calFirstIdxPos / (calFirstIdxPosSize + data.spacing));//一个元素的位置:[i*(size+spacing),i*(size+spacing)+size]
        calFirstIdxIdx -= data.bufferHalfAmt;
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
    private ILayoutItem tmpLayoutItem;
    private void RefreshData()
    {
        curLastIdx = curFirstIdx + data.oneScreenAmt + data.bufferHalfAmt * 2;
        if (curLastIdx >= data.TotalAmt)
        {
            curLastIdx = data.TotalAmt - 1;
        }

        foreach (ILayoutItem item in itemList)
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
                tmpLayoutItem = data.CreateOneItem(i, this.transform);
                itemList.Add(tmpLayoutItem);
            }
            tmpLayoutItem.SetGridState(true);
            tmpLayoutItem.SetGridIdx(i, CalGridPos(i), data.datas[i]);
        }
    }

    private Vector3 CalGridPos(int idx)
    {
        Vector3 ret = Vector3.zero;
        switch (data.dir)
        {
            case Dir.Horizontal:
                {
                    ret.x += idx * (data.spacing + data.cellSize.x);
                }
                break;
            case Dir.Vertical:
                {
                    ret.x -= idx * (data.spacing + data.cellSize.x);
                }
                break;
        }
        return ret;
    }

    private void ResetPos()
    {
        Vector3 pos = Vector3.zero;
        switch (data.dir)
        {
            case Dir.Horizontal:
                {
                    pos.x -= data.startIdx * (data.spacing + data.cellSize.x);
                }
                break;
            case Dir.Vertical:
                {
                    pos.y += data.startIdx * (data.spacing + data.cellSize.x);
                }
                break;
        }
        this.transform.localPosition = pos;
    }

    private void ResetDelta()
    {
        Vector2 size = data.cellSize;
        switch (data.dir)
        {
            case Dir.Horizontal:
                {
                    size.x = data.TotalAmt * data.cellSize.x;
                    if (data.TotalAmt > 0)
                    {
                        size.x += (data.TotalAmt - 1) * data.spacing;
                    }
                }
                break;
            case Dir.Vertical:
                {
                    size.y = data.TotalAmt * data.cellSize.y;
                    if (data.TotalAmt > 0)
                    {
                        size.y += (data.TotalAmt - 1) * data.spacing;
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

    [SerializeField]
    public class InitData<T, F>
        where T : ILayoutItemData
        where F : LayoutItem
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

        public List<T> datas = new List<T>();

        public ILayoutItem CreateOneItem(int idx, Transform parent)
        {
            return JerryUtil.CloneGo<F>(new JerryUtil.CloneGoData()
            {
                name = idx.ToString(),
                parant = parent,
                prefab = prefab.gameObject,
                active = true,
            });
        }

        public int TotalAmt
        {
            get
            {
                return (datas == null) ? 0 : datas.Count;
            }
        }
    }
}