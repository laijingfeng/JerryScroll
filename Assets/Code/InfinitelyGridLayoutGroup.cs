using System.Collections.Generic;
using Jerry;
using UnityEngine;
using UnityEngine.UI;

public abstract class InfinitelyGridLayoutGroup<T, F> : MonoBehaviour
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
    /// <summary>
    /// 是否初始化好了
    /// </summary>
    public bool Inited
    {
        get
        {
            return inited;
        }
    }
    private bool ready = false;

    private LayoutConfig config = new LayoutConfig();

    /// <summary>
    /// 长度方向可视数量，取上整
    /// </summary>
    private int fiexdDirViewCount = 1;
    /// <summary>
    /// 长度方向可视数量，浮点数
    /// </summary>
    private float fiexdDirViewCountF = 1f;

    /// <summary>
    /// 数据
    /// </summary>
    private List<F> datas = new List<F>();
    /// <summary>
    /// 数据总量
    /// </summary>
    private int TotalCount
    {
        get
        {
            return (datas == null) ? 0 : datas.Count;
        }
    }

    protected virtual void Awake()
    {
        awaked = true;
        TryWork();
    }

    protected virtual void Update()
    {
        CheckUpdate();
    }

    private bool SetNewData(List<F> tdatas = null)
    {
        bool change = false;
        if (tdatas == null)
        {
            change = datas != null;
        }
        else
        {
            change = datas == null || datas.Count != tdatas.Count;
        }
        datas = tdatas;
        return change;
    }

    #region 对外接口

    public void DoInit(LayoutConfig tconfig, List<F> tdatas)
    {
        config = tconfig;
        datas = tdatas;
        inited = true;
        TryWork();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="tdatas">新的数据数量</param>
    /// <param name="modifyConfig">调整部分配置</param>
    public void RefreshDatas(List<F> tdatas = null, ModifyConfig modifyConfig = null)
    {
        if (!awaked
            || !inited
            || !ready)
        {
            return;
        }

        bool dataAmtChange = SetNewData(tdatas);
        bool modify = modifyConfig != null && modifyConfig.HaveChange(CurProgress(), config.spacing);
        if (!modify && !dataAmtChange)
        {
            foreach (T t in itemList)
            {
                if (t.GetGridState() == true)
                {
                    t.TryRefreshUI(datas[t.GetGridIdx()]);
                }
            }
        }
        else
        {
            updateDirty = false;
            if (modify)
            {
                bool change = false;
                if (modifyConfig.spacing.HasValue)
                {
                    config.spacing = modifyConfig.spacing.Value;
                    switch (config.dir)
                    {
                        case GridLayoutGroup.Axis.Horizontal:
                            {
                                fiexdDirViewCountF = config.dirViewLen / (config.cellSize.x + config.spacing.x);
                            }
                            break;
                        case GridLayoutGroup.Axis.Vertical:
                            {
                                fiexdDirViewCountF = config.dirViewLen / (config.cellSize.y + config.spacing.y);
                            }
                            break;
                    }
                    fiexdDirViewCount = Mathf.CeilToInt(fiexdDirViewCountF);
                    change = true;
                }

                if (modifyConfig.progress.HasValue)
                {
                    config.progress = Mathf.Clamp01(modifyConfig.progress.Value);
                    change = true;
                }

                if (change)
                {
                    ResetPos();
                }
            }

            ResetDelta();
            CalFirstIdx();
            curFirstIdx = calFirstIdxIdx;
            RefreshData(true);
        }
    }

    /// <summary>
    /// 当前进度
    /// </summary>
    /// <returns></returns>
    public float CurProgress()
    {
        Vector3 pos = this.transform.localPosition;
        float dirLen = DirLen * 1.0f;
        float fDirLen = 0;
        float ret = 0;
        switch (config.dir)
        {
            case GridLayoutGroup.Axis.Horizontal:
                {
                    fDirLen = config.cellSize.x * dirLen;
                    if (dirLen > 1)
                    {
                        fDirLen += config.spacing.x * (dirLen - 1);
                    }
                    fDirLen -= config.dirViewLen;
                    if (fDirLen < 0)
                    {
                        fDirLen = 0;
                    }

                    if (fDirLen > 0)
                    {
                        ret = Mathf.Clamp01(-pos.x / fDirLen);
                    }
                }
                break;
            case GridLayoutGroup.Axis.Vertical:
                {
                    fDirLen = config.cellSize.y * dirLen;
                    if (dirLen > 1)
                    {
                        fDirLen += config.spacing.y * (dirLen - 1);
                    }
                    fDirLen -= config.dirViewLen;
                    if (fDirLen < 0)
                    {
                        fDirLen = 0;
                    }

                    if (fDirLen > 0)
                    {
                        ret = Mathf.Clamp01(pos.y / fDirLen);
                    }
                }
                break;
        }
        return ret;
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
            case GridLayoutGroup.Axis.Horizontal:
                {
                    (rectTran.parent as RectTransform).pivot = new Vector2(0, 0.5f);
                    rectTran.anchorMin = new Vector2(0, 0.5f);
                    rectTran.anchorMax = new Vector2(0, 0.5f);
                    fiexdDirViewCountF = config.dirViewLen / (config.cellSize.x + config.spacing.x);
                }
                break;
            case GridLayoutGroup.Axis.Vertical:
                {
                    (rectTran.parent as RectTransform).pivot = new Vector2(0.5f, 1.0f);
                    rectTran.anchorMin = new Vector2(0.5f, 1.0f);
                    rectTran.anchorMax = new Vector2(0.5f, 1.0f);
                    fiexdDirViewCountF = config.dirViewLen / (config.cellSize.y + config.spacing.y);
                }
                break;
        }
        rectTran.pivot = new Vector2(0, 1);

        fiexdDirViewCount = Mathf.CeilToInt(fiexdDirViewCountF);
        config.progress = Mathf.Clamp01(config.progress);

        itemList.Clear();
        JerryUtil.DestroyAllChildren(this.transform, false);

        curFirstIdx = -1;
        ResetPos();
        ResetDelta();

        ready = true;
    }

    private float calFirstIdxPos, calFirstIdxSize, calFirstIdxSpacing;
    private int calFirstIdxIdx;
    private void CalFirstIdx()
    {
        calFirstIdxPos = config.dir == GridLayoutGroup.Axis.Horizontal ? -this.transform.localPosition.x : this.transform.localPosition.y;
        calFirstIdxSize = config.dir == GridLayoutGroup.Axis.Horizontal ? config.cellSize.x : config.cellSize.y;
        calFirstIdxSpacing = config.dir == GridLayoutGroup.Axis.Horizontal ? config.spacing.x : config.spacing.y;
        calFirstIdxIdx = (int)(calFirstIdxPos / (calFirstIdxSize + calFirstIdxSpacing)) * config.dirCellWidth;
        calFirstIdxIdx -= config.bufHalfCnt * config.dirCellWidth;
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

        curLastIdx = curFirstIdx + (fiexdDirViewCount + config.bufHalfCnt * 2) * config.dirCellWidth - 1;
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
                && config.frameWorkCnt > 0
                && updateWork > config.frameWorkCnt)
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
                    case GridLayoutGroup.Axis.Horizontal:
                        {
                            (tmpLayoutItem.transform as RectTransform).anchorMin = new Vector2(0.5f, 0.5f);
                            (tmpLayoutItem.transform as RectTransform).anchorMax = new Vector2(0.5f, 0.5f);
                        }
                        break;
                    case GridLayoutGroup.Axis.Vertical:
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
                if (itemList[i].GetGridState() == false)
                {
                    itemList[i].SetGridHide();
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
            case GridLayoutGroup.Axis.Horizontal:
                {
                    gridX = idx / config.dirCellWidth;
                    gridY = idx % config.dirCellWidth;
                }
                break;
            case GridLayoutGroup.Axis.Vertical:
                {
                    gridX = idx % config.dirCellWidth;
                    gridY = idx / config.dirCellWidth;
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
        float dirLen = DirLen * 1.0f;
        float fDirLen = 0;
        switch (config.dir)
        {
            case GridLayoutGroup.Axis.Horizontal:
                {
                    fDirLen = config.cellSize.x * dirLen;
                    if (dirLen > 1)
                    {
                        fDirLen += config.spacing.x * (dirLen - 1);
                    }
                    fDirLen -= config.dirViewLen;
                    if (fDirLen < 0)
                    {
                        fDirLen = 0;
                    }
                    pos.x -= fDirLen * config.progress;

                    pos.y += config.cellSize.y * config.dirCellWidth + config.spacing.y * (config.dirCellWidth - 1);
                    pos.y *= 0.5f;
                }
                break;
            case GridLayoutGroup.Axis.Vertical:
                {
                    pos.x -= config.cellSize.x * config.dirCellWidth + config.spacing.x * (config.dirCellWidth - 1);
                    pos.x *= 0.5f;

                    fDirLen = config.cellSize.y * dirLen;
                    if (dirLen > 1)
                    {
                        fDirLen += config.spacing.y * (dirLen - 1);
                    }
                    fDirLen -= config.dirViewLen;
                    if (fDirLen < 0)
                    {
                        fDirLen = 0;
                    }
                    pos.y += fDirLen * config.progress;
                }
                break;
        }
        this.transform.localPosition = pos;
    }

    /// <summary>
    /// 方向总长度（Item数）
    /// </summary>
    private int DirLen
    {
        get
        {
            return (TotalCount / config.dirCellWidth) + (TotalCount % config.dirCellWidth == 0 ? 0 : 1);
        }
    }

    private void ResetDelta()
    {
        Vector2 size = config.cellSize * config.dirCellWidth;
        int dirLen = DirLen;
        switch (config.dir)
        {
            case GridLayoutGroup.Axis.Horizontal:
                {
                    size.x = dirLen * config.cellSize.x;
                    if (dirLen > 0)
                    {
                        size.x += (dirLen - 1) * config.spacing.x;
                    }

                    if (config.dirCellWidth > 1)
                    {
                        size.y += (config.dirCellWidth - 1) * config.spacing.y;
                    }
                }
                break;
            case GridLayoutGroup.Axis.Vertical:
                {
                    if (config.dirCellWidth > 1)
                    {
                        size.x += (config.dirCellWidth - 1) * config.spacing.x;
                    }

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
}