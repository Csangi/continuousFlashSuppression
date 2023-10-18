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
    private bool pix;       //if the mondrian should be pixelated
    private string mond = "";   //the name of the mond when it is saved to the csv
    private int palette, sh, minWidth, maxWidth, minHeight, maxHeight, density, num, mondNum, numOfMasksWritten;    //holder values for all the input fields
    public RawImage rawImage;   //the main image in the mask maker
    public Image[] paletteHolders;  //all the images around the mondrain
    public GameObject errorText;    //the error text in the bottom left of the screen
    List<List<Color>> colors = new List<List<Color>>();     //2d list of palettes
    List<Color> newPalette = new List<Color>(); //new palette used for the palette maker
    private bool savingMond, mondDrawn;         //for when the mond is drawn and we are currently trying to save it to an output folder
    private string outputPath = "";             //string output path
    private string folderName = "";             //name of the folder the masks will be output to
    private string maskPath = "";               //where the mond csv is uploaded from
    private string palettePath = "";            //where the palette csv is uploaded from
    public Dropdown paletteDropdown, maskDropdown, shapeDropdown, paletteDropdown2;   //all the dropdowns from the mask maker
    public InputField minH, maxH, minW, maxW, DensityIF;    //input fields from the mask maker identified with IF
    public Canvas maskMakerCanvas, paletteCanvas;   //both canvases
    public Transform maskBuildTransform, paletteBuildTransform, saveMaskTransform, savePaletteTransform;    //the game objects for the animations when switching between mask maker and palette maker
    List<Mondrian> MondList = new List<Mondrian>();     //the list of all the mondrians used for saving them to a csv
    List<string> colorPalettenames = new List<string>();    //the names of the color palettes as they are usually stored in a dictionary
    public Toggle pixels;       //the switch for if the image should be pixelated or not
    public Scrollbar redSB, greenSB, blueSB;    //the scrollbar game objects for the palette maker
    private float newPaletteRed = 0, newPaletteGreen = 0, newPaletteBlue = 0;       //the float holders for the palette maker
    public Text greenText, blueText, redText;       //the text for the scroll bars in the palette makers
    private string nameOfNewPalette = "", nameOfNewMask = "";   //naming for both the palette and the mask makers
    public List<Button> paletteHolderButtons = new List<Button>();      //the buttons that allow the user to edit their palettes in the palette maker


    void Start()
    {
        setColorValues();
        palette = 0;
        sh = 0;
        colorPalettenames.Add("Neon");
        colorPalettenames.Add("Black & White");
        redSB.onValueChanged.AddListener(RedOnScrollBarChange);
        blueSB.onValueChanged.AddListener(BlueOnScrollBarChange);
        greenSB.onValueChanged.AddListener(GreenOnScrollBarChange);
        displayNewPalette();
    }

    public void removeColor()
    {
        if (newPalette.Count > 0)
        {
            newPalette.RemoveAt(newPalette.Count - 1);
            displayNewPalette();
        }
    }

    public void addColorToPalette()
    {
        newPalette.Add(new Color(newPaletteRed, newPaletteGreen, newPaletteBlue));
        displayNewPalette();
    }

    public void addPaletteToSet()
    {
        if (newPalette.Count < 1)
        {
            errorText.GetComponent<Text>().text = "There are no colors in this palette. ";
            errorText.GetComponent<Text>().color = Color.red;
        }
        else if(colorPalettenames.Count(n => n == nameOfNewPalette) > 0)
        {
            errorText.GetComponent<Text>().text = "There is already a Palette with this name in the palette file.";
            errorText.GetComponent<Text>().color = Color.red;
        }
        else if (nameOfNewPalette == String.Empty)
        {
            colors.Add(newPalette);
            colorPalettenames.Add("New Palette " + colors.Count);
            paletteDropdown.options.Add(new Dropdown.OptionData("New Palette " + colors.Count));
            paletteDropdown2.options.Add(new Dropdown.OptionData("New Palette " + colors.Count));
            newPalette = new List<Color>();
            displayNewPalette();
            errorText.GetComponent<Text>().text = "Palette " + "New Palette " + colors.Count + " has been added.";
            errorText.GetComponent<Text>().color = Color.white;
        }
        else
        {
            colors.Add(newPalette);
            colorPalettenames.Add(nameOfNewPalette);
            paletteDropdown.options.Add(new Dropdown.OptionData(nameOfNewPalette));
            paletteDropdown2.options.Add(new Dropdown.OptionData(nameOfNewPalette));
            newPalette = new List<Color>();
            displayNewPalette();
            errorText.GetComponent<Text>().text = "Palette " + nameOfNewPalette + " has been added.";
            errorText.GetComponent<Text>().color = Color.white;
        }
    }

    public void clearPaletteColors()
    {
        for (int i = 0; i < 16; i++)
            paletteHolders[i].color = Color.white;
    }

    private void displayNewPalette(int index = 0)
    {
        clearPaletteColors();
        if (maskMakerCanvas.enabled)
            for (int i = 0; i < 16 && i < colors[index].Count; i++)
                paletteHolders[i].color = colors[index][i];
        else if(paletteCanvas.enabled)
            for (int i = 0, j = newPalette.Count - 1; i < 16 && j > -1; i++, j--)
                paletteHolders[i].color = newPalette[j];

        for(int i = 0; i < 16; i++)
        {
            paletteHolderButtons[i].image.color = paletteHolders[i].color;
        }
    }

    public void addPaletteToCSVCaller()
    {
        if (palettePath == String.Empty)
        {
            errorText.GetComponent<Text>().text = "You need to upload a Palette CSV to add this palette to";
            errorText.GetComponent<Text>().color = Color.red;
        }
        else
        {
            addPaletteToCSV(newPalette);
            addPaletteToSet();
            errorText.GetComponent<Text>().text = "Palette " + nameOfNewPalette + " has been added to CSV.";
            errorText.GetComponent<Text>().color = Color.white;
            clearPaletteColors();
        }
    }

    private void addPaletteToCSV(List<Color> ListToPrint)
    {
        if(palettePath != String.Empty)
        {
            try
            {
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(palettePath, true))
                {
                    string itemToWrite = "";
                    if(nameOfNewPalette == String.Empty || colorPalettenames.Count(n => n == nameOfNewPalette) > 0)
                        itemToWrite += "New Palette " + (colors.Count + 1) + ","; 
                    else
                        itemToWrite += nameOfNewPalette + ",";

                    foreach (Color c in ListToPrint)
                    {
                        // Format the RGB values as a comma-separated string (CSV format).
                        string csvData = $"{(int)(c.r * 255f)},{(int)(c.g * 255f)},{(int)(c.b * 255f)},";
                        // Write the data to the CSV file.
                        itemToWrite += csvData;
                    }
                    writer.Write("\n" + itemToWrite);
                }
                Debug.Log("Colors written to CSV successfully.");
            }
            catch (Exception ex)
            {
                Debug.Log("Error writing colors to CSV: " + ex.Message);
            }
        }
    }

    public void addMaskToCSV()
    {
        if (maskPath != String.Empty)
        {
            try
            {
                using (System.IO.StreamWriter writer = new System.IO.StreamWriter(maskPath, true))
                {
                    if (nameOfNewMask == String.Empty)
                        writer.Write("\n" + "New Mask " + numOfMasksWritten.ToString() + "," + colorPalettenames[palette] + "," + (sh + 1).ToString() + "," + Convert.ToInt32(pix) + "," + minWidth.ToString() + "," + maxWidth.ToString() + "," + minHeight.ToString() + "," + maxHeight.ToString() + "," + density.ToString());
                    else
                        writer.Write("\n" + nameOfNewMask + "," + colorPalettenames[palette] + "," + (sh + 1).ToString() + "," + Convert.ToInt32(pix) + "," + minWidth.ToString() + "," + maxWidth.ToString() + "," + minHeight.ToString() + "," + maxHeight.ToString() + "," + density.ToString());
                }
                Debug.Log("Masks written to CSV successfully.");
                if(nameOfNewMask != String.Empty)
                    errorText.GetComponent<Text>().text = "Mask " + nameOfNewMask + " has been added to CSV.";
                else
                    errorText.GetComponent<Text>().text = "New Mask " + numOfMasksWritten.ToString() + " has been added to CSV.";
                errorText.GetComponent<Text>().color = Color.white;
                numOfMasksWritten++;
            }
            catch (Exception ex)
            {
                Debug.Log("Error writing Masks to CSV: " + ex.Message);
            }
        }
        else
        {
            errorText.GetComponent<Text>().color = Color.red;
            errorText.GetComponent<Text>().text = "You need to upload a Mask CSV to add this Mask to";
        }
    }

    private void RedOnScrollBarChange(float value)
    {
        newPaletteRed = value;
        redText.text = (Math.Round(value * 255, 0)).ToString();
    }

    private void GreenOnScrollBarChange(float value)
    {
        newPaletteGreen = value;
        greenText.text = (Math.Round(value * 255,0)).ToString();
    }

    private void BlueOnScrollBarChange(float value)
    {
        newPaletteBlue = value;
        blueText.text = (Math.Round(value * 255, 0)).ToString();
    }

    private void Update()
    {
        if(paletteCanvas.enabled)
        {
            rawImage.color = new Color(newPaletteRed, newPaletteGreen, newPaletteBlue);
        }
    }


    //Most of the code in this section is for the input GUI
    //--------------------------------GUI------------------------------------------
    public void changePalette(int pal)
    {
        palette = pal;
        newPalette.Clear();
        foreach(Color c in colors[pal])
            newPalette.Add(c);
        displayNewPalette(palette);
    }

    public void changePaletteName(string str)
    {
        nameOfNewPalette = str;
    }

    public void changeNewMaskName(string str)
    {
        nameOfNewMask = str;
    }

    public void useMask(int m)
    {
        int holder = colorPalettenames.IndexOf(MondList[m].palette);
        if (holder != -1)
        {
            palette = holder;
            paletteDropdown.value = holder;
        }
        else
        {
            palette = 0;
            paletteDropdown.value = 0;
        }

        switch (MondList[m].shape)
        {
            case Shape.ellipse:
                sh = 0;
                shapeDropdown.value = 0;
                break;
            case Shape.rectangle:
                sh = 1;
                shapeDropdown.value = 1;
                break;
            case Shape.triangle:
                sh = 2;
                shapeDropdown.value = 2;
                break;
            case Shape.pixelated:
                sh = 3;
                shapeDropdown.value = 3;
                break;
            case Shape.circle:
                sh = 4;
                shapeDropdown.value = 4;
                break;
            case Shape.square:
                sh = 5;
                shapeDropdown.value = 5;
                break;
            case Shape.mixed:
                sh = 6;
                shapeDropdown.value = 6;
                break;
            default:
                sh = 0;
                shapeDropdown.value = 0;
                break;
        }

        pixels.isOn = MondList[m].addPixelated;
        pix = MondList[m].addPixelated;

        minHeight = MondList[m].minHeight;
        minH.text = MondList[m].minHeight.ToString();

        maxHeight = MondList[m].maxHeight;
        maxH.text = MondList[m].maxHeight.ToString();

        minWidth = MondList[m].minWidth;
        minW.text = MondList[m].minWidth.ToString();

        maxWidth = MondList[m].maxWidth;
        maxW.text = MondList[m].maxWidth.ToString();

        density = MondList[m].density;
        DensityIF.text = MondList[m].density.ToString();

        runMondrianCreator();
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

    public void clearMondrian()
    {
        createMond.makeBlank(rawImage.texture as Texture2D);
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
        Mondrian m = new Mondrian("0", sh + 1, pix, minWidth, maxWidth, minHeight, maxHeight, density);
        mondDrawn = false;
        m.printMond();

        createMond.makeBlank(rawImage.texture as Texture2D);

        Debug.Log(colors[palette].Count());

        if (m.shape == Shape.ellipse)
        {
            rawImage.texture = createMond.DrawEllipse(rawImage.texture as Texture2D, colors[palette], m);
            Debug.Log("ellipse");
        }
        else if (m.shape == Shape.rectangle)
        {
            rawImage.texture = createMond.DrawRectangle(rawImage.texture as Texture2D, colors[palette], m);
            Debug.Log("rectangle");
        }
        else if (m.shape == Shape.triangle)
        {
            rawImage.texture = createMond.DrawTriangle(rawImage.texture as Texture2D, colors[palette], m);
            Debug.Log("triangle");
        }
        else if (m.shape == Shape.pixelated)
        {
            rawImage.texture = createMond.CreatePixelated(rawImage.texture as Texture2D, colors[palette], m);
            Debug.Log("Pixelated");
        }
        else if (m.shape == Shape.circle)
        {
            rawImage.texture = createMond.DrawCircle(rawImage.texture as Texture2D, colors[palette], m);
            Debug.Log("Circle");
        }
        else if (m.shape == Shape.square)
        {
            rawImage.texture = createMond.DrawSquare(rawImage.texture as Texture2D, colors[palette], m);
            Debug.Log("Square");
        }
        else if (m.shape == Shape.mixed)
        {
            rawImage.texture = createMond.DrawMixed(rawImage.texture as Texture2D, colors[palette], m);
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

    public void openPaletteMaker()
    {
        StartCoroutine(OpenPaletteAnimations());
    }

    private IEnumerator OpenPaletteAnimations()
    {
        //animations for the canvas
        maskBuildTransform.LeanMoveLocalX(-1600, .5f).setEaseOutExpo();
        saveMaskTransform.LeanMoveLocalX(1700, .5f).setEaseOutExpo();
        yield return new WaitForSecondsRealtime(.5f);
        maskMakerCanvas.enabled = false;
        clearMondrian();            //White out the image
        //more animations for the canvas
        paletteBuildTransform.localPosition = new Vector2(-Screen.width,0);
        savePaletteTransform.localPosition = new Vector2(Screen.width,-300);
        paletteCanvas.enabled = true;
        paletteBuildTransform.LeanMoveLocalX(0, .5f).setEaseInExpo();
        savePaletteTransform.LeanMoveLocalX(510, .5f).setEaseInExpo();
        displayNewPalette();        //Display a new palette
        setButtons(true);
    }

    public void openMaskMaker()
    {
        StartCoroutine(OpenMaskMakerAnimations());
    }

    private IEnumerator OpenMaskMakerAnimations()
    {
        //animations for the canvas
        paletteBuildTransform.LeanMoveLocalX(-Screen.width, .5f).setEaseOutExpo();
        savePaletteTransform.LeanMoveLocalX(2 * Screen.width, .5f).setEaseOutExpo();
        yield return new WaitForSecondsRealtime(.5f);
        paletteCanvas.enabled = false;
        rawImage.color = Color.white;
        maskMakerCanvas.enabled = true;
        //more animations for the canvas
        maskBuildTransform.localPosition = new Vector2(-1600,-570);
        saveMaskTransform.localPosition = new Vector2(1700, -870);
        maskBuildTransform.LeanMoveLocalX(-410, .5f).setEaseInExpo();
        saveMaskTransform.LeanMoveLocalX(510, .5f).setEaseInExpo();
        displayNewPalette(palette); //Display the new palette
        setButtons(false);
    }

    private void setButtons(bool boolean)
    {
        foreach (Button but in paletteHolderButtons)
            but.enabled = boolean;
    }

    public void paletteHolderButtonPress(int holderNumber)
    {
        holderNumber = newPalette.Count - holderNumber - 1; 
        if (!(holderNumber >= newPalette.Count) && newPalette.Count != 0 && holderNumber >= 0 )
        {
            Color holder = newPalette[holderNumber];
            newPalette.RemoveAt(holderNumber);
            greenSB.value = holder.g;
            blueSB.value = holder.b;
            redSB.value = holder.r;
            errorText.GetComponent<Text>().color = Color.white;
            errorText.GetComponent<Text>().text = "";
            displayNewPalette();
        }
        else
        {
            errorText.GetComponent<Text>().color = Color.red;
            errorText.GetComponent<Text>().text = "Please select a color that has a value";
        }
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
        if (!savingMond && mondDrawn && Directory.Exists(outputPath))
        {
            savingMond = true;
            StartCoroutine(saveMondrian());     //send to the mondrian saver
            savingMond = false;
        }
        else
        {
            errorText.GetComponent<Text>().color = Color.red;
            errorText.GetComponent<Text>().text = "No Mask was drawn. (Make sure to upload a folder)";
        }
    }

    public IEnumerator saveMondrian()       // this function will find the output path and draw the specified number of mondrians and save them
    {
        string path = outputPath + "\\" + folderName;
        if (!Directory.Exists(path))                  //if the directory DNE then create it
            Directory.CreateDirectory(path);

        runMondrianCreator();
        for (int i = 0; i < mondNum; ++i)                                           //loop waiting for the mond to be drawn. 
        {
            yield return new WaitForEndOfFrame();                                   //wait until it is drawn
            byte[] byteArr = (rawImage.texture as Texture2D).EncodeToPNG();         //change the texture 2D to a byte Arr
            System.IO.File.WriteAllBytes(path + "\\" + mond + num + ".png", byteArr);     //and save that byte Arr to png in the specified directory
            Debug.Log(path + "\\" + mond + num + ".png");
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
        row.Add(new Color((float)255 / 255, 0, 0));      //red
        row.Add(new Color(0, (float)255 / 255, 0));      //green
        row.Add(new Color(0, 0, (float)255 / 255));      //blue
        row.Add(new Color((float)255 / 255, 0, (float)255 / 255));    //magenta
        row.Add(new Color((float)255 / 255, (float)255 / 255, 0));    //yellow
        row.Add(new Color(0, (float)255 / 255, (float)255 / 255));    //cyan
        colors.Add(row);
        row = new List<Color>();
        row.Add(new Color(0, 0, 0));            //black
        row.Add(new Color((float)255 / 255, (float)255 / 255, (float)255 / 255));      //white
        row.Add(new Color((float)192/255, (float)192 / 255, (float)192 / 255));      //light gray
        row.Add(new Color((float)128 / 255, (float)128 / 255, (float)128 / 255));      //grey
        colors.Add(row);
    }

    public void uploadPaletteButton()
    {
        string[] holderPath;
        string path = "";
        try
        {
            holderPath = SFB.StandaloneFileBrowser.OpenFilePanel("Open File", "", "csv", true);
            foreach (var i in holderPath)
                path += i;
            palettePath = path;
            uploadPalette(path);
        }
        catch (IOException)
        {
            errorText.GetComponent<Text>().color = Color.red;
            errorText.GetComponent<Text>().text = "There was an error with the upload (make sure that the csv is closed in all editors, the file cannot be read from 2 places at once";
        }
        catch (IndexOutOfRangeException)
        {
            errorText.GetComponent<Text>().color = Color.red;
            errorText.GetComponent<Text>().text = "Something was wrong with the formating of your path upload (IOOR)";
        }
        catch (NullReferenceException)
        {
            errorText.GetComponent<Text>().color = Color.red;
            errorText.GetComponent<Text>().text = "Something was wrong with the formating of your path upload (NRE)";
        }
    }

    public void uploadMasksButton()
    {
        string[] holderPath;
        string path = "";
        try
        {
            holderPath = SFB.StandaloneFileBrowser.OpenFilePanel("Open File", "", "csv", true);
            foreach (var i in holderPath)
                path += i;
            maskPath = path;
            readMondrians(path);
        }
        catch (IOException)
        {
            errorText.GetComponent<Text>().color = Color.red;
            errorText.GetComponent<Text>().text = "There was an error with the upload (make sure that the csv is closed in all editors, the file cannot be read from 2 places at once";
        }
        catch (IndexOutOfRangeException)
        {
            errorText.GetComponent<Text>().color = Color.red;
            errorText.GetComponent<Text>().text = "Something was wrong with the formating of your path upload (IOOR)";
        }
        catch (NullReferenceException)
        {
            errorText.GetComponent<Text>().color = Color.red;
            errorText.GetComponent<Text>().text = "Something was wrong with the formating of your path upload (NRE)";
        }
    }

    private void readMondrians(string mondPath)     //exact logic from previous upload 
    {
        Debug.Log(mondPath);
        maskDropdown.options.Clear();
        int counter = 0, shape = 0, minW = 0, maxW = 0, minH = 0, maxH = 0, Density = 0;
        string key = "", palette = "0";
        bool pix = false;
        if (mondPath.Length != 0 && File.Exists(mondPath))
        {
            using (var sr = new StreamReader(mondPath))
            {
                bool EOF = false;
                while (!EOF)
                {
                    string data = sr.ReadLine();
                    if (data == null)
                    {
                        EOF = true;
                        break;
                    }
                    var values = data.Split(',');
                    for (int i = 0; i < values.Length; i++)
                    {
                        if (counter > 0)
                        {
                            switch (i)
                            {
                                case 0:                         //here we make our default mondrain and take in the palette
                                    if (counter > 1)
                                    {
                                        MondList.Add(new Mondrian(palette, shape, pix, minW, maxW, minH, maxH, Density));
                                        maskDropdown.options.Add(new Dropdown.OptionData(key));
                                    }
                                    key = values[i];
                                    break;
                                case 1:                         //here we take in the shape
                                    palette = values[i];
                                    break;
                                case 2:
                                    shape = Int32.Parse(values[i]);
                                    break;
                                case 3:                         //minimum width
                                    if (Int32.Parse(values[i]) == 0)
                                        pix = false;
                                    else
                                        pix = true;
                                    break;
                                case 4:
                                    minW = Int32.Parse(values[i]);
                                    break;
                                case 5:                         //maximum width
                                    maxW = Int32.Parse(values[i]);
                                    break;
                                case 6:                         //minimum height
                                    minH = Int32.Parse(values[i]);
                                    break;
                                case 7:                         //maximum height
                                    maxH = Int32.Parse(values[i]);
                                    break;
                                case 8:                         //density
                                    Density = Int32.Parse(values[i]);
                                    break;
                            }
                        }
                    }
                    counter++;
                }
                sr.Close();
            }
            MondList.Add(new Mondrian(palette, shape, pix, minW, maxW, minH, maxH, Density));
            maskDropdown.options.Add(new Dropdown.OptionData(key));
            errorText.GetComponent<Text>().color = Color.white;
            errorText.GetComponent<Text>().text = "Mond file uploaded";
        }
        else
        {
            errorText.GetComponent<Text>().color = Color.red;
            errorText.GetComponent<Text>().text = "Could not find Mond file. ";
        }
    }

    public void uploadFolderButton()
    {
        string[] holderPath;
        string path = "";
        try
        {
            holderPath = SFB.StandaloneFileBrowser.OpenFolderPanel("Open File", "", false);
            foreach (var i in holderPath)
                path += i;
            Debug.Log(path);
            changePath(path);
        }
        catch (IOException)
        {
            errorText.GetComponent<Text>().color = Color.red;
            errorText.GetComponent<Text>().text = "There was an error with the upload (make sure that the csv is closed in all editors, the file cannot be read from 2 places at once";
        }
        catch (IndexOutOfRangeException)
        {
            errorText.GetComponent<Text>().color = Color.red;
            errorText.GetComponent<Text>().text = "Something was wrong with the formating of your path upload (IOOR)";
        }
        catch (NullReferenceException)
        {
            errorText.GetComponent<Text>().color = Color.red;
            errorText.GetComponent<Text>().text = "Something was wrong with the formating of your path upload (NRE)";
        }
    }

    private void uploadPalette(string palettePath)
    {
        /*
        colors.Clear();
        setColorValues();
        paletteDropdown.options.Clear();
        paletteDropdown2.options.Clear();
        colorPalettenames.Clear();
        paletteDropdown.options.Add(new Dropdown.OptionData("Neon"));
        paletteDropdown.options.Add(new Dropdown.OptionData("Black & White"));
        paletteDropdown2.options.Add(new Dropdown.OptionData("Neon"));
        paletteDropdown2.options.Add(new Dropdown.OptionData("Black & White"));
        colorPalettenames.Add("Neon");
        colorPalettenames.Add("Black & White");
        */
        int counter = 0, red = -1, green = -1, blue = -1;
        string name = "";
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
                    red = green = blue = -1;
                    for (int i = 0; i < values.Length; i++)             //loop through each line with a line being its own palette
                    {
                        if (counter > 1)
                        {
                            Debug.Log(values[i]);
                            if (i == 0)                                 //ignore first column
                            {
                                name = values[i].ToString();
                            }
                            else if (red == -1)                         //the order goes r g b so everytime we loop through a line we will populate accordingly
                            {
                                if(values[i] != String.Empty)
                                    red = System.Int32.Parse(values[i]);
                            }
                            else if (green == -1)
                            {
                                if (values[i] != String.Empty)
                                    green = System.Int32.Parse(values[i]);
                            }
                            else if (blue == -1)
                            {
                                if (values[i] != String.Empty)
                                    blue = System.Int32.Parse(values[i]);
                            }
                            else
                            {                                           //once these are all filled add teh color and start again
                                row.Add(new Color((float)red/255, (float)green/255, (float)blue/255));
                                red = green = blue = -1;
                                if (values[i] != String.Empty)
                                    red = System.Int32.Parse(values[i]);
                            }
                        }
                    }
                    if (counter > 1)            //ignore first row
                    {
                        if(!( red == blue && blue == green && green == -1))
                            row.Add(new Color((float)red / 255, (float)green / 255, (float)blue / 255));
                        colors.Add(row);
                        row = new List<Color>();
                        paletteDropdown.options.Add(new Dropdown.OptionData(name));
                        paletteDropdown2.options.Add(new Dropdown.OptionData(name));
                        colorPalettenames.Add(name);
                    }               //add the drop down option
                    counter++;
                    Debug.Log("counter = " + counter);
                }
                sr.Close();
            }
            int p = 0;
            foreach(List<Color> c in colors)
            {
                p++;
                Debug.Log(p);
                foreach(Color C in c)
                {
                    Debug.Log("Color: " + C);
                }
            }
            errorText.GetComponent<Text>().color = Color.white;
            errorText.GetComponent<Text>().text = "Palette file uploaded";
        }
        else
        {
            errorText.GetComponent<Text>().color = Color.red;
            errorText.GetComponent<Text>().text = "Could not find palette file. ";
        }
    }
}
