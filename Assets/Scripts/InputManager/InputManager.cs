using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Merge this class with InputManager class created Death Hawai Ludum Dare Jam game
/// <summary>
/// Class to manage extra generic cross inputs (mobile/mouse) mapping, for example
/// </summary>
public static class InputManager
{

    public static TouchOrMouse touch = new TouchOrMouse();
    public static bool IsTouchedOnce {
        get
        {
            if (Input.touchCount == 1)
            {
                unityTouch = Input.GetTouch(0);

                if (unityTouch.phase == TouchPhase.Moved)
                {
                    return false;
                }

                if (unityTouch.phase == TouchPhase.Began)
                {
                    return true;
                }
            }

            return false;
        }
    }

    public static bool IsTouched
    {
        get
        {
            if (Input.touchCount > 0)
            {
                unityTouch = Input.GetTouch(0);

                return true;
            }

            return false;
        }
    }

    public static Touch unityTouch;
}
