using UnityEngine;

[ExecuteInEditMode]
public class AutoScaleChildrenSprite : MonoBehaviour
{
    public enum ScaleMode
    {
        FitScreen,
        FitWidth,
        FitHeight
    }

    public ScaleMode Mode = ScaleMode.FitScreen;

    // Start is called before the first frame update
    private void Start()
    {
        ScaleObject();
    }

    // Update is called once per frame
    private void Update()
    {
        ScaleObject();
    }

    // scale child object to fit screen and keep aspect ratio
    private void ScaleObject()
    {
        var sr = GetComponentInChildren<SpriteRenderer>();
        if (sr == null) return;

        transform.localScale = new Vector3(1, 1, 1);

        var width = sr.sprite.bounds.size.x;
        var height = sr.sprite.bounds.size.y;

        var worldScreenHeight = Camera.main.orthographicSize * 2.0f;
        var worldScreenWidth = worldScreenHeight / Screen.height * Screen.width;

        var widthScale = worldScreenWidth / width;
        var heightScale = worldScreenHeight / height;
        var scale = Mode switch
        {
            ScaleMode.FitWidth => widthScale,
            ScaleMode.FitHeight => heightScale,
            _ => Mathf.Max(widthScale, heightScale)
        };

        transform.localScale = new Vector3(scale, scale, 1);
    }
}