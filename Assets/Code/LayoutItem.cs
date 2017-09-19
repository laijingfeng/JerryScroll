using UnityEngine;
using UnityEngine.UI;

namespace Jerry
{
    public interface ILayoutItemData
    {
    }

    public abstract class LayoutItem : MonoBehaviour
    {
        private int gridIdx;
        private bool gridInUse;

        public void SetGridHide()
        {
            gridIdx = -1;
            if (this.gameObject.activeSelf)
            {
                this.gameObject.SetActive(false);
            }
        }

        public void SetGridIdx(int idx, Vector3 localPos, ILayoutItemData data)
        {
            gridIdx = idx;
            this.transform.localPosition = localPos;
            TryRefreshUI(data);
            //Debug.LogWarning("xxx " + idx);
        }

        public abstract void TryRefreshUI(ILayoutItemData data);

        public int GetGridIdx()
        {
            return gridIdx;
        }

        public void SetGridState(bool inUse)
        {
            gridInUse = inUse;
            if (gridInUse == true && !this.gameObject.activeSelf)
            {
                this.gameObject.SetActive(true);
            }
        }

        public bool GetGridState()
        {
            return gridInUse;
        }

        public static bool ValDiff<T>(T a, T b)
        {
            if (a == null)
            {
                return !(b == null);
            }
            //备注：为null的string执行Equals要报错的
            return !a.Equals(b);
        }
    }

    [System.Serializable]
    public class LayoutConfig
    {
        /// <summary>
        /// 预设
        /// </summary>
        [Tooltip("预设，模版")]
        public Transform prefab;

        /// <summary>
        /// 方向
        /// </summary>
        [Tooltip("滑动方向")]
        public GridLayoutGroup.Axis dir = GridLayoutGroup.Axis.Horizontal;

        /// <summary>
        /// dir方向元素扩展
        /// </summary>
        [Tooltip("滑动方向的宽度，横向滑动则是有几排，竖向滑动则是有几列")]
        public int dirCellWidth = 1;

        /// <summary>
        /// 可视区域dir方向长度，用来算进度
        /// </summary>
        [Tooltip("可是区域在滑动方向的长度")]
        public float dirViewLen = 1f;

        /// <summary>
        /// 元素大小
        /// </summary>
        [Tooltip("预设，模板的大小")]
        public Vector2 cellSize = new Vector2(100f, 100f);

        /// <summary>
        /// 两个元素间的间隔
        /// </summary>
        [Tooltip("创建的两个元素间的间隔")]
        public Vector2 spacing = new Vector2(10, 10);

        /// <summary>
        /// 两端各额外缓存的行数或列数
        /// </summary>
        [Tooltip("【美术可以忽略】两端各额外缓存的行数或列数")]
        [HideInInspector]
        public int bufHalfCnt = 1;

        /// <summary>
        /// 设置进度
        /// </summary>
        [Tooltip("【美术可以忽略】设置进度")]
        [HideInInspector]
        public float progress = 0;

        /// <summary>
        /// 一帧工作数量，0无限
        /// </summary>
        [Tooltip("【美术可以忽略】一帧工作(刷新)数量")]
        [HideInInspector]
        public int frameWorkCnt = 0;

        public LayoutConfig()
        {
        }

        public LayoutConfig(Transform content)
        {
            if (content != null
                && content.GetComponent<LayoutEditor>() != null)
            {
                FillData(content.GetComponent<LayoutEditor>().config);
            }
        }

        public LayoutConfig(LayoutConfig config)
        {
            FillData(config);
        }

        public void FillData(LayoutConfig config)
        {
            this.prefab = config.prefab;
            this.dir = config.dir;
            this.dirCellWidth = config.dirCellWidth;
            this.dirViewLen = config.dirViewLen;
            this.cellSize = config.cellSize;
            this.spacing = config.spacing;
            this.bufHalfCnt = config.bufHalfCnt;
            this.progress = config.progress;
        }
    }

    /// <summary>
    /// <para>修改配置</para>
    /// <para>重刷数据的时候，可以修改部分配置</para>
    /// <para>特别是想复用之前的Prefab，有些细微配置不一样，可以用RefreshDatas而不用DoInit</para>
    /// </summary>
    public class ModifyConfig
    {
        public float? progress = null;
        public Vector2? spacing = null;

        public bool HaveChange(float curProgress, Vector2 curSpacing)
        {
            bool ret = false;
            if (ret == false
                && progress.HasValue
                && !Mathf.Approximately(progress.Value, curProgress))
            {
                ret = true;
            }
            if (ret == false
                && spacing.HasValue
                && !Vector2.Equals(spacing.Value, curSpacing))
            {
                ret = true;
            }
            return ret;
        }
    }
}