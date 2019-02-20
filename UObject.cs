// Developed By Roberto C. Tan Jr Under CreativeDev. Copyright (c) 2017 All Rights Reserved.
using System;
using System.Collections;
using System.Collections.Generic;
using Engine;
using UnityEngine;

/// <summary>
/// Is an abstract class where basic implementation happens.
/// </summary>
public abstract class UObject : UEngineCoreFrameWork
{
    #region Variables
    [Tooltip("Should we place tick on fix time?")]
    [SerializeField]
    protected bool bCanUseFixedTick = false;
    [Tooltip("Should this object use events?")]
    [SerializeField]
    protected bool bCanUseEvents = true;

    private FGateActionHandler[] engine_multiMethodPTR_internal;
    private int engine_initMultiGateCounter_internal = 0;
    private int engine_maxMultiGateCounter_internal = 0;
    private bool engine_onMultiGateDone_internal = true;
    private bool engine_isMultiGateLooping_internal = false;

    private FGateActionHandler engine_singleMethodPTR_internal;
    private bool engine_onSingleGateDone_internal = true;
    private bool engine_isSingleGateLooping_internal = false;
    #endregion

    #region Abstract Methods
    // Use this if you want your object to be called when this is enabled.
    private void OnEnable()
    {
        OnEnabled();
    }

    // Use this if you want your object to be called when this is disabled.
    private void OnDisable()
    {
        OnDisabled();
    }

    // Use this if you are trying to implement a behaviour that needs early access.
    private void Awake()
    {
        OnConstruct();   
    }

    // Use this for initialization.
    private void Start()
    {
        OnBeginPlay();
    }

    // Use this if you are trying to implement a behaviour that requires every frame update.
    private void Update()
    {
        if(!bCanUseFixedTick)
        {
            OnTick(Time.deltaTime);
        }
    }

    // Use this if you are trying to implement a behaviour that requires fix frame update such as physics.
    private void FixedUpdate()
    {
        if (bCanUseFixedTick)
        {
            OnTick(Time.fixedDeltaTime);
        }
    }

    private void LateUpdate()
    {
        SetOnLastTick(Time.deltaTime);
    }

    private void OnTriggerEnter(Collider otherActor)
    {
        if(bCanUseEvents)
        {
            CPrimitiveObject engine_ePO = new CPrimitiveObject();
            engine_ePO.Name = otherActor.gameObject.name;
            engine_ePO.eObject = otherActor.gameObject;
            engine_ePO.eCollider = otherActor;
            engine_ePO.eTag = otherActor.transform.tag;
            OnActorBeginOverlap(engine_ePO);
        }
    }

    private void OnTriggerEnter2D(Collider2D otherActor)
    {
        if (bCanUseEvents)
        {
            CPrimitiveObject engine_ePO = new CPrimitiveObject();
            engine_ePO.Name = otherActor.gameObject.name;
            engine_ePO.eObject = otherActor.gameObject;
            engine_ePO.eCollider2D = otherActor;
            engine_ePO.eTag = otherActor.transform.tag;
            OnActorBeginOverlap(engine_ePO);
        }
    }

    private void OnTriggerExit(Collider otherActor)
    {
        if (bCanUseEvents)
        {
            CPrimitiveObject engine_ePO = new CPrimitiveObject();
            engine_ePO.Name = otherActor.gameObject.name;
            engine_ePO.eObject = otherActor.gameObject;
            engine_ePO.eCollider = otherActor;
            engine_ePO.eTag = otherActor.transform.tag;
            OnActorEndOverlap(engine_ePO);
        }
    }

    private void OnTriggerExit2D(Collider2D otherActor)
    {
        if (bCanUseEvents)
        {
            CPrimitiveObject engine_ePO = new CPrimitiveObject();
            engine_ePO.Name = otherActor.gameObject.name;
            engine_ePO.eObject = otherActor.gameObject;
            engine_ePO.eCollider2D = otherActor;
            engine_ePO.eTag = otherActor.transform.tag;
            OnActorEndOverlap(engine_ePO);
        }
    }

    private void OnCollisionEnter(Collision otherActor)
    {
        if (bCanUseEvents)
        {
            CPrimitiveObject engine_ePO = new CPrimitiveObject();
            engine_ePO.Name = otherActor.gameObject.name;
            engine_ePO.eObject = otherActor.gameObject;
            engine_ePO.eCollision = otherActor;
            engine_ePO.eTag = otherActor.transform.tag;
            OnActorBeginOverlap(engine_ePO);
        }
    }

    private void OnCollisionEnter2D(Collision2D otherActor)
    {
        if (bCanUseEvents)
        {
            CPrimitiveObject engine_ePO = new CPrimitiveObject();
            engine_ePO.Name = otherActor.gameObject.name;
            engine_ePO.eObject = otherActor.gameObject;
            engine_ePO.eCollision2D = otherActor;
            engine_ePO.eTag = otherActor.transform.tag;
            OnActorBeginOverlap(engine_ePO);
        }
    }

    private void OnCollisionExit(Collision otherActor)
    {
        if (bCanUseEvents)
        {
            CPrimitiveObject engine_ePO = new CPrimitiveObject();
            engine_ePO.Name = otherActor.gameObject.name;
            engine_ePO.eObject = otherActor.gameObject;
            engine_ePO.eCollision = otherActor;
            engine_ePO.eTag = otherActor.transform.tag;
            OnActorEndOverlap(engine_ePO);
        }
    }

    private void OnCollisionExit2D(Collision2D otherActor)
    {
        if (bCanUseEvents)
        {
            CPrimitiveObject engine_ePO = new CPrimitiveObject();
            engine_ePO.Name = otherActor.gameObject.name;
            engine_ePO.eObject = otherActor.gameObject;
            engine_ePO.eCollision2D = otherActor;
            engine_ePO.eTag = otherActor.transform.tag;
            OnActorEndOverlap(engine_ePO);
        }
    }
    #endregion

    #region Overrideable Methods
    protected override void OnConstruct() { }
    protected override void OnBeginPlay() { }
    protected override void OnTick(float deltaTime) { }
    protected override void SetOnLastTick(float deltaTime) { }
    protected override void OnActorBeginOverlap(CPrimitiveObject otherActor) { }
    protected override void OnActorEndOverlap(CPrimitiveObject otherActor) { }
    protected override void OnEnabled() { }
    protected override void OnDisabled() { }
    #endregion

    #region UObject Custom Logic
    /// <summary>
    /// Returns the actual object.
    /// </summary>
    public GameObject GetObject()
    {
        return gameObject;
    }

    /// <summary>
    /// Returns the name of this object.
    /// </summary>
    public string GetName()
    {
        return gameObject.name;
    }

    /// <summary>
    /// Returns the instance id of this object.
    /// </summary>
    public int GetInstanceId()
    {
        return gameObject.GetInstanceID();
    }

    /// <summary>
    /// Execute multiple delegates in uninterruptible sequence on a different thread with an option to inifite loop if 'isLooping' parameter is set to true.
    /// </summary>
    /// <param name="pMethodPtr">Delegates where you can store all methods that will be executed in sequence.</param>
    /// <param name="step">Distance time between each call.</param>
    /// <param name="isLooping">Determines if it's infinite.</param>
    public void Sequence(FGateActionHandler[] pMethodPtr, float step, bool isLooping)
    {
        if (engine_onMultiGateDone_internal)
        {
            engine_isMultiGateLooping_internal = isLooping;
            engine_initMultiGateCounter_internal = 0;
            engine_maxMultiGateCounter_internal = pMethodPtr.Length;
            engine_multiMethodPTR_internal = new FGateActionHandler[engine_maxMultiGateCounter_internal];
            engine_multiMethodPTR_internal = pMethodPtr;
            InvokeRepeating("ExecSequence_internal", step, step);
            engine_onMultiGateDone_internal = false;
        }
    }

    private void ExecSequence_internal()
    {
        if (engine_initMultiGateCounter_internal < engine_maxMultiGateCounter_internal)
        {
            engine_multiMethodPTR_internal[engine_initMultiGateCounter_internal].Invoke();
            engine_initMultiGateCounter_internal++;
        }
        else
        {
            if (!engine_isMultiGateLooping_internal)
            {
                engine_onMultiGateDone_internal = true;
                CancelInvoke();
            }
            else
            {
                engine_onMultiGateDone_internal = false;
                engine_initMultiGateCounter_internal = 0;
            }
        }
    }

    /// <summary>
    /// Execute single delegate in uninterruptible sequence on a different thread with an option to inifite loop if 'isLooping' parameter is set to true.
    /// </summary>
    /// <param name="pMethodPtr">Delegate to execute in time.</param>
    /// <param name="step">Distance time between each call.</param>
    /// <param name="isLooping">Determines if it's infinite.</param>
    public void Gate(FGateActionHandler pMethodPtr, float step, bool isLooping)
    {
        if (engine_onSingleGateDone_internal)
        {
            engine_isSingleGateLooping_internal = isLooping;
            engine_singleMethodPTR_internal = pMethodPtr;
            InvokeRepeating("ExecGate_internal", step, step);
            engine_onSingleGateDone_internal = false;
        }
    }

    private void ExecGate_internal()
    {
        engine_singleMethodPTR_internal.Invoke();

        if (!engine_isSingleGateLooping_internal)
        {
            engine_onSingleGateDone_internal = true;
            CancelInvoke();
        }
        else
        {
            engine_onSingleGateDone_internal = false;
        }
    }

    /// <summary>
    /// Returns the delta time.
    /// </summary>
    protected float GetDeltaTime()
    {
        return Time.deltaTime;
    }

    /// <summary>
    /// Calls an IEnumerator method uses StartCoroutine.
    /// </summary>
    /// <param name="invokeMethodReceiver">IEnumerator method.</param>
    protected void InvokeMethodIn(IEnumerator invokeMethodReceiver)
    {
        StartCoroutine(invokeMethodReceiver);
    }

    /// <summary>
    /// Calls an IEnumerator method uses StartCoroutine.
    /// </summary>
    /// <param name="invokeMethodReceiver">IEnumerator method name.</param>
    /// <param name="step">Interval time.</param>
    protected void InvokeMethodIn(string invokeMethodReceiver, float step)
    {
        InvokeRepeating(invokeMethodReceiver, step, step);
    }

    /// <summary>
    /// Returns uobject class from another object in the scene. Warning this approach could be slow depending on how many objects are there in the scene.
    /// </summary>
    /// <param name="objectName">Object to find.</param>
    /// <returns></returns>
    protected UObject Cast(string objectName)
    {
        return GameObject.Find(objectName).GetComponent<UObject>();
    }

    /// <summary>
    /// Returns object class type from another object in the scene. Warning this approach could be slow depending on how many objects are there in the scene.
    /// </summary>
    /// <param name="objectName">Object to find.</param>
    /// <param name="objectClass">Class to find in component.</param>
    /// <returns></returns>
    protected object Cast(string objectName, string objectClass)
    {
        return GameObject.Find(objectName).GetComponent(objectClass);
    }

    /// <summary>
    /// Returns object class type from this object. It only looks for the component that is in the same object.
    /// </summary>
    /// <returns></returns>
    protected UObject SelfCast()
    {
        return gameObject.GetComponent<UObject>();
    }

    /// <summary>
    /// Writes message to the console.
    /// </summary>
    /// <param name="writeMessage">Debug message.</param>
    /// <param name="textColor">Text color if none default is green.</param>
    protected void Log(object writeMessage, string textColor = "green")
    {
        string localMessage = "<color=" + textColor + ">" + writeMessage + "</color>";
        Debug.Log(localMessage);
    }
    #endregion
}
