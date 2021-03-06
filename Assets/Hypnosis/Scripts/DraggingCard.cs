﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;

public class DraggingCard : DragAndDropItem
{
    public CardType type = 0;
    protected Coroutine pulseCoroutine;

    public void Activate()
    {
        pulseCoroutine = StartCoroutine(Pulse(0.5f, 0.5f, 1.1f));
    }
    public void DeActivate()
    {
        StopCoroutine(pulseCoroutine);
    }

    private IEnumerator Pulse(float breakTime, float delay, float scaleFactor)
    {
        var baseScale = transform.localScale;
        while (true)
        {
            float growingTime = Time.time;
            while (growingTime + delay > Time.time)
            {
                transform.localScale = Vector3.Lerp(baseScale * scaleFactor, baseScale, (growingTime + delay) - Time.time);
                yield return 0;
            }

            float shrinkingTime = Time.time;
            while (shrinkingTime + delay > Time.time)
            {
                transform.localScale = Vector3.Lerp(baseScale, baseScale * scaleFactor, (shrinkingTime + delay) - Time.time);
                yield return 0;
            }

            yield return new WaitForSeconds(breakTime);
        }
    }
}

