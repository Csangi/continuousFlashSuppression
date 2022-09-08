using UnityEngine;

public class Mondrian
{
    public int palette { get; set; }
    public Shape shape { get; set; }
    public int maxWidth { get; set; }
    public int minWidth { get; set; }
    public int maxHeight { get; set; }
    public int minHeight { get; set; }
    public int density { get; set; }
    public bool isUsed { get; set; }
    public int minRange { get; set; }       //this refers how far in the line of mondrians this one appears in the mond array
    public int maxRange { get; set; }
    public bool addPixelated { get; set; }
    public Mondrian(int palette, int sh, bool pix, int minWidth, int maxWidth, int minHeight, int maxHeight, int density)
    {
        this.palette = palette;
        switch (sh)
        {
            case 0:
                this.shape = Shape.ellipse;
                break;
            case 1:
                this.shape = Shape.rectangle;
                break;
            case 2:
                this.shape = Shape.triangle;
                break;
            case 3:
                this.shape = Shape.pixelated;
                break;
            case 4:
                this.shape = Shape.circle;
                break;
            case 5:
                this.shape = Shape.square;
                break;
            case 6:
                this.shape = Shape.mixed;
                break;
            default:
                this.shape = Shape.ellipse;
                break;
        }
        this.maxWidth = maxWidth;
        this.minWidth = minWidth;
        this.maxHeight = maxHeight;
        this.minHeight = minHeight;
        this.density = density;
        this.isUsed = false;
        this.minRange = 0;
        this.maxRange = 0;
        this.addPixelated = pix;
    }

    public void printMond()
    {
        Debug.Log("Palette: " + palette);
        Debug.Log("Shape: " + shape);
        Debug.Log("Min Width: " + minWidth);
        Debug.Log("Max Width: " + maxWidth);
        Debug.Log("Min Height: " + minHeight);
        Debug.Log("Max Height: " + maxHeight);
        Debug.Log("Density: " + density);
        Debug.Log("Min Range: " + minRange);
        Debug.Log("Max Range: " + maxRange);
        Debug.Log("Is used: " + isUsed);
    }
}


public enum Shape
{
    ellipse,
    rectangle,
    triangle,
    pixelated,
    circle,
    square,
    mixed
}