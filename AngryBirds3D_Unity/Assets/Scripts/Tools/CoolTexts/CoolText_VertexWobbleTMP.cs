using UnityEngine;

public class CoolText_VertexWobbleTMP : CoolText_Base
{
    protected override void AnimationLogic()
    {
        for (int i = 0; i < _textVertices.Length; i++)
        {
            Vector3 offset = AnimationCurve(Time.time + i);
            _textVertices[i] = _textVertices[i] + offset;
        }
    }
}