using System;
using UnityEngine;

public class SpinController : MonoBehaviour
{
    public class SpinEventArgs : EventArgs
    {
        public SpinController.SpinState SpinState;

        public SpinEventArgs(SpinController.SpinState state)
        {
            SpinState = state;
        }
    }

    public enum SpinState
    {
        Waiting,
        Starting,
        Accelerating,
        Playing,
        Ending,
        Decelerating
    }

    #region Private Fields
    private const float SpinBuffer = 180;

    private float targetTime = 0;

    private float currentAngle = 0;

    private float currentSpeed = 0;

    private float configuredSpeed = 0;

    private float startAngle = 0;

    private float stopAngle = 0;

    private float endAngle = 0;

    private float startTime = 0;

    private float prevTime = 0;

    private SpinState state = SpinState.Waiting;
    #endregion

    #region Inspector Variables
    public float ChangeSlope = 45;

    [SerializeField]
    private float targetSpeed = 180;

    [SerializeField]
    private float accelTime = 1;

    [SerializeField]
    private float decelTime = 1;
    #endregion

    #region Properties
    public float CurrentSpeed
    {
        get { return currentSpeed; }
    }

    public SpinState CurrentState
    {
        get { return state; }
    }
    #endregion

    #region Accel/Decel Methods
    private float SinusoidalAccelSpeed(float now)
    {
        float currentDPS = currentSpeed;
        float elapsedTime = now - startTime;
        float target = targetSpeed / 2;

        if(state == SpinState.Accelerating)
        {
            currentDPS = -1 * target * Mathf.Cos((Mathf.PI / accelTime) * elapsedTime) + target;
        }
        else if(state == SpinState.Decelerating)
        {
            currentDPS = target * Mathf.Cos((Mathf.PI / decelTime) * elapsedTime) + target;
        }

        return currentDPS;
    }

    private float SinusoidalAccelPosition(float now)
    {
        float elapsedTime = now - startTime;
        float target = targetSpeed / 2;
        float currentPosition = currentAngle;

        if(state == SpinState.Accelerating)
        {
            currentPosition = startAngle + (target * elapsedTime);
            currentPosition += ((accelTime * target * -1) / Mathf.PI) *
                Mathf.Sin((Mathf.PI / accelTime) * elapsedTime);
        }
        else if(state == SpinState.Decelerating)
        {
            currentPosition = stopAngle + (target * elapsedTime);
            currentPosition += ((decelTime * target) / Mathf.PI) * Mathf.Sin((Mathf.PI / decelTime) * elapsedTime);
        }

        currentPosition = Limit(currentPosition, 360);

        return currentPosition;
    }

    private float SinusoidalAccelSlope(float duration)
    {
        return (duration / 2);
    }
    #endregion

    #region Event Handlers
    public event EventHandler<SpinEventArgs> OnAccelerationStart;

    public event EventHandler<SpinEventArgs> OnAccelerationEnd;

    public event EventHandler<SpinEventArgs> OnDecelerationStart;

    public event EventHandler<SpinEventArgs> OnNaturalSpinStop;
    #endregion

    #region Public Methods
    public void StartSpin()
    {
        if(state == SpinState.Waiting)
        {
            targetSpeed = configuredSpeed;
            targetTime = 0;
            state = SpinState.Starting;
        }
    }

    public void StopSpin(float angle)
    {
        if(state == SpinState.Playing || state == SpinState.Accelerating)
        {
            targetSpeed = currentSpeed;

            float stopLength = CalculateStopLength();
            float stopBuffer = 360 * Mathf.Ceil(Mathf.Abs(stopLength / 360));

            endAngle = angle % 360;
            stopAngle = ((endAngle + stopBuffer) - stopLength) % 360;

            state = SpinState.Ending;
        }
    }

    public void HaltSpin(float angle)
    {
        currentSpeed = 0;
        targetTime = 0;

        state = SpinState.Waiting;

        currentAngle = angle;
        endAngle = angle;
        SetPosition(angle);
    }
    #endregion

    #region Helper Methods
    protected float Limit(float left, float right)
    {
        return (left % right + right) % right;
    }

    protected float CalculateStopLength()
    {
        float stopLength = CalculateStopSlope() * targetSpeed;
        return stopLength;
    }

    protected float CalculateStopSlope()
    {
        float stopSlope = SinusoidalAccelSlope(decelTime);
        return stopSlope;
    }

    protected bool PassedStopAngle(float oldAngle, float newAngle)
    {
        return PassedStopAngle(stopAngle, oldAngle, newAngle);
    }

    protected bool PassedStopAngle(float targetAngle, float oldAngle, float newAngle)
    {
        bool passedAngle = false;

        if(currentSpeed > 0)
        {
            if((oldAngle > newAngle && (newAngle >= targetAngle || oldAngle <= targetAngle)) ||
                (oldAngle <= targetAngle && newAngle >= targetAngle))
            {
                passedAngle = true;
            }
        }
        else
        {
            if((oldAngle < currentAngle && (currentAngle <= targetAngle || oldAngle >= targetAngle)) ||
                (oldAngle >= targetAngle && currentAngle <= targetAngle))
            {
                passedAngle = true;
            }
        }

        return passedAngle;
    }

    protected void ProcessNaturalSpinComplete()
    {
        currentSpeed = 0;

        targetTime = 0;

        var eventHandler = OnNaturalSpinStop;
        if(eventHandler != null)
        {
            SpinEventArgs eventArgs = new SpinEventArgs(state);
            eventHandler(this, eventArgs);
        }
    }
    #endregion

    protected void Start()
    {
        configuredSpeed = targetSpeed;
    }

    #region Set/Get Position Methods
    protected float GetPosition()
    {
        return transform.localEulerAngles.z;
    }

    protected void SetPosition(float angle)
    {
        Vector3 eulerAngles = transform.eulerAngles;
        transform.rotation = Quaternion.Euler(eulerAngles.x, eulerAngles.y, angle);
    }
    #endregion

    #region Update Methods
    protected void SetNextPosition(float now)
    {
        currentAngle += currentSpeed * (now - prevTime);
        currentAngle = Limit(currentAngle, 360);
        SetPosition(currentAngle);
    }

    protected void Update()
    {
        float now = Time.time;
        switch(state)
        {
            case SpinState.Starting:
            {
                if(targetSpeed == 0)
                {
                    state = SpinState.Waiting;
                }

                startTime = prevTime = now;
                startAngle = currentAngle = GetPosition();
                currentSpeed = 0;
                state = SpinState.Accelerating;

                var eventHandler = OnAccelerationStart;
                if(eventHandler != null)
                {
                    SpinEventArgs eventArgs = new SpinEventArgs(state);
                    eventHandler(this, eventArgs);
                }
                break;
            }
            case SpinState.Accelerating:
            {
                SetNextPosition(now);
                prevTime = now;

                currentSpeed = SinusoidalAccelSpeed(prevTime);

                if(prevTime - startTime >= accelTime)
                {
                    currentSpeed = targetSpeed;
                    state = SpinState.Playing;

                    var eventHandler = OnAccelerationEnd;
                    if(eventHandler != null)
                    {
                        SpinEventArgs eventArgs = new SpinEventArgs(state);
                        eventHandler(this, eventArgs);
                    }
                }
                break;
            }
            case SpinState.Playing:
            {
                SetNextPosition(now);

                float fpsAdjust = ChangeSlope * (now - prevTime);
                if(currentSpeed < targetSpeed)
                {
                    currentSpeed += fpsAdjust;
                    if(CurrentSpeed > targetSpeed)
                    {
                        currentSpeed = targetSpeed;
                    }
                }
                else if(currentSpeed > targetSpeed)
                {
                    currentSpeed -= fpsAdjust;
                    if(CurrentSpeed < targetSpeed)
                    {
                        currentSpeed = targetSpeed;
                    }
                }

                prevTime = now;
                break;
            }
            case SpinState.Ending:
            {
                float oldAngle = currentAngle;
                SetNextPosition(now);

                if(PassedStopAngle(oldAngle, currentAngle))
                {
                    startTime = prevTime;
                    state = SpinState.Decelerating;

                    var eventHandler = OnDecelerationStart;
                    if(eventHandler != null)
                    {
                        SpinEventArgs eventArgs = new SpinEventArgs(state);
                        eventHandler(this, eventArgs);
                    }
                }

                prevTime = now;
                break;
            }
            case SpinState.Decelerating:
            {
                if((now - startTime) >= decelTime)
                {
                    ProcessNaturalSpinComplete();

                    state = SpinState.Waiting;
                }
                else
                {
                    currentAngle = Limit(SinusoidalAccelPosition(now), 360);
                    SetPosition(currentAngle);
                    prevTime = now;
                }
                break;
            }
        }
    }
    #endregion
}