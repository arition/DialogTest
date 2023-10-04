using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.Events;

public class StoryController : MonoBehaviour
{
    private readonly int _maxIndex = 10;
    private int _index;
    public FeelingIndicator FeelingIndicator;
    public UnityEvent OnFinished;
    public TextBoxController TextBoxController;

    private void OnEnable()
    {
        _index = 0;
        TextBoxController.ClearText();
        TextBoxController.AddNewLine("Hello World!");
        FeelingIndicator.SetExp(0);
        UpdateAsync().Forget();
    }

    private async UniTaskVoid UpdateAsync()
    {
        while (true)
        {
            // add new text when clicked
            if (Input.GetMouseButtonDown(0))
            {
                if (_index < _maxIndex)
                {
                    var result = TextBoxController.AddNewLine("Hello World!" + _index, _index != _maxIndex - 1);
                    if (result) {
                        _index++;
                        FeelingIndicator.AddExp(50);
                    }
                }
                else
                {
                    OnFinished?.Invoke();
                    break;
                }
            }
            await UniTask.Yield();
        }
    }
}