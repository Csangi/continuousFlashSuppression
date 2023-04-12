using System.Collections.Generic;
using System.IO;
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

    //used for loading screen
    public static int numOfMonds = 1;
    public static int currentMond = 0;

    public Experiment exp = Experiment.current;
    // Start is called before the first frame update

    public void updateMond(RawImage[] rightImgArr, RawImage[] leftImgArr)
    {
        colors.Clear();
        setColorValues();
        int whichmond = 0,
            i = 0,
            numOfMondsUsed = 0, // this refers to the total number of mondrians used in this experiment
            counter = 0;        //this counts the mondrians as they are being written. 
        bool endOfMonds = false;
        for (int k = 0; k < exp.Mondrians.Count(); ++k)
        {   //here we count how many mondrians are used in the mondrian array 
            if (exp.Mondrians[k].isUsed)
                ++numOfMondsUsed;
        }
        numOfMonds = numOfMondsUsed * 20;
        makeImagesBlank(rightImgArr, leftImgArr);       //here we blank out all 100 mondrian images to make sure we dont draw over another mondrian
        //here we loop through all 100 mondrian images and find the mondrians which are actually used and drawn them
        //We also log there minimum range and Maximum range in the mondrian array
        while (i < rightImgArr.Length && !endOfMonds && counter != numOfMondsUsed)
        {
            while (!exp.Mondrians[whichmond].isUsed)
                ++whichmond;

            //this is logging the minimum range which this particular mondrian will be in
            exp.Mondrians[whichmond].minRange = i;

            for (int j = 0; j < 20; ++j, ++i)
            {
                mondTask(whichmond, rightImgArr[i], leftImgArr[i]);
            }
            exp.Mondrians[whichmond].maxRange = i - 1;
            //this is the maximum range which the mondrian will be in
            //we do this so that the experiment runner actually knows where each mondrians textures are while running. 

            ++whichmond;
            ++counter;
        }
        exp.mondsHaveBeenDrawn = true;
        exp.printMondrians();
    }

    public void mondTask(int whichmond, RawImage rightImg, RawImage leftImg)
    {
        if (exp.Mondrians[whichmond].shape == Shape.ellipse)
        {
            leftImg.texture = DrawEllipse(rightImg.texture as Texture2D, colors[exp.Mondrians[whichmond].palette], exp.Mondrians[whichmond]);
            (leftImg.texture as Texture2D).Apply();
        }
        else if (exp.Mondrians[whichmond].shape == Shape.rectangle)
        {
            leftImg.texture = DrawRectangle(rightImg.texture as Texture2D, colors[exp.Mondrians[whichmond].palette], exp.Mondrians[whichmond]);
            (leftImg.texture as Texture2D).Apply();
        }
        else if (exp.Mondrians[whichmond].shape == Shape.triangle)
        {
            leftImg.texture = DrawTriangle(rightImg.texture as Texture2D, colors[exp.Mondrians[whichmond].palette], exp.Mondrians[whichmond]);
            (leftImg.texture as Texture2D).Apply();
        }
        else if (exp.Mondrians[whichmond].shape == Shape.pixelated)
        {
            leftImg.texture = DrawPixelated(rightImg.texture as Texture2D, colors[exp.Mondrians[whichmond].palette], exp.Mondrians[whichmond]);
            (leftImg.texture as Texture2D).Apply();
        }
        else if (exp.Mondrians[whichmond].shape == Shape.circle)
        {
            leftImg.texture = DrawCircle(rightImg.texture as Texture2D, colors[exp.Mondrians[whichmond].palette], exp.Mondrians[whichmond]);
            (leftImg.texture as Texture2D).Apply();
        }
        else if (exp.Mondrians[whichmond].shape == Shape.square)
        {
            leftImg.texture = DrawSquare(rightImg.texture as Texture2D, colors[exp.Mondrians[whichmond].palette], exp.Mondrians[whichmond]);
            (leftImg.texture as Texture2D).Apply();
        }
        else if (exp.Mondrians[whichmond].shape == Shape.mixed)
        {
            leftImg.texture = DrawMixed(rightImg.texture as Texture2D, colors[exp.Mondrians[whichmond].palette], exp.Mondrians[whichmond]);
            (leftImg.texture as Texture2D).Apply();
        }
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
        int x, y, xradius, yradius, width, height, size;
        Color c;
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
                xradius = Random.Range(mond.minWidth / 2, mond.maxWidth / 2) + 10;
                y = Random.Range(0, 225);
                yradius = Random.Range(mond.minHeight / 2, mond.maxHeight / 2) + 10;
                for (int u = x - xradius; u < x + xradius + 1; u++)
                    for (int v = y - yradius; v < y + yradius + 1; v++)
                        if (Mathf.Pow(v - y, 2) / (yradius * yradius) + Mathf.Pow(u - x, 2) / (xradius * xradius) < 1)
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
        int counter = 0, red = -1, green = -1, blue = -1;
        List<Color> row = new List<Color>();
        row.Add(new Color(255, 0, 0));      //red
        row.Add(new Color(0, 255, 0));      //green
        row.Add(new Color(0, 0, 255));      //blue
        row.Add(new Color(255, 0, 255));    //magenta
        row.Add(new Color(255, 255, 0));    //yellow
        row.Add(new Color(0, 255, 255));    //cyan
        colors.Add(row);
        row = new List<Color>();
        string palettePath = Experiment.current.inputPath;                                                //its called image path because I copied my code from the image uploader
        Debug.Log(palettePath);
        while (!palettePath[palettePath.Length - 1].Equals('\\'))                   //we want to remove the original file name so that we can find the directory and navigate to the correct location
        {
            palettePath = palettePath.Remove(palettePath.Length - 1, 1);
        }
        palettePath += "colorPalette.csv";
        Debug.Log(palettePath);

        if (palettePath.Length != 0 && File.Exists(palettePath))
        {
            using (var sr = new StreamReader(palettePath))          //open up a stream reader on the path
            {
                bool EOF = false;
                while (!EOF)
                {
                    string data = sr.ReadLine();                //read in line by line
                    if (data == null)
                    {
                        EOF = true;
                        break;
                    }
                    var values = data.Split(',');               //input into an array
                    for (int i = 0; i < values.Length; i++)     //loop through each line with a line being its own palette
                    {
                        Debug.Log(values.Length);
                        Debug.Log(values[i]);
                        if (counter > 0)
                        {
                            int result = 0;
                            result = System.Int32.Parse(values[i]);
                            Debug.Log(result);
                            if (i == 0)                         //ignore first column
                            {
                                Debug.Log("do nothing");
                            }
                            else if (red == -1)                 //the order goes r g b so everytime we loop through a line we will populate accordingly
                            {
                                red = result;
                            }
                            else if (green == -1)
                            {
                                green = result;
                            }
                            else if (blue == -1)
                            {
                                blue = result;
                            }
                            else
                            {                                   //once these are all filled add teh color and start again
                                row.Add(new Color(red, green, blue));
                                red = green = blue = -1;
                                red = result;
                            }
                        }
                    }
                    if (counter > 0)                //ignore first row
                    {
                        colors.Add(row);
                        row = new List<Color>();
                    }
                    counter++;
                }
                sr.Close();
            }

        }
    }
}
