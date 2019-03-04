using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SCR_InputManager : MonoBehaviour
{

    private List<SCR_InputAxis_1D> OneDimensionalInputs;
    private List<SCR_InputAxis_2D> TwoDimensionalInputs;

    // Call on awake
    protected void InitializeLists()
    {
        OneDimensionalInputs = new List<SCR_InputAxis_1D>();
        TwoDimensionalInputs = new List<SCR_InputAxis_2D>();
    }

    // Call on update
    protected void UpdateInputAxises()
    {
        foreach (SCR_InputAxis_1D currInput in OneDimensionalInputs)
        {
            currInput.Update();
        }
        foreach (SCR_InputAxis_2D currInput in TwoDimensionalInputs)
        {
            currInput.Update();
        }

        return;
    }

    public SCR_InputAxis_1D BindInputAxis1D(string inAxisName, float inDeadZone)
    {
        SCR_InputAxis_1D retVal = new SCR_InputAxis_1D(inAxisName, inDeadZone);
        OneDimensionalInputs.Add(retVal);
        return retVal;
    }

    public SCR_InputAxis_2D BindInputAxis2D(string inAxisNameX, string inAxisNameY, float inDeadZone)
    {
        SCR_InputAxis_2D retVal = new SCR_InputAxis_2D(inAxisNameX, inAxisNameY, inDeadZone);
        TwoDimensionalInputs.Add(retVal);
        return retVal;
    }
}
