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
    }
}
