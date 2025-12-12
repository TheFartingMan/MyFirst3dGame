using UnityEngine;
using UnityEngine.AI;

public class NavTowardPlayer : MonoBehaviour
{
    [SerializeField] private NavMeshAgent navMeshAgent;
    [SerializeField] private Transform playerTransform;
    void Update()
    {
        navMeshAgent.SetDestination(playerTransform.position);
    }
}
