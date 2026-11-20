using DG.Tweening;
using UnityEngine;
using UnityEngine.Rendering;

namespace UnityTemplate.UserInterface
{
    /// <summary>Saves battery in the menus: renders every frame while anything moves or is touched, and only every few frames once the screen has been still for a moment.</summary>
    public class MenuRenderThrottle
    {
        private const int IdleRenderFrameInterval = 4;
        private const float StillSecondsBeforeThrottling = 2f;
        private float lastActivityTime;
        private Vector3 lastPointerPosition;

        /// <summary>Called every frame: renders at full rate in a stage or while there is input or a playing tween, otherwise throttles.</summary>
        public void Tick(bool inMenus)
        {
            if (!inMenus || IsAnythingHappening()) lastActivityTime = Time.unscaledTime;
            var still = Time.unscaledTime - lastActivityTime > StillSecondsBeforeThrottling;
            OnDemandRendering.renderFrameInterval = still ? IdleRenderFrameInterval : 1;
        }

        /// <summary>Returns rendering to every frame.</summary>
        public void Stop()
        {
            OnDemandRendering.renderFrameInterval = 1;
        }

        /// <summary>Returns true while a key, button or touch is down, the pointer or scroll wheel moves, or a tween is playing.</summary>
        private bool IsAnythingHappening()
        {
            var pointerMoved = Input.mousePosition != lastPointerPosition;
            lastPointerPosition = Input.mousePosition;
            return pointerMoved || Input.anyKey || Input.touchCount > 0 || Input.mouseScrollDelta != Vector2.zero || DOTween.TotalPlayingTweens() > 0;
        }
    }
}
