using System.Collections.Generic;
using Jerry;
using UnityEngine;
using UnityEngine.UI;

public class Sample : MonoBehaviour
{
    public GameObject prefab;
    public Transform layoutHContent;
    public Transform layoutVContent;
    public Button genData;
    public Button sortData;
    public Button addData;
    public Button minusData;
    public InputField dataNum;

    public Slider layoutHProgress;
    public InputField layoutHSpacing;

    private List<Item.ItemData> datas = new List<Item.ItemData>();

    private Layout layoutH;
    private Layout layoutV;

    private bool sortFlag = true;
    private float lastProgressV = 1f;

    void Awake()
    {
        layoutHSpacing.text = "0";

        layoutH = layoutHContent.gameObject.AddComponent<Layout>();
        layoutV = layoutVContent.gameObject.AddComponent<Layout>();
        RefreshDataNum(0);

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
            layoutH.RefreshDatas();
            layoutV.RefreshDatas();
        });

        genData.onClick.AddListener(() =>
        {
            GenDatas();

            if (!layoutH.Inited)
            {
                layoutH.DoInit(new LayoutConfig()
                {
                    startProgress = layoutHProgress.value,
                    viewCountHalfBuffer = 1,
                    cellSize = new Vector2(190, 190),
                    dir = GridLayoutGroup.Axis.Horizontal,
                    viewMaskLen = 590f,
                    prefab = prefab.transform,
                    spacing = new Vector2(JerryUtil.String2TArray<float>(layoutHSpacing.text)[0], JerryUtil.String2TArray<float>(layoutHSpacing.text)[0]),
                    fixedRowOrColumnCount = 1,
                    workCountPerFrame = 0,
                }, datas);
            }
            else
            {
                layoutH.RefreshDatas(datas, new ModifyConfig()
                {
                    progress = layoutHProgress.value,
                    spacing = new Vector2(JerryUtil.String2TArray<float>(layoutHSpacing.text)[0], JerryUtil.String2TArray<float>(layoutHSpacing.text)[0]),
                });
            }

            if (!layoutV.Inited)
            {
                layoutV.DoInit(new LayoutConfig()
                {
                    startProgress = lastProgressV,
                    viewCountHalfBuffer = 1,
                    cellSize = new Vector2(190, 190),
                    dir = GridLayoutGroup.Axis.Vertical,
                    viewMaskLen = 500f,
                    prefab = prefab.transform,
                    spacing = new Vector2(10, 10),
                    fixedRowOrColumnCount = 3,
                    workCountPerFrame = 3,
                }, datas);
            }
            else
            {
                lastProgressV = layoutV.CurProgress();

                layoutV.RefreshDatas(datas);
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

    void Update()
    {
    }

    private void RefreshDataNum(int num)
    {
        dataNum.text = num.ToString();
    }

    private int GetDataNum()
    {
        return JerryUtil.String2TArray<int>(dataNum.text)[0];
    }

    #region 随机数据

    private void GenDatas()
    {
        idKey = 0;
        datas.Clear();
        for (int i = 0; i < GetDataNum(); i++)
        {
            datas.Add(RandomOneData());
        }
    }

    private int idKey = 0;
    private int GenId()
    {
        idKey++;
        return idKey;
    }

    private Item.ItemData RandomOneData()
    {
        Item.ItemData ret = new Item.ItemData();
        ret.id = GenId();
        ret.num = Random.Range(0, 1000);
        ret.head = string.Format("head{0}", Random.Range(0, 3));
        return ret;
    }

    #endregion 随机数据
}