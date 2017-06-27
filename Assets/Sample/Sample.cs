using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Sample : MonoBehaviour
{
    public GameObject prefab;
    public Transform layoutHContent;
    public Transform layoutVContent;
    public Button genData;
    public Button addData;
    public Button minusData;
    public Text dataNum;

    private List<Item.ItemData> datas = new List<Item.ItemData>();
    private int dataAmt = 0;

    private Layout layoutH;
    private Layout layoutV;

    void Awake()
    {
        layoutH = layoutHContent.gameObject.AddComponent<Layout>();
        layoutV = layoutVContent.gameObject.AddComponent<Layout>();
        RefreshDataNum();

        genData.onClick.AddListener(() =>
        {
            GenDatas();
            layoutH.DoInit(new Layout.ConfigData()
            {
                startIdx = 0,
                bufferHalfAmt = 1,
                cellSize = new Vector2(190, 190),
                dir = Layout.Dir.Horizontal,
                oneScreenAmt = 3,
                prefab = prefab.transform,
                spacing = 10,
            }, datas);

            layoutV.DoInit(new Layout.ConfigData()
            {
                startIdx = 0,
                bufferHalfAmt = 1,
                cellSize = new Vector2(190, 190),
                dir = Layout.Dir.Vertical,
                oneScreenAmt = 3,
                prefab = prefab.transform,
                spacing = 10,
            }, datas);
        });

        addData.onClick.AddListener(() =>
        {
            if (dataAmt > 100000)
            {
                return;
            }
            if (dataAmt == 0)
            {
                dataAmt++;
            }
            else
            {
                dataAmt *= 2;
            }
            RefreshDataNum();
        });

        minusData.onClick.AddListener(() =>
        {
            dataAmt /= 2;
            RefreshDataNum();
        });
    }

    private void RefreshDataNum()
    {
        dataNum.text = dataAmt.ToString();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.M))
        {
            datas[0].id += 100;
            //Debug.LogWarning(datas[0].id + " " + datas[0].num);
            layoutH.RefreshItemDatas();
            layoutV.RefreshItemDatas();
        }
    }

    #region 随机数据

    private void GenDatas()
    {
        datas.Clear();
        for (int i = 0; i < dataAmt; i++)
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