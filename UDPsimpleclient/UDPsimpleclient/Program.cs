/*
 * Created by SharpDevelop.
 * User: user
 * Date: 8/14/2020
 * Time: 12:28 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Net;
using System.Net.Sockets;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using System.IO;

namespace UDPsimpleclient
{
    class Program
    {
    	/// <summary>
    	/// Main client function
    	/// </summary>
        public static void Main()
        {
        	Console.WriteLine("Which IP do you want to use?");
        	string ipaddress = Console.ReadLine();
        	Console.WriteLine("Which port do you want to use?");
            int portnumber = Int32.Parse(Console.ReadLine());
            
            string path = Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "\\Downloads";
            string defaultFilename = "main.xml";
            Console.WriteLine(path);
            Console.WriteLine("Please pick the path of the file you wish to send to the server. if the default is ok please type no.");
            string pathdecision = Console.ReadLine();
            Console.WriteLine("Please type the file name you wish to send to server, otherwise type no if the default is ok.");
            string filename = Console.ReadLine();
            string[] lines;
            if (pathdecision == "no")
            {
                lines = File.ReadAllLines(path + "/" + defaultFilename);
            }
            else
            {
                lines = File.ReadAllLines(pathdecision + "/" + filename);
            }
            

            byte[] data = new byte[1024];
            string input, stringData;
            //IPEndPoint ipep = new IPEndPoint(IPAddress.Parse("192.168.0.59"), 4444);
            IPEndPoint ipep = new IPEndPoint(IPAddress.Parse(ipaddress), portnumber);
            Socket server = new Socket(AddressFamily.InterNetwork, SocketType.Dgram, ProtocolType.Udp);

            string welcome = "Hello, are you there?";
            data = Encoding.ASCII.GetBytes(welcome);
            server.SendTo(data, data.Length, SocketFlags.None, ipep);

            IPEndPoint sender = new IPEndPoint(IPAddress.Any, 0);
            EndPoint Remote = (EndPoint)sender;

            data = new byte[1024];
            int recv = server.ReceiveFrom(data, ref Remote);

            Console.WriteLine("Message received from {0}:", Remote.ToString());
            Console.WriteLine(Encoding.ASCII.GetString(data, 0, recv));
            //Console.WriteLine(path);
            //foreach (string line in lines)        {                       Console.WriteLine("\t" + line);        }// Use a tab to indent each line of the file.
            int i = 0;

            while (true)
            {
            	string md5Hashed = CalculateMD5(path + "/main.xml");
            	Console.WriteLine(md5Hashed);
                if (i < lines.Length)
                {
                    input = lines[i];
                    server.SendTo(Encoding.ASCII.GetBytes(input), Remote);
                    i++;
                    if (i == lines.Length)
                    {
                        input = "DONE";
                        server.SendTo(Encoding.ASCII.GetBytes(input), Remote);
                        input = "";
                        server.SendTo(Encoding.ASCII.GetBytes(input), Remote);
                    }
                }
                else
                {
                    input = Console.ReadLine();
                    if (input == "exit") break;
                    if (input == "again") i = 0;
                    server.SendTo(Encoding.ASCII.GetBytes(input), Remote);
                }

                data = new byte[1024];
                recv = server.ReceiveFrom(data, ref Remote);
                stringData = Encoding.ASCII.GetString(data, 0, recv);
                Console.WriteLine(stringData);
            }
            Console.WriteLine("Stopping client");
            server.Close();
        }
        /// <summary>
        /// MD5 function
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>

        public static string CalculateMD5(string filename)
        {
            using (var md5 = MD5.Create())
            {
                using (var stream = File.OpenRead(filename))
                {
                    var hash = md5.ComputeHash(stream);
                    return BitConverter.ToString(hash).Replace("-", "").ToLowerInvariant();
                }
            }
        }
    }



}
