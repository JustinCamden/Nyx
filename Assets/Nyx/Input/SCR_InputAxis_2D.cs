using UnityEngine;

public class SCR_InputAxis_2D
{
    private Vector2 internalAxisValue;
    public Vector2 AxisValue
    {
        get
        {
            return internalAxisValue;
        }
    }

    private Vector2 internalScaledAxisValue;
    public Vector2 ScaledAxisValue
    {
        get
        {
            return internalScaledAxisValue;
        }
    }

    private bool internalIsPressed = false;
    public bool IsPressed
    {
        get
        {
            return internalIsPressed;
        }
    }

    private bool internalWasPressed = false;
    public bool WasPressed
    {
        get
        {
            return internalWasPressed;
        }
    }
    private bool internalWasReleased = false;
    public bool WasReleased
    {
        get
        {
            return internalWasReleased;
        }
    }

    private string axisNameX = "";
    private string axisNameY = "";
    private float deadZone = 0.0f;
    private float axisScale = 1.0f;

    public SCR_InputAxis_2D(string inAxisNameX, string inAxisNameY, float inDeadZone)
    {
        axisNameX = inAxisNameX;
        axisNameY = inAxisNameY;
        deadZone = Mathf.Clamp(inDeadZone, 0.0f, 0.9f);
        axisScale = 1.0f - deadZone;
    }

    public void Update()
    {
        // Updated pressed and released events
        if (internalWasPressed)
        {
            internalWasPressed = false;
        }
        else if (internalWasReleased)
        {
            internalWasReleased = false;
        }

        // Check if the axis changed value
        Vector2 newAxisValue = new Vector2(Input.GetAxisRaw(axisNameX), Input.GetAxisRaw(axisNameY));
        if (newAxisValue != internalAxisValue)
        {
            // Update axis value
            internalAxisValue = newAxisValue;
            float axisMagnitude = internalAxisValue.magnitude;

            // Check if the axis is currently pressed
            if (internalIsPressed)
            {
                // Check if the axis was just released
                if (axisMagnitude <= deadZone)
                {
                    // If so, set the state to just released
                    internalIsPressed = false;
                    internalWasReleased = true;
                    internalScaledAxisValue.x = 0.0f;
                    internalScaledAxisValue.y = 0.0f;
                }

                // Otherwise update the axis
                else
                {
                    Vector2 axisNormalized = internalAxisValue / axisMagnitude;
                    internalScaledAxisValue = internalAxisValue - (axisNormalized * deadZone);
                    internalScaledAxisValue = axisNormalized * (internalScaledAxisValue.magnitude) / axisScale;
                }
            }

            // Otherwise the axis is currently released
            else
            {
                // Check if the axis was just pressed
                if (axisMagnitude > deadZone)
                {
                    // If so update events and axis
                    internalIsPressed = true;
                    internalWasPressed = true;

                    Vector2 axisNormalized = internalAxisValue / axisMagnitude;
                    internalScaledAxisValue = internalAxisValue - (axisNormalized * deadZone);
                    internalScaledAxisValue = axisNormalized * (internalScaledAxisValue.magnitude) / axisScale;
                }
            }
        }

        return;
    }
}

