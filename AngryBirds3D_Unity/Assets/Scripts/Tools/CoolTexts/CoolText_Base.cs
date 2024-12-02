using System.Collections;
using UnityEngine;
using TMPro;

public abstract class CoolText_Base : MonoBehaviour
{
    [SerializeField] protected TextMeshProUGUI _textToAnimate;
    [SerializeField] protected bool _isAnimating;
    protected Coroutine _animationRoutine = null;

    protected Mesh _textMesh;
    protected Vector3[] _textVertices;

    protected virtual void Start()
    {
        if (_isAnimating) AnimateCoolText();
    }

    protected virtual void AnimateCoolText()
    {
        if (_animationRoutine != null && _isAnimating) return;

        _isAnimating = true;
        _animationRoutine = StartCoroutine(AnimationRoutine());
    }
    protected virtual void StopAnimatingCoolText()
    {
        if (_animationRoutine == null && !_isAnimating) return;

        _isAnimating = false;
        StopCoroutine(_animationRoutine);
        _animationRoutine = null;
    }
    protected IEnumerator AnimationRoutine()
    {
        while (_isAnimating)
        {
            _textToAnimate.ForceMeshUpdate();
            _textMesh = _textToAnimate.mesh;
            _textVertices = _textMesh.vertices;

            AnimationLogic();

            _textMesh.vertices = _textVertices;
            _textToAnimate.canvasRenderer.SetMesh(_textMesh);
            yield return null;
        }
    }
    protected virtual Vector2 AnimationCurve(float time)
    {
        return new Vector2(Mathf.Sin(time * 3.3f), Mathf.Cos(time * 2.5f));
    }
    protected abstract void AnimationLogic();
}
