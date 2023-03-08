**Suppression Test Alpha**

Psychological experiment created with Unity to test image suppression using the VR.
Commissioned by Dr. David March (https://psy.fsu.edu/faculty/marchd/march.dp.php) at Florida State University.

**Important Info**

Use 1 for true and 0 for false. 

All CSV cells must be filled! Instruction cells can be filled with 0 as needed.

CSV file order:

!!  First header line will be ignored  !!

  1. Condition
  2. Condition Random
  3. Block
  4. Block Random
  5. Trial type
  6. Trial
  7. Trial Random
  8. Image
  9. Duration(ms)
  10. Flash Duration(ms)
  11. Opacity(%)
  12. Mask Delay(ms)
  13. Static Delay(ms)
  14. Masks (optional)
    *for response input*
  15. UP (optional)
  16. DOWN (optional)
  17. Left (optional)
  18. Right (optional)
  19. Flash Period(ms) (optional)

*Uploading images*

!!  All images must be in a folder named "Static" in the same directory as the uploaded experiement file  !!

The images used in the expirement must be uploaded in the csv. The program does not provide any images to the user aside from pregenerated mondrians. 

  In the csv the user will state the name of a image file which they wish to use 
  or 
  They can list a text file containing the names of multiple image files, which should be found in the same "Static" folder, predesignated with a symbol refering to the type of input shown below. (no spaces)
  
    *Randomization types*
      # = pull from the list in the order it’s presented
      $ = pull randomly from the list without replacement
      & = pull randomly from the list with replacement
  
*Creating Mondrians* (optional)

!!  Mond.csv must be the name of your file and must be in the same directory as the originally uploaded csv  !!

  You can use the mondrian creator in the program to get a feel for your mondrian.
  Once you have found the mondrian or mondrians you wish to use add them to the mond csv and specify them in the Mask column of your upload
  
    -Mond.csv
    
    !!  First row is ignored  !!
    
      CSV file order:
      
      1. Number Mondrian
      2. Palette
      3. Shape
        - 0 : Ellipse
        - 1 : Rectangle
        - 2 : Triangle (triangle not currently working)
        - 3 : Pixelated
        - 4 : Circle
        - 5 : Square
        - 6 : Mixed
      4. Pixelated
      5. Minimum width
      6. Maximum width
      7. Minimum height
      8. Maximum height
      9. Density
  
  *Uploading Color Palettes* (optional)
  
  !!  colorPalette.csv must be the name of your file and must be in the same directory as the originally uploaded csv  !!
  
    If you wish to use your own color palettes for your mondrians you can upload them in the colorPalette csv
     -colorPalette.csv
      In colorPalette.csv you will type the RGB color values of your colors sequentially in a row per every palette. 
      The header row will be ignored and the first column is also ignored for numbering. Every row is a new palette and every 3 columns is a color. 
