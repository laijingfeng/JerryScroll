using UnityEngine;
using UnityEngine.UI;
using Jerry;

public class SampleItem : LayoutItem
{
    private Transform head;
    private Text id;
    private Text num;

    private ItemData curData = null;
    private ItemData newData = null;
    private ItemDataChange dataChange = new ItemDataChange();
    private bool isDataDirty = false;
    private bool _awaked = false;

    void Awake()
    {
        head = this.transform.Find("head");
        id = this.transform.Find("id").GetComponent<Text>();
        num = this.transform.Find("num").GetComponent<Text>();
        _awaked = true;
        TryFillData();
    }

    public override void TryRefreshUI(ILayoutItemData data)
    {
        if (data == null)
        {
            return;
        }
        newData = data as ItemData;
        isDataDirty = true;
        TryFillData();
    }

    private void TryFillData()
    {
        if (!_awaked
            || !isDataDirty)
        {
            return;
        }
        isDataDirty = false;
        curData = dataChange.Diff(curData, newData);

        if (dataChange.headChanged)
        {
            JerryUtil.CloneGo(new JerryUtil.CloneGoData()
            {
                active = true,
                clean = true,
                parant = head,
                prefab = Resources.Load<GameObject>(curData.head),
            });
        }

        id.text = curData.id.ToString();
        num.text = curData.num.ToString();
    }

    #region 数据结构

    public class ItemData : ILayoutItemData
    {
        public int id;
        public int num;
        public string head;
    }

    public class ItemDataChange
    {
        public bool headChanged = false;
        
        /// <summary>
        /// 比较
        /// </summary>
        /// <param name="oriD"></param>
        /// <param name="newD"></param>
        /// <returns></returns>
        public ItemData Diff(ItemData oriD, ItemData newD)
        {
            headChanged = (oriD == null || LayoutItem.ValDiff<string>(oriD.head, newD.head));
            return newD;
        }
    }

    #endregion 数据结构
}