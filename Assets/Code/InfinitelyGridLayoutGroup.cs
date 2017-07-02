using System.Collections.Generic;
using Jerry;
using UnityEngine;
using UnityEngine.UI;

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

    public LayoutConfig config = new LayoutConfig();

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
            if (editorMode)
            {
                return editorTotalCount;
            }
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
    /// <param name="tdatas">数据数量有变化，全部刷新</param>
    /// <param name="modifyConfig">辅助第一个参数，调整部分配置</param>
    /// <param name="idxs">数据内容有变化(内容修改/排序修改)，可单点刷新</param>
    public void RefreshDatas(List<F> tdatas = null, ModifyConfig modifyConfig = null, List<int> idxs = null)
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

            if (modifyConfig != null)
            {
                bool change = false;
                if (modifyConfig.spacing.HasValue)
                {
                    config.spacing = modifyConfig.spacing.Value;
                    switch (config.dir)
                    {
                        case GridLayoutGroup.Axis.Horizontal:
                            {
                                fiexdDirViewCountF = config.viewMaskLen / (config.cellSize.x + config.spacing.x);
                            }
                            break;
                        case GridLayoutGroup.Axis.Vertical:
                            {
                                fiexdDirViewCountF = config.viewMaskLen / (config.cellSize.y + config.spacing.y);
                            }
                            break;
                    }
                    fiexdDirViewCount = Mathf.CeilToInt(fiexdDirViewCountF);
                    change = true;
                }
                if (modifyConfig.progress.HasValue)
                {
                    config.startProgress = Mathf.Clamp01(modifyConfig.progress.Value);
                    change = true;
                }

                if (change)
                {
                    ResetPos();
                }
            }

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
                    fDirLen -= config.viewMaskLen;
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
                    fDirLen -= config.viewMaskLen;
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
                    fiexdDirViewCountF = config.viewMaskLen / (config.cellSize.x + config.spacing.x);
                }
                break;
            case GridLayoutGroup.Axis.Vertical:
                {
                    (rectTran.parent as RectTransform).pivot = new Vector2(0.5f, 1.0f);
                    rectTran.anchorMin = new Vector2(0.5f, 1.0f);
                    rectTran.anchorMax = new Vector2(0.5f, 1.0f);
                    fiexdDirViewCountF = config.viewMaskLen / (config.cellSize.y + config.spacing.y);
                }
                break;
        }
        rectTran.pivot = new Vector2(0, 1);

        fiexdDirViewCount = Mathf.CeilToInt(fiexdDirViewCountF);
        config.startProgress = Mathf.Clamp01(config.startProgress);

        itemList.Clear();
        if (!editorMode)
        {
            JerryUtil.DestroyAllChildren(this.transform);
        }

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
        if (editorMode
            || !awaked
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
            case GridLayoutGroup.Axis.Horizontal:
                {
                    gridX = idx / config.fixedRowOrColumnCount;
                    gridY = idx % config.fixedRowOrColumnCount;
                }
                break;
            case GridLayoutGroup.Axis.Vertical:
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
                    fDirLen -= config.viewMaskLen;
                    if (fDirLen < 0)
                    {
                        fDirLen = 0;
                    }
                    pos.x -= fDirLen * config.startProgress;

                    pos.y += config.cellSize.y * config.fixedRowOrColumnCount + config.spacing.y * (config.fixedRowOrColumnCount - 1);
                    pos.y *= 0.5f;
                }
                break;
            case GridLayoutGroup.Axis.Vertical:
                {
                    pos.x -= config.cellSize.x * config.fixedRowOrColumnCount + config.spacing.x * (config.fixedRowOrColumnCount - 1);
                    pos.x *= 0.5f;

                    fDirLen = config.cellSize.y * dirLen;
                    if (dirLen > 1)
                    {
                        fDirLen += config.spacing.y * (dirLen - 1);
                    }
                    fDirLen -= config.viewMaskLen;
                    if (fDirLen < 0)
                    {
                        fDirLen = 0;
                    }
                    pos.y += fDirLen * config.startProgress;
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
            return (TotalCount / config.fixedRowOrColumnCount) + (TotalCount % config.fixedRowOrColumnCount == 0 ? 0 : 1);
        }
    }

    private void ResetDelta()
    {
        Vector2 size = config.cellSize * config.fixedRowOrColumnCount;
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

                    if (config.fixedRowOrColumnCount > 1)
                    {
                        size.y += (config.fixedRowOrColumnCount - 1) * config.spacing.y;
                    }
                }
                break;
            case GridLayoutGroup.Axis.Vertical:
                {
                    if (config.fixedRowOrColumnCount > 1)
                    {
                        size.x += (config.fixedRowOrColumnCount - 1) * config.spacing.x;
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

    #region 编辑器功能_用来对位置(为了DLL的通用性，不加宏)

    /// <summary>
    /// 编辑器模式
    /// </summary>
    public bool editorMode = false;
    public int editorTotalCount = 1;

    [ContextMenu("CreateItemsForEditor")]
    protected void CreateItemsForEditor()
    {
        awaked = true;
        inited = true;
        TryWork();
        CalFirstIdx();

        while (true)
        {
            if (this.transform.childCount <= 0)
            {
                break;
            }
            Transform tf = this.transform.GetChild(0);
            if (tf != null && tf.gameObject != null)
            {
                GameObject.DestroyImmediate(tf.gameObject);
            }
        }

        for (int i = 0; i < TotalCount; i++)
        {
            GameObject go = JerryUtil.CloneGo(new JerryUtil.CloneGoData()
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
                        (go.transform as RectTransform).anchorMin = new Vector2(0.5f, 0.5f);
                        (go.transform as RectTransform).anchorMax = new Vector2(0.5f, 0.5f);
                    }
                    break;
                case GridLayoutGroup.Axis.Vertical:
                    {
                        (go.transform as RectTransform).anchorMin = new Vector2(0.5f, 0.5f);
                        (go.transform as RectTransform).anchorMax = new Vector2(0.5f, 0.5f);
                    }
                    break;
            }
            (go.transform as RectTransform).pivot = new Vector2(0, 1);
            go.transform.localPosition = Idx2LocalPos(i);
        }
    }

    [ContextMenu("Progress")]
    protected void PrintProgress()
    {
        Debug.LogWarning("进度:" + CurProgress());
    }

    #endregion 编辑器功能_用来对位置(为了DLL的通用性，不加宏)
}