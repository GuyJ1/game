using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridBehavior : MonoBehaviour
{
    [SerializeField] public uint width, height; // Width and height of the grid
    [SerializeField] public int tilesize; // Size of each tile

    public GameObject Tile; // A cell or tile that makes up the grid
    public GameObject [,] grid; // The actual grid, represented by a 2d array
    
    // Start is called before the first frame update
    void Start()
    {
        grid = new GameObject[width, height];
        GenerateGrid();
    }

    // Update is called once per frame
    void Update()
    {

    }

    void GenerateGrid()
    {
        for (uint x = 0; x < width; x++)
        {
            for (uint y = 0; y < height; y++)
            {
                Vector3 pos = new Vector3(transform.position.x + x*tilesize, transform.position.y, transform.position.z + y*tilesize);
                GameObject newTile = Instantiate(Tile, pos, transform.rotation, this.transform);
                newTile.name = "Tile " + x + " " + y;
                newTile.GetComponent<TileScript>().x = x;
                newTile.GetComponent<TileScript>().y = y;
                Debug.Log("Instantiating " + newTile.name);
                grid[x, y] = newTile;
            }
        }
    }
}
