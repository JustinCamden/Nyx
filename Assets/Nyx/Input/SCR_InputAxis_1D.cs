using UnityEngine;

public class SCR_InputAxis_1D
{
    private float internalAxisValue = 0.0f;
    public float AxisValue
    {
        get
        {
            return internalAxisValue;
        }
    }

    private float internalScaledAxisValue = 0.0f;
    public float ScaledAxisValue
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

    private string axisName = "";
    private float deadZone = 0.0f;
    private float axisScale = 1.0f;

    public SCR_InputAxis_1D(string inAxisName, float inDeadZone)
    {
        axisName = inAxisName;
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
        float newAxisValue = Input.GetAxis(axisName);
        if (newAxisValue != internalAxisValue)
        {
            // Update axis value
            internalAxisValue = newAxisValue;
            float axisMagnitude = Mathf.Abs(internalAxisValue);

            // Check if the axis is currently pressed
            if (internalIsPressed)
            {
                // Check if the axis was just released
                if (axisMagnitude <= deadZone)
                {
                    // If so, set the state to just released
                    internalIsPressed = false;
                    internalWasReleased = true;
                    internalScaledAxisValue = 0.0f;
                }

                // Otherwise update the axis
                else
                {
                    internalScaledAxisValue = ((axisMagnitude - deadZone) / axisScale) * internalAxisValue > 0.0f ? 1.0f : -1.0f;
                }
            }

            // Otherwise the axis is currently released
            else
            {
                // Check if the axis was just pressed
                if (Mathf.Abs(newAxisValue) > deadZone)
                {
                    // If so update events and axis
                    internalIsPressed = true;
                    internalWasPressed = true;
                    internalScaledAxisValue = ((axisMagnitude - deadZone) / axisScale) * internalAxisValue > 0.0f ? 1.0f : -1.0f;
                }
            }
        }

        return;
    }
}

