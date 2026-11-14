using TMPro;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.UI;

namespace UnityTemplate.Achievements
{
    /// <summary>Shared parts of an achievement row and an achievement popup: background, icon, title, description and reward texts.</summary>
    [RequireComponent(typeof(RawImage))]
    public abstract class AchievementItemBase : MonoBehaviour
    {
        [SerializeField] protected Image graphic;
        [SerializeField] protected TMP_Text title;
        [SerializeField] protected TMP_Text description;
        [SerializeField] protected TMP_Text reward;
        private RawImage background;
        private AsyncOperationHandle<Sprite> iconHandle;

        public RawImage Background => background != null ? background : background = GetComponent<RawImage>();

        /// <summary>Fills in the texts and loads the icon of an achievement.</summary>
        protected async void ShowTexts(AchievementInfo achievement)
        {
            title.text = achievement.Title;
            description.text = achievement.Description;
            reward.SetText("{0:0}", achievement.Reward);
            if (achievement.Icon == null || !achievement.Icon.RuntimeKeyIsValid()) return;

            ReleaseIcon();
            iconHandle = Addressables.LoadAssetAsync<Sprite>(achievement.Icon);
            var icon = await iconHandle.Task;
            if (this == null) return;
            graphic.sprite = icon;
            graphic.color = Color.white;
        }

        /// <summary>Releases the loaded icon when the row or toast is destroyed.</summary>
        protected virtual void OnDestroy()
        {
            ReleaseIcon();
        }

        /// <summary>Releases the previously loaded icon handle, if any.</summary>
        private void ReleaseIcon()
        {
            if (iconHandle.IsValid()) Addressables.Release(iconHandle);
        }
    }
}
