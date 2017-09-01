using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using SFML;
using SFML.Audio;
using SFML.Graphics;
using static System.Windows.Media.ColorConverter;
namespace Horse.Engine.Utils
{
    /// <summary>
    /// An asset manager class that handles the loading and rendering of all assets
    /// </summary>
    public static class AssetManager
    {
        private static readonly Dictionary<string, string> StringAssets = new Dictionary<string, string>();

        private static readonly Dictionary<string, string> TextureAssests = new Dictionary<string, string>();

        private static readonly Dictionary<string, string> AudioAssets = new Dictionary<string, string>();

        private static readonly Dictionary<string, string> FontAssets = new Dictionary<string, string>();

        private static readonly Dictionary<string, string> ImageAssets = new Dictionary<string, string>();

        private static readonly Dictionary<string, string> ColorAssets = new Dictionary<string, string>();

        private static readonly string BaseFileLocation = Environment.CurrentDirectory + @"\Assets";

        private static readonly string AssetXmlFile = BaseFileLocation + @".\assets.xml";

        private static bool _loaded;

        /// <summary>
        /// Load in file locations of all assest found in assets.xml to be used for the game
        /// </summary>
        public static void LoadAssets()
        {
            if (_loaded)
                return;
            var fs = new FileStream(AssetXmlFile, FileMode.Open, FileAccess.Read);

            var xdoc = XDocument.Load(fs);
            var assets = from u in xdoc.Descendants("asset")
                        select new
                        {
                            Type = (string)u.Element("type"),
                            Name = (string)u.Element("name"),
                            Location = BaseFileLocation + (string)u.Element("location"),
                            Text = (string)u.Element("text"),
                            Order = (string)u.Element("order"),
                            FirstFrame = (string)u.Element("firstFrame"),
                            LastFrame = (string)u.Element("lastFrame"),
                            Frames = u.Element("frames"),
                            Width = (string)u.Element("width"),
                            Height = (string)u.Element("height")
                        };

            foreach (var asset in assets)
            {
                switch (asset.Type)
                {
                    case "font":
                        FontAssets.Add(asset.Name, asset.Location);
                        break;
                    case "texture":
                        TextureAssests.Add(asset.Name, asset.Location);
                        break;
                    case "audio":
                        AudioAssets.Add(asset.Name, asset.Location);
                        break;
                    case "text":
                        StringAssets.Add(asset.Name, asset.Text);
                        break;
                    case "image":
                        ImageAssets.Add(asset.Name, asset.Text);
                        break;
                    case "color":
                        ColorAssets.Add(asset.Name, asset.Text);
                        break;
                    default:
                        LogManager.LogWarning("Following asset could not be mapped"+asset);
                        break;
                }
            }
            fs.Close();
            _loaded = true;
        }

        /// <summary>
        /// Load a sprite
        /// </summary>
        /// <param name="name">The name of the texture for the sprite</param>
        /// <returns>The loaded sprite</returns>
        public static Sprite LoadSprite(string name)
        {
            Sprite sprite = null;
            try
            {
                var text = new Texture(TextureAssests[name]) { Smooth = true };
                sprite = new Sprite(text);
            }
            catch (Exception ex) when (ex is LoadingFailedException || ex is KeyNotFoundException)
            {
                LogManager.LogError(ex.Message + "\r\n" + ex.StackTrace);
            }
            return sprite;
        }

        /// <summary>
        /// Load an audio asset
        /// </summary>
        /// <param name="name">The nme of the asset</param>
        /// <returns>The loaded audio file</returns>
        public static Sound LoadSound(string name)
        {
            Sound sound = null;
            try
            {
                var buffer = new SoundBuffer(AudioAssets[name]);
                sound = new Sound(buffer);
            }
            catch (Exception ex) when (ex is LoadingFailedException || ex is KeyNotFoundException)
            {
                LogManager.LogError(ex.Message + "\r\n" + ex.StackTrace);
            }
            return sound;
        }

        /// <summary>
        /// Load a music asset
        /// </summary>
        /// <param name="name">The nme of the asset</param>
        /// <returns>The loaded music file</returns>
        public static Music LoadMusic(string name)
        {
            Music music = null;
            try
            {
                music = new Music(AudioAssets[name]);
            }
            catch (Exception ex) when (ex is LoadingFailedException || ex is KeyNotFoundException)
            {
                LogManager.LogError(ex.Message + "\r\n" + ex.StackTrace);
            }
            return music;
        }

        /// <summary>
        /// Load a string asset
        /// </summary>
        /// <param name="name">The nme of the asset</param>
        /// <returns>The loaded string</returns>
        public static string GetMessage(string name)
        {
            var mess = "";
            try
            {
                mess = StringAssets[name];
            }
            catch (KeyNotFoundException kex)
            {
                LogManager.LogError(kex.Message + "," + kex.Source);
            }
            return mess;
        }

        /// <summary>
        /// Load a font asset
        /// </summary>
        /// <param name="name">The name of the asset</param>
        /// <returns>The loaded font</returns>
        public static Font LoadFont(string name)
        {
            Font font = null;
            try
            {
                font = new Font(FontAssets[name]);
            }
            catch (Exception ex) when (ex is LoadingFailedException || ex is KeyNotFoundException)
            {
                LogManager.LogError(ex.Message + "\r\n" + ex.StackTrace);
            }
            return font;
        }

        /// <summary>
        /// Load an image asset
        /// </summary>
        /// <param name="name">The name of the asset</param>
        /// <returns>The loaded image</returns>
        public static Image LoadImage(string name)
        {
            Image image = null;
            try
            {
                image = new Image(ImageAssets[name]);
            }
            catch (Exception ex) when (ex is LoadingFailedException || ex is KeyNotFoundException)
            {
                LogManager.LogError(ex.Message + "\r\n" + ex.StackTrace);
            }
            return image;
        }

        /// <summary>
        /// Load an texture asset
        /// </summary>
        /// <param name="name">The name of the asset</param>
        /// <returns>The loaded image</returns>
        public static Texture LoadTexture(string name)
        {
            Texture text = null;
            try
            {
                text = new Texture(TextureAssests[name]);
            }
            catch (Exception ex) when (ex is LoadingFailedException || ex is KeyNotFoundException)
            {
                LogManager.LogError(ex.Message + "\r\n" + ex.StackTrace);
            }
            return text;
        }


        public static Color LoadColor(string name)
        {
            Color color = Color.Transparent;
            try
            {
                var hexVal = ConvertFromString(ColorAssets[name]);
                if (hexVal == null)
                    return color;
                var mediaColor = (System.Windows.Media.Color)hexVal;
                color = new Color(mediaColor.R, mediaColor.G, mediaColor.B, mediaColor.A);
            }
            catch (Exception ex) when (ex is LoadingFailedException || ex is KeyNotFoundException)
            {
                LogManager.LogError(ex.Message + "\r\n" + ex.StackTrace);
            }
            return color;
        }
    }
}
