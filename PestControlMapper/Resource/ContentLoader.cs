using Microsoft.Xna.Framework.Audio;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended.BitmapFonts;
using PestControlAnimation.Objects;
using PestControlEngine.Libs.Helpers;
using PestControlMapper.shared.Managers;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PestControlEngine.Resource
{
    public static class ContentLoader
    {
        private static Dictionary<string, Texture2D> _textures = new Dictionary<string, Texture2D>();
        private static Dictionary<string, BitmapFont> _fonts = new Dictionary<string, BitmapFont>();
        private static Dictionary<string, Effect> _effects = new Dictionary<string, Effect>();
        private static Dictionary<string, SoundEffect> _sounds = new Dictionary<string, SoundEffect>();
        private static Dictionary<string, Animation> _animations = new Dictionary<string, Animation>();

        public static string ContentPath 
        { 
            get
            {
                foreach(GameConfiguration config in PreferencesConfiguration.Preferences.GameConfigs)
                {
                    if (config.GameName == PreferencesConfiguration.Preferences.SelectedConfig)
                    {
                        return config.ContentPath;
                    }
                }

                return string.Empty;
            }
        }

        public static void LoadTextures(ContentManager Content)
        {

        }


        public static void LoadFonts(ContentManager Content)
        {
            BitmapFont EngineFont = Content.Load<BitmapFont>("engine_font");
            LoadFont("engine_font", EngineFont);
        }

        public static void LoadShaders(ContentManager Content)
        {
            //Effect blackwhite_gradient = Content.Load<Effect>("blackwhite_gradient");
            //LoadShader("blackwhite_gradient", blackwhite_gradient);
        }

        public static void LoadSounds(ContentManager Content)
        {

        }

        /// <summary>
        /// Returns texture from the _textures list. If the texture has not been loaded into memory, it will return null even if the texture exists in the game files or it will attempt to load the texture.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="device"></param>
        /// <param name="loadIfMissing"></param>
        /// <returns></returns>
        public static Texture2D GetTexture(string key, GraphicsDevice device, bool loadIfMissing = true)
        {
            _textures.TryGetValue(key.Replace(@"\\", "/"), out Texture2D tex);

            if (loadIfMissing && tex == null)
            {
                string filePath = $"{ContentPath}\\{key}.png";

                if (File.Exists(filePath))
                {
                    FileStream stream = new FileStream(Path.GetFullPath(filePath), FileMode.Open);
                    LoadTexture(key.Replace(@"\\", "/"), Texture2D.FromStream(device, stream));
                    stream.Dispose();

                    _textures.TryGetValue(key.Replace(@"\\", "/"), out tex);

                    if (tex == null)
                        Console.WriteLine("still null");
                }
            }

            return tex;
        }

        /// <summary>
        /// Returns animation from the _animations list. If the animation has not been loaded into memory, it will return null even if the animation exists in the game files or it will attempt to load the animation.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="device"></param>
        /// <param name="loadIfMissing"></param>
        /// <returns></returns>
        public static Animation GetAnimation(string key, bool loadIfMissing = true)
        {
            _animations.TryGetValue(key.Replace(@"\\", "/"), out Animation anim);

            if (loadIfMissing && anim == null)
            {
                string filePath = $"{ContentPath}\\{key}.pcaf";

                if (File.Exists(filePath))
                {
                    LoadAnimation(key.Replace(@"\\", "/"), Animation.ReadAnimationFile(filePath));

                    _animations.TryGetValue(key.Replace(@"\\", "/"), out anim);
                }
            }

            if (anim != null)
            {
                return anim.Copy();
            }

            return null;
        }

        /// <summary>
        /// Returns font from the _fonts list. If the font has not been loaded into memory, it will return null even if the font exists in the game files.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static BitmapFont GetFont(string key)
        {
            _fonts.TryGetValue(key, out BitmapFont fnt);

            if (fnt == null)
            {
                Console.WriteLine($"Content Loader Error: Could not find Font {key}");
                throw new FileNotFoundException($"Content Loader could not find the Font \"{key}\"");
            }

            return fnt;
        }
        /// <summary>
        /// Returns an effect from the _effect list. If the shader has not been loaded into memory, it will return null even if the shader exists in the game files.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static Effect GetShader(string key)
        {
            _effects.TryGetValue(key, out Effect fx);

            if (fx == null)
            {
                Console.WriteLine($"Content Loader Error: Could not find Shader {key}");
                throw new FileNotFoundException($"Content Loader could not find the Shader \"{key}\"");
            }

            return fx;
        }

        /// <summary>
        /// Returns a sound from the _sounds list. If the sound has not been loaded into memory, it will return null even if the sound exists in the game files.
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        public static SoundEffect GetSound(string key)
        {
            _sounds.TryGetValue(key, out SoundEffect snd);

            if (snd == null)
            {
                Console.WriteLine($"Content Loader Error: Could not find Sound {key}");
                throw new FileNotFoundException($"Content Loader could not find the Sound \"{key}\"");
            }

            return snd;
        }

        /// <summary>
        /// Loads the texture into the dictionary and in turn the memory.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="texture"></param>
        public static void LoadTexture(string key, Texture2D texture)
        {
            _textures[key] = texture;
        }

        /// <summary>
        /// Loads the font into the dictionary and in turn the memory.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="font"></param>
        public static void LoadFont(string key, BitmapFont font)
        {
            _fonts[key] = font;
        }

        /// <summary>
        /// Loads the font into the dictionary and in turn the memory.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="font"></param>
        public static void LoadShader(string key, Effect shader)
        {
            _effects[key] = shader;
        }

        /// <summary>
        /// Loads the sound into the dictionary and in turn the memory.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sound"></param>
        public static void LoadSound(string key, SoundEffect sound)
        {
            _sounds[key] = sound;
        }

        /// <summary>
        /// Loads the animation into the dictionary and in turn the memory.
        /// </summary>
        /// <param name="key"></param>
        /// <param name="sound"></param>
        public static void LoadAnimation(string key, Animation anim)
        {
            _animations[key] = anim;
        }
    }

}
