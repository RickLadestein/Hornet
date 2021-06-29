using System;
using System.Collections.Generic;
using System.Text;

namespace HornetEngine.Util
{
    public static class DirectoryManager
    {
        private static Dictionary<String, String> resource_dirs = new Dictionary<string, string>();
        public static bool RegisterResourceDir(String identifier, String a_path, bool overwrite = true)
        {
            if (!overwrite)
            {
                if (resource_dirs.ContainsKey(identifier))
                {
                    return false;
                }
                else
                {
                    resource_dirs.Add(identifier, a_path);
                }
            }
            else
            {
                if (resource_dirs.ContainsKey(identifier))
                {
                    resource_dirs[identifier] = a_path;
                }
                else
                {
                    resource_dirs.Add(identifier, a_path);
                }
            }
            return true;
        }

        public static void InitResourceDir()
        {
            if(!System.IO.Directory.Exists("resources"))
            {
                System.IO.Directory.CreateDirectory("resources");
            }

            if (!System.IO.Directory.Exists("resources/textures"))
            {
                System.IO.Directory.CreateDirectory("resources/textures");
            }

            if (!System.IO.Directory.Exists("resources/shaders"))
            {
                System.IO.Directory.CreateDirectory("resources/shaders");
            }

            if (!System.IO.Directory.Exists("resources/models"))
            {
                System.IO.Directory.CreateDirectory("resources/models");
            }

            if (!System.IO.Directory.Exists("resources/samples"))
            {
                System.IO.Directory.CreateDirectory("samples");
            }
        }

        public static bool DeleteResourceDir(String identifier)
        {
            return resource_dirs.Remove(identifier);
        }

        public static String GetResourceDir(String identifier)
        {
            resource_dirs.TryGetValue(identifier, out string found);
            return found;
        }

        public static String ConcatDirFile(String dir_path, String file)
        {
            String path = System.IO.Path.Combine(dir_path, file);
            return path;
        }


        public static String ImportSceneFromFile(String folder_id, String file, out Assimp.Scene scene)
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
