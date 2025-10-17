using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using FluentFTP;
using System.Net;

class Program
{
    static string ftpHost = "f33-preview.awardspace.net";
    static string ftpUser = "4692880_vladislavus";
    static string ftpPassword = "vXnrjj2!qLygjV4"; 

    static void Main()
    {
        Console.OutputEncoding = Encoding.UTF8;
        string localDir = Path.Combine(Environment.CurrentDirectory, "This PC/Documents/ftp_test");
        string remoteDir = "/home/www/test/";

        try
        {
            Directory.CreateDirectory(localDir);

            Console.WriteLine("Generating files...");
            GenerateFiles(localDir, 1000);

            Console.WriteLine("Connecting to FTP...");
            using var client = new FtpClient(ftpHost, new NetworkCredential(ftpUser, ftpPassword));
            client.Connect();

            Console.WriteLine("Creating remote folder...");
            if (!client.DirectoryExists(remoteDir))
                client.CreateDirectory(remoteDir);

            Console.WriteLine("Uploading files...\n");

            var stopwatch = Stopwatch.StartNew();
            int success = 0, failed = 0;

            foreach (var file in Directory.GetFiles(localDir))
            {
                string remotePath = remoteDir + Path.GetFileName(file);
                try
                {
                    client.UploadFile(file, remotePath, FtpRemoteExists.Overwrite);
                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine($" {Path.GetFileName(file)} loaded");
                    Console.ResetColor();
                    success++;
                }
                catch (Exception ex)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Fail! {Path.GetFileName(file)} — {ex.Message}");
                    Console.ResetColor();
                    failed++;
                }
            }

        }
        catch (Exception ex)
        {
            Console.ForegroundColor = ConsoleColor.Red;
            Console.WriteLine($"Error: {ex.Message}");
            Console.ResetColor();
        }
    }

    static void GenerateFiles(string dir, int count)
    {
        byte[] data = Encoding.ASCII.GetBytes("A");

        for (int i = 1; i <= count; i++)
        {
            string filePath = Path.Combine(dir, $"file_{i:D4}.txt");
            File.WriteAllBytes(filePath, data);
        }
    }
}
