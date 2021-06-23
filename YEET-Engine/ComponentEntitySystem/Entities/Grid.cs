using System.Collections.Generic;
using ImGuiNET;
using OpenTK.Mathematics;
using System;


namespace YEET
{
    public class Grid : Entity
    {
        public ShaderLoader _loader;
        List<Vector3> FinalVertices = new List<Vector3>();
        private List<Vector3> PlaneVertices = new List<Vector3>();
        public System.Numerics.Vector3 rgb_grid, rgb_plane;
        Guid mesh,plane;
        

        public Grid(Vector2i _dimensions, float _line_thickness=0.2f, bool drawGui=false) : base(drawGui)
        {
            
            _loader = new ShaderLoader("Grid");
            mesh = AddComponent(new Mesh(this,_loader));
            plane = AddComponent(new Mesh(this,_loader));
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

            GetComponent<Mesh>(plane).SetData(MakeQuad(new Vector3(0,0,0),new Vector3(100,0,0),new Vector3(100,0,100),new Vector3(0,0,100)),new List<Mesh.VertexAttribType>(){Mesh.VertexAttribType.V3});

            GetComponent<Mesh>(mesh).SetData(FinalVertices,new List<Mesh.VertexAttribType>(){Mesh.VertexAttribType.V3});
        }

        public override void OnGui()
        {
            if (ShowGUI)
            {
                ImGui.Begin("Grid "+ID);
                ImGui.SetWindowFontScale(1.5f);
                base.OnGui();
                ImGui.Checkbox("Active", ref Active);
                ImGui.BeginChild("Colors");
                ImGui.ColorEdit3("Grid Color", ref rgb_grid);
                ImGui.ColorEdit3("Plane Color", ref rgb_plane);
                ImGui.EndChild();
                ShowGUI = !ImGui.Button("Remove Grid-Gui");
                ImGui.End();
            }

            ImGui.Checkbox("Grid " + ID, ref ShowGUI);
        }


        public override void OnRender()
        {
            GetComponent<Mesh>(mesh).SetUniform("rgb",new Vector3(rgb_grid.X,rgb_grid.Y,rgb_grid.Z));
            GetComponent<Mesh>(mesh).OnDraw();
            GetComponent<Mesh>(plane).SetUniform("rgb",new Vector3(rgb_plane.X,rgb_plane.Y,rgb_plane.Z));
            GetComponent<Mesh>(plane).OnDraw();
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