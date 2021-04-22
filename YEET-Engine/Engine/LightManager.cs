using System;
using System.Collections.Generic;
using System.Numerics;
using System.Runtime.CompilerServices;
using YEET.ComponentEntitySystem.Entities;

namespace YEET
{
    

    public static class LightManager
    {
        private static int AmountChunksRendered;
        private static List<PointLight> _pointLights = new List<PointLight>();
        private static int _chunkSize;
        public static int ChunkSize
        {
            get { return _chunkSize;}
            set
            {
                _chunkSize = value;
                RebuildAllChunks();
            }
        }



        static LightManager()
        {
        }


        public static void OnStart()
        {
            ChunkSize = 32;
        }
        
        public static void OnUpdate()
        {
            AmountChunksRendered = Convert.ToInt32(Camera.RenderingDistance / ChunkSize);
        }
        
        
        private static void RebuildAllChunks()
        {
            Console.WriteLine("Rebuild all Chunks");
        }
    }
}