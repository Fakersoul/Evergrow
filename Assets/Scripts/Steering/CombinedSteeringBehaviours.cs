using System.Collections.Generic;

public sealed class BlendedSteering : SteeringBehaviour
{
    public struct WeightedBehaviour 
    {
        SteeringBehaviour behaviour;
        float weight;

        public WeightedBehaviour(SteeringBehaviour behaviour, float weight)
        {
            this.behaviour = behaviour;
            this.weight = weight;
        }
    }

    public BlendedSteering() { weightedBehaviours = new List<SteeringBehaviour>(); }
    public BlendedSteering(List<SteeringBehaviour> steeringBehaviours) { weightedBehaviours = steeringBehaviours; }

    public void AddBehaviour(SteeringBehaviour behaviour) { weightedBehaviours.Add(behaviour); }

    public override SteeringOutput CalculateSteering(float deltaTime, SteeringParameters parameters)
    {
        throw new System.NotImplementedException();
    }

    List<SteeringBehaviour> weightedBehaviours;
}

public sealed class PrioritySteering : SteeringBehaviour
{
    public PrioritySteering() { priorityBehaviours = new List<SteeringBehaviour>(); }
    public PrioritySteering(List<SteeringBehaviour> behaviours) { priorityBehaviours = behaviours; }

    public void AddBehaviour(SteeringBehaviour behaviour) { priorityBehaviours.Add(behaviour); }

    public override SteeringOutput CalculateSteering(float deltaTime, SteeringParameters parameters)
    {
        throw new System.NotImplementedException();
    }

    List<SteeringBehaviour> priorityBehaviours;
}