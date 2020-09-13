using System;
using System.Collections;
using UnityEngine;

public static class FlashSprite
{
    private const float FlashTime = 0.1f;
    private static readonly WaitForSeconds WaitFlashTime = new WaitForSeconds(FlashTime);

    public static IEnumerator Flash(SpriteRenderer renderer, float cycles, Action completionCallBack)
    {
        for (var i = 0; i < cycles; i++)
        {
            renderer.enabled = !renderer.enabled;
            yield return WaitFlashTime;
        }
        renderer.enabled = true;
        completionCallBack();
    }
}
