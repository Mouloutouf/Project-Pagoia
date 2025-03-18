using System;
using System.Collections;
using UnityEngine;
using UnityEngine.UI;

[Serializable]
public class LoadingBar
{
    public float speed;
    public Slider slider;

    public delegate void AfterLoadAction_();

    public AfterLoadAction_ _afterLoadAction_;

    public IEnumerator Load()
    {
        float fill = 0.0f;
        for (var ft = 0f; ft < 1f; ft += Time.deltaTime * speed)
        {
            yield return new WaitForEndOfFrame();
            fill = ft;
            slider.value = fill;
        }
        _afterLoadAction_.Invoke();
    }
}
