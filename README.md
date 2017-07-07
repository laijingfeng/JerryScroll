项目 | 内容
---|---
标题 | JerryScroll
目录 | Github/项目
标签 | Github
备注 | [Github](https://github.com/laijingfeng/JerryScroll)
创建 | 2017-06-24 09:49:42
更新 | 2017-07-07 17:13:23

[TOC]

# 说明

对UGUI下LayoutGroup的优化

# InfinitelyGridLayoutGroup(无穷格子布局组)

原理：利用Grid大小固定的特性（每一个元素在Content的位置是可以计算的），Content的大小设置为总大小，这样就不用关心滑动的细节，滑动过程根据Content位置计算视野内的元素，刷新边界的元素。从而减少同时存在的元素量，减少元素的创建销毁。

> 注意：
>
> 数据只支持左对齐或上对齐，不支持居中对齐
>
> 数据到达一定的量的时候（如10w），Pos的值可能会非常大，会影响滑动的效果（可以明显看到progress为0和1时滑动手感是不一样的，所以感觉是数值[单排时(spacing+cell)*count]太大造成的）当然实际使用中，数据到1w都极少极少吧

![image](http://odk2uwdl8.bkt.clouddn.com/InfinitelyGridLayoutGroup00.png)

## 使用（参考Sample）

- 新建元素UI类，继承LayoutItem，重写如下刷新UI的函数
    - `public override void TryRefreshUI(ILayoutItemData data)`
- 新建元素数据类，继承ILayoutItemData
- 新建一个新的具体Layout类，继承`InfinitelyLinearGridLayoutGroup<T, F>`，指定具体的元素UI结构T(LayoutItem)和元素数据结构F(ILayoutItemData)
    - 不需要其他内容，直接可用了
    - 为什么不直接用父类？
        - Unity不支持挂载这样的Mono
- 脚本挂载在`Scroll View/Viewport/Content`结点
- ==初始化配置和数据==（配置说明见后面）
    - `public void DoInit(ConfigData tconfig, List<F> tdatas)`
- ==数据变更==
    - `public void RefreshDatas(List<F> tdatas = null, ModifyConfig modifyConfig = null)`
    - 新的数据
    - 可以微调部分参数，结构见后面说明
- ==获取进度==
    - `public float CurProgress()`
    - 辅助保存进度下次恢复
- 优化建议
    - 元素数据结构填充代价大的内容（比如头像/图标等要加载卸载资源）在刷新UI的时候进行差异比较，刷新UI的时候判断差异才修改

配置数据（LayoutConfig）：
- LayoutDir dir 方向
- Vector2 cellSize 大小
- Vector2 spacing 间隔
- Transform prefab 预设
- int fixedRowOrColumnCount 固定的行数或者列数（单排或多排）
- float viewMaskLen 可视区域长度，用来算进度
- int viewCountHalfBuffer 单边额外缓存的数量，建议为1
- float startProgress 初始进度
- int workCountPerFrame 一帧最多更新几个元素，0是无限
    - 当一屏显示的数据很多，第一次初始化会卡顿，可以设置这个数量

配置微调（ModifyConfig）：
- float? progress
- Vector2? spacing

## 编辑器辅助

新建的Layout类可以直接挂载到资源上，设置配置信息，右击脚本执行`CreateItemsForEditor`，创建元素来观察和调整参数的效果

## QA

- Q：如何实现Padding？
    - A：把Mask从`Viewport`移到`Scroll View`，设置`Viewport`中`RectTransform`的`Left/Right/Top/Bottom`。