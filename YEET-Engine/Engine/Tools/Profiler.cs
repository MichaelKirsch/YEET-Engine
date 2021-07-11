

using System;
using System.Diagnostics;
using System.Collections.Generic;
using ImGuiNET;
using System.Linq;
using YEET.Engine.Core;

namespace YEET{

    public class TimeSlot{
        public string Name;
        public bool Active=true;
        public float  Duration;
        public double StartTime;
        public double EndTime;
        public TimeSlot(string name){
            StartTime = StateMaschine.GetElapsedTimeTicks();
            Name = name;
            Profiler.AddTimeSlot(this);
        }
        public void Stop(){
            EndTime = StateMaschine.GetElapsedTimeTicks();
            Duration = Convert.ToSingle(EndTime-StartTime);
            Active = false;
        }
    }

    public static class Profiler{
        private static bool _frameIsStarted = false;
        
        private static double _startTime,_endTime,_duration;

        public static bool DebugPrint = false;

        public static bool RenderProfiler = false;
        private static List<TimeSlot> _timeSlots = new List<TimeSlot>();

        public static void StartFrame(){
            if(!_frameIsStarted)
            {
                _frameIsStarted = true;
                _timeSlots.Clear();
                _startTime = StateMaschine.GetElapsedTimeTicks();
            }
        }
        public static void StopFrame(){
            if(_frameIsStarted){
                _frameIsStarted = false;
                _endTime = StateMaschine.GetElapsedTimeTicks();
                _duration = _endTime-_startTime;
                if(DebugPrint){
                    Console.WriteLine($"Frametime:{_duration}ms");
                    foreach (var slot in _timeSlots)
                    {
                        Console.WriteLine($"{slot.Name}:{slot.Duration}ms");
                    }
                    Console.WriteLine("******************************");
                }
            }
        }
        
        public static void RenderProfilerWindow(){
            if(RenderProfiler){
            ImGui.Begin("Profiler");
            float[] y =_timeSlots.Select(x=>x.Duration).ToArray();
            ImGui.PlotHistogram("Measured Times",ref y[0],y.Length);
            float[] past_rendertimes = StateMaschine.Context.ListLastFrameTimes.ToArray();
            ImGui.PlotHistogram("Last 50 RenderTimes",ref past_rendertimes[0],past_rendertimes.Length);
            foreach (var slot in _timeSlots)
            {
                ImGui.Text($"{slot.Name}:{slot.Duration} ticks");
            }
            ImGui.End();        
            }
        }

        public static void AddTimeSlot(TimeSlot to_add){
            _timeSlots.Add(to_add);
        }
    }
}