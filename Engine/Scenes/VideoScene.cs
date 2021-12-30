using System;
using Engine.Managers;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using Microsoft.Xna.Framework.Media;

namespace Engine.Scenes
{
    /// <summary>Draw video to the screen
    /// -this scene can be used either as a full scene or an overlapping scene,
    /// -initialize filename, looping, and callback before loading this scene from scene manager,
    /// -if you do not assign a callback without looping, video will stop and display nothing, awaiting external commands
    /// -feature: externally set this Rectangle member variable, and play the video within it's own "viewport"
    /// -Warning: as of now this scene WILL STRETCH TO THE GAME'S ASPECT RATIO</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class VideoScene : Scene
    {
        #region Member Vars

        /// <summary>filename of the video in content</summary>
        public string videoFilename;
        /// <summary>wether the video loops or not</summary>
        public bool loop = false;

        Video video;
        VideoPlayer player;
        Texture2D texture;

        /// <summary>destination rect of the video</summary>
        public Rectangle destRect = Rectangle.Empty;

        /// <summary>delegate called when this video is complete</summary>
        public delegate void VideoCompleteCallback();
        /// <summary>video complete callback</summary>
        public VideoCompleteCallback callback;
        
        #endregion Member Vars

        #region Initialization and Closing

        /// <summary>Default CTOR</summary>
        public VideoScene(Microsoft.Xna.Framework.Game g, String name, SceneManager sm)
            : base(g, name, sm)
        {        }

        /// <summary>Load assets and initialize player</summary>
        public override void Initialize()
        {
            if (videoFilename == null)
                throw new ArgumentNullException("videoFilename has not been initialized, do this before you switch scenes");
            try
            {
                video = content.Load<Video>(videoFilename);
                player = new VideoPlayer();
                player.IsLooped = loop;
                player.Play(video);
                isInitialized = true;
            }
            catch (Exception e)
            {
                if (EngineFlags.drawDebug)
                    Math_Physics.MyUtility.DumpException(e);
                isInitialized = false;
            }
        }

        /// <summary>release assets</summary>
        public override void Close()
        {
            loop = false;
            video = null;
            player = null;
            texture = null;
            videoFilename = null;
            destRect = Rectangle.Empty;
            isInitialized = false;
        }

        #endregion Initialization and Closing

        #region API

        /// <summary>Main Input method, called BEFORE update()</summary>
        public override void Input(KeyboardState kb, MouseState ms, GamePadState gp1)
        {
        }

        /// <summary>Main update Method called AFTER input()</summary>
        public override void Update(GameTime time)
        {
            if (player.State == MediaState.Stopped)
            {
                if (callback != null)
                {
                    callback();
                }
            }
        }

        /// <summary>Main Render 3D call</summary>
        public override void Render3D(GameTime time)
        {
        }

        /// <summary>Render 3D debugging information</summary>
        public override void RenderDebug3D(GameTime time)
        {
        }

        /// <summary>Main Render 2D call using spritebatch</summary>
        public override void Render2D(GameTime time, SpriteBatch batch)
        {
            if (player.State != MediaState.Stopped)
                texture = player.GetTexture();

            if (texture != null)
            {
                if (destRect != Rectangle.Empty)
                {
                    batch.Draw(texture, destRect, Color.White);
                }
                else
                {
                    batch.Draw(texture, new Rectangle(
                        device.Viewport.X,
                        device.Viewport.Y,
                        device.Viewport.Width,
                        device.Viewport.Height), 
                        Color.White);
                }
            }
        }

        /// <summary>Render any 2D debugging information here</summary>
        public override void RenderDebug2D(GameTime time, SpriteBatch batch)
        {
        }

        #endregion API
    }
}
