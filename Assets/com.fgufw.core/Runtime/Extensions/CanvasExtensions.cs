using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

namespace FGUFW
{
    public static class CanvasExtensions
    {
        static private List<Canvas> panelCanvas = new List<Canvas>();

        public static void SetPanelSortOrder(this Canvas canvas)
        {
            panelCanvas.Add(canvas);
            resetPanelCanvasSortOrder();
        }

        public static void ClearSortOrder(this Canvas canvas)
        {
            panelCanvas.Remove(canvas);
            resetPanelCanvasSortOrder();
        }

        private static void resetPanelCanvasSortOrder()
        {
            for (int i = 0; i < panelCanvas.Count; i++)
            {
                panelCanvas[i].sortingOrder = i;
            }
        }

        public static List<RaycastResult> GetRaycastResults()
        {
            PointerEventData ped = new PointerEventData(EventSystem.current);
            ped.position = Input.mousePosition;
            List<RaycastResult> results = new List<RaycastResult>();
            EventSystem.current.RaycastAll(ped,results);
            return results;
        }

        /// <summary>
        /// 获取中心世界坐标
        /// </summary>
        /// <param name="self"></param>
        /// <returns></returns>
        public static Vector3 GetCenterPosition(this RectTransform self)
        {
            var size = self.sizeDelta;
            var pivot = self.pivot;
            var pos = self.position;
            var center = new Vector2(0.5f,0.5f);
            var offset = center-pivot;
            pos.x += size.x*pivot.x;
            pos.y += size.y*pivot.y;
            return pos;
        }
    }
}
