using System.Collections.Generic;
using UnityEngine;

public static class Utility
{
    public static float Calculate(AI_ScrObject pProperties, Vector3 pAgentPosition, Vector3 pTargetPosition)
    {
        float distanceFactor = 0;
        float typeFactor = 0;

        RaycastHit hit = new RaycastHit();

        Physics.Linecast(pAgentPosition, pTargetPosition, out hit);
        Debug.DrawRay(pAgentPosition, (pTargetPosition - pAgentPosition));

        if (hit.distance == 0)
        {
            Debug.Log("Linecast didnt hit anything.");
            return 0;
        }

        switch (hit.collider.tag)
        {
            case "Player":
                distanceFactor = pProperties.Distance_PlayerFactor;
                typeFactor = pProperties.Type_PlayerFactor;
                break;
            case "Bee_NPC":
                distanceFactor = pProperties.Distance_BeeFactor;
                typeFactor = pProperties.Type_BeeFactor;
                break;
            default:
                Debug.Log($"Hit gameobject with tag {hit.collider.tag}");
                break;
        }

        return (1 - (hit.distance / pProperties.MaxSight)) * distanceFactor + typeFactor +
               Random.Range(pProperties.MinRandomValue, pProperties.MaxRandomValue);
    }
}