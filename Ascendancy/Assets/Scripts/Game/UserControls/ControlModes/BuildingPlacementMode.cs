﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class BuildingPlacementMode : ControlMode
{
    public EntityInfo building;
    public GameObject preview;
    
    public BuildingPlacementMode()
    {
        preview = GameObject.Find("BuildingPreview");
        if (preview == null)
            Debug.Log("BuildingPreview not found!");
        preview.SetActive(false);
    }

    public override void HandleInput()
    {
        if (EventSystem.current.IsPointerOverGameObject())
            // if Mouse over UI element, do not do anything
            return;

        if (Input.GetMouseButtonUp(1))
            //RMB, so cancel build mode
            gameManager.SwitchToMode(ControlModeEnum.gameMode);

        Ray ray = gameManager.camScript.MouseCursorRay();
        RaycastHit hit;
        
        int layerMask = 1 << LayerMask.NameToLayer("Ground");
        if (Physics.Raycast(ray, out hit, 100, layerMask))
        {
            Tile tile = gameManager.world.GetTile(hit.point);

            int x = (int)tile.worldX, y = (int)tile.worldZ;
            preview.transform.position = new Vector3(x, tile.height, y);

            // Location is valid if tile is both flatland and empty of other Entities of the same BuildingLayer.
            bool flatArea = gameManager.world.IsAreaFlat(preview.transform.position, building.dimensions);
            bool freeSpace = gameManager.occupationMap.AreTilesFree(preview.transform.position, building.dimensions);
            bool validLocation = flatArea && freeSpace;

            preview.GetComponent<BuildingPreview>().valid = validLocation;
            
            if (Input.GetMouseButtonDown(0))
                if (validLocation)
                {
                    // valid spot, place building
                    Debug.Log("Placing a " + building.name + " at: " + preview.transform.position);
                    GameObject newBuildingGO = building.CreateInstance(gameManager.GetPlayer, preview.transform.position);
                    Entity b = newBuildingGO.GetComponent<Entity>();
                    
                    // Mark all the spots that this building occupies as occupied in the world map.
                    gameManager.occupationMap.NewOccupation(preview.transform.position, b, TileOccupation.OccupationLayer.Building);
                }
                else
                {
                    // invalid spot, do NOT place building
                    if (!flatArea)
                        Debug.Log("Area not flat");
                    if (!freeSpace)
                        Debug.Log("Other building here");
                }
        }
        
    }
    public override void Start()
    {
        preview.SetActive(true);
    }
    public override void Stop()
    {
        preview.SetActive(false);
    }
}
