using UnityEngine;

[ExecuteInEditMode]
public class AutoPositionChildrenSprite : AutoPosition
{
    protected override (Vector3, Vector3) CalculatePosition()
    {
        var sr = GetComponentInChildren<SpriteRenderer>();

        var height = sr.sprite.bounds.size.y * transform.localScale.y;

        var worldScreenHeight = Camera.main.orthographicSize * 2.0f;
        var worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        var x = worldScreenWidth * (PositionX - 0.5f);
        var startY = worldScreenHeight * (StartPositionY - 0.5f) - height / 2f;
        var endY = worldScreenHeight * (EndPositionY - 0.5f) - height / 2f;

        var startPosition = new Vector3(x, startY);
        var targetPosition = new Vector3(x, endY);

        return (startPosition, targetPosition);
    }
}