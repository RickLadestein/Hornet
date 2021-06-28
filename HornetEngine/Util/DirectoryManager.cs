using System;
using System.Collections.Generic;
using System.Text;

namespace HornetEngine.Util
{
    public static class DirectoryManager
    {
        private static Dictionary<String, String> resource_dirs = new Dictionary<string, string>();

        /// <summary>
        /// A function which registers a resource directory
        /// </summary>
        /// <param name="identifier">The ID of the directory</param>
        /// <param name="a_path">The path to the directory</param>
        /// <param name="overwrite">A bool containing whether this should be overwritten</param>
        /// <returns></returns>
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

        /// <summary>
        /// A function which can delete a resource directory
        /// </summary>
        /// <param name="identifier">The ID of the resource directory</param>
        /// <returns></returns>
        public static bool DeleteResourceDir(String identifier)
        {
            return resource_dirs.Remove(identifier);
        }

        /// <summary>
        /// A function which obtains a resource directory
        /// </summary>
        /// <param name="identifier">The ID of the resource directory</param>
        /// <returns>Returnss a path to the resource directory</returns>
        public static String GetResourceDir(String identifier)
        {
            resource_dirs.TryGetValue(identifier, out string found);
            return found;
        }

        /// <summary>
        /// A function which concats a string to a file
        /// </summary>
        /// <param name="dir_path">The path to the directory</param>
        /// <param name="file">The path to the file</param>
        /// <returns>Returns a concatted string to the file</returns>
        public static String ConcatDirFile(String dir_path, String file)
        {
            String path = System.IO.Path.Combine(dir_path, file);
            return path;
        }
    }
}
