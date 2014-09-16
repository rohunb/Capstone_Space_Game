﻿using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;

public class HexTileMapGenerator : MonoBehaviour
{

    public GameObject tile;
    public GameObject ship;
    public int shipLayer=8;

    List<Transform> tiles;

    Vector2 hexTileSize;
    Vector3 shipSize;

    Vector3 startPos;
    float tileSpawnHeight;
    float raycastHeight;
    int tileGridWidth;
    int tileGridHeight;



    void Start()
    {
        Init();
        CreatedHexTileGrid();
        DeleteExtraTiles();
    }

    void Init()
    {
        tiles = new List<Transform>();

        hexTileSize.x = tile.renderer.bounds.size.x;
        hexTileSize.y = tile.renderer.bounds.size.z;
        shipSize.x = ship.renderer.bounds.size.x;
        shipSize.y = ship.renderer.bounds.size.y;
        shipSize.z = ship.renderer.bounds.size.z;
        

        CalculateGridSize();

        tileSpawnHeight = ship.transform.position.y - shipSize.y;
        raycastHeight = ship.transform.position.y + shipSize.y*2f;

        startPos = new Vector3(ship.transform.position.x - shipSize.x / 2f, tileSpawnHeight ,
                               ship.transform.position.z + shipSize.z / 2f - hexTileSize.x / 2f);

    }

    void CalculateGridSize()
    {
        //hexagon's side length is half the height
        float sideLength = hexTileSize.y / 2.0f;
        //the number of whole hex sides that fit inside inside ship's z length
        int numSides = Mathf.RoundToInt(shipSize.z / sideLength);

        tileGridHeight = Mathf.RoundToInt(numSides * 2f / 3f);
        tileGridWidth = Mathf.RoundToInt(shipSize.x / hexTileSize.x);

    }
    void CreatedHexTileGrid()
    {
        Transform shipTileMap = new GameObject("ShipTileMap").transform;
        shipTileMap.transform.position = ship.transform.position;
        Vector3 tilePos;
        for (int y = 0; y < tileGridHeight; y++)
        {
            for (int x = 0; x < tileGridWidth + y % 2; x++)
            {
                tilePos = GetWorldCoords(x, y);
                GameObject tileClone = Instantiate(tile, tilePos, tile.transform.rotation) as GameObject;
                tileClone.transform.SetParent(shipTileMap.transform, true);
                tiles.Add(tileClone.transform);
                tilePos.x += hexTileSize.x * 2f;
            }
        }
    }

    Vector3 GetWorldCoords(int xGridPos, int yGridPos)
    {
        float offset = 0f;
        if (yGridPos % 2 == 0)
        {
            offset = hexTileSize.x / 2f;
        }
        Vector3 tilePos = Vector3.zero;
        tilePos.x = startPos.x + xGridPos * hexTileSize.x + offset;
        tilePos.y = tileSpawnHeight;
        tilePos.z = startPos.z - yGridPos * hexTileSize.y * .75f;

        return tilePos;
    }

    void DeleteExtraTiles()
    {
        Vector3 rayOrigin;
        Ray ray=new Ray();
        for (int i = tiles.Count-1; i >= 0; i--)
        {
            rayOrigin = tiles[i].position + Vector3.up * raycastHeight;
            ray.origin = rayOrigin;
            ray.direction = Vector3.down;
            if(!Physics.Raycast(ray, 500f,1<<shipLayer))
            {
                //DestroyImmediate(tiles[i], false);
                Destroy(tiles[i].gameObject);
                tiles.RemoveAt(i);
            }
        }
    }


}
