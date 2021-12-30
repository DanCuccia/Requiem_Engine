using System;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Engine.Math_Physics
{
    /// <summary>This is the class which gets Serialized, obtain through WorldMatrix.GetForXml()</summary>
    [Serializable]
    public class WorldMatrixXml
    {
        /// <summary>world position</summary>
        public Vector3 position;
        /// <summary>world rotation</summary>
        public Vector3 rotation;
        /// <summary>world scale</summary>
        public Vector3 scale;
    }

    /// <summary>This is the main world-space component to any 3D drawing object,
    /// this contains position, rotation, and scaling to Matrix functioanlity,
    /// as well as features like saving and reverting complete states.</summary>
    /// <author>Daniel Cuccia</author>
    public class WorldMatrix
    {
        #region Member Variables

        private bool        isDirty         = true;
        private Vector3     position        = Vector3.Zero;
        private Vector3     rotation        = Vector3.Zero;
        private Vector3     scale           = Vector3.One;
        private Matrix      worldMatrix;
        private WorldMatrix lastState       = null;

        /// <summary>delegate called when the matrix updates</summary>
        public delegate void OnChange();
        private OnChange     onChange;

        /// <summary>delegate called to override the update matrix computation</summary>
        /// <param name="world">reference to this world (to avoid any confusion)</param>
        /// <returns>custom computed world matrix</returns>
        public delegate Matrix UpdateOverride(ref WorldMatrix world);
        private UpdateOverride updateOverride;
        
        #endregion Member Variables

        #region Construction

        /// <summary>Default CTOR, all values initialized to 0</summary>
        public WorldMatrix()
        {   }

        /// <summary>Set any Variables CTOR, any parameters may be null for 0 initialization</summary>
        /// <param name="position">position component</param>
        /// <param name="rotation">rotation component</param>
        /// <param name="scale">scale component</param>
        /// <param name="world">world matrix component</param>
        public WorldMatrix(Vector3 position, Vector3 rotation, Vector3 scale, Matrix world)
        {
            if(position != null)
                this.position = position;
            if(rotation != null)
                this.rotation = rotation;
            if(scale != null)
                this.scale = scale;
            if (world != null)
            {
                this.worldMatrix = world;
                this.isDirty = false;
            }
            isDirty = true;
        }

        /// <summary>Copy all data to a new WorldMatrix object, no components will point back to original</summary>
        /// <returns>A newly allocated Clone of this WorldMatrix</returns>
        public WorldMatrix Clone()
        {
            return new WorldMatrix(this.position, this.rotation, this.scale, this.worldMatrix);
        }
        
        #endregion Construction

        #region API


        /// <summary>Get the Fully Computed Matrix of this object</summary>
        /// <returns>World Matrix component of this object</returns>
        public Matrix GetWorldMatrix()
        {
            if (worldMatrix == null ||
                isDirty == true)
            {
                updateWorldMatrix();
            }
            return this.worldMatrix;
        }

        /// <summary>privately called to do the complete matrix calculation, when this object is dirty</summary>
        private void updateWorldMatrix()
        {
            if (updateOverride != null)
            {
                WorldMatrix w = this;
                worldMatrix = updateOverride(ref w);
            }
            else
            {
                worldMatrix = Matrix.CreateScale(scale) *
                    Matrix.CreateRotationX(MathHelper.ToRadians(rotation.X)) *
                    Matrix.CreateRotationY(MathHelper.ToRadians(rotation.Y)) *
                    Matrix.CreateRotationZ(MathHelper.ToRadians(rotation.Z)) *
                    Matrix.CreateTranslation(position);
            }
            isDirty = false;
        }

        /// <summary>public call of updateWorldMatrix()</summary>
        public void ForceUpdateMatrix()
        {
            this.updateWorldMatrix();
        }

        /// <summary>Save this object's state, so it may be restored to when this function was called</summary>
        public void SaveState()
        {
            if (isDirty == true)
            {
                updateWorldMatrix();
            }
            lastState = this.Clone();
        }

        /// <summary>Restore the state of this object to when SaveState() was called,
        /// the previous state we are reverting to will be nullified</summary>
        public void RestoreState()
        {
            if (this.lastState == null)
                throw new ArgumentNullException("WorldMatrix::RestoreLastState: var lastState is null, SaveState() must be called first");

            position = lastState.position;
            rotation = lastState.rotation;
            scale = lastState.scale;
            if (lastState.isDirty == false)
                worldMatrix = lastState.worldMatrix;

            lastState = null;
        }

        /// <summary>Translate the Position relative from where it is now</summary>
        /// <param name="translation">distance to move</param>
        public void Move(Vector3 translation)
        {
            position.X += translation.X;
            position.Y += translation.Y;
            position.Z += translation.Z;
            isDirty = true;
        }

        /// <summary>Rotate the world matrix relative from where it is now</summary>
        /// <param name="rotation">rotation value to add to current rotation values</param>
        public void Rotate(Vector3 rotation)
        {
            this.rotation.X += rotation.X;
            this.rotation.Y += rotation.Y;
            this.rotation.Z += rotation.Z;
            isDirty = true;
        }

        /// <summary>Scale the world matrix relative from how it is now</summary>
        /// <param name="scale">scaling values to add to current values</param>
        public void ScaleAdd(Vector3 scale)
        {
            this.scale.X += scale.X;
            this.scale.Y += scale.Y;
            this.scale.Z += scale.Z;
            isDirty = true;
        }

        /// <summary>Reset all values back to default values, position - rotation = 0, scale = 1</summary>
        public void Zero()
        {
            position.X = position.Y = position.Z = 0f;
            rotation.X = rotation.Y = rotation.Z = 0f;
            scale.X = scale.Y = scale.Z = 1f;
            isDirty = true;
        }

        /// <summary>decompose values using a pre-existing world matrix </summary>
        /// <param name="world">pre-computed world matrix</param>
        public void SetFromWorld(Matrix world)
        {
            Quaternion oquat;
            world.Decompose(out this.scale, out oquat, out this.position);
            rotation = MyMath.QuaternionToEuler(oquat);
            isDirty = true;
        }

        /// <summary>To serialize this worldmatrx, call this to get a slim form of the worldMatrix</summary>
        /// <returns>slim version that holds only position, rotation, and scale to be serialized</returns>
        public WorldMatrixXml GetXml()
        {
            WorldMatrixXml output = new WorldMatrixXml();
            output.position = position;
            output.rotation = rotation;
            output.scale = scale;
            return output;
        }

        /// <summary>Load this worldMatrix from a WorldMatrixXml deserialized object</summary>
        /// <param name="input">deserialized world matrix from xml</param>
        public void FromXML(WorldMatrixXml input)
        {
            this.position = input.position;
            this.rotation = input.rotation;
            this.scale = input.scale;
            this.isDirty = true;
        }

        #endregion API

        #region Setters / Getters

        /// <summary>Position Parameter, isDirty flag will be flipped</summary>
        public Vector3 Position
        {
            set
            {
                position = value;
                isDirty = true;
                if (this.onChange != null)
                    this.onChange();
            }
            get { return position; }
        }
        /// <summary>Position X Component, isDirty flag will be flipped</summary>
        public float X
        {
            set 
            { 
                position.X = value;
                isDirty = true;
                if (this.onChange != null)
                    this.onChange();
            }
            get { return position.X; }
        }
        /// <summary>Position Y Component, isDirty flag will be flipped</summary>
        public float Y
        {
            set
            {
                position.Y = value;
                isDirty = true;
                if (this.onChange != null)
                    this.onChange();
            }
            get { return position.Y; }
        }
        /// <summary>Position Z Component, isDirty flag will be flipped</summary>
        public float Z
        {
            set
            {
                position.Z = value;
                isDirty = true;
                if (this.onChange != null)
                    this.onChange();
            }
            get { return position.Z; }
        }

        /// <summary>Rotation Parameter, isDirty flag will be flipped</summary>
        public Vector3 Rotation
        {
            set
            {
                rotation = value;
                isDirty = true;
                if (this.onChange != null)
                    this.onChange();
            }
            get { return rotation; }
        }
        /// <summary>Rotation X Component, isDirty flag will be flipped</summary>
        public float rX
        {
            set
            {
                rotation.X = value;
                isDirty = true;
                if (this.onChange != null)
                    this.onChange();
            }
            get { return rotation.X; }
        }
        /// <summary>Rotation Y Component, isDirty flag will be flipped</summary>
        public float rY
        {
            set
            {
                rotation.Y = value;
                isDirty = true;
                if (this.onChange != null)
                    this.onChange();
            }
            get { return rotation.Y; }
        }
        /// <summary>Rotation Z Component, isDirty flag will be flipped</summary>
        public float rZ
        {
            set
            {
                rotation.Z = value;
                isDirty = true;
                if (this.onChange != null)
                    this.onChange();
            }
            get { return rotation.Z; }
        }

        /// <summary>Scaling Parameter, isDirty flag will be flipped</summary>
        public Vector3 Scale
        {
            set
            {
                scale = value;
                isDirty = true;
                if (this.onChange != null)
                    this.onChange();
            }
            get { return scale; }
        }
        /// <summary>scale all components to value, returns the largest component from XYZ</summary>
        public float UniformScale
        {
            set
            {
                scale.X = scale.Y = scale.Z = value; 
                if (this.onChange != null)
                    this.onChange();
            }
            get
            {
                if (scale.X > scale.Y)
                {
                    if (scale.X > scale.Z)
                        return scale.X;
                    else if (scale.Z > scale.Y)
                        return scale.Z;
                }
                else if (scale.Y > scale.Z)
                    return scale.Y;
                return scale.Z;
            }
        }
        /// <summary>Scale X Component, isDirty flag will be flipped</summary>
        public float sX
        {
            set
            {
                scale.X = value;
                isDirty = true;
                if (this.onChange != null)
                    this.onChange();
            }
            get
            { return scale.X; }
        }
        /// <summary>Scale Y Component, isDirty flag will be flipped</summary>
        public float sY
        {
            set
            {
                scale.Y = value;
                isDirty = true;
                if (this.onChange != null)
                    this.onChange();
            }
            get { return scale.Y; }
        }
        /// <summary>Scale Z Component, isDirty flag will be flipped</summary>
        public float sZ
        {
            set
            {
                scale.Z = value;
                isDirty = true;
                if (this.onChange != null)
                    this.onChange();
            }
            get { return scale.Z; }
        }

        /// <summary>Optional callback executed when the matrix was called to compute</summary>
        public OnChange OnChangeCallBack
        {
            set { this.onChange = value; }
        }

        /// <summary>Optional way to override how the matrix updates</summary>
        public UpdateOverride Override
        {
            set { this.updateOverride = value; }
        }

        #endregion Setters / Getters
    }
}
