using System;
using System.Collections;
using Cysharp.Threading.Tasks;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.UI;

public class IntroAlpha : MonoBehaviour
{
    public float IntroTime = 0.3f;
    public GameObject[] Children;

    [Range(0, 1)]
    public float StartAlpha = 0f;
    [Range(0, 1)]
    public float EndAlpha = 1f;

    public void OnEnable()
    {
        PlayIntros().Forget();
    }

    public async UniTask PlayIntros()
    {
        await UniTask.WhenAll(Children.Select(async child => await UpdateAlpha(child, StartAlpha, EndAlpha)));
    }

    public async UniTask PlayOutros()
    {
        await UniTask.WhenAll(Children.Select(async child => await UpdateAlpha(child, EndAlpha, StartAlpha)));
    }

    private IEnumerator UpdateAlpha(GameObject child, float startAlpha, float endAlpha)
    {
        var updateAlphaAction= FindUpdateAlphaAction(child);
        if (updateAlphaAction == null) yield break;

        for (var time = 0f; time < IntroTime; time += Time.deltaTime)
        {
            var t = TweenFunction(time / IntroTime);
            updateAlphaAction(Mathf.Lerp(startAlpha, endAlpha, t));
            yield return null;
        }

        updateAlphaAction(endAlpha);
    }

    private Action<float> FindUpdateAlphaAction(GameObject child)
    {
        var sr = child.GetComponent<SpriteRenderer>();
        if (sr != null) return alpha => sr.color = sr.color.WithAlpha(alpha);

        var text = child.GetComponent<TMP_Text>();
        if (text != null) return alpha => text.color = text.color.WithAlpha(alpha);

        var image = child.GetComponent<Image>();
        if (image != null) return alpha => image.color = image.color.WithAlpha(alpha);

        return null;
    }

    private float TweenFunction(float t)
    {
        return t * t * t * (t * (6f * t - 15f) + 10f);
    }
}