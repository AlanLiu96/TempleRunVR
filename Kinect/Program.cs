using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Windows;
using System.Windows.Media;
using Microsoft.Kinect;

namespace JumpAndRun
{
    public static class Program
    {
        static KinectSensor kinect = null;
        static BodyFrameReader bfr = null;
        static Body[] bodies = null;
        static CoordinateMapper coordinateMapper = null;
        static IReadOnlyDictionary<JointType, Joint> joints;
        static Dictionary<JointType, Point> jointPoints;
        private const float InferredZPositionClamp = 0.1f;
        static bool bodyTracked = false;
        static int bodyIndex;
        static double headXAvg = 0.0f;
        static double headYAvg = 0.0f;
        static double tolerance = 20.0;
        static void arrHandle(object sender, BodyFrameArrivedEventArgs e)
        {
            bool dataReceived = false;

            using (BodyFrame bodyFrame = e.FrameReference.AcquireFrame())
            {
                if (bodyFrame != null)
                {
                    if (bodies == null)
                    {
                        bodies = new Body[bodyFrame.BodyCount];
                    }

                    // The first time GetAndRefreshBodyData is called, Kinect will allocate each Body in the array.
                    // As long as those body objects are not disposed and not set to null in the array,
                    // those body objects will be re-used.
                    bodyFrame.GetAndRefreshBodyData(bodies);
                    dataReceived = true;
                }
            }
            if (dataReceived)
            {
                Body body = null;

                if (bodyTracked)
                {
                    if (bodies[bodyIndex].IsTracked)
                    {
                        body = bodies[bodyIndex];
                    }
                    else
                    {
                        bodyTracked = false;
                        return;
                    }
                }
                if (!bodyTracked)
                {
                    for (int i = 0; i < bodies.Length; ++i)
                    {
                        if (bodies[i].IsTracked)
                        {
                            bodyIndex = i;
                            bodyTracked = true;
                            body = bodies[bodyIndex];
                            break;
                        }
                    }
                }
                if (bodyTracked)//(body.IsTracked)
                {
                    //this.DrawClippedEdges(body, dc);

                    joints = body.Joints;

                    // convert the joint points to depth (display) space
                    jointPoints = new Dictionary<JointType, Point>();

                    foreach (JointType jointType in joints.Keys)
                    {
                        // sometimes the depth(Z) of an inferred joint may show as negative
                        // clamp down to 0.1f to prevent coordinatemapper from returning (-Infinity, -Infinity)
                        CameraSpacePoint position = joints[jointType].Position;
                        if (position.Z < 0)
                        {
                            position.Z = InferredZPositionClamp;
                        }

                        DepthSpacePoint depthSpacePoint = coordinateMapper.MapCameraPointToDepthSpace(position);
                        jointPoints[jointType] = new Point(depthSpacePoint.X, depthSpacePoint.Y);
                    }
                    if (jointPoints[JointType.Head] != null && jointPoints[JointType.Head] != default(Point))
                    {
                        if (headXAvg == 0)
                        {
                            headXAvg = jointPoints[JointType.Head].X;
                        }
                        if (headYAvg == 0)
                        {
                            headYAvg = jointPoints[JointType.Head].Y;
                        }
                        Console.WriteLine("Head found");
                        Console.WriteLine("X: {0}", jointPoints[JointType.Head].X);
                        Console.WriteLine("Y: {0}", jointPoints[JointType.Head].Y);
                        Console.WriteLine();

                        if (jointPoints[JointType.Head].Y < headYAvg - 30)
                        {
                            Console.WriteLine("Jump detected");
                        }
                        if (jointPoints[JointType.ShoulderLeft] != null && jointPoints[JointType.ShoulderRight] != null)
                        {
                            double leftS = jointPoints[JointType.ShoulderLeft].Y;
                            double rightS = jointPoints[JointType.ShoulderRight].Y;
                            if ((leftS + tolerance) >= rightS && (leftS - tolerance) <= rightS)
                            {
                                // No lean
                            }
                            else if (leftS < rightS)
                            {
                                Console.WriteLine("Leaning right");
                            }
                            else
                            {
                                Console.WriteLine("Leaning left");
                            }

                        }
                    }
                    else
                    {
                        //Console.WriteLine("No head found");
                    }
                }
                else
                {

                    //Console.WriteLine("No bodies found");
                }
            }
        }
        static void Main(string[] args)
        {
            kinect = KinectSensor.GetDefault();
            coordinateMapper = kinect.CoordinateMapper;
            bfr = kinect.BodyFrameSource.OpenReader();
            bfr.FrameArrived += arrHandle;
            kinect.Open();
            Console.ReadLine();
            bfr.FrameArrived -= arrHandle;
            Console.ReadLine();

        }
    }
}