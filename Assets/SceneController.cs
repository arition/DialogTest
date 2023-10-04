using System.Linq;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

public class SceneController : MonoBehaviour
{
    public Button StartButton;
    public GameObject StoryRoot;

    public void ShowStoryRoot()
    {
        StoryRoot.SetActive(true);
        StartButton.gameObject.SetActive(false);
    }

    public void HideStoryRoot()
    {
        HideStoryRootAsync().Forget();
    }

    public async UniTaskVoid HideStoryRootAsync()
    {
        await UniTask.WhenAll(
            GetComponentsInChildren<IntroAlpha>().Select(async t => await t.PlayOutros())
                .Concat(GetComponentsInChildren<AutoPosition>().Select(async t => await t.PlayOutros())
                ));

        StoryRoot.SetActive(false);
        StartButton.gameObject.SetActive(true);
    }
}