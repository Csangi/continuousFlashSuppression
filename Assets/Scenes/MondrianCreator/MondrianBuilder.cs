using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MondrianBuilder : MonoBehaviour
{
    //public Mondrian(int palette, int sh, bool pix, int minWidth, int maxWidth, int minHeight, int maxHeight, int density)
    public bool pix;
    public string mond = "";
    public int palette, sh, minWidth, maxWidth, minHeight, maxHeight, density, num, mondNum;
    public RawImage rawImage;
    public GameObject errorText;
    List<List<Color>> colors = new List<List<Color>>();
    public bool savingMond, mondDrawn;
    public string outputPath = "";
    public string folderName = "";
    public Dropdown paletteDropdown;

    void Start()
    {
        setColorValues();
        palette = 0;
        sh = 0;
    }

    //Most of the code in this section is for the input GUI
    //--------------------------------GUI------------------------------------------
    public void changePalette(int pal)
    {
        palette = pal;
    }

    public void changeShape(int shape)
    {
        sh = shape;
    }

    public void changePix(bool pixelated)
    {
        pix = pixelated;
    }

    public void changeDensity(string d)
    {
        try
        {
            density = Int32.Parse(d);
            errorText.GetComponent<Text>().text = "";
        }
        catch (System.FormatException)
        {
            errorText.GetComponent<Text>().color = Color.red;
            errorText.GetComponent<Text>().text = "Please enter a number for density";
        }
    }

    public void changeMinWidth(string MW)
    {
        try
        {
            minWidth = Int32.Parse(MW);
            errorText.GetComponent<Text>().text = "";
        }
        catch (System.FormatException)
        {
            errorText.GetComponent<Text>().color = Color.red;
            errorText.GetComponent<Text>().text = "Please enter a number for Min Width";
        }
    }

    public void changeMaxWidth(string MW)
    {
        try
        {
            maxWidth = Int32.Parse(MW);
            errorText.GetComponent<Text>().text = "";
        }
        catch (System.FormatException)
        {
            errorText.GetComponent<Text>().color = Color.red;
            errorText.GetComponent<Text>().text = "Please enter a number for Max Width";
        }
    }

    public void changeMinHeight(string MH)
    {
        try
        {
            minHeight = Int32.Parse(MH);
            errorText.GetComponent<Text>().text = "";
        }
        catch (System.FormatException)
        {
            errorText.GetComponent<Text>().color = Color.red;
            errorText.GetComponent<Text>().text = "Please enter a number for Min Height";
        }
    }

    public void changeMaxHeight(string MH)
    {
        try
        {
            maxHeight = Int32.Parse(MH);
            errorText.GetComponent<Text>().text = "";
        }
        catch (System.FormatException)
        {
            errorText.GetComponent<Text>().color = Color.red;
            errorText.GetComponent<Text>().text = "Please enter a number for Max Height";
        }
    }

    public void backToOpening()
    {
        createMond.makeBlank(rawImage.texture as Texture2D);
        Experiment.current.sceneToBeLoaded = 0;
        SceneManager.LoadScene(0);
    }

    //--------------------------------------------End of input GUI for mondrians

    public void runMondrianCreator()        //this simplily creates the mondrian and makes it depending on the shape
    {
        Debug.Log("increator");
        Mondrian m = new Mondrian(palette, sh, pix, minWidth, maxWidth, minHeight, maxHeight, density);
        mondDrawn = false;
        m.printMond();

        createMond.makeBlank(rawImage.texture as Texture2D);

        Debug.Log(colors[m.palette].Count());

        if (m.shape == Shape.ellipse)
        {
            rawImage.texture = createMond.DrawEllipse(rawImage.texture as Texture2D, colors[m.palette], m);
            Debug.Log("ellipse");
        }
        else if (m.shape == Shape.rectangle)
        {
            rawImage.texture = createMond.DrawRectangle(rawImage.texture as Texture2D, colors[m.palette], m);
            Debug.Log("rectangle");
        }
        else if (m.shape == Shape.triangle)
        {
            rawImage.texture = createMond.DrawTriangle(rawImage.texture as Texture2D, colors[m.palette], m);
            Debug.Log("triangle");
        }
        else if (m.shape == Shape.pixelated)
        {
            rawImage.texture = createMond.DrawPixelated(rawImage.texture as Texture2D, colors[m.palette], m);
            Debug.Log("Pixelated");
        }
        else if (m.shape == Shape.circle)
        {
            rawImage.texture = createMond.DrawCircle(rawImage.texture as Texture2D, colors[m.palette], m);
            Debug.Log("Circle");
        }
        else if (m.shape == Shape.square)
        {
            rawImage.texture = createMond.DrawSquare(rawImage.texture as Texture2D, colors[m.palette], m);
            Debug.Log("Square");
        }
        else if (m.shape == Shape.mixed)
        {
            rawImage.texture = createMond.DrawMixed(rawImage.texture as Texture2D, colors[m.palette], m);
            Debug.Log("Mixed");
        }
        mondDrawn = true;
    }

    public void changeNameofMonds(string name)
    {
        mond = name;
    }

    public void changePath(string path)
    {
        outputPath = path;
    }

    public void changeFolderName(string name)
    {
        folderName = name;
    }

    public void changeNumberofMonds(string number)
    {
        try
        {
            mondNum = Int32.Parse(number);
            errorText.GetComponent<Text>().text = "";
        }
        catch (System.FormatException)
        {
            errorText.GetComponent<Text>().color = Color.red;
            errorText.GetComponent<Text>().text = "Please enter a number for density";
        }
    }

    public void saveMond()
    {
        if (!savingMond && mondDrawn)
        {
            savingMond = true;
            StartCoroutine(saveMondrian());     //send to the mondrian saver
            savingMond = false;
        }
        else
        {
            errorText.GetComponent<Text>().color = Color.red;
            errorText.GetComponent<Text>().text = "No Mondrian was drawn.";
        }
    }

    public IEnumerator saveMondrian()       // this function will find the output path and draw the specified number of mondrians and save them
    {
        outputPath += "\\" + folderName;
        if (!Directory.Exists(outputPath))                  //if the directory DNE then create it
            Directory.CreateDirectory(outputPath);

        for (int i = 0; i < mondNum; ++i)                                           //loop waiting for the mond to be drawn. 
        {
            yield return new WaitForEndOfFrame();                                   //wait until it is drawn
            byte[] byteArr = (rawImage.texture as Texture2D).EncodeToPNG();         //change the texture 2D to a byte Arr
            System.IO.File.WriteAllBytes(outputPath + "\\" + mond + num + ".png", byteArr);     //and save that byte Arr to png in the specified directory
            Debug.Log(outputPath + "\\" + mond + num + ".png");
            ++num;
            runMondrianCreator();                           //draw new mondrians
            yield return new WaitUntil(() => mondDrawn);    //wait until they are done. 
        }
        num = 0;        //reset num for the next mondrians. 
                        //this is how most people will expect it to run
    }

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
    }


    public void uploadPalette(string palettePath)
    {
        int counter = 0, red = -1, green = -1, blue = -1;
        List<Color> row = new List<Color>();
        Debug.Log(palettePath);

        if (palettePath.Length != 0 && File.Exists(palettePath))        //If the file exists 
        {
            using (var sr = new StreamReader(palettePath))              //open up a stream reader on the path
            {
                bool EOF = false;
                while (!EOF)                                            //read in line by line
                {
                    string data = sr.ReadLine();
                    if (data == null)
                    {
                        EOF = true;
                        break;
                    }
                    var values = data.Split(',');                       //input into an array
                    for (int i = 0; i < values.Length; i++)             //loop through each line with a line being its own palette
                    {
                        Debug.Log(values.Length);
                        Debug.Log(values[i]);
                        if (counter > 0)
                        {
                            int result = 0;
                            result = System.Int32.Parse(values[i]);
                            Debug.Log(result);
                            if (i == 0)                                 //ignore first column
                            {
                                Debug.Log("do nothing");
                            }
                            else if (red == -1)                         //the order goes r g b so everytime we loop through a line we will populate accordingly
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
                            {                                           //once these are all filled add teh color and start again
                                row.Add(new Color(red, green, blue));
                                red = green = blue = -1;
                                red = result;
                            }
                        }
                    }
                    if (counter > 0)            //ignore first row
                    {
                        colors.Add(row);
                        row = new List<Color>();
                        paletteDropdown.options.Add(new Dropdown.OptionData("input palette #" + counter));
                    }               //add the drop down option
                    counter++;
                }
                sr.Close();
            }
            errorText.GetComponent<Text>().color = Color.white;
            errorText.GetComponent<Text>().text = "";
        }
        else
        {
            errorText.GetComponent<Text>().color = Color.red;
            errorText.GetComponent<Text>().text = "Could not find palette file. ";
        }
    }
}
