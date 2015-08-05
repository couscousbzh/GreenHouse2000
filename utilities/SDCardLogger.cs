

/*-------------------------------------------------------------------------------+
|
| Copyright (c) 2012, Embedded-Lab. All Rights Reserved.
|
| Limited permission is hereby granted to reproduce and modify this 
| copyrighted material provided that this notice is retained 
| in its entirety in any such reproduction or modification.
|
| Author: ANir
| First Version Date: 2013/2/14
+-------------------------------------------------------------------------------*/

			

using System;
using System.IO;
using Microsoft.SPOT;

namespace Bouineur2000.lib
{
	public class SDCardLogger
	{
		#region Public Static Methods
public static void Log(params object[] strings)
{
	string message = string.Empty;
	for (int i = 0; i < strings.Length; i++)
	{
		message = message + strings[i].ToString() + " ";
	}
	WriteLog(message, StreamWriter, PrefixDateTime, LogToFile );
}
		public static void Flush()
		{
			if (_streamWriter == null) return;
			StreamWriter.Flush();
		}
		public static void Close()
		{
			if (_streamWriter == null) return;
			StreamWriter.Flush();
			StreamWriter.Close();
			StreamWriter.Dispose();
		}
		#endregion

		#region  Private Static Methods
		private static string GetDirectoryPath(string trimmedDirectoryPath)
		{
			if (!Directory.Exists(SDCardDirectory))
				throw new Exception("SD card (directory) not found");

			string directoryPath = SDCardDirectory + Path.DirectorySeparatorChar + trimmedDirectoryPath;

			if (!Directory.Exists(directoryPath))
				Directory.CreateDirectory(directoryPath);

			return directoryPath;
		}
		private static string GetFilePath(string fullFileName, bool append)
		{
			if (!File.Exists(fullFileName) || !append)
			{
				File.Create(fullFileName);
			}
			return fullFileName;
		}
private static void WriteLog(string message, StreamWriter streamWriter, bool addDateTime, bool logToFile)
{
	if (addDateTime)
	{
		DateTime current = DateTime.Now;
		message = "[" + current + ":" + current.Millisecond + "] " + message;
	}

	Debug.Print(message);
	if (logToFile) streamWriter.WriteLine(message);
}
		#endregion

		#region Public Static Properties
		public static bool PrefixDateTime { set; get; }
		public static bool LogToFile { set; get; }
		public static bool Append { set; get; }
		public static string LogFilePath
		{
			get
			{
				if (_logFilePath == null)
					_logFilePath = GetFilePath(GetDirectoryPath("Report") +
									Path.DirectorySeparatorChar + "Log.txt", Append);
				return _logFilePath;
			}
		}
		#endregion

		#region Private Static Properties
		private static string SDCardDirectory { get { return "SD"; } }
private static StreamWriter StreamWriter
{
	get
	{
		if (_streamWriter == null) _streamWriter = new StreamWriter(LogFilePath,(bool)Append);
		return _streamWriter;
	}
}
		#endregion
        
		#region Constructor
		public SDCardLogger(string directoryName, string fileNameWithExtension, bool append = true)
		{
			CustomDirectoryName = directoryName;
			CustomFileNameWithExtension = fileNameWithExtension;
			CustomAppend = append;
		}
		#endregion

		#region Public Methods
		public void LogCustom(params object[] strings)
		{
			string message = string.Empty;
			for (int i = 0; i < strings.Length; i++)
			{
				message = message + strings[i].ToString() + " ";
			}
			WriteLog(message, CustormStreamWriter, CustomPrefixDateTime, CustomLogToFile );
		}
		public void FlushCustomLogger()
		{
			if (CustormStreamWriter == null) return;
			CustormStreamWriter.Flush();
		}
		public void CloseCustomStreamWriter()
		{
			if (CustormStreamWriter == null) return;
			CustormStreamWriter.Flush();
			CustormStreamWriter.Close();
			CustormStreamWriter.Dispose();
		}
		#endregion

		#region Private Properties
		private string CustomDirectoryName { get; set; }
		private string CustomFileNameWithExtension { get; set; }
		private bool CustomAppend { get; set; }
		private StreamWriter CustormStreamWriter
		{
			get
			{
				if (_customStreamWriter == null) _customStreamWriter = new StreamWriter(CustomFilePath, CustomAppend);
				return _customStreamWriter;
			}
		}
		#endregion

		#region Public Properties
		public string CustomFilePath
		{
			get
			{
				if (CustomDirectoryName == string.Empty) throw new Exception("Custom directory cannot be blank");
				if (CustomFileNameWithExtension == string.Empty) throw new Exception("File name cannot be blank");

				if (_customLogFilePath == null)
					_customLogFilePath = GetFilePath(GetDirectoryPath(CustomDirectoryName) + Path.DirectorySeparatorChar + CustomFileNameWithExtension, CustomAppend);

				return _customLogFilePath;
			}
		}
		public bool CustomPrefixDateTime { set; get; }
		public bool CustomLogToFile { set; get; }
		#endregion

		#region Fields
		private static string _logFilePath;
		private static StreamWriter _streamWriter;

		private StreamWriter _customStreamWriter;
		private string _customLogFilePath;
		#endregion


        public static void LoggerTestSDCard()
        {
            // Directly start logging, no need to create any instance of Logger class
            SDCardLogger.LogToFile = true;    // if false it will only do Debug.Print()
            SDCardLogger.Append = true;       // will append the information to existing if any
            SDCardLogger.PrefixDateTime = true; // add a time stamp on each Log call. Note: Netduino time is not same as clock time.

            // any number of arguments can be passed. They will appended by a white space
            SDCardLogger.Log("All", "these", "will", "be", "combined", "in", "to", "one", "string");
            SDCardLogger.Log("This should go into the second line.");
            Debug.Print(SDCardLogger.LogFilePath);


            // Create an instance of Logger if you need to write to a custom location.
            SDCardLogger customLogger = new SDCardLogger(@"One\OneOne", "one.txt", true);
            customLogger.CustomPrefixDateTime = false;
            customLogger.CustomLogToFile = true;
            customLogger.LogCustom("All", "these", "will", "be", "combined", "in", "to", "one", "string", "-CustomLogger1.");
            Debug.Print(customLogger.CustomFilePath);

            // MUST always call Close to save information
            // you may also call Flush() when you need to save information.
            SDCardLogger.Close();
            customLogger.CloseCustomStreamWriter();
        }

	}
}
