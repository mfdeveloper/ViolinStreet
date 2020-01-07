using System;
using UnityEngine;

public class PlatformRhythm : MonoBehaviour
{
    [NonSerialized]
    public Animator animatorController;

    void Awake()
    {
        animatorController = GetComponent<Animator>();
    }

    void OnTriggerStay2D(Collider2D collision)
    {
        Player player = collision.GetComponent<Player>();
        if (player != null)
        {
            player.PlatformJump = this;
        }
    }
}
