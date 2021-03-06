﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "NewRecruitmentFeature", menuName = "Entity Features/Recruitment Feature")]
public class RecruitmentFeature : EntityFeature
{
    public List<EntityInfo> recruitableUnits;

    private List<QueueObject> queue;
    private float timer = 0f;
    private readonly int maxQueueSize = 10;

    public override void Initialize(Entity entity)
    {
        base.Initialize(entity);
        queue = new List<QueueObject>();
    }

    public override void UpdateOverride()
    {
        if (Input.GetKeyDown(KeyCode.T))
            AddToQueue(recruitableUnits[0]);
        
        if (queue.Count > 0)
            if (timer <= 0f)
            {
                Recruit(queue[0].BaseUnit);
                queue.RemoveAt(0);

                if (queue.Count > 0)
                    timer = queue[0].BaseUnit.buildTime;
                else
                    timer = 0f;
            }
            else
            {
                timer -= Time.deltaTime;
                Debug.Log("TIMER: " + timer + "s / QUEUE: " + queue.Count + " $" + queue[0].BaseUnit.name);
            }
    }

    /// <summary>
    /// Recruits a specific Unit.
    /// </summary>
    /// <param name="unitInfo">The Unit we wish to spawn.</param>
    /// <returns>True on a success, false otherwise.</returns>
    public bool AddToQueue(EntityInfo unit)
    {
        //Debug.Log("Add to queue: " + unit.name);
        // if unit is not allowed, abort
        if (!recruitableUnits.Contains(unit))
            return false;

        //Debug.Log("Recruitable: YES");

        if (queue.Count >= maxQueueSize)
        {
            //Debug.Log("Queue is already full!");
            return false;
        }

        //check resource amount
        List<ResourceAmount> unitRecruitmentCosts = unit.resourceAmount;

        bool enough = true;
        List<ResourceAmount> missingResources = new List<ResourceAmount>();

        foreach (ResourceAmount amount in unitRecruitmentCosts)
        {
            float availableResource = entity.Owner.PlayerEconomy.GetResourceAmount(amount.resource);
            
            if (availableResource < amount.amount)
            {
                missingResources.Add(new ResourceAmount(amount.resource, amount.amount - availableResource));
                enough = false;
            }
        }

        if (enough == true)
        {
            foreach (ResourceAmount amount in unitRecruitmentCosts)
            {
                float newAmount = entity.Owner.PlayerEconomy.GetResourceAmount(amount.resource) - amount.amount; ;
                entity.Owner.PlayerEconomy.RemoveResourceAmount(amount);
            }

            if (queue.Count == 0)
                timer = unit.buildTime;

            queue.Add(new QueueObject(unit, unit.resourceAmount));

            //Debug.Log("Successfully added " + unit.name + " to the queue!");

            return true;
        }
        else
        {
            // alert the player
            string message = "Not enough resources for " + unit.name + "!";
            foreach (ResourceAmount amount in missingResources)
                message += "\n" + amount.amount + " " + amount.resource;
            Debug.Log(message);


            // TODO: Make an ingame error message

            return false;
        }
    }

    private void Recruit(EntityInfo unit)
    {
        //Transform parent = entity.Owner.UnitsGO.transform;

        entity.Owner.CmdSpawnUnit(unit.name, entity.transform.position);
        //GameObject newUnit = unit.CreateInstance(entity.Owner, entity.transform.position);

        //if (newUnit != null)
        //    Entity newEntity = newUnit.GetComponent<Entity>();

        //if (newEntity.Controller == null)
        //    newUnit.AddComponent<EntityOrderController>();
    }
    
}
