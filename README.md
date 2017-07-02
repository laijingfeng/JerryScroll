��Ŀ | ����
---|---
���� | JerryScroll
Ŀ¼ | Github/��Ŀ
��ǩ | Github
��ע | [Github](https://github.com/laijingfeng/JerryScroll)
���� | 2017-06-24 09:49:42
���� | 2017-07-02 17:57:24

[TOC]

# ˵��

��UGUI��LayoutGroup���Ż�

# InfinitelyGridLayoutGroup(������Ӳ�����)

ԭ������Grid��С�̶������ԣ�ÿһ��Ԫ����Content��λ���ǿ��Լ���ģ���Content�Ĵ�С����Ϊ�ܴ�С�������Ͳ��ù��Ļ�����ϸ�ڣ��������̸���Contentλ�ü�����Ұ�ڵ�Ԫ�أ�ˢ�±߽��Ԫ�ء��Ӷ�����ͬʱ���ڵ�Ԫ����������Ԫ�صĴ������١�

> ע�⣺
>
> ����ֻ֧���������϶��룬��֧�־��ж���
>
> ���ݵ���һ��������ʱ����10w����Pos��ֵ���ܻ�ǳ��󣬻�Ӱ�컬����Ч�����������Կ���progressΪ0��1ʱ�����ָ��ǲ�һ���ģ����Ըо�����ֵ[����ʱ(spacing+cell)*count]̫����ɵģ���Ȼʵ��ʹ���У����ݵ�1w�����ټ��ٰ�

![image](http://odk2uwdl8.bkt.clouddn.com/InfinitelyGridLayoutGroup00.png)

## ʹ�ã��ο�Sample��

- �½�Ԫ��UI�࣬�̳�LayoutItem����д����ˢ��UI�ĺ���
    - `public override void TryRefreshUI(ILayoutItemData data)`
- �½�Ԫ�������࣬�̳�ILayoutItemData
- �½�һ���µľ���Layout�࣬�̳�`InfinitelyLinearGridLayoutGroup<T, F>`��ָ�������Ԫ��UI�ṹT(LayoutItem)��Ԫ�����ݽṹF(ILayoutItemData)
    - ����Ҫ�������ݣ�ֱ�ӿ�����
    - Ϊʲô��ֱ���ø��ࣿ
        - Unity��֧�ֹ���������Mono 
- ==��ʼ�����ú�����==������˵�������棩
    - `public void DoInit(ConfigData tconfig, List<F> tdatas)`
- ==���ݱ��==
    - `public void RefreshDatas(List<F> tdatas = null, ModifyConfig modifyConfig = null, List<int> idxs = null)`
    - ���������б仯
        - Ҳ������ɾ������Ҫ��tdatas��������ʱ�ڶ������������壬���ӵ����ݶ���ˢһ��
        - �ڶ��������Ǹ�����һ�������ģ�����΢�����ֲ������ṹ������˵��
    - ���������ޱ仯
        - �������ݱ��������Ҳ�ǣ��������һ������
        - �����������������ǳ��Ը�����ʾ���������ݣ�����ֻ����ָ����idx���
- ==��ȡ����==
    - `public float CurProgress()`
    - ������������´λָ�
- �Ż�����
    - Ԫ�����ݽṹ�����۴�����ݣ�����ͷ��/ͼ���Ҫ����ж����Դ����ˢ��UI��ʱ����в���Ƚϣ�ˢ��UI��ʱ���жϲ�����޸�

�������ݣ�LayoutConfig����
- LayoutDir dir ����
- Vector2 cellSize ��С
- Vector2 spacing ���
- Transform prefab Ԥ��
- int fixedRowOrColumnCount �̶��������������������Ż���ţ�
- float viewMaskLen �������򳤶ȣ����������
- int viewCountHalfBuffer ���߶��⻺�������������Ϊ1
- float startProgress ��ʼ����
- int workCountPerFrame һ֡�����¼���Ԫ�أ�0������
    - ��һ����ʾ�����ݺܶ࣬��һ�γ�ʼ���Ῠ�٣����������������

����΢����ModifyConfig����
- float? progress
- Vector2? spacing

## �༭������

�½���Layout�����ֱ�ӹ��ص���Դ�ϣ�����������Ϣ���һ��ű�ִ��`CreateItemsForEditor`������Ԫ�����۲�͵���������Ч��