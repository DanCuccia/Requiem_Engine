using System;
using System.Collections.Generic;
using Engine.Drawing_Objects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Graphics;
using Engine.Managers.Camera;

namespace Engine.Managers
{
    /// <summary>used to index into the bloomSettings array</summary>
    public enum BloomPresetIndices
    {
        /// <summary>a default mix between subtle and soft</summary>
        BLOOM_DEFAULT = 0,
        /// <summary>soft light with 1.5px blur</summary>
        BLOOM_SOFT,
        /// <summary>desaturates the image</summary>
        BLOOM_DESATURATED,
        /// <summary>heavily saturates the image</summary>
        BLOOM_SATURATED,
        /// <summary>2px blur only</summary>
        BLOOM_BLURRY,
        /// <summary>a nice 'middle' blend</summary>
        BLOOM_SUBTLE,
        /// <summary>the renderer settings during the pause menu</summary>
        BLOOM_PAUSE
    }

    /// <summary>Class holds all the settings used to tweak the bloom effect.</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class BloomSettings
    {
        #region Member Variables

        /// <summary>Name of a preset bloom setting, for display to the user.</summary>
        public string Name;

        /// <summary>Controls how bright a pixel needs to be before it will bloom.
        ///Zero makes everything bloom equally, while higher values select
        ///only brighter colors. Somewhere between 0.25 and 0.5 is good.</summary>
        public float BloomThreshold;

        /// <summary>Controls how much blurring is applied to the bloom image.
        ///The typical range is from 1 up to 10 or so.</summary>
        public float BlurAmount;

        /// <summary>Controls the amount of the bloom and base images that
        /// will be mixed into the final scene. Range 0 to 1.</summary>
        public float BloomIntensity;

        /// <summary>multiplier of the blooming effect</summary>
        public float BaseIntensity;

        /// <summary>Independently control the color saturation of the bloom and
        /// base images. Zero is totally desaturated, 1.0 leaves saturation
        /// unchanged, while higher values increase the saturation level.</summary>
        public float BloomSaturation;

        /// <summary>saturation of the scene</summary>
        public float BaseSaturation;

        #endregion Member Variables

        /// <summary>Constructs a new bloom settings descriptor.</summary>
        public BloomSettings(string name, float bloomThreshold, float blurAmount,
                             float bloomIntensity, float baseIntensity,
                             float bloomSaturation, float baseSaturation)
        {
            Name = name;
            BloomThreshold = bloomThreshold;
            BlurAmount = blurAmount;
            BloomIntensity = bloomIntensity;
            BaseIntensity = baseIntensity;
            BloomSaturation = bloomSaturation;
            BaseSaturation = baseSaturation;
        }


        /// <summary>Table of preset bloom settings</summary>
        public static BloomSettings[] PresetSettings =
        {
            //                Name           Thresh  Blur   Bloom  Base  BloomSat BaseSat
            new BloomSettings("Default",     0.25f,   4f,   1.25f,  1f,    1f,       1f),
            new BloomSettings("Soft",        0f,      3f,   1f,     1f,    1f,       1f),
            new BloomSettings("Desaturated", 0.5f,    8f,   2f,     1f,    0f,       1f),
            new BloomSettings("Saturated",   0.25f,   4f,   2f,     1f,    2f,       0f),
            new BloomSettings("Blurry",      0f,      2f,   1f,     0.1f,  1f,       1f),
            new BloomSettings("Subtle",      0.5f,    2f,   1f,     1f,    1f,       1f),
            new BloomSettings("Pause",       0.8f,    12f,  4f,     1f,    3f,       0f)
        };
    }

    /// <summary>When objects are called to draw, they actually go into one of these DrawBatches
    /// This is done for optimization so the renderer will draw objects in batches according to material</summary>
    public sealed class MaterialDrawBatch
    {
        /// <summary>id of the material</summary>
        public int materialID;
        /// <summary>batch of drawables using this material</summary>
        public List<Object3D> batch = new List<Object3D>();
        private Effect effect;
        private Renderer renderer = Renderer.getInstance();

        /// <summary>Adds any type of Object3D to the batch list</summary>
        /// <param name="obj">an object that will be drawn with this material</param>
        public void Add(Object3D obj)
        {
            if(obj != null)
                batch.Add(obj);
        }

        /// <summary>Initialize the shader parameters which all objects will share
        /// eg. view matrix, projection matrix etc..</summary>
        public void StartBatch()
        {
            if (batch.Count == 0)
                return;
            this.effect = batch[0].Material.Effect;
            batch[0].Material.InitializeShader();
        }

        /// <summary>Undoes any rasterizing changes that would screw up other shaders</summary>
        public void EndBatch()
        {
            batch[0].Material.EndShader();
        }

        /// <summary>Initialize shader parameters using the depth material</summary>
        public void StartDepthBatch()
        {
            if (batch.Count == 0)
                return;
            this.effect = batch[0].DepthMaterial.Effect;
            batch[0].DepthMaterial.InitializeShader();
        }

        /// <summary> Now that the correct effect is active, preRender updates any per-model shader fields
        /// eg. textures, colors, light positions etc...</summary>
        public void DrawBatch()
        {
            if (EngineFlags.sortBillboards == true && materialID == GameIDList.Shader_Billboard)
            {
                Camera.Camera cam = renderer.Camera;
                billboardSort(ref batch, ref cam);
            }
            foreach (Object3D obj in batch)
            {
                obj.Material.PreRenderUpdate();
                obj.Material.ApplyTechnique();
                obj.RenderImplicit(this.effect);
            }
        }
        
        /// <summary>prepare a float array of distances of all quads from camera, and sort from furthest to closest</summary>
        /// <param name="input">reference of a list of drawables</param>
        /// <param name="camera">reference to whatever camera</param>
        private void billboardSort(ref List<Object3D> input, ref Managers.Camera.Camera camera)
        {
            float[] distMap = new float[input.Count];
            for (int ind = 0; ind < input.Count; ind++)
                distMap[ind] = Vector3.Distance(camera.Position, input[ind].WorldMatrix.Position);
            quickSort(distMap, 0, distMap.Length -1);
        }

        /// <summary>basic recursive quicksort alg</summary>
        /// <param name="a">distance array</param>
        /// <param name="left">left index</param>
        /// <param name="right">right index</param>
        private void quickSort(float[] a, int left, int right)
        {
            if (a == null) return;
            int i = left;
            int j = right;
            float p = a[(left + right) / 2];

            while (i <= j)
            {
                while (a[i] > p) i++;
                while (a[j] < p) j--;

                if (i <= j)
                {
                    float tf = a[i];
                    Object3D to = batch[i];
                    a[i] = a[j];
                    batch[i] = batch[j];
                    a[j] = tf;
                    batch[j] = to;
                    i++;
                    j--;
                }
            }
            if (j > left)
            {
                quickSort(a, left, j);
            }
            if (i < right)
            {
                quickSort(a, i, right);
            }
        }

        /// <summary>Draw drawables using their DepthMaterial</summary>
        public void DrawDepthBatch()
        {
            foreach (Object3D obj in batch)
            {
                obj.DepthMaterial.PreRenderUpdate();
                obj.DepthMaterial.ApplyTechnique();
                obj.RenderImplicit(this.effect);
            }
        }
    }

    /// <summary>This class is designed to be the meca of drawing
    /// Encapsulates Rendering Algorithms for 2D and 3D drawing,
    /// Is a singleton, Accessable anywhere,
    /// note: the camera variable must be assigned elsewhere</summary>
    public sealed class Renderer
    {
        private static Renderer myInstance;
        /// <summary>get singleton instance</summary>
        /// <returns>singleton instance</returns>
        public static Renderer getInstance()
        {
            if (myInstance == null)
                myInstance = new Renderer();
            return myInstance;
        }

        #region Member Variables

        int                         gameStart;
        GameTime                    currentGameTime;

        GraphicsDevice              device;       //ref
        Camera.Camera               camera;       //ref

        Effect                      hdrProcess;
        Effect                      megaProcess;

        RenderTarget2D              sceneTarget;
        RenderTarget2D              blurTarget1;
        RenderTarget2D              megaPtlTarget;

        BloomSettings               bloomSettings = BloomSettings.PresetSettings[2];
        Vector2                     halfPixel;
        ScreenAlignedQuad           screenAlignedQuad;
        EffectManager               effectManager;
        SpriteFont                  font = null;

        /// <summary>Color the backbuffers get cleared to, set/get</summary>
        public Color ClearColor { set; get; }

        /// <summary>This is the whole list of environment lighting</summary>
        List<PointLight>            environmentLightList;
        /// <summary>This is the actual draw list, which scenes queue within</summary>
        List<MaterialDrawBatch>     drawBatchList = new List<MaterialDrawBatch>();
        /// <summary>EXPERIMENTAL - extra draw to scene depth batch</summary>
        List<MaterialDrawBatch>     depthBatchList = new List<MaterialDrawBatch>();
        /// <summary>megaSpheres need to render into their own texture, and are called to do so seperately</summary>
        List<MaterialDrawBatch>     megaParticleBatchList = new List<MaterialDrawBatch>();

        #endregion Member Variables

        #region Initialization

        /// <summary>Null Constructor, must call Initialize()</summary>
        private Renderer() 
        {
            ClearColor = Color.DarkSlateGray;
        }

        /// <summary>Main Initialization </summary>
        /// <param name="graphics">the device we'll be rendering with</param>
        /// <param name="startTime">the time our renderer turned on</param>
        /// <param name="content">xna content manager</param>
        public void Initialize(GraphicsDevice graphics, ContentManager content, int startTime)
        {
            device = graphics;
            gameStart = startTime;
            halfPixel = new Vector2(
                .5f / (float)device.Viewport.Width,
                .5f / (float)device.Viewport.Height);

            createRenderTargets();

            effectManager = EffectManager.getInstance();
            hdrProcess = effectManager.GetEffect("shaders//BloomPostProcess");
            megaProcess = effectManager.GetEffect("shaders//MegaParticlePostProcess");
            megaProcess.Parameters["ScreenHalfPixel"].SetValue(halfPixel);
            font = content.Load<SpriteFont>("fonts//Arial");

            screenAlignedQuad = new ScreenAlignedQuad(device);
        }

        /// <summary>initialize the imposter textures</summary>
        private void createRenderTargets()
        {
            sceneTarget = new RenderTarget2D(device,
                device.PresentationParameters.BackBufferWidth,
                device.PresentationParameters.BackBufferHeight,
                true,
                device.DisplayMode.Format,
                DepthFormat.Depth24,
                device.PresentationParameters.MultiSampleCount,
                RenderTargetUsage.DiscardContents);

            blurTarget1 = new RenderTarget2D(device,
                device.PresentationParameters.BackBufferWidth,
                device.PresentationParameters.BackBufferHeight,
                true,
                SurfaceFormat.Alpha8,
                DepthFormat.Depth24,
                device.PresentationParameters.MultiSampleCount,
                RenderTargetUsage.DiscardContents);

            megaPtlTarget = new RenderTarget2D(device,
                device.PresentationParameters.BackBufferWidth,
                device.PresentationParameters.BackBufferHeight,
                true,
                SurfaceFormat.Alpha8,
                DepthFormat.Depth24,
                device.PresentationParameters.MultiSampleCount,
                RenderTargetUsage.DiscardContents);
        }

        /// <summary>Registers the list of lights which 3D drawing objects will be lit by.
        /// if no lights are registered, default global light will be used</summary>
        /// <param name="lightList">list of lights, this must be initialized</param>
        public void RegisterLightList(ref List<PointLight> lightList)
        {
            if (lightList != null)
                this.environmentLightList = lightList;
            else Console.WriteLine("Renderer::RegisterLightList - the input lightLight was found null, switching to global lighting");
        }

        /// <summary>Sets the LightList to null - shaders will use global lighting instead</summary>
        public void UnRegisterLightList()
        {
            environmentLightList = null;
        }

        #endregion Init

        #region Run-Time

        /// <summary>Adds the drawable Object3D to a batch of other objects of the same material,
        /// this way we can optimize our shaders to draw 1 batch of same-material objects at one</summary>
        /// <param name="drawable">drawable object to add to the draw list</param>
        /// <param name="materialId">the id of which material this object is drawing</param>
        public void AddToBatch(Object3D drawable, int materialId)
        {
            if (materialId == GameIDList.Shader_MegaParticle)
            {
                addToMegaBatch(drawable, materialId);
                return;
            }

            foreach (MaterialDrawBatch batch in drawBatchList)
            {
                if (batch.materialID == materialId)
                {
                    batch.Add(drawable);
                    return;
                }
            }

            MaterialDrawBatch materialBatch = new MaterialDrawBatch();
            materialBatch.materialID = materialId;
            materialBatch.Add(drawable);
            drawBatchList.Add(materialBatch);
        }

        /// <summary>Add a mega-particle type to the batch list for rendering</summary>
        /// <param name="drawable">drawable to be rendered</param>
        /// <param name="materialId">what shader it uses</param>
        private void addToMegaBatch(Object3D drawable, int materialId)
        {
            foreach (MaterialDrawBatch batch in megaParticleBatchList)
            {
                if (batch.materialID == materialId)
                {
                    batch.Add(drawable);
                    return;
                }
            }
            MaterialDrawBatch materialBatch = new MaterialDrawBatch();
            materialBatch.materialID = materialId;
            materialBatch.Add(drawable);
            megaParticleBatchList.Add(materialBatch);
        }

        /// <summary>will draw to scene depth pass</summary>
        /// <param name="drawable">obj to be drawn into scene depth pass</param>
        /// <param name="materialId">id of the material this object is drawing with</param>
        public void AddToDepthBatch(Object3D drawable, int materialId)
        {
            foreach (MaterialDrawBatch batch in depthBatchList)
            {
                if (batch.materialID == materialId)
                {
                    batch.Add(drawable);
                    return;
                }
            }

            MaterialDrawBatch materialBatch = new MaterialDrawBatch();
            materialBatch.materialID = materialId;
            materialBatch.Add(drawable);
            depthBatchList.Add(materialBatch);
        }

        /// <summary>After all Scene Draw logic is completed, this will iterate all objects in all material batches,
        /// and draw them to screen. Afterwards, the list is dumped to start the next frame</summary>
        public void DrawDiffuseTexture()
        {
            //force the billboard batch to draw last
            for (int i = 0; i < drawBatchList.Count; i++)
            {
                if (drawBatchList[i].materialID == GameIDList.Shader_Billboard)
                {
                    MaterialDrawBatch t = drawBatchList[drawBatchList.Count - 1];
                    drawBatchList[drawBatchList.Count - 1] = drawBatchList[i];
                    drawBatchList[i] = t;
                    break;
                }
            }

            foreach (MaterialDrawBatch batch in drawBatchList)
            {
                batch.StartBatch();
                batch.DrawBatch();
                batch.EndBatch();
            }

            this.drawBatchList.Clear();
        }

        /// <summary>EXPERIMENTAL - renders the megaParticle pass</summary>
        public void DrawMegaParticles()
        {
            device.SetRenderTarget(megaPtlTarget);
            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, Color.Transparent, 1.0f, 0);

            //draw our invisible objects back into depth
            foreach (MaterialDrawBatch batch in depthBatchList)
            {
                batch.StartDepthBatch();
                batch.DrawDepthBatch();
                batch.EndBatch();
            }
            depthBatchList.Clear();

            //draw our mega particles
            device.RasterizerState = RasterizerState.CullNone;
            foreach (MaterialDrawBatch batch in megaParticleBatchList)
            {
                batch.StartBatch();
                batch.DrawBatch();
                batch.EndBatch();
            }
            megaParticleBatchList.Clear();
            device.RasterizerState = RasterizerState.CullCounterClockwise;

            device.SetRenderTarget(blurTarget1);
            setBlurEffectParameters(1.0f / (float)blurTarget1.Width, 0);
            hdrProcess.CurrentTechnique = hdrProcess.Techniques["GaussianBlur"];
            hdrProcess.Parameters["Texture"].SetValue(megaPtlTarget);
            hdrProcess.CurrentTechnique.Passes[0].Apply();
            screenAlignedQuad.Draw();

            device.SetRenderTarget(megaPtlTarget);
            setBlurEffectParameters(0, 1.0f / (float)blurTarget1.Height);
            hdrProcess.Parameters["Texture"].SetValue(blurTarget1);
            hdrProcess.CurrentTechnique.Passes[0].Apply();
            screenAlignedQuad.Draw();

            device.SetRenderTarget(blurTarget1);
            megaProcess.CurrentTechnique = megaProcess.Techniques["FractalNoise"];
            megaProcess.Parameters["Texture"].SetValue(TextureManager.getInstance().GetTexture("noisemaps//fractalNoise"));
            megaProcess.Parameters["SceneTexture"].SetValue(megaPtlTarget);
            megaProcess.CurrentTechnique.Passes[0].Apply();
            screenAlignedQuad.Draw();

            device.SetRenderTarget(megaPtlTarget);
            megaProcess.CurrentTechnique = megaProcess.Techniques["MegaMerge"];
            megaProcess.Parameters["SceneTexture"].SetValue(sceneTarget);
            megaProcess.Parameters["Texture"].SetValue(blurTarget1);
            megaProcess.CurrentTechnique.Passes[0].Apply();
            screenAlignedQuad.Draw();
        }

        /// <summary>Draw Diffuse Texture to the backbuffer.</summary>
        public void ProcessHDR()
        {
            device.SetRenderTarget(sceneTarget);
            hdrProcess.CurrentTechnique = hdrProcess.Techniques["BloomExtract"];
            hdrProcess.Parameters["BloomThreshold"].SetValue(bloomSettings.BloomThreshold);
            hdrProcess.Parameters["Texture"].SetValue(megaPtlTarget);
            hdrProcess.CurrentTechnique.Passes[0].Apply();
            screenAlignedQuad.Draw();

            device.SetRenderTarget(blurTarget1);
            setBlurEffectParameters(1.0f / (float)blurTarget1.Width, 0);
            hdrProcess.CurrentTechnique = hdrProcess.Techniques["GaussianBlur"];
            hdrProcess.Parameters["Texture"].SetValue(sceneTarget);
            hdrProcess.CurrentTechnique.Passes[0].Apply();
            screenAlignedQuad.Draw();

            device.SetRenderTarget(sceneTarget);
            setBlurEffectParameters(0, 1.0f / (float)blurTarget1.Height);
            hdrProcess.Parameters["Texture"].SetValue(blurTarget1);
            hdrProcess.CurrentTechnique.Passes[0].Apply();
            screenAlignedQuad.Draw();

            device.SetRenderTarget(null); //!!! DONE - GOING TO BACKBUFFER !!!
            hdrProcess.Parameters["BloomIntensity"].SetValue(bloomSettings.BloomIntensity);
            hdrProcess.Parameters["BaseIntensity"].SetValue(bloomSettings.BaseIntensity);
            hdrProcess.Parameters["BloomSaturation"].SetValue(bloomSettings.BloomSaturation);
            hdrProcess.Parameters["BaseSaturation"].SetValue(bloomSettings.BaseSaturation);
            hdrProcess.Parameters["BloomTexture"].SetValue(sceneTarget);
            hdrProcess.Parameters["Texture"].SetValue(megaPtlTarget);

            hdrProcess.CurrentTechnique = hdrProcess.Techniques["BloomCombine"];
            hdrProcess.CurrentTechnique.Passes[0].Apply();
            screenAlignedQuad.Draw();
        }

        /// <summary>begin's the spriteBatch</summary>
        public void Begin2D(SpriteBatch sBatch)
        {
            sBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend);
        }

        /// <summary>Sets the device's render states correctly to draw 3D</summary>
        public void Begin3D()
        {
            device.BlendState = BlendState.Opaque;
            device.DepthStencilState = DepthStencilState.Default;
            device.SetRenderTarget(sceneTarget);
            device.Clear(ClearOptions.Target | ClearOptions.DepthBuffer, ClearColor, 1.0f, 0);
        }

        /// <summary>Resets the device's render target and ends the spriteBatch</summary>
        /// <param name="sBatch">the spritebatch to end()</param>
        public void End2D(SpriteBatch sBatch)
        {
            sBatch.End();
        }

        /// <summary>updates the renderer's time</summary>
        /// <param name="time">time is supplied for any special timing in this class</param>
        public void Update(GameTime time)
        {
            currentGameTime = time;
        }

        /// <summary>Get the light array which is set to the shader for multi-lighting</summary>
        /// <param name="position">what position to test</param>
        /// <returns>array of Vector4 values representing: (color, direction, attenuation)x4</returns>
        public Vector4[] GetLightArray(Vector3 position)
        {
            Vector4[] output = new Vector4[8];
            
            List<PointLight> lights = this.getClosest4Lights(position);
            if (lights == null)
                return null;
            

            float dist;
            float intensity;
            int i = 0;
            foreach (PointLight light in lights)
            {
                if (light == null)
                    continue;

                if(light != null)
                    output[i++] = light.color;

                dist = 0f;
                intensity = 1f;
                if (light != null)
                {
                    dist = Vector3.Distance(position, light.WorldMatrix.Position);
                    intensity = light.intensity;
                }
                output[i++] = new Vector4(position - light.WorldMatrix.Position, MathHelper.Clamp(1 - (dist / light.falloff),0, 1) * intensity);
            }
            return output;
        }

        #endregion Run-Time

        #region Privates

        /// <summary>Computes sample weightings and texture coordinate offsets
        /// for one pass of a separable gaussian blur filter.</summary>
        private void setBlurEffectParameters(float dx, float dy)
        {
            EffectParameter weightsParameter, offsetsParameter;

            weightsParameter = hdrProcess.Parameters["SampleWeights"];
            offsetsParameter = hdrProcess.Parameters["SampleOffsets"];

            int sampleCount = weightsParameter.Elements.Count;

            float[] sampleWeights = new float[sampleCount];
            Vector2[] sampleOffsets = new Vector2[sampleCount];

            sampleWeights[0] = ComputeGaussian(0);
            sampleOffsets[0] = new Vector2(0);

            float totalWeights = sampleWeights[0];
            for (int i = 0; i < sampleCount / 2; i++)
            {
                float weight = ComputeGaussian(i + 1);

                sampleWeights[i * 2 + 1] = weight;
                sampleWeights[i * 2 + 2] = weight;

                totalWeights += weight * 2;

                float sampleOffset = i * 2 + 1.5f;

                Vector2 delta = new Vector2(dx, dy) * sampleOffset;

                sampleOffsets[i * 2 + 1] = delta;
                sampleOffsets[i * 2 + 2] = -delta;
            }

            for (int i = 0; i < sampleWeights.Length; i++)
            {
                sampleWeights[i] /= totalWeights;
            }

            weightsParameter.SetValue(sampleWeights);
            offsetsParameter.SetValue(sampleOffsets);
        }

        /// <summary>Evaluates a single point on the gaussian falloff curve.
        /// Used for setting up the blur filter weightings. </summary>
        private float ComputeGaussian(float n)
        {
            float theta = bloomSettings.BlurAmount;

            return (float)((1.0 / Math.Sqrt(2 * Math.PI * theta)) * Math.Exp(-(n * n) / (2 * theta * theta)));
        }

        /// <summary>Searches through the registered environment light list, for the closest 4, from the given position
        /// NOTE: this may return null lights, the List capacity will always be 4!!</summary>
        /// <param name="position">position to find the closest 4 lights from</param>
        /// <returns>4 closest point lights</returns>
        private List<PointLight> getClosest4Lights(Vector3 position)
        {
            if (environmentLightList == null)
                return null;

            List<PointLight> output = new List<PointLight>(4);
            output.Add(null); output.Add(null); output.Add(null); output.Add(null);

            for(int i = 0; i < this.environmentLightList.Count; i++)
            {
                replaceFurthest(ref output, environmentLightList[i], position);
            }
            return output;
        }

        /// <summary>replaces the farthest light with the input light, will replace any null values if found</summary>
        /// <param name="output">the original list of lights</param>
        /// <param name="light">light to be tested with</param>
        /// <param name="position">position of the new light</param>
        private void replaceFurthest(ref List<PointLight> output, PointLight light, Vector3 position)
        {
            int farthestIndex = -1;

            for (int i = 0; i < output.Count; i++)
            {
                if (output[i] == null)
                {
                    farthestIndex = i;
                    break;
                }
                if (i < output.Count - 1)
                {
                    if (output[i + 1] == null)
                    {
                        continue;
                    }
                }
                if (Vector3.Distance(position, output[i].WorldMatrix.Position) < Vector3.Distance(position, light.WorldMatrix.Position))
                {
                    continue;
                }
                else
                {
                    farthestIndex = i;
                }
            }

            if (farthestIndex != -1)
                output[farthestIndex] = light;
        }


        #endregion Privates

        #region Mutators

        /// <summary>the registered light list renderer is currently using</summary>
        /// <remarks>may return null</remarks>
        public List<PointLight> RegisteredLightList
        {
            get { return environmentLightList; }
        }

        /// <summary>pointer to the scene render target</summary>
        public Texture2D SceneTarget
        {
            get { return this.sceneTarget; }
        }
        /// <summary>pointer to the mega particle render target</summary>
        public Texture2D MegaParticleTarget
        {
            get { return this.megaPtlTarget; }
        }
        /// <summary>a default always loaded spritefont</summary>
        public SpriteFont GameFont
        {
            get { return font; }
        }
        /// <summary>effect manager singleton object</summary>
        public EffectManager EffectManager
        {
            get { return effectManager; }
        }
        /// <summary>camera this renderer is drawing with</summary>
        public Camera.Camera Camera
        {
            set { camera = value; }
            get { return camera; }
        }
        /// <summary>hardware device object</summary>
        public GraphicsDevice Device
        {
            get { return this.device; }
        }
        /// <summary>current frame timing values</summary>
        public GameTime CurrentTiming
        {
            get { return this.currentGameTime; }
        }
        /// <summary>blood settings</summary>
        public BloomSettings BloomSettings
        {
            get { return this.bloomSettings; }
            set { this.bloomSettings = value; }
        }

        #endregion

    }
}
