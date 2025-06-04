using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Kaiju
{
    /// <summary>
    /// Data holder for creating character profiles in MenuState 
    /// </summary>
    public class CharacterProfile
    {
        public string Name { get; set; }
        public Texture2D Texture { get; set; }
        public Vector2 Position { get; set; }
        public bool FlipTexture { get; set; } = false;
        public Vector2 Origin => new Vector2(Texture.Width / 2, Texture.Height / 2);

        public CharacterProfile(string name, Texture2D texture, Vector2 position, bool flip = false)
        {
            this.Name = name;
            this.Texture = texture;
            this.Position = position;
            this.FlipTexture = flip;
        }
    }
}
