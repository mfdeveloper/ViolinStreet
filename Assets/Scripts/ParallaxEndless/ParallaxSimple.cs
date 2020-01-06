using System;
using UnityEngine;

public class ParallaxSimple : MonoBehaviour
{
    [SerializeField]
    private float speed = 0f;

    [SerializeField]
    private ParallaxElement[] elements = { };

    protected float originSpeed = 0f;

    // Update is called once per frame
    void Update()
    {
        MoveElements();
    }

    public virtual void MoveElements()
    {
        if (elements.Length > 0 && speed > 0)
        {
            foreach (var element in elements)
            {
                element.RestartWhenOut();
                element.Move(speed);
            }
        }
    }

    public virtual void Play()
    {
        if (originSpeed > 0f && originSpeed != speed)
        {
            speed = originSpeed;
        }
    }

    public virtual void Stop()
    {
        originSpeed = speed;
        speed = 0f;
    }
}
