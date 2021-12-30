using System;
using Engine.Drawing_Objects.Materials;
using Engine.Managers;
using Engine.Managers.Camera;
using Engine.Managers.Factories;
using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using XNAnimation;
using XNAnimation.Controllers;

namespace Engine.Drawing_Objects
{
    /// <summary>All values needed to save and load this drawing object</summary>
    [Serializable]
    public class AnimatedObject3DXML : XMLMedium
    {
        /// <summary>filepath to the model of this object</summary>
        public string modelFilepath;
        /// <summary>world matrix xml of this object</summary>
        public WorldMatrixXml world;
        /// <summary>material of this object</summary>
        public MaterialXML material;
        /// <summary>is collidable</summary>
        public bool collidable;
    }

    /// <summary>AnimatedObject3D is a wrapper for XNAnimation skeleton animation library</summary>
    /// <author>Daniel Cuccia</author>
    public class AnimatedObject3D : Object3D
    {
        #region Member Variables

        string                  filePath;
        SkinnedModel            model;
        AnimationController     animController;
        float                   animSpeed = .05f;
        public float AnimationSpeed
        {
            set 
            { 
                this.animSpeed = value;
                animController.Speed = this.animSpeed;
            }
            get { return this.animSpeed; }
        }
        public string           CurrentAnimation = "";

        /// <summary>delegate callback for when animations complete</summary>
        public delegate void AnimationCompleteCallback();
        AnimationCompleteCallback callback;

        #endregion Member Variables

        #region Initialization

        /// <summary>Default CTOR</summary>
        public AnimatedObject3D()
            : base() { }

        /// <summary>Initialize model and animation controller,
        /// note: this will always render to the depth texture</summary>
        /// <param name="modelFilepath">filepath to the model in content</param>
        /// <param name="content">xna content manager</param>
        public void Initialize(ContentManager content, string modelFilepath)
        {
            filePath = modelFilepath;
            model = content.Load<SkinnedModel>(modelFilepath);
            if (model == null)
                throw new ArgumentNullException("AnimatedObject3D::Initialize model is null, could not load");
            animController = new AnimationController(model.SkeletonBones);
            animController.Speed = animSpeed;
            base.material = new NullAnimatedMaterial(this);
            this.depthMaterial = new DepthMaterial(this);
            base.renderDepth = true;

            foreach (ModelMesh mesh in model.Model.Meshes)
                foreach (ModelMeshPart part in mesh.MeshParts)
                    triangleCount += part.IndexBuffer.IndexCount;
            base.triangleCount /= 3;
        }

        #endregion Initialization

        #region API

        /// <summary>Update the animation controller</summary>
        public override void Update(ref Camera camera, GameTime time)
        {
            animController.Update(time.ElapsedGameTime, Matrix.Identity);

            if (OBB != null)
                OBB.Update(worldMatrix.GetWorldMatrix());
            
            if (callback != null && animController.IsPlaying == false)
            {
                callback();
                callback = null;
            }
        }

        /// <summary>Add this object to the Renderer's implicit call list</summary>
        public override void Render()
        {
            Renderer.getInstance().AddToBatch(this, this.material.ID);

            if (base.depthMaterial != null)
                renderer.AddToDepthBatch(this, base.depthMaterial.ID);
        }

        /// <summary>Renderer call to actually draw to screen</summary>
        /// <param name="effect">the current effect of the current batch</param>
        public override void RenderImplicit(Effect effect)
        {
            effect.Parameters["World"].SetValue(base.WorldMatrix.GetWorldMatrix());
            effect.Parameters["FinalTransforms"].SetValue(animController.SkinnedBoneTransforms);

            foreach (ModelMesh mesh in model.Model.Meshes)
            {
                foreach (ModelMeshPart part in mesh.MeshParts)
                {
                    part.Effect = effect;
                }
                mesh.Draw();
            }
        }

        /// <summary>draw any additional 3D debugging stuff in here</summary>
        public override void RenderDebug()
        {
            if (OBB != null)
                OBB.Render();
        }

        /// <summary>Begins an animation clip</summary>
        /// <param name="clip">the name of the clip you want to play</param>
        /// <param name="loopEnable">wether to loop or not</param>
        /// <param name="callback">if not looping, you can set a callback for when the animation completes</param>
        public void BeginAnimation(string clip, bool loopEnable = true, AnimationCompleteCallback callback = null)
        {
            CurrentAnimation = clip;
            bool found = false;
            foreach (System.Collections.Generic.KeyValuePair<string, AnimationClip> p in model.AnimationClips)
            {
                if (p.Key == clip)
                {
                    found = true;
                    break;
                }
            }

            if (found == false)
                return;

            animController.StartClip(model.AnimationClips[clip]);
            animController.LoopEnabled = loopEnable;

            if (loopEnable == false && callback != null)
                this.callback = callback;
        }

        /// <summary>Change the playback mode of the animation controller</summary>
        /// <param name="mode">mode to play the animation, forwards/backwards/looping</param>
        public void SetPlaybackMode(PlaybackMode mode)
        {
            animController.PlaybackMode = mode;
        }

        /// <summary>Generate the Oriented Bounding Box, OBB will be null until this is called</summary>
        public override void GenerateBoundingBox()
        {
            if (model == null)
                throw new ArgumentNullException("Object3D::GenerateBoundingBox - model is null");
            
            //todo FIX
            Vector3 modelMin = new Vector3(-model.Model.Meshes[0].BoundingSphere.Radius / 2.7f, 0, -model.Model.Meshes[0].BoundingSphere.Radius / 2.7f);
            Vector3 modelMax = new Vector3(model.Model.Meshes[0].BoundingSphere.Radius / 2.7f, model.Model.Meshes[0].BoundingSphere.Radius * 1.5f, 
                model.Model.Meshes[0].BoundingSphere.Radius/2.7f);

            OBB = new OrientedBoundingBox(modelMin, modelMax);
        }

        /// <summary>Update the OBB's world matrix, to match this model's world matrix</summary>
        public override void UpdateBoundingBox()
        {
            if (this.OBB != null)
                OBB.Update(this.worldMatrix.GetWorldMatrix());
        }

        /// <summary>Get the XML structure containing all information needed save and load this object</summary>
        /// <returns>a serializable class containing all data needed for this object</returns>
        public override XMLMedium GetXML()
        {
            AnimatedObject3DXML output = new AnimatedObject3DXML();
            output.modelFilepath = this.filePath;
            output.world = worldMatrix.GetXml();
            output.material = material.GetXml();
            output.collidable = this.collidable;
            return output;
        }

        /// <summary>Load all member variables from the input parameters</summary>
        /// <param name="inputXml">all information needed to load this object is found here</param>
        /// <param name="content">xna content manager</param>
        public override void CreateFromXML(ContentManager content, XMLMedium inputXml)
        {
            AnimatedObject3DXML input = inputXml as AnimatedObject3DXML;
            this.Initialize(content, input.modelFilepath);
            this.worldMatrix.FromXML(input.world);
            this.collidable = input.collidable;
            this.BeginAnimation("idle");
            MaterialBinder.getInstance().BindMaterial(this, input.material);
        }

        /// <summary> Get a copy of this animated drawable </summary>
        /// <param name="content">xna content manager</param>
        public override Object3D GetCopy(ContentManager content)
        {
            AnimatedObject3D output = new AnimatedObject3D();
            output.Initialize(content, filePath);
            output.Material = this.Material.CopyAndAttach(output);
            output.WorldMatrix = this.WorldMatrix.Clone();
            return output;
        }

        #endregion API

        /// <summary>XNAnimation controller object</summary>
        public AnimationController AnimationController
        {
            get { return this.animController; }
        }

        /// <summary>XNAnimation skinned model</summary>
        public SkinnedModel SkinnedModel
        {
            get { return this.model; }
        }

        /// <summary>overriden render depth to update depthMaterial</summary>
        public override bool RenderDepth
        {
            get { return base.renderDepth; }
            set
            { 
                base.renderDepth = value;
                if (value == true)
                {
                    base.depthMaterial = new DepthAnimatedMaterial(this);
                }
                else
                {
                    base.depthMaterial = null;
                }
            }
        }
    }
}
