using GarticTest.Model;
using GarticTest.Model.Messages.Client;
using GarticTest.Model.Messages.Server;
using SocketIOClient;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace GarticTest
{
    internal class Program
    {
        static void Main(string[] args)
        {
            Application.EnableVisualStyles();
            Console.WriteLine("=== Gartic Phone Auto Drawer ===");
            Console.WriteLine("This tool will automatically draw images in Gartic Phone using human-like strokes");
            Console.WriteLine("Created with DFS drawing algorithm for natural-looking results");
            Console.WriteLine();
            
            Console.Write("Nickname: ");
            string username = Console.ReadLine();
            Console.Write("Invite link: ");
            string code = Console.ReadLine().Replace("https://garticphone.com/ru/?c=", string.Empty);
            
            // Support other language URLs
            code = code.Replace("https://garticphone.com/en/?c=", string.Empty);
            code = code.Replace("https://garticphone.com/?c=", string.Empty);
            
            Console.WriteLine("Connecting to Gartic Phone...");
            GarticClient client = new GarticClient(username, "5", code);
            client.OnGameTurn += GameTurn;
            
            Console.WriteLine("Connected! Waiting for game to start...");
            Console.WriteLine("Press Ctrl+C to exit");
            
            while (true) Thread.Sleep(1000);
        }

        private static void GameTurn(GarticClient client, Model.Enums.TurnType turnType, int turnNumber, int previousUserId, object previousTurnData)
        {
            if (client.Players.ContainsKey(previousUserId))
                Console.WriteLine($"Previous turn was made by: {client.Players[previousUserId].Nick}");

            if (turnType == Model.Enums.TurnType.Sentence)
            {
                Console.Write("Come up with a sentence for others to draw: ");
                client.SendSentence(Console.ReadLine());
                client.SendSubmit();
            }

            if (turnType == Model.Enums.TurnType.Draw)
            {
                Console.WriteLine($"Draw: {previousTurnData}");
                Thread thread = new Thread(() =>
                {
                    try
                    {
                        Console.WriteLine("Select an image to draw...");
                        OpenFileDialog openFileDialog = new OpenFileDialog();
                        openFileDialog.Filter = "Images|*.jpg;*.jpeg;*.png";
                        openFileDialog.Title = "Select an image to draw";
                        
                        if (openFileDialog.ShowDialog() == DialogResult.OK)
                        {
                            Console.WriteLine("Drawing image with DFS algorithm...");
                            Console.WriteLine("This will take some time to complete for a more human-like result.");
                            
                            // Create our DFS drawer and use it to draw the image
                            GarticDFSDrawer dfsDrawer = new GarticDFSDrawer(client.Draw);
                            
                            // Ask for stroke size
                            Console.Write("Enter stroke size (1-5, default 2): ");
                            string strokeSizeInput = Console.ReadLine();
                            int strokeSize = 2;
                            if (!string.IsNullOrEmpty(strokeSizeInput) && int.TryParse(strokeSizeInput, out int parsedSize))
                            {
                                strokeSize = Math.Max(1, Math.Min(5, parsedSize));
                            }
                            
                            // Draw the image using our DFS algorithm
                            dfsDrawer.DrawImageWithDFS(openFileDialog.FileName, strokeSize);
                            Console.WriteLine("Drawing completed!");
                        }
                        else
                        {
                            Console.WriteLine("No image selected. Using default drawing method...");
                            // Fallback to original method if no image selected
                            client.Draw.DrawImage(openFileDialog.FileName, 1);
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"Error while drawing: {ex.Message}");
                    }
                    finally
                    {
                        client.SendSubmit();
                    }
                });
                thread.SetApartmentState(ApartmentState.STA);
                thread.Start();
                thread.Join();
            }

            if (turnType == Model.Enums.TurnType.Guess)
            {
                Console.Write("Guess what's drawn: ");
                client.SendGuess(Console.ReadLine());
                client.SendSubmit();
            }
        }
    }
}
