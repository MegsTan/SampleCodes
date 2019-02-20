// Developed By Roberto C. Tan Jr Under CreativeDev. Copyright (c) 2017 All Rights Reserved.
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Is an abstract class that only initializes the pawn specific methods nothing else. Is not part of hierarchy that can be used by developers.
/// This is internal only.
/// </summary>
public abstract class ObjectPawnInstigator : UObject
{
    #region Abstract Methods
    /// <summary>
    /// Is where we bind inputs from input source.
    /// </summary>
    /// <param name="inputComponent">You can bind inputs and axes via this class.</param>
    protected abstract void SetupPlayerInputComponent(UInputComponent inputComponent);

    /// <summary>
    /// Is a built-in X axis input.
    /// </summary>
    /// <param name="value">input value.</param>
    protected abstract void AddYawInput(float value);

    /// <summary>
    /// Is a built-in Y axis input.
    /// </summary>
    /// <param name="value">input value.</param>
    protected abstract void AddPitchInput(float value);
    #endregion
}
