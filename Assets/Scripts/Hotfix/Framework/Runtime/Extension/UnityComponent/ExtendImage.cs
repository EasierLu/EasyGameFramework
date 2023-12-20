using Cysharp.Threading.Tasks;
using EGFramework.Runtime;
using EGFramework.Runtime.Asset;
using EGFramework.Runtime.Base;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Sprites;
using UnityEngine.UI;

namespace Hotfix.Framework.Runtime
{
    public class ExtendImage : UnityEngine.UI.Image
    {
        /// <summary> 九宫进度条  </summary>
        [SerializeField]
        private bool m_SlicedClipMode = false;

        /// <summary> 圆角模式 </summary>
        [SerializeField]
        private bool m_RoundedMode = false;
        [SerializeField]
        [Range(1,20)]
        private int m_RoundedTriangleNum = 4;
        [SerializeField]
        [Range(0, 1)]
        private float m_RoundedRadius = 0.5f;
        [SerializeField]
        private bool m_RoundedLeftTop = true;
        [SerializeField]
        private bool m_RoundedRightTop = true;
        [SerializeField]
        private bool m_RoundedLeftBottom = true;
        [SerializeField]
        private bool m_RoundedRightBottom = true;

        /// <summary> 异步加载中图片 </summary>
        [SerializeField]
        private Sprite m_AsyncLoadingSprite;
        public Sprite AsyncLoadingSprite { get => m_AsyncLoadingSprite; set => m_AsyncLoadingSprite = value; }
        private string m_CurrentSpritePath = null;


        private IAssetManager m_AssetManager;

        public void SetSprite(Sprite s)
        { 
            m_CurrentSpritePath = null;
            sprite = s;
        }

        /// <summary>
        /// 同步加载图片
        /// </summary>
        /// <param name="path"></param>
        public void LoadSprite(string path)
        {
            m_CurrentSpritePath = path;
            //TODO:同步加载图片
        }

        /// <summary>
        /// 异步加载图片
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public async UniTask LoadSpriteAsync(string path, bool isOverride = false)
        {
            if (path == m_CurrentSpritePath)
            {
                return;
            }
            string oldSpritePath = m_CurrentSpritePath;
            m_CurrentSpritePath = path;
            Sprite currentOverrideSprite = overrideSprite;
            if (m_AsyncLoadingSprite != null)
            {
                overrideSprite = m_AsyncLoadingSprite;
            }
            Sprite s = await InnerLoadSpriteAsync(path);

            if (path == m_CurrentSpritePath && this != null)
            {
                if (isOverride)
                {
                    overrideSprite = s;
                }
                else
                {
                    sprite = s;
                    overrideSprite = currentOverrideSprite;
                }
            }
        }

        protected virtual async UniTask<Sprite> InnerLoadSpriteAsync(string path)
        {
            if (m_AssetManager == null)
            { 
                m_AssetManager = FrameworkCore.GetComponent<AssetManager>();
                if (m_AssetManager == null)
                {
                    return null;
                }
            }
            return await m_AssetManager.LoadAssetAsync<Sprite>(path);
        }

        protected override void OnPopulateMesh(VertexHelper vh)
        {
            switch (type)
            {
                case Type.Filled when m_SlicedClipMode && (fillMethod == FillMethod.Horizontal || fillMethod == FillMethod.Vertical) && hasBorder:
                    GenerateSlicedSprite(vh);
                    break;
                case Type.Simple when m_RoundedMode:
                    GenerateRoundedSprite(vh);
                    break;
                default:
                    base.OnPopulateMesh(vh);
                    break;
            }
        }

        #region 九宫进度条
        private Vector2[] m_VertScratch = new Vector2[4];
        private Vector2[] m_UVScratch = new Vector2[4];

        private void GenerateSlicedSprite(VertexHelper toFill)
        {
            var activeSprite = overrideSprite ?? sprite;

            Vector4 outer, inner, padding, border;

            if (activeSprite != null)
            {
                outer = UnityEngine.Sprites.DataUtility.GetOuterUV(activeSprite);
                inner = UnityEngine.Sprites.DataUtility.GetInnerUV(activeSprite);
                padding = UnityEngine.Sprites.DataUtility.GetPadding(activeSprite);
                border = activeSprite.border;
            }
            else
            {
                outer = Vector4.zero;
                inner = Vector4.zero;
                padding = Vector4.zero;
                border = Vector4.zero;
            }
            Rect rect = GetPixelAdjustedRect();
            Vector4 adjustedBorders = GetAdjustedBorders(border / pixelsPerUnit, rect);
            padding = padding / pixelsPerUnit;

            m_VertScratch[0] = new Vector2(padding.x, padding.y);
            m_VertScratch[3] = new Vector2(rect.width - padding.z, rect.height - padding.w);

            m_VertScratch[1].x = adjustedBorders.x;
            m_VertScratch[1].y = adjustedBorders.y;

            m_VertScratch[2].x = rect.width - adjustedBorders.z;
            m_VertScratch[2].y = rect.height - adjustedBorders.w;

            for (int i = 0; i < 4; ++i)
            {
                m_VertScratch[i].x += rect.x;
                m_VertScratch[i].y += rect.y;
            }

            m_UVScratch[0] = new Vector2(outer.x, outer.y);
            m_UVScratch[1] = new Vector2(inner.x, inner.y);
            m_UVScratch[2] = new Vector2(inner.z, inner.w);
            m_UVScratch[3] = new Vector2(outer.z, outer.w);

            float xLength = m_VertScratch[3].x - m_VertScratch[0].x;
            float yLength = m_VertScratch[3].y - m_VertScratch[0].y;
            float len1XRatio = (m_VertScratch[1].x - m_VertScratch[0].x) / xLength;
            float len1YRatio = (m_VertScratch[1].y - m_VertScratch[0].y) / yLength;
            float len2XRatio = (m_VertScratch[2].x - m_VertScratch[1].x) / xLength;
            float len2YRatio = (m_VertScratch[2].y - m_VertScratch[1].y) / yLength;
            float len3XRatio = (m_VertScratch[3].x - m_VertScratch[2].x) / xLength;
            float len3YRatio = (m_VertScratch[3].y - m_VertScratch[2].y) / yLength;
            int xLen = 3, yLen = 3;
            if (fillMethod == FillMethod.Horizontal)
            {
                if (fillAmount >= (len1XRatio + len2XRatio))
                {
                    float ratio = 1 - (fillAmount - (len1XRatio + len2XRatio)) / len3XRatio;
                    m_VertScratch[3].x = m_VertScratch[3].x - (m_VertScratch[3].x - m_VertScratch[2].x) * ratio;
                    m_UVScratch[3].x = m_UVScratch[3].x - (m_UVScratch[3].x - m_UVScratch[2].x) * ratio;
                }
                else if (fillAmount >= len1XRatio)
                {
                    xLen = 2;
                    float ratio = 1 - (fillAmount - len1XRatio) / len2XRatio;
                    m_VertScratch[2].x = m_VertScratch[2].x - (m_VertScratch[2].x - m_VertScratch[1].x) * ratio;
                }
                else
                {
                    xLen = 1;
                    float ratio = 1 - fillAmount / len1XRatio;
                    m_VertScratch[1].x = m_VertScratch[1].x - (m_VertScratch[1].x - m_VertScratch[0].x) * ratio;
                    m_UVScratch[1].x = m_UVScratch[1].x - (m_UVScratch[1].x - m_UVScratch[0].x) * ratio;
                }
            }
            else if (fillMethod == FillMethod.Vertical)
            {
                if (fillAmount >= (len1YRatio + len2YRatio))
                {
                    float ratio = 1 - (fillAmount - (len1YRatio + len2YRatio)) / len3YRatio;
                    m_VertScratch[3].y = m_VertScratch[3].y - (m_VertScratch[3].y - m_VertScratch[2].y) * ratio;
                    m_UVScratch[3].y = m_UVScratch[3].y - (m_UVScratch[3].y - m_UVScratch[2].y) * ratio;
                }
                else if (fillAmount >= len1YRatio)
                {
                    yLen = 2;
                    float ratio = 1 - (fillAmount - len1YRatio) / len2YRatio;
                    m_VertScratch[2].y = m_VertScratch[2].y - (m_VertScratch[2].y - m_VertScratch[1].y) * ratio;
                }
                else
                {
                    yLen = 1;
                    float ratio = 1 - fillAmount / len1YRatio;
                    m_VertScratch[1].y = m_VertScratch[1].y - (m_VertScratch[1].y - m_VertScratch[0].y) * ratio;
                    m_UVScratch[1].y = m_UVScratch[1].y - (m_UVScratch[1].y - m_UVScratch[0].y) * ratio;
                }
            }

            toFill.Clear();

            for (int x = 0; x < xLen; ++x)
            {
                int x2 = x + 1;

                for (int y = 0; y < yLen; ++y)
                {
                    if (!fillCenter && x == 1 && y == 1)
                        continue;

                    int y2 = y + 1;


                    AddQuad(toFill,
                        new Vector2(m_VertScratch[x].x, m_VertScratch[y].y),
                        new Vector2(m_VertScratch[x2].x, m_VertScratch[y2].y),
                        color,
                        new Vector2(m_UVScratch[x].x, m_UVScratch[y].y),
                        new Vector2(m_UVScratch[x2].x, m_UVScratch[y2].y));
                }
            }
        }

        static void AddQuad(VertexHelper vertexHelper, Vector2 posMin, Vector2 posMax, Color32 color, Vector2 uvMin, Vector2 uvMax)
        {
            int startIndex = vertexHelper.currentVertCount;

            vertexHelper.AddVert(new Vector3(posMin.x, posMin.y, 0), color, new Vector2(uvMin.x, uvMin.y));
            vertexHelper.AddVert(new Vector3(posMin.x, posMax.y, 0), color, new Vector2(uvMin.x, uvMax.y));
            vertexHelper.AddVert(new Vector3(posMax.x, posMax.y, 0), color, new Vector2(uvMax.x, uvMax.y));
            vertexHelper.AddVert(new Vector3(posMax.x, posMin.y, 0), color, new Vector2(uvMax.x, uvMin.y));

            vertexHelper.AddTriangle(startIndex, startIndex + 1, startIndex + 2);
            vertexHelper.AddTriangle(startIndex + 2, startIndex + 3, startIndex);
        }

        private Vector4 GetAdjustedBorders(Vector4 border, Rect adjustedRect)
        {
            Rect originalRect = rectTransform.rect;

            for (int axis = 0; axis <= 1; axis++)
            {
                float borderScaleRatio;

                if (originalRect.size[axis] != 0)
                {
                    borderScaleRatio = adjustedRect.size[axis] / originalRect.size[axis];
                    border[axis] *= borderScaleRatio;
                    border[axis + 2] *= borderScaleRatio;
                }

                float combinedBorders = border[axis] + border[axis + 2];
                if (adjustedRect.size[axis] < combinedBorders && combinedBorders != 0)
                {
                    borderScaleRatio = adjustedRect.size[axis] / combinedBorders;
                    border[axis] *= borderScaleRatio;
                    border[axis + 2] *= borderScaleRatio;
                }
            }
            return border;
        }
        #endregion

        #region 圆角

        private List<Vector2> positionList = new List<Vector2>();
        private List<Vector2> uvList = new List<Vector2>();
        private List<int> vertexList = new List<int>();

        private void GenerateRoundedSprite(VertexHelper vh)
        {
            
            var activeSprite = overrideSprite ?? sprite;

            Rect rect = rectTransform.rect;
            float left = rect.xMin;
            float right = rect.xMax;
            float top = rect.yMax;
            float bottom = rect.yMin;
            
            Vector4 outerUV = activeSprite ? DataUtility.GetOuterUV(activeSprite) : Vector4.zero;
            var color32 = color;
            float r = m_RoundedRadius * Mathf.Min(rect.width, rect.height) / 2;

            // 计算uv中对应的半径值坐标轴的半径
            float uvRadiusX = r / (right - left) * (outerUV.z - outerUV.x);
            float uvRadiusY = r / (top - bottom) * (outerUV.w - outerUV.y);

            vh.Clear();
            positionList.Clear();
            uvList.Clear();
            vertexList.Clear();
            #region base vert
            /*
             *  vertex
             *  
             *  0--4----------8--12
             *  |  |          |  |
             *  1--5          9--13
             *  |  |          |  |
             *  |  |          |  |
             *  2--6         10--14
             *  |  |          |  |
             *  3--7---------11--15
             */
            // 从左往右
            // 0 1 2 3
            vh.AddVert(new Vector3(left, top), color32, new Vector2(outerUV.x, outerUV.w));
            vh.AddVert(new Vector3(left, top - r), color32, new Vector2(outerUV.x, outerUV.w - uvRadiusY));
            vh.AddVert(new Vector3(left, bottom + r), color32, new Vector2(outerUV.x, outerUV.y + uvRadiusY));
            vh.AddVert(new Vector3(left, bottom), color32, new Vector2(outerUV.x, outerUV.y));

            // 4 5 6 7
            vh.AddVert(new Vector3(left + r, top), color32, new Vector2(outerUV.x + uvRadiusX, outerUV.w));
            vh.AddVert(new Vector3(left + r, top - r), color32,
                new Vector2(outerUV.x + uvRadiusX, outerUV.w - uvRadiusY));
            vh.AddVert(new Vector3(left + r, bottom + r), color32,
                new Vector2(outerUV.x + uvRadiusX, outerUV.y + uvRadiusY));
            vh.AddVert(new Vector3(left + r, bottom), color32, new Vector2(outerUV.x + uvRadiusX, outerUV.y));

            // 8 9 10 11
            vh.AddVert(new Vector3(right - r, top), color32, new Vector2(outerUV.z - uvRadiusX, outerUV.w));
            vh.AddVert(new Vector3(right - r, top - r), color32,
                new Vector2(outerUV.z - uvRadiusX, outerUV.w - uvRadiusY));
            vh.AddVert(new Vector3(right - r, bottom + r), color32,
                new Vector2(outerUV.z - uvRadiusX, outerUV.y + uvRadiusY));
            vh.AddVert(new Vector3(right - r, bottom), color32, new Vector2(outerUV.z - uvRadiusX, outerUV.y));

            // 12 13 14 15
            vh.AddVert(new Vector3(right, top), color32, new Vector2(outerUV.z, outerUV.w));
            vh.AddVert(new Vector3(right, top - r), color32, new Vector2(outerUV.z, outerUV.w - uvRadiusY));
            vh.AddVert(new Vector3(right, bottom + r), color32, new Vector2(outerUV.z, outerUV.y + uvRadiusY));
            vh.AddVert(new Vector3(right, bottom), color32, new Vector2(outerUV.z, outerUV.y));
            #endregion

            // 左边矩形
            vh.AddTriangle(2, 1, 5);
            vh.AddTriangle(2, 5, 6);
            // 中间矩形
            vh.AddTriangle(7, 4, 8);
            vh.AddTriangle(7, 8, 11);
            // 右边矩形
            vh.AddTriangle(10, 9, 13);
            vh.AddTriangle(10, 13, 14);

            // 右上角圆心
            positionList.Add(new Vector2(right - r, top - r));
            uvList.Add(new Vector2(outerUV.z - uvRadiusX, outerUV.w - uvRadiusY));
            vertexList.Add(9);

            // 左上角圆心
            positionList.Add(new Vector2(left + r, top - r));
            uvList.Add(new Vector2(outerUV.x + uvRadiusX, outerUV.w - uvRadiusY));
            vertexList.Add(5);

            // 左下角圆心
            positionList.Add(new Vector2(left + r, bottom + r));
            uvList.Add(new Vector2(outerUV.x + uvRadiusX, outerUV.y + uvRadiusY));
            vertexList.Add(6);

            // 右下角圆心
            positionList.Add(new Vector2(right - r, bottom + r));
            uvList.Add(new Vector2(outerUV.z - uvRadiusX, outerUV.y + uvRadiusY));
            vertexList.Add(10);

            // 每个三角形角度
            float degreeDelta = Mathf.PI / 2 / m_RoundedTriangleNum;

            // 当前角度
            float degree = 0;
            for (int i = 0; i < vertexList.Count; i++)
            {
                int currVertCount = vh.currentVertCount;
                for (int j = 0; j <= m_RoundedTriangleNum; j++)
                {
                    float cos = Mathf.Cos(degree);
                    float sin = Mathf.Sin(degree);
                    Vector3 position = new Vector3(positionList[i].x + cos * r, positionList[i].y + sin * r);
                    Vector3 uv0 = new Vector2(uvList[i].x + cos * uvRadiusX,
                        uvList[i].y + sin * uvRadiusY);
                    vh.AddVert(position, color32, uv0);
                    degree += degreeDelta;
                }

                degree -= degreeDelta;

                if (i == 0 && !m_RoundedRightTop)
                {
                    vh.AddTriangle(vertexList[i], 8, 12);
                    vh.AddTriangle(vertexList[i], 12, 13);
                    continue;
                }

                if (i == 1 && !m_RoundedLeftTop)
                {
                    vh.AddTriangle(vertexList[i], 0, 4);
                    vh.AddTriangle(vertexList[i], 0, 1);
                    continue;
                }

                if (i == 2 && !m_RoundedLeftBottom)
                {
                    vh.AddTriangle(vertexList[i], 3, 2);
                    vh.AddTriangle(vertexList[i], 3, 7);
                    continue;
                }

                if (i == 3 && !m_RoundedRightBottom)
                {
                    vh.AddTriangle(vertexList[i], 15, 14);
                    vh.AddTriangle(vertexList[i], 15, 11);
                    continue;
                }

                for (int j = 0; j <= m_RoundedTriangleNum - 1; j++)
                {
                    vh.AddTriangle(vertexList[i], currVertCount + j + 1, currVertCount + j);
                }
            }
            //Debug.Log("positionList:" + positionList.Count);
            //Debug.Log("uvList:" + positionList.Count);
            //Debug.Log("vertexList:" + positionList.Count);
        }

        #endregion
    }
}
