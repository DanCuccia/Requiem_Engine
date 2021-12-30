using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Engine.Managers;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;

namespace Engine.Drawing_Objects.Materials
{
    /// <summary>material used to interact with the AnimatedCloud.fx file</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class AnimatedCloudMaterial : Material
    {
        TextureCube noiseMap;
        Vector4 cloudColor = new Vector4(0.2f, 0.4f, 1f, 1f);

        /// <summary>Default CTOR</summary>
        /// <param name="drawable">drawable in which this material will apply to</param>
        public AnimatedCloudMaterial(Object3D drawable)
            : base(drawable)
        {
            base.ID = GameIDList.Shader_AnimatedClouds;
            base.effect = EffectManager.getInstance().GetEffect("shaders//AnimatedClouds");
            noiseMap = TextureManager.getInstance().GetTextureCube("noisemaps//noiseVolume");
        }

        /// <summary>Load all values that every same-type-material will use</summary>
        public override void InitializeShader()
        {
            effect.Parameters["view_proj_matrix"].SetValue(renderer.Camera.ViewMatrix * renderer.Camera.ProjectionMatrix);
            effect.Parameters["Noise_Tex"].SetValue(noiseMap);
            effect.Parameters["time_0_X"].SetValue(renderer.CurrentTiming.TotalGameTime.Milliseconds);
            effect.Parameters["cloudColor"].SetValue(cloudColor);
        }

        /// <summary>Update only values needed for this material</summary>
        public override void PreRenderUpdate()
        { }

        /// <summary>Apply the correct technique/pass to gpu</summary>
        public override void ApplyTechnique()
        {
            base.effect.CurrentTechnique = effect.Techniques["Clouds"];
            effect.CurrentTechnique.Passes[0].Apply();
        }

        //public override void EndShader()
        //{

        //}

        /// <summary>Create a copy of this material and attach it</summary>
        /// <param name="drawable">object to attach copied material to</param>
        /// <returns>the newly created material</returns>
        public override Material CopyAndAttach(Object3D drawable)
        { throw new NotImplementedException(); }

        /// <summary>Get the XML for this object -- unused</summary>
        public override MaterialXML GetXml()
        { throw new NotImplementedException(); }
    }
}
