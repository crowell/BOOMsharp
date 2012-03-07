using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters;
using System.Runtime.Serialization.Formatters.Binary;
using System.IO;
using System.Windows.Forms;


/*
 * Idea 100% Lifted from Zach Holman's BOOM.
 * BOOMsharp (booms) was created as I hate ruby and could not
 * get it work under cygwin...
*/

namespace boomsharp
{
    [Serializable()]
    public class Clippy
    {
        public Dictionary<string, Dictionary<string, string>> savedItems;
        /// <summary>
        /// Initializes a new instance of the <see cref="boomsharp.Clippy"/> class.
        /// </summary>
        public Clippy()
        {
            savedItems = new Dictionary<string, Dictionary<string, string>>();
        }
        /// <summary>
        /// Saves the clippy.
        /// </summary>
        /// <param name='board'>
        /// Board.
        /// </param>
        public void saveClippy(Clippy board)
        {
            //thanks to the good people @ stackoverflow!
            string homePath = (Environment.OSVersion.Platform == PlatformID.Unix ||
                   Environment.OSVersion.Platform == PlatformID.MacOSX)
                    ? Environment.GetEnvironmentVariable("HOME")
                    : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
            string clipSave = homePath + "/.boomsharp";
            Stream stream = File.Open(clipSave, FileMode.Create);
            BinaryFormatter bFormatter = new BinaryFormatter();
            bFormatter.Serialize(stream, board);
            stream.Close();
        }
        /// <summary>
        /// Loads the clippy.
        /// </summary>
        /// <returns>
        /// The clippy.
        /// </returns>
        public Clippy loadClippy()
        {
            Clippy objectToSerialize;
            //thanks to the good people @ stackoverflow!
            string homePath = (Environment.OSVersion.Platform == PlatformID.Unix ||
                   Environment.OSVersion.Platform == PlatformID.MacOSX)
                    ? Environment.GetEnvironmentVariable("HOME")
                    : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
            string clipSave = homePath + "/.boomsharp";
            Stream stream = File.Open(clipSave, FileMode.Open);
            BinaryFormatter bFormatter = new BinaryFormatter();
            objectToSerialize = (Clippy)bFormatter.Deserialize(stream);
            stream.Close();
            return objectToSerialize;

        }
    }
    class BOOMsharp
    {
        /// <summary>
        /// Gets the platform.
        /// </summary>
        /// <returns>
        /// The platform.
        /// </returns>
        public static char getPlatform()
        {
            if (Environment.OSVersion.Platform == PlatformID.Unix)
            {
                return 'l';
            }
            else if (Environment.OSVersion.Platform == PlatformID.MacOSX)
            {
                return 'm';
            }
            return 'w';
        }

        /// <summary>
        /// Clips the string.
        /// </summary>
        /// <param name='toClip'>
        /// String for clipping clip.
        /// </param>
        /// <param name='sys'>
        /// System Platform.
        /// </param>
        public static void clipString(string toClip, char sys)
        {
            //do the actual clipping here
            Clipboard.SetText(toClip);

            //end clipping
            System.Console.WriteLine("Boom! Just copied " + toClip + " to your clipboard.");
        }

        public static bool okToDel(string aList)
        {
            System.Console.Write("You sure you want to delete everything in \"" + aList + "\"? (y/n): ");
            string str = System.Console.ReadLine();
            if (str == "n" || str == "N")
            {
                return false;
            }
            else if (str == "y" || str == "Y")
            {
                return true;
            }
            else return okToDel(aList);
        }

        /// <summary>
        /// Opens the string url in the browser
        /// </summary>
        /// <param name="url">url</param>
        /// <param name="platform">char describing the platform</param>
        public static void openInBrowser(string sUrl, char platform)
        {
            //open the string in the browser.
            try
            {
                System.Diagnostics.Process.Start(sUrl);
            }
            catch (Exception exc1)
            {
                // System.ComponentModel.Win32Exception is a known exception that occurs when Firefox is default browser.  
                // It actually opens the browser but STILL throws this exception so we can just ignore it.  If not this exception,
                // then attempt to open the URL in IE instead.
                if (exc1.GetType().ToString() != "System.ComponentModel.Win32Exception")
                {
                    // sometimes throws exception so we have to just ignore
                    // this is a common .NET bug that no one online really has a great reason for so now we just need to try to open
                    // the URL using IE if we can.
                    try
                    {
                        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo("IExplore.exe", sUrl);
                        System.Diagnostics.Process.Start(startInfo);
                        startInfo = null;
                    }
                    catch (Exception exc2)
                    {
                        // still nothing we can do so just show the error to the user here.
                    }
                }
            }
        }


        [STAThreadAttribute]
        public static void Main(string[] args)
        {
            //boomsharp usage 
            //booms.exe <list> <name> <value>



            //first find the platform that we're on!
            //'w' = windows
            //'l' = linux
            //'m' = mac
            char pFrom = getPlatform();

            Clippy board = new Clippy();
            string homePath = (Environment.OSVersion.Platform == PlatformID.Unix ||
                   Environment.OSVersion.Platform == PlatformID.MacOSX)
                    ? Environment.GetEnvironmentVariable("HOME")
                    : Environment.ExpandEnvironmentVariables("%HOMEDRIVE%%HOMEPATH%");
            string clipSave = homePath + "/.boomsharp";

            //check to see if we have an existing clipboard
            if (File.Exists(clipSave))
            {
                board = new Clippy().loadClippy();
            }
            //otherwise, just use this other one that we've got
            string aList = String.Empty;
            string aName = String.Empty;
            List<string> toCopy = new List<string>();

            //first just check if we have args
            if (args.Length > 0)
            {
                //now parse the input
                aList = args[0]; //the "BIG" bucket, bucket of litt	

                if (args.Length > 1)
                {
                    aName = args[1];
                }

                if (args.Length > 2)
                {
                    for (int ii = 2; ii < args.Length; ii++)
                    {
                        toCopy.Add(args[ii]);
                    }
                }
            }


            //now, first case (noargs)
            if (args.Length == 0)
            {
                foreach (KeyValuePair<string, Dictionary<string, string>> kvp in board.savedItems)
                {
                    System.Console.WriteLine(kvp.Key + " (" + kvp.Value.Count + ")");
                }
            }
            //first args case!
            else if (args.Length == 1)
            {
                if (aList == "all") //first check special case!
                {
                    //print out EVERYTHING
                    foreach (KeyValuePair<string, Dictionary<string, string>> kvp in board.savedItems)
                    {
                        System.Console.WriteLine(kvp.Key);
                        foreach (KeyValuePair<string, string> sstr in kvp.Value)
                        {
                            System.Console.WriteLine("\t" + sstr.Key + " \t " + sstr.Value);
                        }
                    }
                }
                else if (!(board.savedItems.ContainsKey(aList)))
                {
                    board.savedItems[aList] = new Dictionary<string, string>();
                    System.Console.WriteLine("Boom! Created a new list called \"" + aList + "\".");
                    board.saveClippy(board);
                }

                else
                {
                    //just do some printing
                    foreach (KeyValuePair<string, string> kvp in board.savedItems[aList])
                    {
                        System.Console.WriteLine(kvp.Key + " " + kvp.Value);
                    }
                }
            }

            else if (args.Length == 2)
            {
                //first check special cases
                if (aName == "delete")
                {
                    //delete the entire list
                    if (board.savedItems.ContainsKey(aList))
                    {
                        if (okToDel(aList))
                        {
                            board.savedItems.Remove(aList);
                            System.Console.WriteLine("Boom! Deleted all your " + aList + ".");
                        }
                    }
                }
                else if (aList == "open")
                {
                    //open each of the items in the browser
                    if (board.savedItems.ContainsKey(aName))
                    {
                        Dictionary<string, string> dItems = board.savedItems[aName];
                        foreach (KeyValuePair<string, string> kvp in dItems)
                        {
                            openInBrowser(kvp.Value, pFrom);
                        }
                        System.Console.WriteLine("Boom! We just opened all of \"" + aName + "\" for you.");
                    }
                }
                else if (aList == "echo")
                {
                    if (board.savedItems.ContainsKey(aName))
                    {
                        //just do some printing
                        foreach (KeyValuePair<string, string> kvp in board.savedItems[aName])
                        {
                            System.Console.WriteLine(kvp.Key + " " + kvp.Value);
                        }
                    }
                }
                else //so, goes list name, then copy to board
                {
                    if (board.savedItems.ContainsKey(aList))
                    {
                        Dictionary<string, string> dstr = board.savedItems[aList];
                        if (dstr.ContainsKey(aName))
                        {
                            clipString(dstr[aName], pFrom);
                        }
                    }
                }


            }
            else
            {
                //more than 2 args
                //first check the special cases
                if (toCopy[0] == "delete")
                {
                    if (board.savedItems.ContainsKey(aList) && board.savedItems[aList].ContainsKey(aName))
                    {
                        board.savedItems[aList].Remove(aName);
                        System.Console.WriteLine("Boom! \"" + aName +"\" is gone forever.");
                    }
                }
                else if (aList == "open")
                {
                    if(board.savedItems.ContainsKey(aName) && board.savedItems[aName].ContainsKey(toCopy[0]))
                    {
                        openInBrowser(board.savedItems[aName][toCopy[0]], pFrom);
                        System.Console.WriteLine("Boom! We just opened " + board.savedItems[aName][toCopy[0]] +" for you.");
                    }
                }
                else if (aList == "echo")
                {
                    //just print it out!
                    if (board.savedItems.ContainsKey(aName) && board.savedItems[aName].ContainsKey(toCopy[0]))
                    {
                        System.Console.WriteLine(board.savedItems[aName][toCopy[0]]);
                    }
                }
                else
                {
                    //just the save to clipboard
                    string str = String.Empty;
                    foreach(string ing in toCopy)
                    {
                        str += ing + " ";
                    }
                    str = str.Substring(0,str.Length - 1);
                    if (board.savedItems.ContainsKey(aList))
                    {
                        if (board.savedItems[aList].ContainsKey(aName))
                        {
                            board.savedItems[aList].Remove(aName);
                        }
                        board.savedItems[aList].Add(aName,str);
                        System.Console.WriteLine("Boom! \"" + aName + "\" in \"" + aList + "\" is \"" + str + "\". Got it.");
                        board.saveClippy(board);
                    }
                }

            }

        }
    }
}
