using System.Collections;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

[ExecuteInEditMode]
public class TextBoxController : MonoBehaviour
{
    public enum TextAnimationStatus
    {
        NotStarted,
        Writing,
        ShouldFinishNow
    }

    private bool _isResizing;
    private float _maxTextHeight;
    private TextAnimationStatus _textAnimationStatus = TextAnimationStatus.NotStarted;
    
    public Canvas ParentCanvas;
    public TMP_Text Text;
    public Image Background;
    public RectTransform Mask;
    public Component NextLineHint;
    
    public float PaddingX = 50f;
    public float PaddingY = 25f;
    [Range(0, 1)] public float Width = 0.8f;
    public int MaxLine = 3;

    public float HeightResizeTime = 0.2f;
    public float TextSpeed = 0.05f;
    

    // Start is called before the first frame update
    private void OnEnable()
    {
        _textAnimationStatus = TextAnimationStatus.NotStarted;
        if (Text != null && Mask != null && Background != null && ParentCanvas != null)
        {
            _maxTextHeight = CalculateHeightForMaxLine();
            WidthResize();
            if (!Application.isPlaying) HeightResize();
        }
    }

    // Update is called once per frame
    private void Update()
    {
        if (Text != null && Mask != null && Background != null && ParentCanvas != null)
        {
            WidthResize();
            if (!Application.isPlaying) HeightResize();
        }
    }

    private void WidthResize()
    {
        var parentCanvasScale = ParentCanvas.transform.localScale.x;

        var worldScreenHeight = Camera.main.orthographicSize * 2.0f;
        var worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        var textWidth = worldScreenWidth * Width * (1 / parentCanvasScale);
        var width = textWidth + PaddingX * 2;

        Text.rectTransform.sizeDelta = new Vector2(textWidth, Text.rectTransform.sizeDelta.y);
        Mask.sizeDelta = new Vector2(textWidth, Mask.sizeDelta.y);
        Background.rectTransform.sizeDelta = new Vector2(width, Background.rectTransform.sizeDelta.y);

        var nextLineHintTransform = NextLineHint.GetComponent<RectTransform>();
        if (nextLineHintTransform != null) nextLineHintTransform.localPosition = new Vector3(width / 2 - PaddingX, PaddingY);
    }

    private void HeightResize()
    {
        var textHeight = Text.preferredHeight;
        var maskHeight = Mathf.Min(Text.preferredHeight, _maxTextHeight);
        var height = maskHeight + PaddingY * 2;

        Text.rectTransform.sizeDelta = new Vector2(Text.rectTransform.sizeDelta.x, textHeight);
        Mask.sizeDelta = new Vector2(Mask.sizeDelta.x, maskHeight);
        Background.rectTransform.sizeDelta = new Vector2(Background.rectTransform.sizeDelta.x, height);
    }

    private IEnumerator HeightResizeCoroutine()
    {
        _isResizing = true;
        var originalTextHeight = Text.rectTransform.sizeDelta.y;
        var originalMaskHeight = Mask.sizeDelta.y;
        var originalBackgroundHeight = Background.rectTransform.sizeDelta.y;

        for (var time = 0f; time < HeightResizeTime; time += Time.deltaTime)
        {
            var textHeight = Text.preferredHeight;
            var maskHeight = Mathf.Min(Text.preferredHeight, _maxTextHeight);
            var height = maskHeight + PaddingY * 2;

            var t = TweenFunction(time / HeightResizeTime);
            Text.rectTransform.sizeDelta = new Vector2(Text.rectTransform.sizeDelta.x,
                Mathf.Lerp(originalTextHeight, textHeight, t));
            Mask.sizeDelta = new Vector2(Text.rectTransform.sizeDelta.x,
                Mathf.Lerp(originalMaskHeight, maskHeight, t));
            Background.rectTransform.sizeDelta = new Vector2(Background.rectTransform.sizeDelta.x,
                Mathf.Lerp(originalBackgroundHeight, height, t));
            yield return null;
        }

        HeightResize();
        _isResizing = false;
    }

    public void ClearText()
    {
        Text.text = "";
        HeightResize();
    }

    public bool AddNewLine(string text, bool showNextLineHint = true)
    {
        if (_textAnimationStatus != TextAnimationStatus.NotStarted)
        {
            _textAnimationStatus = TextAnimationStatus.ShouldFinishNow;
            return false;
        }
        StartCoroutine(TextAnimationCoroutine(text, showNextLineHint));
        if (!_isResizing) StartCoroutine(HeightResizeCoroutine());
        return true;
    }

    public IEnumerator TextAnimationCoroutine(string text, bool showNextLineHint)
    {
        _textAnimationStatus = TextAnimationStatus.Writing;
        NextLineHint.gameObject.SetActive(false);

        if (!string.IsNullOrEmpty(Text.text)) Text.text += "\n";
        var previousText = Text.text;

        var textLength = text.Length;
        var textIndex = 0;
        var time = 0f;
        while (textIndex < textLength)
        {
            if (_textAnimationStatus == TextAnimationStatus.ShouldFinishNow) break;
            if (time >= TextSpeed)
            {
                Text.text += text[textIndex];
                textIndex++;
                time = 0f;
            }
            else
            {
                time += Time.deltaTime;
            }

            yield return null;
        }

        Text.text = previousText + text;
        NextLineHint.gameObject.SetActive(showNextLineHint);
        _textAnimationStatus = TextAnimationStatus.NotStarted;
    }

    private float CalculateHeightForMaxLine()
    {
        var oldText = Text.text;
        var tempText = string.Join("\n", Enumerable.Repeat("A", MaxLine));
        Text.text = tempText;
        var textHeight = Text.preferredHeight;
        Text.text = oldText;
        return textHeight;
    }

    private float TweenFunction(float t)
    {
        return t * t * t * (t * (6f * t - 15f) + 10f);
    }
}