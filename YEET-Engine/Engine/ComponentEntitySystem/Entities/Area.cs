using System;
using OpenTK.Mathematics;
using ImGuiNET;
using System.Xml;
using System.Collections.Generic;
using YEET.Engine.Core;

namespace YEET.Engine.ECS
{
    public class Area : Entity
    {
        private Guid mesh;
        public Vector3 Color, Point_one, Point_two;
        public float minX, minZ, maxX, maxZ;
        private Random new_rand = new Random();
        private int counter =0;
        private List<Vector3> housestoBuild = new List<Vector3>();

        public enum AreaType{
            SmallSuburb,DenseHouses,Industry,Financial,CommercialSmall,CommercialDense,Holiday,Tourist
        };


        public AreaType Areatype = AreaType.SmallSuburb;

        public Area(Vector3 point_one, Vector3 point_two, Vector3 color, AreaType type = AreaType.SmallSuburb)
        {
            Areatype = type;
            Name = "Area";
            Point_two = point_two;
            Point_one = point_one;
            var _loader = new ShaderLoader("Grid");
            Color = color;
            mesh = AddComponent(new Mesh(this, _loader));
            GetComponent<Mesh>(mesh).SetData(MakeForm.MakeQuad(new Vector3(point_one.X, 0, point_one.Z),
            new Vector3(point_one.X, 0, point_two.Z),
            new Vector3(point_two.X, 0, point_two.Z),
            new Vector3(point_two.X, 0, point_one.Z)), new List<Mesh.VertexAttribType>() { Mesh.VertexAttribType.V3 });
            var col = new Collider(this, point_one, new Vector3(point_two.X, point_two.Y + 0.1f, point_two.Z));
            var bbox = col.GetBox();
            AddComponent(col);
            minX = bbox.minX;
            minZ = bbox.minZ;
            maxX = bbox.maxX;
            maxZ = bbox.maxZ;

            int nbr_houses_x = Convert.ToInt32(maxX - minX) / 2;
            int nbr_houses_z = Convert.ToInt32(maxZ - minZ) / 2;
            var startpoint = new Vector3(minX, 0, minZ);
            for (int x = 0; x < nbr_houses_x; x += 1)
            {
                for (int z = 0; z < nbr_houses_z; z += 1)
                {
                    housestoBuild.Add(new Vector3(startpoint.X+1 + x*2, startpoint.Y, startpoint.Z+1 + z*2));
                }
            }
        }
        public override void OnGui()
        {
            ImGui.Checkbox("Active", ref Active);
            System.Numerics.Vector3 ref_c = new System.Numerics.Vector3(Color.X, Color.Y, Color.Z);
            ImGui.ColorEdit3("Color", ref ref_c);
            Color = new Vector3(ref_c.X, ref_c.Y, ref_c.Z);
            ImGui.Text($"Entities in Queue:{housestoBuild.Count}");
            ImGui.Text($"Area Type:{Areatype.ToString()}");
            base.OnGui();
        }

        public override void OnRender()
        {
            base.OnRender();
            GetComponent<Mesh>(mesh).SetUniform("rgb", new Vector3(Color.X, Color.Y, Color.Z));
            GetComponent<Mesh>(mesh).OnDraw();
        }


        public override void OnUpdate()
        {
            base.OnUpdate();
            if (housestoBuild.Count > 0)
            {
                var which = new_rand.Next(housestoBuild.Count-1);
                counter++;
                if (new_rand.Next()%400<300)
                {
                    counter =0;
                    //AddChildEntity(new StaticOBJModel("road_drivewaySingle",housestoBuild[which],false));
                    switch(Areatype){
                        case AreaType.SmallSuburb:
                            AddChildEntity(new House("small_house", housestoBuild[which]));
                        break;
                        case AreaType.DenseHouses:
                            AddChildEntity(new House("dense_house", housestoBuild[which]));
                        break;
                    }
                    
                    housestoBuild.RemoveAt(which);
                }
            }
        }
    }
}