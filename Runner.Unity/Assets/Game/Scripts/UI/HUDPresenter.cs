using System.Collections.Generic;
using Runner.Game;
using UnityEngine;
using UnityEngine.UI;
using Zenject;
using static UnityEditor.Rendering.FilterWindow;
using static UnityEngine.Rendering.DebugUI;

namespace Runner.UI
{
    // Canvas
    public class HUDPresenter : MonoBehaviour
    {
        [SerializeField] private RectTransform indicatorPanel;
        [SerializeField] public RectTransform elementContainer;
        [SerializeField] private Camera playerCamera;
        [SerializeField] private Transform player;

        private HUDIndicator.Factory _indicatorFactory;
        private List<HUDIndicator> _indicators = new();

        public float indicatorRadius = 25f;
        public float indicatorHideDistance = 3f;
        public float indicatorOffscreenBorder = .075f;
        public float indicatorScaleRadius = 15f;
        public float indicatorMinScale = .8f;
        public float indicatorFadeRadius = 15f;
        public float indicatorMinFade = 0f;

        [Inject]
        private void Construct(HUDIndicator.Factory indicatorFactory)
        {
            _indicatorFactory = indicatorFactory;
        }

        public void CreateIndicator(EnemyModel model)
        {
            HUDIndicator indicator = _indicatorFactory.Create();
            _indicators.Add(indicator);
        }

        private void LateUpdate()
        {
            UpdateIndicators();
        }

        private void UpdateIndicators()
        {
            if (_indicators.Count <= 0) { return; }

            foreach (HUDIndicator indicator in _indicators)
            {
                if (indicator is null) { continue; }
                
                Vector3 worldPos = indicator.transform.position;
                Vector3 screenPos = playerCamera.WorldToScreenPoint(worldPos);
                float distance = Vector2.Distance(new Vector2(indicator.transform.position.x, indicator.transform.position.z),
                                                  new Vector2(player.position.x, player.position.z));

                UpdateIndicator(indicator, screenPos, distance);
            }
        }

        private void UpdateIndicator(HUDIndicator indicator, Vector3 screenPos, float distance)
        {
            if (distance > indicatorRadius)
            {
                indicator.gameObject.SetActive(false);
                if (indicator.IsInIndicatorRadius)
                {
                    indicator.IsInIndicatorRadius = false;
                }
            }
            else
            {
                // check if element is visible on screen
                bool isVisibleOnScreen = indicator.IsVisibleOnScreen(screenPos);
                if (isVisibleOnScreen is false)
                {
                    // flip if indicator is behind us
                    if (screenPos.z < 0f)
                    {
                        screenPos.x = Screen.width - screenPos.x;
                        screenPos.y = Screen.height - screenPos.y;
                    }

                    // calculate off-screen position/rotation
                    Vector3 screenCenter = new Vector3(Screen.width, Screen.height, 0f) / 2f;
                    screenPos -= screenCenter;

                    float angle = Mathf.Atan2(screenPos.y, screenPos.x);
                    angle -= 90f * Mathf.Deg2Rad;

                    float cos = Mathf.Cos(angle);
                    float sin = -Mathf.Sin(angle);
                    float cotangent = cos / sin;
                    float offset = Mathf.Min(screenCenter.x, screenCenter.y);
                    offset = Mathf.Lerp(0f, offset, indicatorOffscreenBorder);

                    Vector3 screenBounds = screenCenter - new Vector3(offset, offset, 0f);
                    float boundsY = (cos > 0f) ? screenBounds.y : -screenBounds.y;
                    screenPos = new Vector3(boundsY / cotangent, boundsY, 0f);

                    // when out of bounds, get point on appropriate side
                    // out => right
                    if (screenPos.x > screenBounds.x) { screenPos = new Vector3(screenBounds.x, screenBounds.x * cotangent, 0f); }
                    // out => left
                    else if (screenPos.x < -screenBounds.x) { screenPos = new Vector3(-screenBounds.x, -screenBounds.x * cotangent, 0f); }
                    screenPos += screenCenter;

                    // update indicator rotation
                    indicator.SetIndicatorOffscreenRotation(Quaternion.Euler(0f, 0f, angle * Mathf.Rad2Deg));

                    // show indicator distance?
                    indicator.ShowIndicatorDistance(isVisibleOnScreen, (int)distance);

                    // set indicator on/offscreen
                    indicator.SetIndicatorOnOffscreen(isVisibleOnScreen);

                    // update indicator values
                    indicator.SetIndicatorPosition(screenPos, elementContainer);
                    indicator.SetMarkerScale(distance, indicatorScaleRadius, indicatorMinScale, indicator.rectTransform);
                    indicator.SetMarkerFade(distance, indicatorFadeRadius, indicatorMinFade, indicator.canvasGroup);
                    indicator.gameObject.SetActive(distance > indicatorHideDistance);

                    if (indicator.IsInIndicatorRadius is false)
                    {
                        indicator.IsInIndicatorRadius = true;
                    }
                }
            }
        }
    }
}