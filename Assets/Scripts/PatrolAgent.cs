using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatrolAgent
{
    private float minRange = 1.5f;
    private float maxRange = 20;
    private float maxVisionRange = 20;

    private State state = State.PATROL;

    public State CurrentState { get => this.state; }

    public enum State
    {
        PATROL,
        INVESTIGATE,
        CHASE,
        ATTACK
    }

    public PatrolAgent(float minRange, float maxRange, float maxVisionRange)
    {
        this.minRange = minRange;
        this.maxRange = maxRange;
        this.maxVisionRange = maxVisionRange;
    }

    public void OnChangeRange(float range)
    {
        if (state == State.CHASE || state == State.ATTACK)
        {
            if (range <= minRange)
            {
                state = State.ATTACK;
            }
            else if (range <= maxRange)
            {
               state = State.CHASE;
            }
            else
            {
               state = State.PATROL;
            }
        } 
    }

    public void OnChangeTargetVision(float distance)
    {
        if(state == State.PATROL || state == State.INVESTIGATE)
        {
            if (distance <= maxVisionRange)
                state = State.CHASE;
        }   
    }

    public void OnHearSound(float soundPower, float distanceFromSource)
    {
        if(state == State.PATROL)
        {
            if (distanceFromSource <= soundPower)
            {
                state = State.INVESTIGATE;
            }
        }
    }

    public void LostTargetChase()
    {
        if (state == State.CHASE)
        {
            state = State.INVESTIGATE;
        }
    }


    public void InvestigationOver()
    {
        state = State.PATROL;
    }
}
