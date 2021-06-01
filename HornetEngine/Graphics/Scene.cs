using HornetEngine.Ecs;
using System;
using System.Collections.Generic;
using System.Text;
using GlmSharp;
using HornetEngine.Util;

namespace HornetEngine.Graphics
{
    public class Scene
    {
        public List<Entity> scene_content;
        private List<Entity> normal_objects;
        private List<Entity> transperant_objects;

        public Scene()
        {
            scene_content = new List<Entity>();
            normal_objects = new List<Entity>();
            transperant_objects = new List<Entity>();
        }

        public List<Entity> SortByDistanceToCam(Camera cam)
        {
            Entity[] entities = transperant_objects.ToArray();
            Array.Sort(entities, (item1, item2) => {
                float distance_to_item1 = glm.DistanceSqr(cam.Position, item1.Transform.Position);
                float distance_to_item2 = glm.DistanceSqr(cam.Position, item2.Transform.Position);

                if(distance_to_item1 > distance_to_item2)
                {
                    return 1;
                } else if(distance_to_item1 < distance_to_item2)
                {
                    return -1;
                } else
                {
                    return 0;
                }
            });
            return new List<Entity>(entities);
        }

        public void LoadScene(string folder_id, string scene_file)
        {
            scene_content.Clear();
            List<Mesh> meshes = new List<Mesh>();
            string error = Mesh.ImportMeshesFromFile(folder_id, scene_file, out Assimp.Scene s);

            if(error != string.Empty)
            {
                throw new Exception($"Could not load scene: {error}");
            }

            for(int i = 0; i < s.MeshCount; i++)
            {
                Mesh tmp = Mesh.ImportMeshFromScene(s, (uint)i);
                if(tmp.Error.Length != 0)
                {
                    throw new Exception($"Mesh {tmp.Name} loading encountered an error: {tmp.Error}");
                }
                MeshResourceManager.GetInstance().AddResource(tmp.Name, tmp);

                Entity e = new Entity(tmp.Name);

                MeshComponent meshcomp = new MeshComponent();
                meshcomp.SetTargetMesh(tmp.Name);
                e.AddComponent(meshcomp);

                MaterialComponent matcomp = new MaterialComponent();
                matcomp.SetShaderFromId("default");
                e.AddComponent(matcomp);

                scene_content.Add(e);
            }

        }
    }
}
