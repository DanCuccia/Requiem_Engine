using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Engine.Managers;
#pragma warning disable 1591

namespace Engine.Drawing_Objects.Materials
{
    /// <summary>Water Material serializable class</summary>
    [Serializable]
    public class WaterMaterialXML : MaterialXML
    {
        public int id;
        public WaterSettings settings;
        public string refractionTexture;
        public string skyboxTexture;
    }

    /// <summary>A wrapper to contain all possilble water settings</summary>
    /// <author>Daniel Cuccia</author>
    [Serializable]
    public class WaterSettings
    {
        public float        BumpHeight          = .5f;
        public Vector2      TextureScale        = new Vector2(.5f, .5f);
        public Vector2      BumpSpeed           = Vector2.Zero;
        public float        FresnelBias         = .025f;
        public float        FresnelPower        = 128f;
        public float        HDRMultiplier       = 1f;
        public Vector4      DeepColor           = new Vector4(0f, .4f, .5f, 1f);
        public Vector4      ShallowColor        = new Vector4(.55f, .75f, .75f, 1f);
        public Vector4      ReflectionColor     = new Vector4(1f, 1f, 1f, 1f);
        public float        ReflectionAmount    = .5f;
        public float        WaterAmount         = .5f;

        public const float Max_FresnalBias = 2f;
        public const float Min_FresnalBias = 0.001f;

        public const float Max_FresnalPower = 256;
        public const float Min_FresnalPower = 1f;

        public const float Min_HDRMultiplier = 0f;
        public const float Max_HDRMultiplier = 12;

        public const float Min_ReflectionAmount = 0f;
        public const float Max_ReflectionAmount = 1f;

        public const float Min_WaterAmount = 0f;
        public const float Max_WaterAmount = 1f;

        public const float Min_TextureScale = .5f;
        public const float Max_TextureScale = 11f;

        public const float Min_BumpSpeed = -1f;
        public const float Max_BumpSpeed = 1f;
    }

    /// <summary> Water Material </summary>
    public sealed class WaterMaterial : Material
    {
        Texture2D refractionTexture;
        TextureCube skyboxTexture;
        WaterSettings settings = new WaterSettings();

        /// <summary>Default CTOR</summary>
        public WaterMaterial(Object3D drawable, Texture2D refractionMap, TextureCube skyboxTexture)
            : base(drawable)
        {
            base.id = GameIDList.Shader_Water;
            base.Initialize(EffectManager.getInstance().GetEffect("shaders//Water"));

            if (refractionMap != null)
                this.refractionTexture = refractionMap;
            else refractionMap = TextureManager.getInstance().GetTexture("error");

            if (skyboxTexture != null)
                this.skyboxTexture = skyboxTexture;
        }

        /// <summary>initialize shader parameters</summary>
        public override void InitializeShader()
        {
            base.effect.Parameters["View"].SetValue(renderer.Camera.ViewMatrix);
            base.effect.Parameters["ViewInverse"].SetValue(renderer.Camera.ViewInverseMatrix);
            base.effect.Parameters["Projection"].SetValue(renderer.Camera.ProjectionMatrix);
            base.effect.Parameters["Time"].SetValue((float)renderer.CurrentTiming.TotalGameTime.TotalSeconds);
            renderer.Device.BlendState = BlendState.Opaque;
        }

        /// <summary>update shader parameters</summary>
        public override void PreRenderUpdate()
        {
            if (refractionTexture != null)
                base.effect.Parameters["WaterNormalMap"].SetValue(this.refractionTexture);
            if (skyboxTexture != null)
                base.effect.Parameters["SkyBoxTexture"].SetValue(this.skyboxTexture);

            base.effect.Parameters["BumpHeight"].SetValue(settings.BumpHeight);
            base.effect.Parameters["TextureScale"].SetValue(settings.TextureScale);
            base.effect.Parameters["BumpSpeed"].SetValue(settings.BumpSpeed);
            base.effect.Parameters["FresnelBias"].SetValue(settings.FresnelBias);
            base.effect.Parameters["FresnelPower"].SetValue(settings.FresnelPower);
            base.effect.Parameters["HDRMultiplier"].SetValue(settings.HDRMultiplier);
            base.effect.Parameters["DeepColor"].SetValue(settings.DeepColor);
            base.effect.Parameters["ShallowColor"].SetValue(settings.ShallowColor);
            base.effect.Parameters["ReflectionColor"].SetValue(settings.ReflectionColor);
            base.effect.Parameters["ReflectionAmount"].SetValue(settings.ReflectionAmount);
            base.effect.Parameters["WaterAmount"].SetValue(settings.WaterAmount);
            
        }

        /// <summary>Apply the correct technique and pass to gpu</summary>
        public override void ApplyTechnique()
        {
            base.effect.CurrentTechnique = base.effect.Techniques["Water"];
            base.effect.CurrentTechnique.Passes[0].Apply();
        }

        /// <summary>All settings used to variate the display</summary>
        public WaterSettings Settings
        {
            get { return this.settings; }
            set { this.settings = value; }
        }

        /// <summary>Get the serializable object for this class</summary>
        /// <returns>blah blah blah, I've typed this a dozen times now</returns>
        public override MaterialXML GetXml()
        {
            WaterMaterialXML output = new WaterMaterialXML();
            output.id = this.id;
            output.settings = this.settings;
            output.refractionTexture = TextureManager.getInstance().GetFilepath(this.refractionTexture);
            output.skyboxTexture = TextureManager.getInstance().GetFilepathCube(this.skyboxTexture);
            return output;
        }

        /// <summary>and a copy of this material</summary>
        /// <param name="drawable">what you want the copy to attach to</param>
        /// <returns>newly created water material</returns>
        public override Material CopyAndAttach(Object3D drawable)
        {
            WaterMaterial mat = new WaterMaterial(drawable, this.refractionTexture, this.skyboxTexture);
            mat.Ambient = this.Ambient;
            mat.Diffuse = this.Diffuse;
            mat.Specular = this.Specular;
            mat.Shine = this.Shine;
            mat.settings = this.settings;
            return mat;
        }
    }
}
