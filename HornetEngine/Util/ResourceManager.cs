using HornetEngine.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
namespace HornetEngine.Util
{
    public class ResourceManager<T> where T : class
    {
        private Dictionary<String, T> resources;
        private Mutex access_mutex;
        public ResourceManager()
        {
            resources = new Dictionary<string, T>();
            access_mutex = new Mutex();
        }

        public bool AddResource(String identifier, T resource)
        {
            if(resource == null)
            {
                throw new ArgumentException("Resource cannot be null");
            }

            if (identifier == null || identifier.Length == 0)
            {
                throw new ArgumentException("Identifier cannot be null or empty");
            }

            access_mutex.WaitOne();
            bool succes = resources.TryAdd(identifier, resource);
            if(succes)
            {
                access_mutex.ReleaseMutex();
                return true;
            } else
            {
                access_mutex.ReleaseMutex();
                throw new Exception("An error ocurred while adding resource to the ResourceManager");
            }
            
        }

        public T GetResource(String identifier)
        {
            if(identifier == null || identifier.Length == 0)
            {
                throw new ArgumentException("Identifier cannot be null or empty");
            }

            access_mutex.WaitOne();
            bool succes = resources.TryGetValue(identifier, out T res);
            if (succes)
            {
                access_mutex.ReleaseMutex();
                return res;
            } else
            {
                access_mutex.ReleaseMutex();
                return null;
            }
        }

        public bool SetResource(String identifier, T resource)
        {
            if (resource == null)
            {
                throw new ArgumentException("Resource cannot be null");
            }

            if (identifier == null || identifier.Length == 0)
            {
                throw new ArgumentException("Identifier cannot be null or empty");
            }

            access_mutex.WaitOne();
            bool found = resources.ContainsKey(identifier);
            if(found)
            {
                access_mutex.ReleaseMutex();
                resources[identifier] = resource;
                return true;
            } else
            {
                access_mutex.ReleaseMutex();
                throw new Exception("Could not add resource to identifier because manager does not contain identifier");
            }
        }

        public bool DeleteResource(String identifier)
        {
            if (identifier == null || identifier.Length == 0)
            {
                throw new ArgumentException("Identifier cannot be null or empty");
            }
            access_mutex.WaitOne();
            bool success = resources.Remove(identifier);
            access_mutex.ReleaseMutex();
            return success;
        }
    }

    public class MeshResourceManager : ResourceManager<Mesh>
    {
        private static MeshResourceManager instance;
        public static MeshResourceManager GetInstance()
        {
            if(instance == null)
            {
                instance = new MeshResourceManager();
            }
            return instance;
        }

        public new void AddResource(String identifier, Mesh resource)
        {
            if(resource == null)
            {
                throw new ArgumentNullException("Mesh");
            }
            if(resource.Status != MeshStatus.READY)
            {
                throw new Exception($"Could not add resource: Mesh creation failed or is not done yet! \n Error_str: {resource.Error}");
            }
            base.AddResource(identifier, resource);
        }

        private MeshResourceManager() : base() { }
    }


    public class ShaderResourceManager : ResourceManager<ShaderProgram>
    {
        private static ShaderResourceManager instance;
        public static ShaderResourceManager GetInstance()
        {
            if (instance == null)
            {
                instance = new ShaderResourceManager();
            }
            return instance;
        }

        private ShaderResourceManager() : base() { }
    }

    public class TextureResourceManager : ResourceManager<Texture>
    {
        private static TextureResourceManager instance;
        public static TextureResourceManager GetInstance()
        {
            if(instance == null)
            {
                instance = new TextureResourceManager();
            }
            return instance;
        }

        private TextureResourceManager() : base() { }
    }
}
