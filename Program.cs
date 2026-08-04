/*
 * Project: PC Info
 * Developer / Brand: KIRILL GROUP
 * License: MIT License (See LICENSE file for details)
 * 
 * Notice: Any distribution, modification, or inclusion of this source code 
 * or its binaries must retain the original copyright notice, the project name "PC Info", 
 * and the brand name "KIRILL GROUP".
 */

using System;
using System.Collections;
using System.IO;
using System.Text;
using System.Management;
using System.Net;
using OldKEngine;
namespace PC_Info
{
    class Program
    {
        static void Main(string[] args)
        {
            if (File.Exists("token.kgt"))
            {
                string token = ReadFile("token.kgt");
                if (token != null && token.Length > 0)
                {
                    Console.WriteLine("Hello, " + Account.get_name(token) + "!");
                }
            }

            info();
        }

        public static void exit()
        {
            Environment.Exit(0);
        }

        public static void info()
        {
            Console.WriteLine("KIRILL GROUP");
            Console.WriteLine("Welcome to the PC Info! V 1.3.0");
            Console.WriteLine("OldKEngine V "+InfoApp.OldKEngineVersion());
			Console.WriteLine("         ");
            Console.WriteLine("Windows Version: {0}", Environment.OSVersion);
            Console.WriteLine(IntPtr.Size == 8 ? "Process Architecture: 64-bit" : "Process Architecture: 32-bit");

            try
            {
                ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT OSArchitecture FROM Win32_OperatingSystem");
                foreach (ManagementObject os in searcher.Get())
                {
                    Console.WriteLine("OS Architecture: " + os["OSArchitecture"]);
                }
            }
            catch
            {
                Console.WriteLine("OS Architecture: Unknown");
            }

            Console.WriteLine("Computer name: {0}", Environment.MachineName);
 
            Console.WriteLine("System folder: {0}", Environment.SystemDirectory);
            Console.WriteLine("Disks: {0}", string.Join(", ", Environment.GetLogicalDrives()));
			OutputResult("Storage Drives: ", GetHardwareInfo("Win32_DiskDrive","Model"));

            Console.WriteLine("         ");
			GetRAMandBoardInfo();
            OutputResult("Processor: ", GetHardwareInfo("Win32_Processor", "Name"));
            OutputResult("Manufacturer: ", GetHardwareInfo("Win32_Processor", "Manufacturer"));
            OutputResult("Description: ", GetHardwareInfo("Win32_Processor", "Description"));

			Console.WriteLine("         ");
            OutputResult("Video card: ", GetHardwareInfo("Win32_VideoController", "Name"));
            OutputResult("Video Processor:", GetHardwareInfo("Win32_VideoController", "VideoProcessor"));
            OutputResult("Driver version: ", GetHardwareInfo("Win32_VideoController", "DriverVersion"));
            OutputResult("Video memory size (bytes):", GetHardwareInfo("Win32_VideoController", "AdapterRAM"));

            Console.WriteLine("         ");
            string hostName = Dns.GetHostName();
            Console.WriteLine("Local IP: {0}", Dns.GetHostByName(hostName).AddressList[0].ToString());
            Console.WriteLine("Host: {0}", hostName);

            Console.WriteLine("         ");
			Console.WriteLine("Current User: {0}", Environment.UserName);
			Console.WriteLine("System Uptime: {0} hours",(Environment.TickCount / 1000) / 3600); 

            Console.ReadLine();
        }

        private static ArrayList GetHardwareInfo(string WIN32_Class, string ClassItemField)
        {
            ArrayList hardwareInfo = new ArrayList();
            ManagementObjectSearcher managementObjectSearcher = new ManagementObjectSearcher("SELECT * FROM " + WIN32_Class);

            try
            {
                foreach (ManagementObject managementObject in managementObjectSearcher.Get())
                {
                    object val = managementObject[ClassItemField];
                    if (val != null)
                        hardwareInfo.Add(val.ToString().Trim());
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return hardwareInfo;
        }

        private static void OutputResult(string info, ArrayList result)
        {
            Console.Write(info);
            string[] arr = (string[])result.ToArray(typeof(string));
            Console.WriteLine(string.Join(", ", arr));
        }
        private static string ReadFile(string path)
        {
            StreamReader reader = null;
            try
            {
                reader = new StreamReader(path);
                return reader.ReadToEnd();
            }
            finally
            {
                if (reader != null)
                    reader.Close();
            }
        }
		public static void GetRAMandBoardInfo()
		{
			
			try
			{
				ManagementObjectSearcher searcher = new ManagementObjectSearcher("SELECT Capacity FROM Win32_PhysicalMemory");
				long totalCapacity = 0;
				foreach (ManagementObject ram in searcher.Get())
				{
					totalCapacity += Convert.ToInt64(ram["Capacity"]);
				}
				Console.WriteLine("RAM Total: {0} GB", totalCapacity / (1024 * 1024 * 1024));
			}
			catch
			{
				Console.WriteLine("RAM Total: Unknown");
			}

			
			OutputResult("Motherboard: ", GetHardwareInfo("Win32_BaseBoard", "Product"));
			OutputResult("Motherboard Manufacturer: ", GetHardwareInfo("Win32_BaseBoard", "Manufacturer"));
		}


    }
 
}
