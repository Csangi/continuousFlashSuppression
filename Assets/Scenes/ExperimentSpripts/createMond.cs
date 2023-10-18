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
    Dictionary<string, List<Color>> colors = new Dictionary<string, List<Color>>();

    //used for loading screen
    public static int numOfMonds = 1;
    public static int currentMond = 0;

    public Experiment exp = Experiment.current;
    // Start is called before the first frame update

    public void updateMond(RawImage[] rightImgArr, RawImage[] leftImgArr)
    {
        colors.Clear();
        setColorValues();
        List<string> mondKeys = new List<string>();
        int i = 0,
            j = 0,
            numOfMondsUsed = 0, // this refers to the total number of mondrians used in this experiment
            counter = 0;        //this counts the mondrians as they are being written. 
        string whichmond = "";
        foreach (KeyValuePair<string,Mondrian> item in exp.Mondrians)
        {
            mondKeys.Add(item.Key);
        }

        for (int k = 0; k < exp.Mondrians.Count(); ++k)
        {   //here we count how many mondrians are used in the mondrian array 
            if (exp.Mondrians[mondKeys[k]].isUsed)
                ++numOfMondsUsed;
        }
        numOfMonds = numOfMondsUsed * 20;
        makeImagesBlank(rightImgArr, leftImgArr);       //here we blank out all 100 mondrian images to make sure we dont draw over another mondrian
        //here we loop through all 100 mondrian images and find the mondrians which are actually used and drawn them
        //We also log there minimum range and Maximum range in the mondrian array
        while (i < rightImgArr.Length && j < mondKeys.Count() && counter != numOfMondsUsed)
        {
            while (!exp.Mondrians[mondKeys[j]].isUsed)
            {
                ++j;
            }

            //this is logging the minimum range which this particular mondrian will be in
            exp.Mondrians[mondKeys[j]].minRange = i;

            for (int k = 0; k < 20; ++k, ++i)
            {
                mondTask(mondKeys[j], rightImgArr[i], leftImgArr[i]);
            }
            exp.Mondrians[mondKeys[j]].maxRange = i - 1;
            //this is the maximum range which the mondrian will be in
            //we do this so that the experiment runner actually knows where each mondrians textures are while running. 

            ++j;
            ++counter;
        }
        exp.mondsHaveBeenDrawn = true;
        exp.printMondrians();
    }

    public void mondTask(string whichmond, RawImage rightImg, RawImage leftImg)
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
            leftImg.texture = CreatePixelated(rightImg.texture as Texture2D, colors[exp.Mondrians[whichmond].palette], exp.Mondrians[whichmond]);
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
        int x, y, width, height, colorNumber = 0;
        Color c;
        for (int i = 0; i < mond.density; ++i)
        {
            if (color.Count > 1)
            {
                colorNumber++;
                if (!(colorNumber < color.Count))
                    colorNumber = 0;

                c = color[colorNumber];
            }
            else
                c = color.First();
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
        int x, y, radius, colorNumber = 0;
        float rSquared;
        Color c;
        for (int i = 0; i < mond.density; ++i)
        {
            if (color.Count > 1)
            {
                colorNumber++;
                if (!(colorNumber < color.Count))
                    colorNumber = 0;

                c = color[colorNumber];
            }
            else
                c = color.First();
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
        int x, y, size, colorNumber = 0;
        Color c;
        for (int i = 0; i < mond.density; ++i)
        {
            if (color.Count > 1)
            {
                colorNumber++;
                if (!(colorNumber < color.Count))
                    colorNumber = 0;

                c = color[colorNumber];
            }
            else
                c = color.First();
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
        int colorNumber = 0;
        Color c = new Color();
        int texWidth = tex.width;
        int texHeight = tex.height;

        for (int i = 0; i < mond.density; i++)
        {
            if (color.Count > 1)
            {
                colorNumber++;
                if (!(colorNumber < color.Count))
                    colorNumber = 0;

                c = color[colorNumber];
            }
            else
                c = color.First();
            // Randomly determine the vertices and sizes of the triangle
            Vector2 vertex1 = new Vector2(Random.Range(0, texWidth), Random.Range(0, texHeight));
            Vector2 vertex2 = new Vector2(Random.Range(vertex1.x + mond.minWidth, vertex1.x + mond.maxWidth), Random.Range(vertex1.y + mond.minHeight, vertex1.y + mond.maxHeight));
            Vector2 vertex3 = new Vector2(Random.Range(vertex1.x + mond.minWidth, vertex1.x + mond.maxWidth), Random.Range(vertex1.y + mond.minHeight, vertex1.y + mond.maxHeight));

            // Random width and height for the triangle
            float width = Random.Range(mond.minWidth, mond.maxWidth); // Adjust the range as needed
            float height = Random.Range(mond.minHeight, mond.maxHeight); // Adjust the range as needed

            // Modify vertices based on width and height
            
            vertex2.x = vertex1.x + width;
            vertex3.x = vertex1.x + (width / 2);
            vertex3.y = vertex1.y - height;
            Debug.Log(vertex1);
            Debug.Log(vertex2);
            Debug.Log(vertex3);

            // Sort vertices by y-coordinate
            Vector2[] vertices = { vertex1, vertex2, vertex3 };
            System.Array.Sort(vertices, (a, b) => a.y.CompareTo(b.y));
            

            float dx1 = (vertices[1].x - vertices[0].x) / (vertices[1].y - vertices[0].y);
            float dx2 = (vertices[2].x - vertices[0].x) / (vertices[2].y - vertices[0].y);

            float x1 = vertices[0].x;
            float x2 = vertices[0].x;
            Debug.Log(dx1);
            Debug.Log(dx2);

            // Scanline filling
            for (int y = (int)vertices[0].y; y <= vertices[2].y; y++)
            {
                for (int x = Mathf.FloorToInt(x1); x <= Mathf.CeilToInt(x2); x++)
                {
                    if (x >= 0 && x < texWidth && y >= 0 && y < texHeight)
                        tex.SetPixel(x, y, c);
                }
                x1 += dx1;
                x2 += dx2;
            }
        }

        tex.Apply();
        return tex;
    }

    public static Texture2D DrawEllipse(Texture2D tex, List<Color> color, Mondrian mond)
    {
        if (mond.addPixelated)
            DrawPixelated(tex, color, mond);
        int x, y, xradius, yradius, colorNumber = 0;
        Color c;
        for (int i = 0; i < mond.density; ++i)
        {
            if (color.Count > 1)
            {
                colorNumber++;
                if (!(colorNumber < color.Count))
                    colorNumber = 0;

                c = color[colorNumber];
            }
            else
                c = color.First();
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
        //int colorNumber = 0;
        for (int i = 0; i <= 225; ++i)
            for (int j = 0; j <= 255; ++j)
            {
                c = color[Random.Range(0,color.Count)];
                /*
                if (color.Count > 1)
                {
                    colorNumber++;
                    if (!(colorNumber < color.Count))
                        colorNumber = 0;

                    c = color[colorNumber];
                }
                else
                    c = color.First();
                */
                tex.SetPixel(i, j, c);
            }
        tex.Apply();
        return tex;
    }

    public static Texture2D CreatePixelated(Texture2D tex, List<Color> color, Mondrian mond)
    {
        Color c;
        int colorNumber = 0;
        for (int i = 0; i <= 225; ++i)
            for (int j = 0; j <= 255; ++j)
            {
                if (color.Count > 1)
                {
                    colorNumber++;
                    if (!(colorNumber < color.Count))
                        colorNumber = 0;

                    c = color[colorNumber];
                }
                else
                    c = color.First();
                
                tex.SetPixel(i, j, c);
            }
        tex.Apply();
        return tex;
    }

    public static Texture2D DrawMixed(Texture2D tex, List<Color> color, Mondrian mond)
    {
        if (mond.addPixelated)
            DrawPixelated(tex, color, mond);
        int x, y, xradius, yradius, width, height, size, colorNumber = 0;
        Color c;
        for (int i = 0; i < mond.density; ++i)
        {
            if (i % 5 == 0) //ellipse
            {
                if (color.Count > 1)
                {
                    colorNumber++;
                    if (!(colorNumber < color.Count))
                        colorNumber = 0;

                    c = color[colorNumber];
                }
                else
                    c = color.First();
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
            else if (i % 5 == 1)   //rectangle
            {
                if (color.Count > 1)
                {
                    colorNumber++;
                    if (!(colorNumber < color.Count))
                        colorNumber = 0;

                    c = color[colorNumber];
                }
                else
                    c = color.First();
                x = Random.Range(0, 225);
                width = Random.Range(mond.minWidth, mond.maxWidth);
                y = Random.Range(0, 225);
                height = Random.Range(mond.minHeight, mond.maxHeight);
                for (int u = x - width; u < x + width + 1; u++)
                    for (int v = y - height; v < y + height + 1; v++)
                        tex.SetPixel(u, v, c);
                tex.Apply();
            }
            else if (i % 5 == 2)     //circle
            {
                if (color.Count > 1)
                {
                    colorNumber++;
                    if (!(colorNumber < color.Count))
                        colorNumber = 0;

                    c = color[colorNumber];
                }
                else
                    c = color.First();
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
            else if (i % 5 == 3)     //square
            {
                if (color.Count > 1)
                {
                    colorNumber++;
                    if (!(colorNumber < color.Count))
                        colorNumber = 0;

                    c = color[colorNumber];
                }
                else
                    c = color.First();
                x = Random.Range(0, 225);
                size = Random.Range(mond.minWidth, mond.maxWidth);
                y = Random.Range(0, 225);
                for (int u = x - size; u < x + size + 1; u++)
                    for (int v = y - size; v < y + size + 1; v++)
                        tex.SetPixel(u, v, c);
                tex.Apply();
            }
            else if(i % 5 == 4)
            {
                if (color.Count > 1)
                {
                    colorNumber++;
                    if (!(colorNumber < color.Count))
                        colorNumber = 0;

                    c = color[colorNumber];
                }
                else
                    c = color.First();
                // Randomly determine the vertices and sizes of the triangle
                Vector2 vertex1 = new Vector2(Random.Range(0, tex.width), Random.Range(0, tex.height));
                Vector2 vertex2 = new Vector2(Random.Range(vertex1.x + mond.minWidth, vertex1.x + mond.maxWidth), Random.Range(vertex1.y + mond.minHeight, vertex1.y + mond.maxHeight));
                Vector2 vertex3 = new Vector2(Random.Range(vertex1.x + mond.minWidth, vertex1.x + mond.maxWidth), Random.Range(vertex1.y + mond.minHeight, vertex1.y + mond.maxHeight));

                // Random width and height for the triangle
                width = Random.Range(mond.minWidth, mond.maxWidth); // Adjust the range as needed
                height = Random.Range(mond.minHeight, mond.maxHeight); // Adjust the range as needed

                // Modify vertices based on width and height

                vertex2.x = vertex1.x + width;
                vertex3.x = vertex1.x + (width / 2);
                vertex3.y = vertex1.y - height;
                Debug.Log(vertex1);
                Debug.Log(vertex2);
                Debug.Log(vertex3);

                // Sort vertices by y-coordinate
                Vector2[] vertices = { vertex1, vertex2, vertex3 };
                System.Array.Sort(vertices, (a, b) => a.y.CompareTo(b.y));


                float dx1 = (vertices[1].x - vertices[0].x) / (vertices[1].y - vertices[0].y);
                float dx2 = (vertices[2].x - vertices[0].x) / (vertices[2].y - vertices[0].y);

                float x1 = vertices[0].x;
                float x2 = vertices[0].x;
                Debug.Log(dx1);
                Debug.Log(dx2);

                // Scanline filling
                for (int h = (int)vertices[0].y; h <= vertices[2].y; h++)
                {
                    for (int j = Mathf.FloorToInt(x1); j <= Mathf.CeilToInt(x2); j++)
                    {
                        if (j >= 0 && j < tex.width && h >= 0 && h < tex.height)
                            tex.SetPixel(j, h, c);
                    }
                    x1 += dx1;
                    x2 += dx2;
                }
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
        row.Add(new Color((float)255/255, 0, 0));      //red
        row.Add(new Color(0, (float)255 / 255, 0));      //green
        row.Add(new Color(0, 0, (float)255 / 255));      //blue
        row.Add(new Color((float)255 / 255, 0, (float)255 / 255));    //magenta
        row.Add(new Color((float)255 / 255, (float)255 / 255, 0));    //yellow
        row.Add(new Color(0, (float)255 / 255, (float)255 / 255));    //cyan
        colors.Add("0", row);
        row = new List<Color>();
        if (Experiment.current.paletteUploadNeeded)
        {
            string palettePath = Experiment.current.inputPath, key = "";                //its called image path because I copied my code from the image uploader
            Debug.Log(palettePath);
            while (!palettePath[palettePath.Length - 1].Equals('\\'))                   //we want to remove the original file name so that we can find the directory and navigate to the correct location
            {
                palettePath = palettePath.Remove(palettePath.Length - 1, 1);
            }
            palettePath += "colorPalette.csv";
            Debug.Log(palettePath);

            if (palettePath.Length != 0 && File.Exists(palettePath))
            {
                using (var sr = new StreamReader(palettePath))      //open up a stream reader on the path
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
                        red = green = blue = -1;
                        for (int i = 0; i < values.Length; i++)     //loop through each line with a line being its own palette
                        {
                            //Debug.Log(values.Length);
                            Debug.Log(values[i]);
                            if (counter > 1)
                            {
                                if (i == 0)                                 //ignore first column
                                {
                                    key = values[i].ToString();
                                }
                                else if (red == -1)                         //the order goes r g b so everytime we loop through a line we will populate accordingly
                                {
                                    if (values[i] != System.String.Empty)
                                        red = System.Int32.Parse(values[i]);
                                }
                                else if (green == -1)
                                {
                                    if (values[i] != System.String.Empty)
                                        green = System.Int32.Parse(values[i]);
                                }
                                else if (blue == -1)
                                {
                                    if (values[i] != System.String.Empty)
                                        blue = System.Int32.Parse(values[i]);
                                }
                                else
                                {                                           //once these are all filled add teh color and start again
                                    row.Add(new Color((float)red / 255, (float)green / 255, (float)blue / 255));
                                    red = green = blue = -1;
                                    if (values[i] != System.String.Empty)
                                        red = System.Int32.Parse(values[i]);
                                }
                            }
                        }
                        if (counter > 1)                //ignore first row
                        {
                            if (!(red == blue && blue == green && green == -1))
                                row.Add(new Color((float)red / 255, (float)green / 255, (float)blue / 255));
                            colors.Add(key, row);
                            row = new List<Color>();
                        }
                        counter++;
                    }
                    sr.Close();
                }
            }
        }
    }
}
