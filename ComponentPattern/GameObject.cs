using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;

namespace Kaiju.ComponentPattern
{
    public class GameObject
    {
        public Transform Transform { get; set; } = new Transform();
        private List<Component> components = new List<Component>();
        public string tag { get; set; }
        public void Awake()
        {
            foreach (var component in components)
            {
                component.Awake();
            }
        }
        public void Start()
        {
            foreach (var component in components)
            {
                component.Start();
            }
        }
        public void Update()
        {
            foreach (var component in components)
            {
                component.Update();
            }
        }
        public void Draw(SpriteBatch spriteBatch)
        {
            foreach (var component in components)
            {
                component.Draw(spriteBatch);
            }
        }

        public void OnCollisionEnter(Collider collider)
        {
            foreach (var component in components)
            {
                component.OnCollisionEnter(collider);
            }
        }

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
        public Component GetComponent<T>() where T : Component
        {
            return components.Find(x => x.GetType() == typeof(T));
        }
    }
}
