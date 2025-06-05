using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Kaiju.ComponentPattern
{
    /// <summary>
    /// The main gameobject class all gameobject inherit from
    /// Part of the component design pattern
    /// </summary>
    public class GameObject
    {
        public Transform Transform { get; set; } = new Transform();
        private List<Component> components = new List<Component>();
        public string tag { get; set; }

        /// <summary>
        /// Initializes the gameobject
        /// Gets run before start
        /// </summary>
        public void Awake()
        {
            foreach (var component in components)
            {
                component.Awake();
            }
        }

        /// <summary>
        /// Initializes the gameobject
        /// Gets run after awake
        /// </summary>
        public void Start()
        {
            foreach (var component in components)
            {
                component.Start();
            }
        }

        /// <summary>
        /// The main loop of the gameobject
        /// Runs every frame
        /// </summary>
        public void Update()
        {
            foreach (var component in components)
            {
                component.Update();
            }
        }

        /// <summary>
        /// The main drawing loop of the gameobject
        /// </summary>
        /// <param name="spriteBatch">The SpriteBatch used to draw</param>
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var component in components)
            {
                component.Draw(spriteBatch);
            }
        }

        /// <summary>
        /// Handles collisions for the gameobject
        /// </summary>
        /// <param name="collider">The collider the gameobject collided with</param>
        public void OnCollisionEnter(Collider collider)
        {
            foreach (var component in components)
            {
                component.OnCollisionEnter(collider);
            }
        }

        /// <summary>
        /// Adds a component to the gameobject
        /// </summary>
        /// <typeparam name="T">component</typeparam>
        /// <param name="additionalParameters">Additional parameters the component might need</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"></exception>
        public T AddComponent<T>(params object[] additionalParameters) where T : Component
        {
            Type componentType = typeof(T);
            try
            {
                // Opret en instans ved hjælp af den fundne konstruktør og leverede parametre
                object[] allParameters = new object[1 + additionalParameters.Length];
                allParameters[0] = this;
                Array.Copy(additionalParameters, 0, allParameters, 1, additionalParameters.Length);

                T component = (T)Activator.CreateInstance(componentType, allParameters);
                components.Add(component);
                return component;
            }
            catch (Exception)
            {
                // Håndter tilfælde, hvor der ikke er en passende konstruktør
                throw new InvalidOperationException($"Klassen {componentType.Name} har ikke en " +
                    "konstruktør, der matcher de leverede parametre.");
            }
        }

        /// <summary>
        /// Gets the component of a gameobject
        /// </summary>
        /// <typeparam name="T">Components</typeparam>
        /// <returns></returns>
        public Component GetComponent<T>() where T : Component
        {
            return components.Find(x => x.GetType() == typeof(T));
        }
    }
}
