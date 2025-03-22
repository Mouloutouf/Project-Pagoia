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
        for (var fill = 0f; fill < 1f; fill += Time.deltaTime * speed)
        {
            yield return new WaitForEndOfFrame();
            slider.value = fill;
        }
        _afterLoadAction_.Invoke();
    }
}
