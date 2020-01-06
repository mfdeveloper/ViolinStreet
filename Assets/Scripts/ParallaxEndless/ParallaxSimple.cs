using System;
using UnityEngine;

public class ParallaxSimple : MonoBehaviour
{
    [SerializeField]
    private float speed = 0f;

    [SerializeField]
    private ParallaxElement[] elements = { };

    // Update is called once per frame
    void Update()
    {
        MoveElements();
    }

    public virtual void MoveElements()
    {
        if (elements.Length > 0)
        {
            foreach (var element in elements)
            {
                element.RestartWhenOut();
                element.Move(speed);
            }
        }
    }
}
