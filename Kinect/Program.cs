using System;
using System.Linq;
using System.Reactive.Linq;
using Microsoft.Kinect;

namespace JumpAndRun
{
    public static class Program
    {
        static KinectSensor kinect = null;
        static BodyFrameReader bfr = null;
        static Body[] bodies = null;

        static void arrHandle(object sender, EventArgs e)
        {
            Console.WriteLine("Frame received");
        }
        static void Main(string[] args)
        {
            kinect = KinectSensor.GetDefault();
            
            bfr = kinect.BodyFrameSource.OpenReader();
            bfr.FrameArrived += arrHandle;
            kinect.Open();
            while(true)
            {
                /*if (kinect.IsAvailable)
                {
                    Console.Clear();
                    Console.WriteLine("Available!");
                }
                else
                {
                    Console.Clear();
                    Console.WriteLine("Unavailable!");
                }*/
            }
        }
    }
}