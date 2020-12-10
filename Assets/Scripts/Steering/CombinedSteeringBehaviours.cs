using System;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public sealed class BlendedSteering : SteeringBehaviour
{
    [Serializable]
    public struct WeightedBehaviour 
    {
        public readonly SteeringBehaviour behaviour;
        public readonly float weight;

        public WeightedBehaviour(SteeringBehaviour behaviour, float weight)
        {
            this.behaviour = behaviour;
            this.weight = weight;
        }
    }

    public BlendedSteering() { weightedBehaviours = new List<WeightedBehaviour>(); }
    public BlendedSteering(List<WeightedBehaviour> steeringBehaviours) { weightedBehaviours = steeringBehaviours; }

    public void AddBehaviour(WeightedBehaviour behaviour) { weightedBehaviours.Add(behaviour); }

    public override SteeringOutput CalculateSteering(float deltaTime, SteeringParameters parameters)
    {
        SteeringOutput steering = new SteeringOutput();
        float totalWeight = 0.0f;

        foreach (WeightedBehaviour weightedBehaviour in weightedBehaviours)
        {
            SteeringOutput newSteering = weightedBehaviour.behaviour.CalculateSteering(deltaTime, parameters);
            
            steering.linearVelocity += weightedBehaviour.weight * newSteering.linearVelocity;
            steering.angularVelocity += weightedBehaviour.weight * newSteering.angularVelocity;
            steering.succesful |= newSteering.succesful;

            totalWeight += weightedBehaviour.weight;
        }

        if (totalWeight != 0.0f)
        {
            steering.linearVelocity.Normalize();
            //steering.angularVelocity /= totalWeight;
        }

        return steering;
    }

    public override void Draw(SteeringParameters parameters)
    {
        foreach (WeightedBehaviour weightedBehaviour in weightedBehaviours)
        {
            weightedBehaviour.behaviour.Draw(parameters);
        }
    }

    List<WeightedBehaviour> weightedBehaviours;
}

[Serializable]
public sealed class PrioritySteering : SteeringBehaviour
{
    public PrioritySteering() { priorityBehaviours = new List<SteeringBehaviour>(); }
    public PrioritySteering(List<SteeringBehaviour> behaviours) { priorityBehaviours = behaviours; }

    public void AddBehaviour(SteeringBehaviour behaviour) { priorityBehaviours.Add(behaviour); }

    public override SteeringOutput CalculateSteering(float deltaTime, SteeringParameters parameters)
    {
        SteeringOutput steering = new SteeringOutput();

        foreach (SteeringBehaviour behaviour in priorityBehaviours)
        {
            steering = behaviour.CalculateSteering(deltaTime, parameters);

            if (steering.succesful)
                break;
        }
        return steering;
    }

    public override void Draw(SteeringParameters parameters)
    {
        foreach (SteeringBehaviour behaviour in priorityBehaviours)
        {
            behaviour.Draw(parameters);
        }
    }

    List<SteeringBehaviour> priorityBehaviours;
}