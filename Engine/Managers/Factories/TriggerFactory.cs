using Engine.Game_Objects;
using Microsoft.Xna.Framework.Content;

namespace Engine.Managers.Factories
{
    /// <summary>this Factory takes care of the creation of triggers coming from XML,
    /// using pointers to all of our main managers, callbacks and assets are assigned and loaded</summary>
    /// <author>Daniel Cuccia</author>
    public sealed class TriggerFactory
    {
        static TriggerFactory myInstance;
        /// <summary>singleton instance getter</summary>
        /// <returns>singleton instance</returns>
        public static TriggerFactory getInstance()
        {
            if (myInstance == null)
                myInstance = new TriggerFactory();
            return myInstance;
        }

        ContentManager  content;//ref
        SceneManager    sceneManager;//ref

        /// <summary>Default CTOR - must use Initialize</summary>
        private TriggerFactory() { }

        /// <summary>initialize all reference parameters</summary>
        /// <param name="content">xna content manager</param>
        /// <param name="sceneManager">reference to the scene manager</param>
        public void Initialize(ContentManager content, SceneManager sceneManager)
        {
            this.content = content;
            this.sceneManager = sceneManager;
        }

        /// <summary>Get the complete trigger translated from xml</summary>
        /// <param name="input">xml structure of a saved trigger</param>
        /// <returns>fully constructed and initialized trigger from xml</returns>
        public Trigger GetTrigger(TriggerXML input)
        {
            Trigger output = new Trigger();
            output.CreateFromXML(content, input);
            output.GenerateBoundingBox();
            output.UpdateBoundingBox();
            sceneManager.GetCurrentScene().AssignTrigger(ref output);
            return output;
        }
    }
}
