using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Unity.MLAgents;
using Unity.MLAgents.Actuators;
using Unity.MLAgents.Sensors;

public class AgentController : Agent
{
    [SerializeField] private Transform target;
    [SerializeField] private MeshRenderer floor;
    [SerializeField] private Material winMaterial;
    [SerializeField] private Material loseMaterial;

    public override void OnEpisodeBegin()
    {
        // To spawn the agent in a random location
        transform.localPosition = new Vector3(Random.Range(-4f, +1f), 0, Random.Range(-1.7f, +1.7f));
        // To spawn the target in a random location
        target.localPosition = new Vector3(Random.Range(+1.5f, +4.5f), 0, Random.Range(-1.7f, 1.7f));
    }

    public override void CollectObservations(VectorSensor sensor)
    {
        // This will allow the agent to know where it is in space
        sensor.AddObservation(transform.localPosition);
        // This will allow the agent to know where the target is in space
        sensor.AddObservation(target.localPosition);
    }
    public override void OnActionReceived(ActionBuffers actions)
    {
        if (actions.ContinuousActions.Length > 0)
        {
            float moveX = actions.ContinuousActions[0];
            float moveZ = actions.ContinuousActions[1];

            float moveSpeed = 2f;

            transform.localPosition += new Vector3(moveX, 0f, moveZ) * Time.deltaTime * moveSpeed;
        }
    }

    public override void Heuristic(in ActionBuffers actionsOut)
    {
        ActionSegment<float> continuousActions = actionsOut.ContinuousActions;
        if (continuousActions.Length > 0)
        {
            continuousActions[0] = Input.GetAxisRaw("Horizontal");
            continuousActions[1] = Input.GetAxisRaw("Vertical");
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Target")
        {
            SetReward(5f);
            floor.material = winMaterial;
            EndEpisode();
        }
        if (other.gameObject.tag == "Wall")
        {
            AddReward(-10f);
            floor.material = loseMaterial;
            EndEpisode();
        }
    }
}
