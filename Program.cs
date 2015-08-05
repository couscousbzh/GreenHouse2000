using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using Microsoft.SPOT;
using Microsoft.SPOT.Hardware;
using SecretLabs.NETMF.Hardware;

//using SecretLabs.NETMF.Hardware.Netduino;
using SecretLabs.NETMF.Hardware.NetduinoPlus;

using Bouineur2000.lib;

namespace Bouineur2000
{
    public class Program
    {
        private static int timerLoop = 10000; //Temps en milliseconde entre chaque acquisitions.

        private static OutputPort led;
        private static LCD lcd;
        private static Dht22Sensor dhtSensor;
        private static AnalogInput A0;

        private static string tokenLogEntries = "6e48705f-8273-412d-b7a0-404e014a80f8";
        private static Igloocoder.MF.LogEntries.LogEntries myLogEntries;

        public static void Main()
        {
            try
            {
                Debug.Print("Green House 2000 starting");

                //Debug.Print("Memory available: " + Debug.GC(true));

                Init();
                
                SplashScreen();

                SetSystemTime();

                InitLoggerSDCard();

                //Infinite loop :
                DoJob();

                Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception ex)
            {
                Debug.Print("Error Main : " + ex.Message);

                if(myLogEntries != null)
                    myLogEntries.For(Igloocoder.MF.LogEntries.LogLevel.Error, "Error Main : " + ex.Message);
            }
        }

        public static void Init()
        {
            led = new OutputPort(Pins.ONBOARD_LED, false);

            lcd = new LCD(Pins.GPIO_PIN_D5, // RS
                Pins.GPIO_PIN_D4, // Enable
                Pins.GPIO_PIN_D3, // D4
                Pins.GPIO_PIN_D2, // D5
                Pins.GPIO_PIN_D1, // D6
                Pins.GPIO_PIN_D0, // D7
                20, // Number of Columns
                LCD.Operational.DoubleLIne, // LCD Row Format
                4, // Number of Rows in LCD 
                LCD.Operational.Dot5x8); // Dot Size of LCD               

            dhtSensor = new Dht22Sensor(Pins.GPIO_PIN_D8, Pins.GPIO_PIN_D9, PullUpResistor.External);

            A0 = new AnalogInput(Cpu.AnalogChannel.ANALOG_0);


            //Online LOG
            Igloocoder.MF.LogEntries.LogEntries.Initialize(tokenLogEntries);
            myLogEntries = Igloocoder.MF.LogEntries.LogEntries.Instance();
            myLogEntries.For(Igloocoder.MF.LogEntries.LogLevel.Debug, "GreenHouse bot is started and initiated.");

            //myLogEntries.For(Igloocoder.MF.LogEntries.LogLevel.Error, "Error de test.");
            //myLogEntries.For(Igloocoder.MF.LogEntries.LogLevel.Debug, "Error de test.");


        }

        public static void SplashScreen()
        {
            lcd.IsBlinking = false;
            lcd.ShowCursor = false;
            lcd.ClearDisplay();

            lcd.Show("Green House 2000", 100, true);
            Thread.Sleep(1000); // reading time for the viewer 

            led.Write(false);

            Debug.Print("data initiated");
        }

        public static void DoJob()
        {
            try
            {
                while (true)
                {
                    //Debug.Print("Memory available: " + Debug.GC(false));// affiche la mémoire mais fait planter les interupts
                    //Debug.Print("CPU Clock : " + Microsoft.SPOT.Hardware.Cpu.SlowClock.ToString());

                    ReadAndDisplay();

                    Thread.Sleep(timerLoop);
                }

                //Thread.Sleep(Timeout.Infinite);
            }
            catch (Exception ex)
            {
                Debug.Print("Error DoJob : " + ex.Message);
                myLogEntries.For(Igloocoder.MF.LogEntries.LogLevel.Debug, "Error DoJob : " + ex.Message);

                //Relance le job
                DoJob();
            }
        }

        /*******************/


        public static void ReadAndDisplay()
        {
            try
            {
                string temperature = "";
                string humidity = "";
                string luminosity = "";

                //Debug.Print("Sensor Read : " + analogInput0.Read().ToString() );

                //var dhtSensor = new Dht22Sensor(Pins.GPIO_PIN_D8, Pins.GPIO_PIN_D9, PullUpResistor.External);

                
                /*** READ ***/

                if (dhtSensor.Read())
                {
                    temperature = dhtSensor.Temperature.ToString("F1");
                    humidity = dhtSensor.Humidity.ToString("F1");

                    Debug.Print("DHT sensor Read() ok, RH = " + dhtSensor.Humidity.ToString("F1") + "%, Temp = " + dhtSensor.Temperature.ToString("F1") + "°C");
                }
                else
                {
                    Debug.Print("DHT sensor Read() failed");
                }

                double analogReading = A0.Read();
                luminosity = (analogReading * 1000).ToString("F0");
                Debug.Print("Lum sensor Read() ok, Lum = " + luminosity);

                

                /*** DISPLAY LCD ***/

                if (temperature.Length > 6)
                    temperature = temperature.Substring(0, 6);

                if (humidity.Length > 6)
                    humidity = humidity.Substring(0, 6);

                if (luminosity.Length > 6)
                    luminosity = luminosity.Substring(0, 6);

                lcd.JumpAt(0, 1);
                lcd.Show("Temp : " + temperature + " C", 0, true);
                lcd.JumpAt(0, 2);
                lcd.Show("Humi : " + humidity + " %", 0, true);
                lcd.JumpAt(0, 3);
                lcd.Show("Lumi : " + luminosity + " ", 0, true);


                string strData = "temperature=" + temperature + "&humidity=" + humidity + "&luminosity=" + luminosity;
                string strDataThingSpeak = "field1=" + temperature + "&field2=" + humidity + "&field3=" + luminosity;

                /*** LOG DATA TO SD CARD ***/
                DataLogSDCard(strData);

                /*** SEND DATA TO SERVER ***/
                ThingSpeakClient.updateThingSpeak(strDataThingSpeak);               

                /*** LOG DATA TO SERVER ***/
                myLogEntries.For(Igloocoder.MF.LogEntries.LogLevel.Debug, strData);

            }
            catch (Exception ex)
            {
                Debug.Print("Error ReadAndDisplay  : " + ex.Message);
                SDCardLogger.Log("Error ReadAndDisplay  : " + ex.Message);
                myLogEntries.For(Igloocoder.MF.LogEntries.LogLevel.Debug, "Error ReadAndDisplay : " + ex.Message);
            }
        }

        public static void InitLoggerSDCard()
        {
            // Directly start logging, no need to create any instance of Logger class
            SDCardLogger.LogToFile = true;    // if false it will only do Debug.Print()
            SDCardLogger.Append = true;       // will append the information to existing if any
            SDCardLogger.PrefixDateTime = true; // add a time stamp on each Log call. Note: Netduino time is not same as clock time.
                       
        }

        public static void DataLogSDCard(string str)
        {
            try
            {
                // Create an instance of Logger if you need to write to a custom location.
                SDCardLogger customLogger = new SDCardLogger(@"Data", "all.txt", true);
                customLogger.CustomPrefixDateTime = true;
                customLogger.CustomLogToFile = true;
                customLogger.LogCustom(str);               

                SDCardLogger.Flush();

                // MUST always call Close to save information
                // you may also call Flush() when you need to save information.
                SDCardLogger.Close();
                customLogger.CloseCustomStreamWriter();
            }
            catch (Exception ex)
            {
                Debug.Print("Error DataLogSDCard  : " + ex.Message);
            }
            finally {
                SDCardLogger.Close();                
            }

            
        }


        private static bool SetSystemTime()
        {
            var result = Ntp.UpdateTimeFromNtpServer("time.nist.gov", +1);  // Paris Time
            Debug.Print(result ? "Time successfully updated" : "Time not updated");

            return result;
        }


    }
}
