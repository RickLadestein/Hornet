using HornetEngine.Graphics;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.Threading;
namespace HornetEngine.Util
{
    public abstract class ResourceManager<T> where T : class
    {
        private Dictionary<String, T> resources;
        private Mutex access_mutex;
        public ResourceManager()
        {
            resources = new Dictionary<string, T>();
            access_mutex = new Mutex();
        }

        /// <summary>
        /// Checks if the collection contains a resource with given identifier
        /// </summary>
        /// <param name="identifier">The resource identifier</param>
        /// <returns></returns>
        public bool HasResource(string identifier)
        {
            access_mutex.WaitOne();
            bool found = resources.ContainsKey(identifier);
            access_mutex.ReleaseMutex();
            return found;
        }

        /// <summary>
        /// Adds a new resource with matching identifier to the collection
        /// </summary>
        /// <param name="identifier">The identifier for the resource</param>
        /// <param name="resource">The resource</param>
        /// <returns>true if resource was added, false if resource already existed under that specific identifier</returns>
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
            access_mutex.ReleaseMutex();
            if (succes)
            {
                return true;
            } else
            {
                return false;
            }
            
        }

        /// <summary>
        /// Gets a resource at specified identifier
        /// </summary>
        /// <param name="identifier">The resource identifier string</param>
        /// <returns>Resource if found, returns null if not found</returns>
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

        /// <summary>
        /// Sets a the resource at specified identifier if the collection already contains the identifier
        /// </summary>
        /// <param name="identifier">The resource identifier string</param>
        /// <param name="resource">The resource to be set at identifier</param>
        /// <returns>true if resource was set, false if the identifier is foreign to the collection</returns>
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

        /// <summary>
        /// Deletes resource with specified identifier from the collection
        /// </summary>
        /// <param name="identifier">The resource identifier string</param>
        /// <returns>true if resource was deleted, false if the resource was not found within the list</returns>
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
        private static object _lck = new object();
        public static MeshResourceManager Instance 
        {
            get
            {
                lock (_lck)
                {
                    if (instance == null)
                    {
                        instance = new MeshResourceManager();
                    }
                }
                return instance;
            }
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
        private static object _lck = new object();
        public static ShaderResourceManager Instance
        {
            get
            {
                lock (_lck)
                {
                    if (instance == null)
                    {
                        instance = new ShaderResourceManager();
                    }
                }
                return instance;
            }
        }

        private ShaderResourceManager() : base() { }
    }

    public class TextureResourceManager : ResourceManager<Texture>
    {
        private static TextureResourceManager instance;
        private static object _lck = new object();
        public static TextureResourceManager Instance
        {
            get
            {
                lock (_lck)
                {
                    if (instance == null)
                    {
                        instance = new TextureResourceManager();
                    }
                }
                return instance;
            }
        }

        private TextureResourceManager() : base() { }
    }
}
