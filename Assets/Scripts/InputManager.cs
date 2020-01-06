using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//TODO: Merge this class with InputManager class created Death Hawai Ludum Dare Jam game
/// <summary>
/// Class to manage extra generic cross inputs (mobile/mouse) mapping, for example
/// </summary>
public static class InputManager
{
    public class TouchOrMouse
    {
        public bool IsScreendLeft()
        {
            Vector2 position = GetPosition();
            if (position != Vector2.zero && position.x < Screen.width / 2)
            {
                return true;
            }

            return false;
        }

        public bool IsScreenRight()
        {
            Vector2 position = GetPosition();

            if (position != Vector2.zero && position.x > Screen.width / 2)
            {
                return true;
            }

            return false;
        }

        public Vector2 GetPosition()
        {
            if (IsTouched)
            {
                return unityTouch.position;
            }
            else if (Input.GetMouseButtonDown(0))
            {
                return Input.mousePosition;
            }

            return Vector2.zero;
        }
    }

    public static TouchOrMouse touch = new TouchOrMouse();
    public static bool IsTouched {
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

    private static Touch unityTouch;
}
