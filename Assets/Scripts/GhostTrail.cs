using UnityEngine;
using DG.Tweening;

public class GhostTrail : MonoBehaviour
{
    private Movement Movement;
    private AnimationScript Animation;
    public Transform GhostParent;
    public Color TrailColor;
    public Color FadeColor;
    public float GhostInterval;
    public float FadeTime;

    private void Start()
    {
        Animation = FindObjectOfType<AnimationScript>();
        Movement = FindObjectOfType<Movement>();
    }

    public void ShowGhost()
    {
        Sequence s = DOTween.Sequence();

        for (int i = 0; i < GhostParent.childCount; i++)
        {
            Transform currentGhost = GhostParent.GetChild(i);
            s.AppendCallback(()=> currentGhost.position = Movement.transform.position);
            s.AppendCallback(() => currentGhost.GetComponent<SpriteRenderer>().flipX = Animation.SpriteRenderer.flipX);
            s.AppendCallback(()=>currentGhost.GetComponent<SpriteRenderer>().sprite = Animation.SpriteRenderer.sprite);
            s.Append(currentGhost.GetComponent<SpriteRenderer>().material.DOColor(TrailColor, 0));
            s.AppendCallback(() => FadeSprite(currentGhost));
            s.AppendInterval(GhostInterval);
        }
    }

    public void FadeSprite(Transform current)
    {
        current.GetComponent<SpriteRenderer>().material.DOKill();
        current.GetComponent<SpriteRenderer>().material.DOColor(FadeColor, FadeTime);
    }

}
