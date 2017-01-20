using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class GenericUnit : Unit
{
    private Coroutine PulseCoroutine;
    private Coroutine RepeatGlowCoroutine;
    private Vector3 originalScale;

    
    public HealthBar healthBar;

    public override void Initialize()
    {
        base.Initialize();
        transform.position += new Vector3(0, 0, -0.1f);
        originalScale = transform.localScale;
        healthBar = transform.FindChild("HealthBar").GetComponent<HealthBar>();
        InitializeHealthBar(isFriendUnit);
    }

    public override void OnUnitDeselected()
    {
        base.OnUnitDeselected();
        StopCoroutine(PulseCoroutine);
        transform.localScale = originalScale;
    }

    public override void MarkAsAttacking(Unit other)
    {
        StartCoroutine(Jerk(other));
    }
    public override void MarkAsDefending(Unit other)
    {
        StartCoroutine(Glow(new Color(1, 0, 0, 0.5f), 1));
    }
    public override void MarkAsDestroyed()
    {
    }

    public override void MarkAsFriendly()
    {
        
    }
    public override void MarkAsReachableEnemy()
    {
        RepeatGlowCoroutine = StartCoroutine(RepeatGlow(new Color(1, 1, 1, 0.5f), 1, 0.5f));
    }
    public override void UnMarkAsReachableEnemy()
    {
        StopCoroutine(RepeatGlowCoroutine);
        transform.Find("Marker").GetComponent<SpriteRenderer>().color = new Color(1, 1, 1, 0);
    }
    public override void MarkAsSelected()
    {
        PulseCoroutine = StartCoroutine(Pulse(1.0f, 0.5f, 1.25f));
    }
    public override void MarkAsFinished()
    {
        
    }
    public override void UnMark()
    {
        SetColor(Color.white);
    }

    public override void MarkAsInvincible()
    {
        SetColor(Color.cyan);
    }

    public override void MarkAsFirstTargetLocked()
    {
        SetColor(Color.magenta);
    }

    private void SetColor(Color color)
    {
        var _renderer = GetComponent<SpriteRenderer>();
        if (_renderer != null)
        {
            _renderer.color = color;
        }
    }

    private IEnumerator Jerk(Unit other)
    {
        isMoving = true;
        GetComponent<SpriteRenderer>().sortingOrder = 6;
        var heading = other.transform.position - transform.position;
        var direction = heading / heading.magnitude;
        float startTime = Time.time;

        while (startTime + 0.25f > Time.time)
        {
            transform.position = Vector3.Lerp(transform.position, transform.position + (direction / 5f), ((startTime + 0.25f) - Time.time));
            yield return 0;
        }
        startTime = Time.time;
        while (startTime + 0.25f > Time.time)
        {
            transform.position = Vector3.Lerp(transform.position, transform.position - (direction / 5f), ((startTime + 0.25f) - Time.time));
            yield return 0;
        }
        transform.position = Cell.transform.position + new Vector3(0, 0, -0.1f);
        GetComponent<SpriteRenderer>().sortingOrder = 4;
        isMoving = false;
    }
    private IEnumerator Glow(Color color, float cooloutTime)
    {
        var _renderer = transform.Find("Marker").GetComponent<SpriteRenderer>();
        float startTime = Time.time;

        while (startTime + cooloutTime > Time.time)
        {
            _renderer.color = Color.Lerp(new Color(1, 1, 1, 0), color, (startTime + cooloutTime) - Time.time);
            yield return 0;
        }

        _renderer.color = Color.clear;
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

    private IEnumerator RepeatGlow(Color color, float time, float delayTime)
    {
        var renderer = transform.Find("Marker").GetComponent<SpriteRenderer>();

        while(true)
        {
            float growingTime = Time.time;
            while(Time.time < growingTime + delayTime)
            {
                renderer.color = Color.Lerp(new Color(1, 1, 1, 0), color, (growingTime + delayTime - Time.time)/delayTime);
                yield return 0;
            }

            float fadingTime = Time.time;
            while(Time.time < fadingTime + delayTime)
            {
                renderer.color = Color.Lerp(color, new Color(1, 1, 1, 0), (fadingTime + delayTime - Time.time)/delayTime);
                yield return 0;
            }
        }
    }

    public override void InitializeHealthBar(bool isLocalPlayer)
    {
        healthBar.SetHealth(MaxHP);
        Color color = isLocalPlayer ? Color.cyan : Color.red;
        healthBar.SetColor(color);
    }

    protected override void RefreshHealthBar()
    {
        healthBar.SetHealth(HP);
    }
}

