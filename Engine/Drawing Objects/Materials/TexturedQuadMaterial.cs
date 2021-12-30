using System;
using Engine.Managers;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Drawing_Objects.Materials
{
    /// <summary>Serializable class to this texured quad material</summary>
    [Serializable]
    public class TexturedQuadMaterialXML : MaterialXML
    {
        /// <summary>material id</summary>
        public int id = -1;
        /// <summary>diffuse filepath</summary>
        public string textureFilepath = "";
        /// <summary>color blend amount</summary>
        public float colorBlend = .5f;
    }

    /// <summary>Material used to draw 3D billboards</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class TexturedQuadMaterial : Material
    {
        /// <summary>0f = 100% color, 1f = 100% texture</summary>
        float colorBlend = .5f;

        Texture2D texture = null;

        /// <summary>Default CTOR</summary>
        /// <param name="drawable">reference to the drawable this material is being applied to</param>
        /// <param name="texture">texture reference</param>
        public TexturedQuadMaterial(Object3D drawable, Texture2D texture)
            : base(drawable)
        {
            base.ID = GameIDList.Shader_Billboard;
            base.Initialize(EffectManager.getInstance().GetEffect("shaders//Billboard"));
            if (texture != null)
                this.texture = texture;
        }

        /// <summary>initialize all shader parameters, which all of this material will be using</summary>
        public override void InitializeShader()
        {
            base.effect.Parameters["View"].SetValue(base.renderer.Camera.ViewMatrix);
            base.effect.Parameters["Projection"].SetValue(base.renderer.Camera.ProjectionMatrix);
            renderer.Device.BlendState = BlendState.AlphaBlend;
            renderer.Device.RasterizerState = RasterizerState.CullNone;
        }

        /// <summary>undo the blendstate changes</summary>
        public override void EndShader()
        {
            renderer.Device.BlendState = BlendState.Opaque;
            renderer.Device.RasterizerState = RasterizerState.CullCounterClockwise;
        }

        /// <summary>update shader parameters relavant only to this material and it's drawable</summary>
        public override void PreRenderUpdate()
        {
            if (this.texture != null)
                base.effect.Parameters["Texture"].SetValue(this.texture);
            base.effect.Parameters["ColorBlendAmount"].SetValue(colorBlend);
            base.effect.Parameters["World"].SetValue(drawable.WorldMatrix.GetWorldMatrix());
        }

        /// <summary>Apply the correct technique from the effect file</summary>
        public override void ApplyTechnique()
        {
            if (texture != null)
                base.effect.CurrentTechnique = base.effect.Techniques["TexturedQuads"];
            else base.effect.CurrentTechnique = base.effect.Techniques["Quads"];
            base.effect.CurrentTechnique.Passes[0].Apply();
        }

        /// <summary>Get the serializable class relating to this material</summary>
        /// <returns>serializable class with all relavent data to replicate this material</returns>
        public override MaterialXML GetXml()
        {
            TexturedQuadMaterialXML output = new TexturedQuadMaterialXML();
            output.colorBlend = this.colorBlend;
            output.id = this.ID;
            output.textureFilepath = TextureManager.getInstance().GetFilepath(texture);
            return output;
        }

        /// <summary>Create a new material of this type, attaching it to the input drawable</summary>
        /// <param name="drawable">what the new material will attach to</param>
        /// <returns>the newly created material, properly attached to the input drawable</returns>
        public override Material CopyAndAttach(Object3D drawable)
        {
            TexturedQuadMaterial mat = new TexturedQuadMaterial(drawable, this.texture);
            mat.ColorBlend = this.ColorBlend;
            return mat;
        }

        /// <summary>this 0-1 factor depicts: 0f=Color, 1f=Texture</summary>
        public float ColorBlend
        {
            get { return this.colorBlend; }
            set { this.colorBlend = value; }
        }

        /// <summary>reference to texture</summary>
        public Texture2D Texture
        {
            get { return this.texture; }
            set { this.texture = value; }
        }
    }
}
