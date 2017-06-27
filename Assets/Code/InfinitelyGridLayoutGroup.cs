using System.Collections.Generic;
using Jerry;
using UnityEngine;

public class InfinitelyGridLayoutGroup<T, F> : MonoBehaviour
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
    public bool Inited
    {
        get
        {
            return inited;
        }
    }
    private bool ready = false;

    private ConfigData config = new ConfigData();

    /// <summary>
    /// 单位宽度可视数量，取上整
    /// </summary>
    private int fiexdDirViewCount = 1;

    /// <summary>
    /// 数据
    /// </summary>
    private List<F> datas = new List<F>();
    private int TotalCount
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

    #region 对外接口

    public void DoInit(ConfigData tconfig, List<F> tdatas)
    {
        config = tconfig;
        datas = tdatas;
        inited = true;
        TryWork();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tdatas">数据数量有变化，全部刷新</param>
    /// <param name="idxs">数据内容有变化(内容修改/排序修改)，可单点刷新</param>
    public void RefreshDatas(List<F> tdatas = null, List<int> idxs = null)
    {
        if (!awaked
            || !inited
            || !ready)
        {
            return;
        }
        if (tdatas != null)
        {
            datas = tdatas;
            ResetDelta();
            CalFirstIdx();
            RefreshData(true);
        }
        else
        {
            foreach (T t in itemList)
            {
                if (idxs == null || idxs.Contains(t.GetGridIdx()))
                {
                    t.TryRefreshUI(datas[t.GetGridIdx()]);
                }
            }
        }
    }

    #endregion 对外接口

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
                    (rectTran.parent as RectTransform).pivot = new Vector2(0, 0.5f);
                    rectTran.anchorMin = new Vector2(0, 0.5f);
                    rectTran.anchorMax = new Vector2(0, 0.5f);
                }
                break;
            case Dir.Vertical:
                {
                    (rectTran.parent as RectTransform).pivot = new Vector2(0.5f, 1.0f);
                    rectTran.anchorMin = new Vector2(0.5f, 1.0f);
                    rectTran.anchorMax = new Vector2(0.5f, 1.0f);
                }
                break;
        }
        rectTran.pivot = new Vector2(0, 1);
        fiexdDirViewCount = Mathf.CeilToInt(config.fiexdDirViewCountF);
        config.startProgress = Mathf.Clamp01(config.startProgress);

        itemList.Clear();
        JerryUtil.DestroyAllChildren(this.transform);

        curFirstIdx = -1;
        ResetPos();
        ResetDelta();

        ready = true;
    }

    private float calFirstIdxPos, calFirstIdxSize, calFirstIdxSpacing;
    private int calFirstIdxIdx;
    private void CalFirstIdx()
    {
        calFirstIdxPos = config.dir == Dir.Horizontal ? -this.transform.localPosition.x : this.transform.localPosition.y;
        calFirstIdxSize = config.dir == Dir.Horizontal ? config.cellSize.x : config.cellSize.y;
        calFirstIdxSpacing = config.dir == Dir.Horizontal ? config.spacing.x : config.spacing.y;
        calFirstIdxIdx = (int)(calFirstIdxPos / (calFirstIdxSize + calFirstIdxSpacing)) * config.fixedRowOrColumnCount;
        calFirstIdxIdx -= config.viewCountHalfBuffer * config.fixedRowOrColumnCount;
        if (calFirstIdxIdx < 0)
        {
            calFirstIdxIdx = 0;
        }
    }

    private bool updateDirty = false;
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
            updateDirty = true;
        }

        if (updateDirty)
        {
            RefreshData();
        }
    }

    private int curLastIdx;
    private T tmpLayoutItem;
    private int updateWork;

    private void RefreshData(bool forceUpdate = false)
    {
        updateWork = 0;
        updateDirty = false;

        curLastIdx = curFirstIdx + (fiexdDirViewCount + config.viewCountHalfBuffer * 2) * config.fixedRowOrColumnCount - 1;
        if (curLastIdx >= TotalCount)
        {
            curLastIdx = TotalCount - 1;
        }

        foreach (T item in itemList)
        {
            if (!forceUpdate
                && item.GetGridIdx() >= curFirstIdx
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
            if (!forceUpdate
                && itemList.Exists((x) => x.GetGridIdx() == i))
            {
                continue;
            }

            //还有新的任务
            if (!forceUpdate
                && config.workCountPerFrame > 0
                && updateWork > config.workCountPerFrame)
            {
                updateDirty = true;
                break;
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
                            (tmpLayoutItem.transform as RectTransform).anchorMin = new Vector2(0.5f, 0.5f);
                            (tmpLayoutItem.transform as RectTransform).anchorMax = new Vector2(0.5f, 0.5f);
                        }
                        break;
                    case Dir.Vertical:
                        {
                            (tmpLayoutItem.transform as RectTransform).anchorMin = new Vector2(0.5f, 0.5f);
                            (tmpLayoutItem.transform as RectTransform).anchorMax = new Vector2(0.5f, 0.5f);
                        }
                        break;
                }
                (tmpLayoutItem.transform as RectTransform).pivot = new Vector2(0, 1);
                itemList.Add(tmpLayoutItem);
            }
            tmpLayoutItem.SetGridState(true);
            tmpLayoutItem.SetGridIdx(i, Idx2LocalPos(i), datas[i]);

            updateWork++;
        }

        if (!updateDirty)
        {
            for (int i = 0, imax = itemList.Count; i < imax; i++)
            {
                if (itemList[i].GetGridState() == false
                    && itemList[i].gameObject != null)
                {
                    GameObject.Destroy(itemList[i].gameObject);

                    itemList.RemoveAt(i);
                    i--;
                    imax--;
                }
            }
        }
    }

    private Vector3 Idx2LocalPos(int idx)
    {
        Vector3 ret = Vector3.zero;
        int gridX = 0, gridY = 0;
        switch (config.dir)
        {
            case Dir.Horizontal:
                {
                    gridX = idx / config.fixedRowOrColumnCount;
                    gridY = idx % config.fixedRowOrColumnCount;
                }
                break;
            case Dir.Vertical:
                {
                    gridX = idx % config.fixedRowOrColumnCount;
                    gridY = idx / config.fixedRowOrColumnCount;
                }
                break;
        }
        ret.x += gridX * (config.spacing.x + config.cellSize.x);
        ret.y -= gridY * (config.spacing.y + config.cellSize.y);
        return ret;
    }

    private void ResetPos()
    {
        Vector3 pos = Vector3.zero;
        float dirLen = DirLen * 1.0f - config.fiexdDirViewCountF;
        if(dirLen < 0)
        {
            dirLen = 0;
        }
        float fDirLen = 0;
        switch (config.dir)
        {
            case Dir.Horizontal:
                {
                    fDirLen = config.cellSize.x * dirLen;
                    if(dirLen > 1)
                    {
                        fDirLen += config.spacing.x * (dirLen - 1);
                    }
                    pos.x -= fDirLen * config.startProgress;

                    pos.y += config.cellSize.y * config.fixedRowOrColumnCount + config.spacing.y * (config.fixedRowOrColumnCount - 1);
                    pos.y *= 0.5f;
                }
                break;
            case Dir.Vertical:
                {
                    pos.x -= config.cellSize.x * config.fixedRowOrColumnCount + config.spacing.x * (config.fixedRowOrColumnCount - 1);
                    pos.x *= 0.5f;

                    fDirLen = config.cellSize.y * dirLen;
                    if (dirLen > 1)
                    {
                        fDirLen += config.spacing.y * (dirLen - 1);
                    }
                    pos.y += fDirLen * config.startProgress;
                    
                }
                break;
        }
        this.transform.localPosition = pos;
    }

    private int DirLen
    {
        get
        {
            return (TotalCount / config.fixedRowOrColumnCount) + (TotalCount % config.fixedRowOrColumnCount == 0 ? 0 : 1);
        }
    }

    private void ResetDelta()
    {
        Vector2 size = config.cellSize * config.fixedRowOrColumnCount;
        int dirLen = DirLen;
        switch (config.dir)
        {
            case Dir.Horizontal:
                {
                    size.x = dirLen * config.cellSize.x;
                    if (dirLen > 0)
                    {
                        size.x += (dirLen - 1) * config.spacing.x;
                    }
                }
                break;
            case Dir.Vertical:
                {
                    size.y = dirLen * config.cellSize.y;
                    if (dirLen > 0)
                    {
                        size.y += (dirLen - 1) * config.spacing.y;
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
        public Transform prefab;

        public Dir dir = Dir.Horizontal;

        public Vector2 cellSize = new Vector2(100f, 100f);

        public Vector2 spacing = new Vector2(10, 10);

        /// <summary>
        /// 固定行数或列数
        /// </summary>
        public int fixedRowOrColumnCount = 1;

        /// <summary>
        /// 单位宽度可视数量
        /// </summary>
        public float fiexdDirViewCountF = 1f;

        /// <summary>
        /// 额外缓存的行数或列数
        /// </summary>
        public int viewCountHalfBuffer = 0;

        public float startProgress = 0;

        /// <summary>
        /// 0无限
        /// </summary>
        public int workCountPerFrame = 0;
    }
}