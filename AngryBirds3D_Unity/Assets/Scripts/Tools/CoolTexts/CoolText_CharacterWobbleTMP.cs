using TMPro;
using UnityEngine;

public class CoolText_CharacterWobbleTMP : CoolText_Base
{
    protected override void AnimationLogic()
    {
        for (int i = 0; i < _textToAnimate.textInfo.characterCount; i++)
        {
            TMP_CharacterInfo charInfo = _textToAnimate.textInfo.characterInfo[i];
            int index = charInfo.vertexIndex;

            Vector3 offset = AnimationCurve(Time.time + i);
            _textVertices[index] += offset;
            _textVertices[index + 1] += offset;
            _textVertices[index + 2] += offset;
            _textVertices[index + 3] += offset;
        }
    }
}