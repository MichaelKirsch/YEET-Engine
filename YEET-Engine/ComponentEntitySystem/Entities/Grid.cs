using System.Collections.Generic;
using ImGuiNET;
using OpenTK.Graphics.ES11;
using OpenTK.Mathematics;
using OpenTK.Graphics.OpenGL4;
using YEET;
using GL = OpenTK.Graphics.OpenGL4.GL;
using PrimitiveType = OpenTK.Graphics.OpenGL4.PrimitiveType;
using VertexAttribPointerType = OpenTK.Graphics.OpenGL4.VertexAttribPointerType;

namespace YEET
{
    public class Grid : Entity
    {
        public ShaderLoader _loader;
        List<Vector3> FinalVertices = new List<Vector3>();
        private List<Vector3> PlaneVertices = new List<Vector3>();
        private int _VAO, VAO_Plane;
        public System.Numerics.Vector3 rgb_grid, rgb_plane;
        private float offsetx, offsety, offsetz;

        public Grid(Vector2i _dimensions, float _line_thickness=0.2f, bool drawGui=false) : base(drawGui)
        {
            _loader = new ShaderLoader("Grid");
            Name = "Grid";
            rgb_grid = new System.Numerics.Vector3(0.072f, 0.293f, 0.294f);
            rgb_plane = new System.Numerics.Vector3(0.158f, 0.158f, 0.158f);
            for (int x = 0; x <= _dimensions.X; x++)
            {
                Vector3 startpoint = new Vector3(x, 0, 0);
                Vector3 endpoint = new Vector3(x, 0, _dimensions.Y);
                Vector3 a = new Vector3(startpoint.X + _line_thickness, startpoint.Y + _line_thickness, startpoint.Z);
                Vector3 b = new Vector3(startpoint.X - _line_thickness, startpoint.Y + _line_thickness, startpoint.Z);
                Vector3 c = new Vector3(startpoint.X + _line_thickness, startpoint.Y - _line_thickness, startpoint.Z);
                Vector3 d = new Vector3(startpoint.X - _line_thickness, startpoint.Y - _line_thickness, startpoint.Z);

                Vector3 a2 = new Vector3(endpoint.X + _line_thickness, endpoint.Y + _line_thickness, endpoint.Z);
                Vector3 b2 = new Vector3(endpoint.X - _line_thickness, endpoint.Y + _line_thickness, endpoint.Z);
                Vector3 c2 = new Vector3(endpoint.X + _line_thickness, endpoint.Y - _line_thickness, endpoint.Z);
                Vector3 d2 = new Vector3(endpoint.X - _line_thickness, endpoint.Y - _line_thickness, endpoint.Z);

                FinalVertices.AddRange(MakeQuad(a, b, c, d)); //front
                FinalVertices.AddRange(MakeQuad(a2, b2, c2, d2)); //back
                FinalVertices.AddRange(MakeQuad(b, a, a2, b2)); //top
                FinalVertices.AddRange(MakeQuad(d, d2, a2, a)); //right
                FinalVertices.AddRange(MakeQuad(c, d, d2, c2)); //bottom
                FinalVertices.AddRange(MakeQuad(b, b2, c2, c)); //left
            }

            for (int x = 0; x <= _dimensions.X; x++)
            {
                Vector3 startpoint = new Vector3(0, 0, x);
                Vector3 endpoint = new Vector3(_dimensions.Y, 0, x);
                Vector3 a = new Vector3(startpoint.X, startpoint.Y + _line_thickness, startpoint.Z + _line_thickness);
                Vector3 b = new Vector3(startpoint.X, startpoint.Y + _line_thickness, startpoint.Z - _line_thickness);
                Vector3 c = new Vector3(startpoint.X, startpoint.Y - _line_thickness, startpoint.Z + _line_thickness);
                Vector3 d = new Vector3(startpoint.X, startpoint.Y - _line_thickness, startpoint.Z - _line_thickness);

                Vector3 a2 = new Vector3(endpoint.X, endpoint.Y + _line_thickness, endpoint.Z + _line_thickness);
                Vector3 b2 = new Vector3(endpoint.X, endpoint.Y + _line_thickness, endpoint.Z - _line_thickness);
                Vector3 c2 = new Vector3(endpoint.X, endpoint.Y - _line_thickness, endpoint.Z + _line_thickness);
                Vector3 d2 = new Vector3(endpoint.X, endpoint.Y - _line_thickness, endpoint.Z - _line_thickness);

                FinalVertices.AddRange(MakeQuad(a, b, c, d)); //front
                FinalVertices.AddRange(MakeQuad(a2, b2, c2, d2)); //back
                FinalVertices.AddRange(MakeQuad(b, a, a2, b2)); //top
                FinalVertices.AddRange(MakeQuad(d, d2, a2, a)); //right
                FinalVertices.AddRange(MakeQuad(c, d, d2, c2)); //bottom
                FinalVertices.AddRange(MakeQuad(b, b2, c2, c)); //left
            }

            int _VBO = GL.GenBuffer();
            _VAO = GL.GenVertexArray();
            GL.BindVertexArray(_VAO);
            GL.BindBuffer(BufferTarget.ArrayBuffer, _VBO);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * 3 * FinalVertices.Count, FinalVertices.ToArray(),
                BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.BindVertexArray(0);


            PlaneVertices.AddRange(MakeQuad((0, 0, 0),
                (_dimensions.X, 0, 0),
                (_dimensions.X, 0, _dimensions.Y),
                (0, 0, _dimensions.Y)));
            int VBO_plane = GL.GenBuffer();
            VAO_Plane = GL.GenVertexArray();
            GL.BindVertexArray(VAO_Plane);
            GL.BindBuffer(BufferTarget.ArrayBuffer, VBO_plane);
            GL.BufferData(BufferTarget.ArrayBuffer, sizeof(float) * 3 * PlaneVertices.Count, PlaneVertices.ToArray(),
                BufferUsageHint.StaticDraw);
            GL.VertexAttribPointer(0, 3, VertexAttribPointerType.Float, false, 3 * sizeof(float), 0);
            GL.EnableVertexAttribArray(0);
            GL.BindVertexArray(0);
        }

        public override void OnGui()
        {
            if (ShowGUI)
            {
                ImGui.Begin("Grid "+ID);
                ImGui.SetWindowFontScale(1.5f);
                ImGui.SliderFloat("Offset X", ref offsetx, -100, 100);
                ImGui.SliderFloat("Offset Y", ref offsety, -100, 100);
                ImGui.SliderFloat("Offset Z", ref offsetz, -100, 100);
                ImGui.BeginChild("Colors");
                ImGui.ColorEdit3("Grid Color", ref rgb_grid);
                ImGui.ColorEdit3("Plane Color", ref rgb_plane);
                ImGui.EndChild();
                ShowGUI = !ImGui.Button("Remove Grid-Gui");
                ImGui.End();
                GetComponent<Transform>().SetPosition(new Vector3(offsetx,offsety,offsetz));
            }

            ImGui.Checkbox("Grid " + ID, ref ShowGUI);
        }


        public override void OnRender()
        {
            _loader.UseShader();
            GL.BindVertexArray(_VAO);
            _loader.SetUniformVec3("rgb", rgb_plane.X, rgb_plane.Y, rgb_plane.Z);
            _loader.SetUniformMatrix4F("view", ref Camera.View);
            _loader.SetUniformVec3("offset", GetComponent<Transform>().GetPosition());
            _loader.SetUniformMatrix4F("projection",ref Camera.Projection);
            GL.DrawArrays(PrimitiveType.Triangles, 0, FinalVertices.Count * 3);
            
            GL.BindVertexArray(VAO_Plane);
            _loader.SetUniformVec3("rgb", rgb_grid.X, rgb_grid.Y, rgb_grid.Z);
            _loader.SetUniformVec3("offset", GetComponent<Transform>().GetPosition());
            _loader.SetUniformMatrix4F("view", ref Camera.View);
            _loader.SetUniformMatrix4F("projection",ref Camera.Projection);
            GL.DrawArrays(PrimitiveType.Triangles, 0, PlaneVertices.Count * 3);
        }

        List<Vector3> MakeQuad(Vector3 a, Vector3 b, Vector3 c, Vector3 d)
        {
            var list = new List<Vector3>();

            list.Add(a);
            list.Add(b);
            list.Add(c);

            list.Add(c);
            list.Add(d);
            list.Add(a);

            return list;
        }
    }
}