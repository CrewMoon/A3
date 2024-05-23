using UnityEngine;

public class InputManager
{
    public HandPositionTracker handPositionTracker;
    private string previousAction = "No Action";


    public enum PhysicalGesture
    {
        FORWARD,
        BACK,
        LEFT,
        RIGHT,
        JUMP,
        SQUAT,
        IDLE
    }

    public enum GestureMeaning
    {
        FORWARD,
        BACK,
        LEFT,
        RIGHT,
        START,
        END,
        NONE
    }

    /// <returns>the move that has just been made this frame. Can be none.</returns>
    public GestureMeaning getCurrentMove()
    {
        if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            return GestureMeaning.FORWARD;
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow))
        {
            return GestureMeaning.BACK;
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            return GestureMeaning.LEFT;
        }
        else if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            return GestureMeaning.RIGHT;
        }
        else if (Input.GetKeyDown(KeyCode.Space))
        {
            return GestureMeaning.START;
        }
        else if (Input.GetKeyDown(KeyCode.LeftControl))
        {
            return GestureMeaning.END;
        }
        else
        {
            return GestureMeaning.NONE;
        }
    }

    /// <summary>
    /// obtains the current physical gesture that the user is making.
    /// </summary>
    /// <returns></returns>
    /// <exception cref="System.NotImplementedException"></exception>
    public GestureMeaning getCurrentGesture()
    {
        //throw new System.NotImplementedException();

        // string currentAction = handPositionTracker.GetAction();
        string currentAction = HandPositionTracker.Instance.GetAction();

        if (previousAction == "No Action" && currentAction != "No Action")
        {
            previousAction = currentAction;
            return MapActionToGesture(currentAction);
        }
        // Check if the action has changed back to "No Action" from something else
        else if (previousAction != "No Action" && currentAction == "No Action")
        {
            // Update previous action to "No Action"
            previousAction = currentAction;
            return GestureMeaning.NONE;
        }
        else
        {
            // If the action remains the same, return IDLE
            return GestureMeaning.NONE;
        }
    }

    private GestureMeaning MapActionToGesture(string action)
    {
        switch (action)
        {
            case "Forward":
                return GestureMeaning.FORWARD;
            case "Back":
                return GestureMeaning.BACK;
            case "Left":
                return GestureMeaning.LEFT;
            case "Right":
                return GestureMeaning.RIGHT;
            case "Jump":
                return GestureMeaning.START;
            case "Squat":
                return GestureMeaning.END;
            default:
                return GestureMeaning.NONE;
        }
    }
}