using UnityEngine;

public class TouchOrMouse
{

    public struct TouchTypeFlags
    {
        public bool swipe;
        public bool tap;
        public bool right;
        public bool left;
        public bool up;
        public bool down;
    }
    public struct SwipeData
    {
        public float dragDistanceWidth;
        public float dragDistanceHeight;

        public Vector3 startPos; //First touch position
        public Vector3 endPos; //Last touch position
    }

    public SwipeData swipeData = new SwipeData();
    public TouchTypeFlags touchFlags = new TouchTypeFlags();

    // TODO: Add Mouse "swipe" support
    public TouchTypeFlags TapOrSwipe()
    {
        swipeData.dragDistanceHeight = Screen.height * 15 / 100;
        swipeData.dragDistanceWidth = Screen.width * 15 / 100;

        if (Input.touchCount == 1) // user is touching the screen with a single touch
        {
            Touch touch = Input.GetTouch(0); // get the touch
            if (touch.phase == TouchPhase.Began) //check for the first touch
            {
                swipeData.startPos = touch.position;
                swipeData.endPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Moved) // update the last position based on where they moved
            {
                swipeData.endPos = touch.position;
            }
            else if (touch.phase == TouchPhase.Ended) //check if the finger is removed from the screen
            {
                swipeData.endPos = touch.position;  //last touch position. Ommitted if you use list

                //Check if drag distance is greater than 20% of the screen height
                if (Mathf.Abs(swipeData.endPos.x - swipeData.startPos.x) > swipeData.dragDistanceWidth 
                    || Mathf.Abs(swipeData.endPos.y - swipeData.startPos.y) > swipeData.dragDistanceHeight)
                {
                //It's a drag
                 //check if the drag is vertical or horizontal
                    if (Mathf.Abs(swipeData.endPos.x - swipeData.startPos.x) > Mathf.Abs(swipeData.endPos.y - swipeData.startPos.y))
                    {   //If the horizontal movement is greater than the vertical movement...
                        if ((swipeData.endPos.x > swipeData.startPos.x))  //If the movement was to the right)
                        {   //Right swipe
                            Debug.Log("Right Swipe");

                            touchFlags.swipe = true;
                            touchFlags.right = true;

                            touchFlags.left = false;
                            touchFlags.up = false;
                            touchFlags.down = false;
                            touchFlags.tap = false;

                            return touchFlags;
                        }
                        else
                        {   //Left swipe
                            Debug.Log("Left Swipe");

                            touchFlags.swipe = true;
                            touchFlags.left = true;

                            touchFlags.right = true;
                            touchFlags.up = false;
                            touchFlags.down = false;
                            touchFlags.tap = false;

                            return touchFlags;
                        }
                    }
                    else
                    {   //the vertical movement is greater than the horizontal movement
                        if (swipeData.endPos.y > swipeData.startPos.y)  //If the movement was up
                        {   //Up swipe
                            Debug.Log("Up Swipe");

                            touchFlags.swipe = true;
                            touchFlags.up = true;

                            touchFlags.left = false;
                            touchFlags.right = false;
                            touchFlags.down = false;
                            touchFlags.tap = false;

                            return touchFlags;
                        }
                        else
                        {   //Down swipe
                            Debug.Log("Down Swipe");

                            touchFlags.swipe = true;
                            touchFlags.down = true;

                            touchFlags.up = false;
                            touchFlags.left = false;
                            touchFlags.right = false;                         
                            touchFlags.tap = false;

                            return touchFlags;
                        }
                    }
                }
                else
                {   //It's a tap as the drag distance is less than 20% of the screen height
                    Debug.Log("Tap");

                    touchFlags.tap = true;

                    touchFlags.swipe = false;
                    touchFlags.down = false;
                    touchFlags.up = false;
                    touchFlags.left = false;
                    touchFlags.right = false;

                    return touchFlags;
                }
            }
        }

        return touchFlags;

    }

    public bool IsScreenLeft(Vector2 position)
    {

        //Vector2 position = GetTapPosition();
        if (position != Vector2.zero && position.x < Screen.width / 2)
        {
            return true;
        }

        return false;
    }

    public bool IsScreenRight(Vector2 position)
    {
        //Vector2 position = GetTapPosition();

        if (position != Vector2.zero && position.x > Screen.width / 2)
        {
            return true;
        }

        return false;
    }

    // TODO: Remove this in a future
    public Vector2 GetTapPosition()
    {
        if (Input.touchCount == 1)
        {
            var result = TapOrSwipe();
            if (!result.swipe && result.tap)
            {
                return Input.GetTouch(0).position;
            }
        } else if (Input.touchCount == 0 && Input.GetMouseButtonDown(0))
        {
            return Input.mousePosition;
        }

        return Vector2.zero;
    }

    public Vector2 GetTapPosition(TouchTypeFlags touchFlags)
    {
        if (InputManager.IsTouchedOnce)
        {
            if (!touchFlags.swipe && touchFlags.tap)
            {
                return Input.GetTouch(0).position;
            }
        }
        else if (Input.touchCount == 0 && Input.GetMouseButtonDown(0))
        {
            return Input.mousePosition;
        }

        return Vector2.zero;
    }

    public virtual void StopSwipe()
    {
        touchFlags = new TouchTypeFlags();
        InputManager.touch.swipeData = new SwipeData();
    }
}
