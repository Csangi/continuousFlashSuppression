using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class createMond : MonoBehaviour
{
    public float actionTime;

    public Texture2D tex2d;

    //CSV variables - we only need the flash value
    static GameObject uiVars;
    public List<int> flash = new List<int>(); //flash of theImg showing
    //eye toggles
    public Toggle rightEye;
    public Toggle leftEye;
    //this is the mod vaule
    private float period;
    public int palette;
    List<List<Color>> colors = new List<List<Color>>();

    public Experiment exp = Experiment.current;
    // Start is called before the first frame update
    void Start()
    {
        setColorValues();
    }

    public void updateMond(RawImage[] rightImgArr, RawImage[] leftImgArr)
    {
        int whichmond = 0, i = 0, numOfMondsUsed = 0, counter = 0;
        bool endOfMonds = false;
        for (int k = 0; k < exp.Mondrians.Count(); ++k)
        {
            if (exp.Mondrians[k].isUsed)
                ++numOfMondsUsed;
        }
        makeImagesBlank(rightImgArr, leftImgArr);
        while (i < rightImgArr.Length && !endOfMonds && counter != numOfMondsUsed)
        {
            while (!exp.Mondrians[whichmond].isUsed)
                ++whichmond;


            exp.Mondrians[whichmond].minRange = i;

            for (int j = 0; j < 20; ++j, ++i)
            {
                if (exp.Mondrians[whichmond].shape == Shape.ellipse)
                    rightImgArr[i].texture = DrawCircle(rightImgArr[i].texture as Texture2D, colors[exp.Mondrians[whichmond].palette], exp.Mondrians[whichmond]);
                else if (exp.Mondrians[whichmond].shape == Shape.rectangle)
                    rightImgArr[i].texture = DrawRectangle(rightImgArr[i].texture as Texture2D, colors[exp.Mondrians[whichmond].palette], exp.Mondrians[whichmond]);
                else if (exp.Mondrians[whichmond].shape == Shape.triangle)
                    rightImgArr[i].texture = DrawTriangle(rightImgArr[i].texture as Texture2D, colors[exp.Mondrians[whichmond].palette], exp.Mondrians[whichmond]);
                else if (exp.Mondrians[whichmond].shape == Shape.pixelated)
                    rightImgArr[i].texture = DrawPixelated(rightImgArr[i].texture as Texture2D, colors[exp.Mondrians[whichmond].palette], exp.Mondrians[whichmond]);
                else if (exp.Mondrians[whichmond].shape == Shape.circle)
                    rightImgArr[i].texture = DrawCircle(rightImgArr[i].texture as Texture2D, colors[exp.Mondrians[whichmond].palette], exp.Mondrians[whichmond]);
                else if (exp.Mondrians[whichmond].shape == Shape.square)
                    rightImgArr[i].texture = DrawSquare(rightImgArr[i].texture as Texture2D, colors[exp.Mondrians[whichmond].palette], exp.Mondrians[whichmond]);
                else if (exp.Mondrians[whichmond].shape == Shape.mixed)
                    rightImgArr[i].texture = DrawMixed(rightImgArr[i].texture as Texture2D, colors[exp.Mondrians[whichmond].palette], exp.Mondrians[whichmond]);
            }
            exp.Mondrians[whichmond].maxRange = i - 1;

            ++whichmond;
            ++counter;
        }
        exp.mondsHaveBeenDrawn = true;
        exp.printMondrians();
    }

    //very inspired from https://stackoverflow.com/questions/30410317/how-to-draw-circle-on-texture-in-unity/56616769
    public static Texture2D DrawRectangle(Texture2D tex, List<Color> color, Mondrian mond)
    {
        if (mond.addPixelated)
            DrawPixelated(tex, color, mond);
        int x, y, width, height;
        Color c;
        for (int i = 0; i < mond.density; ++i)
        {
            c = color[Random.Range(0, color.Count)];
            x = Random.Range(0, 225);
            width = Random.Range(mond.minWidth, mond.maxWidth);
            y = Random.Range(0, 225);
            height = Random.Range(mond.minHeight, mond.maxHeight);
            for (int u = x - width; u < x + width + 1; u++)
                for (int v = y - height; v < y + height + 1; v++)
                    tex.SetPixel(u, v, c);
            tex.Apply();
        }
        return tex;
    }

    public static Texture2D DrawCircle(Texture2D tex, List<Color> color, Mondrian mond)
    {
        if (mond.addPixelated)
            DrawPixelated(tex, color, mond);
        int x, y, radius;
        float rSquared;
        Color c;
        for (int i = 0; i < mond.density; ++i)
        {
            c = color[Random.Range(0, color.Count)];
            x = Random.Range(0, 225);
            radius = Random.Range(mond.minWidth / 2, mond.maxWidth / 2);
            y = Random.Range(0, 225);
            radius = Random.Range(mond.minHeight / 2, mond.maxHeight / 2);
            rSquared = radius * radius;
            for (int u = x - radius; u < x + radius + 1; u++)
                for (int v = y - radius; v < y + radius + 1; v++)
                    if ((x - u) * (x - u) + (y - v) * (y - v) < rSquared)
                        tex.SetPixel(u, v, c);
            tex.Apply();
        }
        return tex;
    }

    public static Texture2D DrawSquare(Texture2D tex, List<Color> color, Mondrian mond)
    {
        if (mond.addPixelated)
            DrawPixelated(tex, color, mond);
        int x, y, size;
        Color c;
        for (int i = 0; i < mond.density; ++i)
        {
            c = color[Random.Range(0, color.Count)];
            x = Random.Range(0, 225);
            size = Random.Range(mond.minWidth, mond.maxWidth);
            y = Random.Range(0, 225);
            for (int u = x - size; u < x + size + 1; u++)
                for (int v = y - size; v < y + size + 1; v++)
                    tex.SetPixel(u, v, c);
            tex.Apply();
        }
        return tex;
    }

    public static Texture2D DrawTriangle(Texture2D tex, List<Color> color, Mondrian mond)
    {
        if (mond.addPixelated)
            DrawPixelated(tex, color, mond);
        int x, y, tbase, height;
        Color c;
        for (int i = 0; i < mond.density; ++i)
        {
            c = color[Random.Range(0, color.Count)];
            x = Random.Range(0, 225);
            tbase = Random.Range(mond.minWidth, mond.maxWidth);
            y = Random.Range(0, 225);
            height = Random.Range(mond.minHeight, mond.maxHeight);
            for (int u = x - tbase; u < x + tbase + 1; u++)
                for (int v = y - height; v < y + height + 1; v++)
                    if (1 / 2 * tbase * height < 1)
                        tex.SetPixel(u, v, c);
            tex.Apply();
        }
        return tex;
    }

    public static Texture2D DrawEllipse(Texture2D tex, List<Color> color, Mondrian mond)
    {
        if (mond.addPixelated)
            DrawPixelated(tex, color, mond);
        int x, y, xradius, yradius;
        Color c;
        for (int i = 0; i < mond.density; ++i)
        {
            c = color[Random.Range(0, color.Count)];
            x = Random.Range(0, 225);
            xradius = Random.Range(mond.minWidth / 2, mond.maxWidth / 2);
            y = Random.Range(0, 225);
            yradius = Random.Range(mond.minHeight / 2, mond.maxHeight / 2);
            for (int u = x - xradius; u < x + xradius + 1; u++)
                for (int v = y - yradius; v < y + yradius + 1; v++)
                    if (Mathf.Pow(v - y, 2) / (yradius * yradius) + Mathf.Pow(u - x, 2) / (xradius * xradius) < 1)
                        tex.SetPixel(u, v, c);
            tex.Apply();
        }
        return tex;
    }

    public static Texture2D DrawPixelated(Texture2D tex, List<Color> color, Mondrian mond)
    {
        Color c;
        for (int i = 0; i <= 225; ++i)
            for (int j = 0; j <= 255; ++j)
            {
                c = color[Random.Range(0, color.Count)];
                tex.SetPixel(i, j, c);
            }
        tex.Apply();
        return tex;
    }

    public static Texture2D DrawMixed(Texture2D tex, List<Color> color, Mondrian mond)
    {
        if (mond.addPixelated)
            DrawPixelated(tex, color, mond);
        int x, y, xradius, yradius, width, height, radius, size;
        Color c;
        float rSquared;
        for (int i = 0; i < mond.density; ++i)
        {
            if (i % 4 == 0) //ellipse
            {
                c = color[Random.Range(0, color.Count)];
                x = Random.Range(0, 225);
                xradius = Random.Range(mond.minWidth / 2, mond.maxWidth / 2);
                y = Random.Range(0, 225);
                yradius = Random.Range(mond.minHeight / 2, mond.maxHeight / 2);
                for (int u = x - xradius; u < x + xradius + 1; u++)
                    for (int v = y - yradius; v < y + yradius + 1; v++)
                        if (Mathf.Pow(v - y, 2) / (yradius * yradius) + Mathf.Pow(u - x, 2) / (xradius * xradius) < 1)
                            tex.SetPixel(u, v, c);
                tex.Apply();
            }
            else if (i % 4 == 1)   //rectangle
            {
                c = color[Random.Range(0, color.Count)];
                x = Random.Range(0, 225);
                width = Random.Range(mond.minWidth, mond.maxWidth);
                y = Random.Range(0, 225);
                height = Random.Range(mond.minHeight, mond.maxHeight);
                for (int u = x - width; u < x + width + 1; u++)
                    for (int v = y - height; v < y + height + 1; v++)
                        tex.SetPixel(u, v, c);
                tex.Apply();
            }
            else if (i % 4 == 2)     //circle
            {
                c = color[Random.Range(0, color.Count)];
                x = Random.Range(0, 225);
                radius = Random.Range(mond.minWidth / 2, mond.maxWidth / 2);
                y = Random.Range(0, 225);
                radius = Random.Range(mond.minHeight / 2, mond.maxHeight / 2);
                rSquared = radius * radius;
                for (int u = x - radius; u < x + radius + 1; u++)
                    for (int v = y - radius; v < y + radius + 1; v++)
                        if ((x - u) * (x - u) + (y - v) * (y - v) < rSquared)
                            tex.SetPixel(u, v, c);
                tex.Apply();
            }
            else if (i % 4 == 3)     //square
            {
                c = color[Random.Range(0, color.Count)];
                x = Random.Range(0, 225);
                size = Random.Range(mond.minWidth, mond.maxWidth);
                y = Random.Range(0, 225);
                for (int u = x - size; u < x + size + 1; u++)
                    for (int v = y - size; v < y + size + 1; v++)
                        tex.SetPixel(u, v, c);
                tex.Apply();
            }
        }
        return tex;
    }

    public void makeImagesBlank(RawImage[] rightImgArr, RawImage[] leftImgArr)
    {
        for (int i = 0; i < rightImgArr.Length; ++i)
        {
            rightImgArr[i].texture = makeBlank(rightImgArr[i].texture as Texture2D);
            exp.mondsHaveBeenDrawn = false;
        }
    }

    public static Texture2D makeBlank(Texture2D tex)
    {
        for (int i = 0; i <= 225; ++i)
            for (int j = 0; j <= 255; ++j)
                tex.SetPixel(i, j, Color.white);
        tex.Apply();
        return tex;
    }

    //color values from https://excelatfinance.com/xlf/xlf-colors-1.php
    void setColorValues()
    {
        List<Color> row = new List<Color>();
        row.Add(new Color(255, 0, 0));      //red
        row.Add(new Color(0, 255, 0));      //green
        row.Add(new Color(0, 0, 255));      //blue
        row.Add(new Color(255, 0, 255));    //magenta
        row.Add(new Color(255, 255, 0));    //yellow
        row.Add(new Color(0, 255, 255));    //cyan
        colors.Add(row);
        row = new List<Color>();
        row.Add(new Color(0, 0, 0));            //black
        row.Add(new Color(255, 255, 255));      //white
        row.Add(new Color(192, 192, 192));      //light gray
        row.Add(new Color(128, 128, 128));      //grey
        colors.Add(row);
        row = new List<Color>();
        row.Add(new Color(128, 0, 0));      //brown
        row.Add(new Color(0, 128, 0));      //dark green
        row.Add(new Color(0, 0, 128));      //dark blue
        row.Add(new Color(128, 128, 0));    //dark yellow/gold
        row.Add(new Color(128, 0, 128));    //dark magenta 
        row.Add(new Color(0, 128, 128));    //dark cyan
        colors.Add(row);
    }
}
