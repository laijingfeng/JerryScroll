using System.Collections.Generic;
using Jerry;
using UnityEngine;
using UnityEngine.UI;

public class Sample : MonoBehaviour
{
    void Awake()
    {
        TestV();
        TestH();
    }

    #region ScrollVivewH

    public bool runH = false;

    private SampleLayout layoutH;
    private Transform layoutHContent;
    private void TestH()
    {
        if (!runH)
        {
            return;
        }

        layoutHContent = this.transform.FindChild("ScrollViewHEditor/Viewport/Content");
        layoutH = layoutHContent.gameObject.AddComponent<SampleLayout>();

        LayoutConfig config = new LayoutConfig(layoutHContent);
        config.frameWorkCnt = 0;
        config.bufHalfCnt = 1;

        layoutH.DoInit(config, GenDatas(10));
    }

    #endregion ScrollVivewH

    #region ScrollVivewV

    public GameObject prefab;
    public Transform layoutVContent;
    public Button genData;
    public Button sortData;
    public Button addData;
    public Button minusData;
    public InputField dataNum;

    public Slider layoutProgress;
    public InputField layoutSpacing;

    private SampleLayout layoutV;
    private bool sortFlag = true;

    private void TestV()
    {
        layoutSpacing.text = "0";

        layoutV = layoutVContent.gameObject.AddComponent<SampleLayout>();
        RefreshDataNum(0);
        List<SampleItem.ItemData> datas = new List<SampleItem.ItemData>();

        sortData.onClick.AddListener(() =>
        {
            if (datas.Count <= 0)
            {
                return;
            }
            if (sortFlag)
            {
                datas.Sort((x, y) => -x.id.CompareTo(y.id));
            }
            else
            {
                datas.Sort((x, y) => x.id.CompareTo(y.id));
            }
            sortFlag = !sortFlag;
            layoutV.RefreshDatas();
        });

        genData.onClick.AddListener(() =>
        {
            datas = GenDatas(GetDataNum());
            if (!layoutV.Inited)
            {
                layoutV.DoInit(new LayoutConfig()
                {
                    progress = layoutProgress.value,
                    bufHalfCnt = 1,
                    cellSize = new Vector2(190, 190),
                    dir = GridLayoutGroup.Axis.Vertical,
                    dirViewLen = 500f,
                    prefab = prefab.transform,
                    spacing = new Vector2(JerryUtil.String2TArray<float>(layoutSpacing.text)[0], JerryUtil.String2TArray<float>(layoutSpacing.text)[0]),
                    dirCellWidth = 3,
                    frameWorkCnt = 3,
                }, datas);
            }
            else
            {
                layoutV.RefreshDatas(datas, new ModifyConfig()
                {
                    progress = layoutProgress.value,
                    spacing = new Vector2(JerryUtil.String2TArray<float>(layoutSpacing.text)[0], JerryUtil.String2TArray<float>(layoutSpacing.text)[0]),
                });
            }
        });

        addData.onClick.AddListener(() =>
        {
            if (GetDataNum() > 100000)
            {
                return;
            }
            RefreshDataNum(GetDataNum() == 0 ? 1 : GetDataNum() * 2);
        });

        minusData.onClick.AddListener(() =>
        {
            RefreshDataNum(GetDataNum() / 2);
        });
    }

    private void RefreshDataNum(int num)
    {
        dataNum.text = num.ToString();
    }

    private int GetDataNum()
    {
        return JerryUtil.String2TArray<int>(dataNum.text)[0];
    }

    #endregion ScrollVivewV

    #region 随机数据

    private List<SampleItem.ItemData> GenDatas(int cnt)
    {
        idKey = 0;
        List<SampleItem.ItemData> datas = new List<SampleItem.ItemData>();
        for (int i = 0; i < cnt; i++)
        {
            datas.Add(RandomOneData());
        }
        return datas;
    }

    private int idKey = 0;
    private int GenId()
    {
        idKey++;
        return idKey;
    }

    private SampleItem.ItemData RandomOneData()
    {
        SampleItem.ItemData ret = new SampleItem.ItemData();
        ret.id = GenId();
        ret.num = Random.Range(0, 1000);
        ret.head = string.Format("head{0}", Random.Range(0, 3));
        return ret;
    }

    #endregion 随机数据
}