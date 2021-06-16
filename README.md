**Suppression Test Beta**

Psychological experiment created with Unity to test image suppression using the VR.
Commissioned by [Dr. David March] (https://psy.fsu.edu/faculty/marchd/march.dp.php) at Florida State University.

**Important Info**
The images used in the expirement must be stored in the same location as the loaded csv. The output logs will be stored within this directory.

One trial is the length of one image's screen time.

CSV file order:
1. trial_type: 1 = instruction, 2 = image
2. block: for randomization within blocks
3. block_rand: for randomazation within blocks
4. trial: trial number
5. trial_rand:for randomazation within blocks
6. duration (ms): length of trial
7. flash duration: how quickly mondrians flash
8. image: file name of image
9. opacity (%): max opacity the image will reach
10. delay (ms): time within a trial for the image to start fading in. if its 200ms, then the image starts fading in at 200ms.
11. time to reach opacity (ms): time it takes for the image to reach the defined max opacity.

All CSV cells must be filled! Instruction cells can be filled with 0 as needed.

**Scenes**
*creator: upload csv, choose dominant eye, enter ID
*exp: actual expirement. flashes images and mondrains based on the uploaded csv
*end: prints output log to "output_logs" dir. creates dir if not found.

**Important Directories and Files**
Oculus
*assest/Oculus/VR/Scripts/OVRCameraRig.cs: commented out function that auto adjusted left/center/right cameras.

Scenes
*CreateLog: automatically prints out log to file after experiment ends
*createMondrians: creates mondrians by adjusting the position and color of 140 circle sprites. 
*fadeStatic: takes uploaded csv infomation and fades in the image to the correct eye. also switchs the mondrians to the correct eye. this is the main file that runs the experiment part.
*UIManager: takes in csv and formats it into lists for each csv header. passes it to next scene with DontDestroyOnLoad.

**Implemented Features**
*Upload UI: takes in csv, patient ID, and dominant eye. saves info
*Generate mondrians,m eliminating need for uploading pictures of mondrians. flash speed controlled by csv
*randomizes trials
*runs full csv file and ends once finished.
*prints output log once finished

**TODO**
*fade in for images doesn't work, possibly because of refresh rate. Images have to slowly fade in in less than a second (changes based on csv, but this is the current testing duration time)
*images need to be overlapped in VR headset. probably need to uncomment my code in the oculus folder.
