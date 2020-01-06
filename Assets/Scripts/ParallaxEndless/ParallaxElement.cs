using System;
using UnityEngine;

[Serializable]
public class ParallaxElement
{
    public GameObject element;
    public bool infinity = false;
    public float speed = 0f;
    public Vector2 OriginPos { get => originPos; set => originPos = value; }

    private Vector2 originPos = Vector2.zero;

    /// <summary>
    /// Restart to original position when out of right boundary
    /// of the main Camera.
    /// This is great to infinity backgrounds in endless
    /// runner games
    /// </summary>
    public virtual void RestartWhenOut()
    {
        if (OriginPos == Vector2.zero)
        {
            OriginPos = element.transform.position;
        }

        var pos = Camera.main.WorldToViewportPoint(element.transform.position);
        if (pos.x <= 1.0f && infinity)
        {
            element.transform.position = OriginPos;
        }
    }

    /// <summary>
    /// Move elements (backgrounds, grounds, clouds...)
    /// simulate parallax and endless runner
    /// </summary>
    /// <param name="extraSpeed"></param>
    public virtual void Move(float extraSpeed = 0f)
    {
        if (speed > 0)
        {
            Vector2 velocity = Vector2.left * speed;
            if (extraSpeed > 0)
            {
                velocity *= extraSpeed;
            }
            element.transform.Translate(velocity * Time.deltaTime);
        }
    }
}
