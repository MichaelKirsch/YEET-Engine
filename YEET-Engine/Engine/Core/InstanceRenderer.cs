using System.Collections.Generic;
using System.Collections;
using System.Linq;
using OpenTK.Graphics.OpenGL4;
using ImGuiNET;

namespace YEET.Engine.Core
{
    /// <summary>
    /// Global instance renderer. You can add as many models as you want. It needs dynamic data in form of a float array about every object that is part of the pipeline
    /// </summary>
    public static class InstanceRenderer
    {
        class PerModel
        {
            public int SSBO;
            public Stack<float> data;
            public OBJLoader loader;
            public int InstaceCounter;
            public PerModel(string modelname)
            {
                InstaceCounter = 0;
                //datapacket is normaly a vec 3 so 3 floats
                SSBO = GL.GenBuffer();
                data = new Stack<float>();
                loader = new OBJLoader(modelname, new ShaderLoader("FlatShadedModelInstanced"));
            }

            public float[] GetData()
            {
                var ret = data.ToArray();
                data.Clear();

                return ret;
            }
        }
        private static Dictionary<string, PerModel> _modelLists = new ();
        static InstanceRenderer(){}

        public static void AddToStack(string modelname,List<float>data)
        {
            Stack<float> _currentStack;
            try
            {
                _currentStack = _modelLists[modelname].data;
            }
            catch (System.Exception)
            {
                _modelLists.Add(modelname,new PerModel(modelname));
                _currentStack = _modelLists[modelname].data;
            }
            
            foreach (var vertex in data)
            {
                _currentStack.Push(vertex);
            }
            _modelLists[modelname].InstaceCounter++;
        }


        public static void OnGui(){
            ImGui.Begin("instanceRenderer");
            int total_assets = 0;
            foreach (var item in _modelLists)
            {
                ImGui.Text(item.Key);
                ImGui.SameLine();
                ImGui.Text($"{item.Value.InstaceCounter}");
                total_assets += item.Value.InstaceCounter;
            }
            ImGui.TextColored(ColorHelper.ConvertColor4(System.Drawing.Color.Chartreuse),$"Total Assets:{total_assets}");
            ImGui.End();
        }

        public static void OnUpdate()
        {
            
        }
        /// <summary>
        /// You need to reset the instance counter after all rendering passes are over
        /// </summary>
        public static void ClearStacks(){
            foreach (var item in _modelLists)
            {
                //the stacks are cleared by this point. but the instance count is still needed for further render passes
                item.Value.InstaceCounter=0;
            }
        }

        public static void OnRender()
        {
            foreach (var modelList in _modelLists)
            {
                GL.BindBuffer(BufferTarget.ShaderStorageBuffer,modelList.Value.SSBO);
                //fill up SSBO with data
                var data = modelList.Value.GetData();
                GL.BufferData(BufferTarget.ShaderStorageBuffer,data.Length*sizeof(float),data,BufferUsageHint.DynamicDraw);
                GL.BindBufferBase(BufferRangeTarget.ShaderStorageBuffer,0,modelList.Value.SSBO);
                //render that model
                modelList.Value.loader.InstanceDraw(modelList.Value.InstaceCounter);
                GL.BindBuffer(BufferTarget.ShaderStorageBuffer,0);
            }
        }
    }
}