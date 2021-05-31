﻿using System;
using System.Collections.Generic;
using System.Text;

namespace HornetEngine.Ecs
{
    public class Entity
    {
        private List<Component> components;

        /// <summary>
        /// The scripts that are bound to the entity
        /// </summary>
        public List<MonoScript> Scripts { get; private set; }

        /// <summary>
        /// The identifier of this Entity
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// The custom name of this Entity
        /// </summary>
        public String Name { get; set; }

        /// <summary>
        /// The current transform of this entity
        /// </summary>
        public Transform Transform { get; private set; }

        /// <summary>
        /// The children this entity has
        /// </summary>
        public List<Entity> Children { get; private set; }

        

        /// <summary>
        /// Creates a new instance of Entity with default parameters
        /// </summary>
        public Entity()
        {
            components = new List<Component>();
            Scripts = new List<MonoScript>();
            Id = Guid.NewGuid();
            Name = "Entity";
            Transform = new Transform();
            Children = new List<Entity>();
        }

        /// <summary>
        /// Creates a new instance of Entity with custom name and default parameters
        /// </summary>
        /// <param name="name">The name that is to be assigned to the Entity</param>
        public Entity(String name)
        {
            if(name.Length == 0)
            {
                throw new ArgumentException("Entity name cannot be empty");
            } else
            {
                components = new List<Component>();
                Id = Guid.NewGuid();
                Name = name;
                Transform = new Transform();
                Children = new List<Entity>();
                Scripts = new List<MonoScript>();
            }
        }

        /// <summary>
        /// Adds a component to this Entity
        /// </summary>
        /// <param name="c">The component to add</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddComponent(Component c)
        {
            if(c == null)
            {
                throw new ArgumentNullException("Component cannot be null");
            } else
            {
                components.Add(c);
            }
        }

        /// <summary>
        /// Retrieves the first found component that matches the search parameter.
        /// </summary>
        /// <typeparam name="T">The component that is to be found</typeparam>
        /// <returns>Found component, if not found returns <c>null</c></returns>
        public T GetComponent<T>() where T : Component
        {
            foreach(Component c in components)
            {
                if(c != null && c is T t)
                {
                    return t;
                }
            }
            return null;
        }

        /// <summary>
        /// Retrieves all components that match the search parameter
        /// </summary>
        /// <typeparam name="T">The type of component that is to be found</typeparam>
        /// <param name="comps">A list where found c</param>
        public void GetComponents<T>(ref List<T> comps) where T : Component
        {
            foreach (Component c in components)
            {
                if (c != null && c is T t)
                {
                    comps.Add(t);
                }
            }
        }


        /// <summary>
        /// Adds a script to this Entity
        /// </summary>
        /// <param name="c">The script to add</param>
        /// <exception cref="ArgumentNullException"></exception>
        public void AddScript(MonoScript c)
        {
            if (c == null)
            {
                throw new ArgumentNullException("Component cannot be null");
            }
            else
            {
                c.entity = this;
                Scripts.Add(c);
            }
        }

        /// <summary>
        /// Retrieves the first found script that matches the search parameter.
        /// </summary>
        /// <typeparam name="T">The script that is to be found</typeparam>
        /// <returns>Found script, if not found returns <c>null</c></returns>
        public T GetScript<T>() where T : MonoScript
        {
            foreach (MonoScript c in Scripts)
            {
                if (c != null && c is T t)
                {
                    return t;
                }
            }
            return null;
        }

        /// <summary>
        /// Retrieves all scripts that match the search parameter
        /// </summary>
        /// <typeparam name="T">The type of script that is to be found</typeparam>
        /// <param name="comps">A list where found scripts are stored</param>
        public void GetScripts<T>(ref List<T> comps) where T : MonoScript
        {
            foreach (Component c in components)
            {
                if (c != null && c is T t)
                {
                    comps.Add(t);
                }
            }
        }
    }
}