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
        private List<Entity> renderable_objects;
        private List<Entity> transperant_objects;

        public Scene()
        {
            scene_content = new List<Entity>();
            renderable_objects = new List<Entity>();
            transperant_objects = new List<Entity>();
        }

        public void UpdateScene()
        {
            foreach(Entity ex in this.scene_content)
            {
                foreach(MonoScript mc in ex.Scripts)
                {
                    mc.Update();
                }
            }
        }

        public List<Entity> SortTransperantObjects(Camera cam)
        {
            transperant_objects.Clear();
            foreach(Entity e in scene_content)
            {
                MeshComponent mc = e.GetComponent<MeshComponent>();
                if (mc.IsTransperant) {
                    transperant_objects.Add(e);
                }
            }

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
            string error = this.ImportSceneFromFile(folder_id, scene_file, out Assimp.Scene s);

            if(error != string.Empty)
            {
                throw new Exception($"Could not load scene: {error}");
            }

            for(int i = 0; i < s.MeshCount; i++)
            {
                Assimp.Mesh _amesh = s.Meshes[i];
                Assimp.Material mat = s.Materials[s.Meshes[i].MaterialIndex];
                Mesh tmp = Mesh.ImportMesh(_amesh, mat);
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

                RenderComponent rendcomp = new RenderComponent();
                e.AddComponent(rendcomp);

                scene_content.Add(e);
            }

        }

        private String ImportSceneFromFile(String folder_id, String file, out Assimp.Scene scene)
        {
            string dir = DirectoryManager.GetResourceDir(folder_id);
            if (dir == String.Empty)
            {
                scene = null;
                return $"Directory with id [{folder_id}] was not found in DirectoryManager";
            }

            string path = DirectoryManager.ConcatDirFile(dir, file);

            try
            {
                Assimp.AssimpContext ac = new Assimp.AssimpContext();
                ac.SetConfig(new Assimp.Configs.NormalSmoothingAngleConfig(66.0f));
                Assimp.Scene s = ac.ImportFile(path, Assimp.PostProcessSteps.Triangulate | Assimp.PostProcessSteps.GenerateNormals);

                if (s.MeshCount == 0)
                {
                    scene = null;
                    return "No meshes were found in the file";
                }
                scene = s;
                return String.Empty;
            }
            catch (Exception ex)
            {
                scene = null;
                return ex.Message;
            }
        }


    }
}
