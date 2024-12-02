using UnityEngine;

public class CoolText_VertexFloatTMP : CoolText_Base
{
    protected override void AnimationLogic()
    {
        for (int i = 0; i < _textVertices.Length; i++)
        {
            Vector3 offset = AnimationCurve(Time.time - Time.deltaTime * i);
            _textVertices[i] = _textVertices[i] + offset;
        }
    }
}