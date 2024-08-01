using System;
using System.IO;
using UnityEngine;

public class UserGuideCopier : MonoBehaviour
{
    [SerializeField]
    private string _directoryName;

    void Start()
    {
        Debug.Log(Application.dataPath);
        
        // Define source (Program Files) and destination (Documents) paths
        string programFilesPath = Path.Combine(Directory.GetParent(Application.dataPath).FullName, _directoryName);
        Debug.Log(programFilesPath);

        string documentsPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), _directoryName);

        // Check if source directory exists
        if (Directory.Exists(programFilesPath))
        {
            // Check if the destination directory already exists
            if (!Directory.Exists(documentsPath))
            {
                // Copy directory
                CopyDirectory(programFilesPath, documentsPath);
            }
            else
            {
                Debug.Log("Directory already exists in Documents.");
            }
        }
        else
        {
            Debug.LogError("Source directory does not exist.");
        }
    }

    void CopyDirectory(string sourceDir, string destDir)
    {
        try
        {
            // Create the destination directory if it doesn't exist
            Directory.CreateDirectory(destDir);

            // Copy all files from source to destination
            foreach (string file in Directory.GetFiles(sourceDir))
            {
                string fileName = Path.GetFileName(file);
                string destFile = Path.Combine(destDir, fileName);
                File.Copy(file, destFile, true); // Overwrite if file exists
            }

            // Recursively copy all subdirectories
            foreach (string subdir in Directory.GetDirectories(sourceDir))
            {
                string dirName = Path.GetFileName(subdir);
                string destSubdir = Path.Combine(destDir, dirName);
                CopyDirectory(subdir, destSubdir);
            }
        }
        catch (Exception ex)
        {
            Debug.LogError($"Error copying directory: {ex.Message}");
        }
    }
}
