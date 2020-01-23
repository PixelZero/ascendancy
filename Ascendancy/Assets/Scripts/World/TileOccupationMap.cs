﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(World))]
public class TileOccupationMap : MonoBehaviour
{
    private World world;
    private TileOccupation[,] occupationMap;

    protected void Start()
    {
        world = transform.GetComponent<World>();
        int worldSize = (int)world.EffectiveWorldSize;
        occupationMap = new TileOccupation[worldSize, worldSize];

        for (int x = 0; x < worldSize; x++)
            for (int y = 0; y < worldSize; y++)
                occupationMap[x, y] = new TileOccupation();
    }

    public bool IsTileFree(int tileX, int tileY, TileOccupation.OccupationLayer layer = TileOccupation.OccupationLayer.Building)
    {
        Debug.Assert(tileX > 0 && tileX < occupationMap.GetLength(0), "Something wrong in TileOccupationMap. X: " + tileX);
        Debug.Assert(tileY > 0 && tileY < occupationMap.GetLength(1), "Something wrong in TileOccupationMap. Y: " + tileY);
        return occupationMap[tileX, tileY].occupation[layer] == null;
    }
    
    public bool AreTilesFree(Vector3 pos, Vector2Int dimensions, TileOccupation.OccupationLayer layer = TileOccupation.OccupationLayer.Building)
    {
        Vector2Int v = world.IntVector(pos);
        
        int halfX = dimensions.x / 2;
        int halfY = dimensions.y / 2;

        // If a single tile is occupied, return false.
        for (int x = 0; x < dimensions.x; x++)
            for (int y = 0; y < dimensions.y; y++)
            {
                int finalX = v.x + x - halfX, finalY = v.y + y - halfY;
                if (!IsTileFree(finalX, finalY, layer))
                    return false;
            }

        // All tiles free.
        return true;
    }

    public void NewOccupation(Vector3 pos, Entity occupyingEntity, TileOccupation.OccupationLayer layer = TileOccupation.OccupationLayer.Building)
    {
        Vector2Int v = world.IntVector(pos);
        Vector2Int dimensions = occupyingEntity.entityInfo.dimensions;

        int halfX = dimensions.x / 2;
        int halfY = dimensions.y / 2;

        // Mark all tiles as occupied.
        for (int x = 0; x < dimensions.x; x++)
            for (int y = 0; y < dimensions.y; y++)
            {
                Debug.Assert(occupationMap[v.x + x - halfX, v.y + y - halfY].occupation[layer] == null, "Tile " + v.x + ":" + v.y + " already Occupied, please check.");
                occupationMap[v.x + x - halfX, v.y + y - halfY].occupation[layer] = occupyingEntity;
            }
    }
}
