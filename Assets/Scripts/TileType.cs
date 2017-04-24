using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileType {

    public string name;
    public bool walkable;
    public int steps;
    public Color defaultColor;
    public Color hoverColor;

    public TileType(string name, bool walkable, int steps, Color defaultColor, Color hoverColor)
    {
        this.name = name;
        this.walkable = walkable;
        this.steps = steps;
        this.defaultColor = defaultColor;
        this.hoverColor = hoverColor;
    }
}
