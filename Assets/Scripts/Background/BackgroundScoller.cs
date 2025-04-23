using UnityEngine;
using UnityEngine.UI;

public class BackgroundScoller : MonoBehaviour
{
    [SerializeField] private RawImage backgroundImage;
    [SerializeField] private float _x, _y;

    void Update()
    {
        backgroundImage.uvRect = new Rect(backgroundImage.uvRect.position + new Vector2(_x, _y) * Time.deltaTime, backgroundImage.uvRect.size);
    }

}
