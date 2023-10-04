using System.Collections;
using TMPro;
using UnityEngine;

[ExecuteInEditMode]
public class AutoPosition : MonoBehaviour
{
    public float IntroTime = 0.3f;

    [Range(0, 1)] public float PositionX = 0.5f;

    [Range(0, 1)] public float StartPositionY = 0f;

    [Range(0, 1)] public float EndPositionY = 1f;

    private bool _isAnimating = false;

    protected virtual void OnEnable()
    {
        if (Application.isPlaying) StartCoroutine(PlayIntros());
    }

    // Update is called once per frame
    protected virtual void Update()
    {
        if (!Application.isPlaying || !_isAnimating) transform.position = CalculatePosition().Item2;
    }

    public IEnumerator PlayIntros()
    {
        var (startPosition, targetPosition) = CalculatePosition();
        yield return UpdatePosition(startPosition, targetPosition);
    }

    public IEnumerator PlayOutros()
    {
        var (startPosition, targetPosition) = CalculatePosition();
        yield return UpdatePosition(targetPosition, startPosition);
    }

    protected virtual IEnumerator UpdatePosition(Vector3 startPosition, Vector3 targetPosition)
    {
        _isAnimating = true;
        for (var time = 0f; time < IntroTime; time += Time.deltaTime)
        {
            var t = TweenFunction(time / IntroTime);
            transform.position = Vector3.Lerp(startPosition, targetPosition, t);
            yield return null;
        }
        transform.position = targetPosition;
        _isAnimating = false;
    }

    protected virtual (Vector3, Vector3) CalculatePosition()
    {
        var worldScreenHeight = Camera.main.orthographicSize * 2.0f;
        var worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        var x = worldScreenWidth * (PositionX - 0.5f);
        var startY = worldScreenHeight * (StartPositionY - 0.5f);
        var endY = worldScreenHeight * (EndPositionY - 0.5f);

        var startPosition = new Vector3(x, startY);
        var targetPosition = new Vector3(x, endY);

        return (startPosition, targetPosition);
    }

    protected virtual float TweenFunction(float t)
    {
        return t * t * t * (t * (6f * t - 15f) + 10f);
    }
}