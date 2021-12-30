#region Using

using Engine.Managers.Camera;
using Engine.Managers.Factories;
using Engine.Math_Physics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Content;
using Microsoft.Xna.Framework.Input;
using Requiem.Spells;
using Requiem.Spells.Base;
using StillDesign.PhysX;
using Requiem.Movement;
using System.Collections.Generic;
using Requiem.Entities.EntitySupport;
using Engine.Drawing_Objects;
using Engine.Drawing_Objects.Materials;
using Engine.Managers;

#endregion Using

namespace Requiem.Entities
{
    /// <summary>The main Player Class</summary>
    /// <author>Gabrial Dubois</author>
    public class Player : Livable
    {
        #region Member Variables

        bool isFirstPerson = false;
        bool lockInput = false;
        Vector3 lastCameraPos;
        Vector3 lastCameraTar;
        Camera camera;
        Timer buttonTimer;
       
        Spell thirdPersonSpell1;
        Spell thirdPersonSpell2;

        Spell firstPersonSpell1;
        Spell firstPersonSpell2;

        Sprite leftSpellIcon;
        Sprite rightSpellIcon;

        int index1 = 0;
        int index2 = 0;

        PointLight light;

        public float zLock { set; get; }

        #endregion Member Variables

        #region Initialization

        /// <summary>Default CTOR </summary>
        public Player()
            : base()
        {
            zLock = 0f;
        }

        /// <summary>Init player</summary>
        public override void Initialize(ContentManager content)
        {
            worldMatrix = new WorldMatrix();
            worldMatrix.Position = new Vector3(0, 0, 0);
            model.Initialize(content, "models//Player");
            model.BeginAnimation("idle");
            model.GenerateBoundingBox();
            model.WorldMatrix.UniformScale = 20f;
            model.AnimationController.Speed = 1f;
            model.WorldMatrix.rY += 90f;

            model.Material = new DiffuseAnimatedMaterial(model,
                TextureManager.getInstance().GetTexture("textures//player_diffuse"));
            model.Material.Ambient = 0.4f;

            Actor a = PhysXPrimitive.GetPlayerActor(new Vector3(15, 25, 15), worldMatrix.Position);
            this.Actor = a;
            WorldMatrix w = this.WorldMatrix;
            movement = new MovementXAxis(ref w, ref a);

            buttonTimer = new Timer(SpellManager.GetInstance().SceneManager.Game, 300);
            buttonTimer.StartTimer();
            SpellManager.GetInstance().SceneManager.Game.Components.Add(buttonTimer);

            health = new Health(200);

            thirdPersonSpell1 = new EnergyBoltSpell(this);
            thirdPersonSpell1.Initialize();

            thirdPersonSpell2 = thirdPersonSpell1;

            spells = new List<Spell>();
            spells.Add(thirdPersonSpell1);

            Spell s = new FireballSpell(this);
            s.Initialize();
            spells.Add(s);

            thirdPersonSpell2 = s;

            s = new RegenerateSpell(this);
            s.Initialize();
            spells.Add(s);

            s = new StrikeSpell(this);
            s.Initialize();
            spells.Add(s);

            s = new WebTrapSpell(this);
            s.Initialize();
            spells.Add(s);

            s = new VampireSpell(this);
            s.Initialize();
            spells.Add(s);

            firstPersonSpell1 = new EnergyBoltSpell(this);
            firstPersonSpell1.Initialize();
            firstPersonSpell2 = new FireballSpell(this);
            firstPersonSpell2.Initialize();

            leftSpellIcon = thirdPersonSpell1.Icon;
            rightSpellIcon = thirdPersonSpell2.Icon;

            light = new PointLight();
            light.Initialize(content);
            light.falloff = 150f;
            light.intensity = 0.8f;
            light.showBulb = false;
            if (Renderer.getInstance().RegisteredLightList != null)
            {
                Renderer.getInstance().RegisteredLightList.Add(light);
            }
        }

        public override void GenerateBoundingBox()
        {
            base.model.OBB = new OrientedBoundingBox(
                new Vector3(-10f, 0f, -10f),
                new Vector3(10f, 55f, 10f));
        }

        public override void UpdateBoundingBox()
        {
            base.model.OBB.Update(Matrix.CreateScale(1f) * Matrix.CreateTranslation(WorldMatrix.Position));
        }

        #endregion Initialization

        #region API

        #region Update

        /// <summary>Main Player-logic Update</summary>
        public override void Update(ref Camera camera, GameTime time) 
        {
            Movement.Update(time);

            if (health.CurrentHealth < 10)
            {
                health.CurrentHealth = 10;
            }
            if (Actor.IsDisposed == false)
            {
                StillDesign.PhysX.MathPrimitives.Vector3 p = this.Actor.GlobalPosition;
                p.Z = base.model.WorldMatrix.Z = zLock;
                this.Actor.GlobalPosition = p;
            }
            base.model.Update(ref camera, time);

            light.WorldMatrix.X = WorldMatrix.X;
            light.WorldMatrix.Y = WorldMatrix.Y + 80f;
            light.WorldMatrix.Z = WorldMatrix.Z + 20f;
        }

        #endregion Udate

        #region Input

        /// <summary>User to Player input logic</summary>
        public void Input(KeyboardState kb, MouseState ms, GamePadState gs)
        {
            if (isFirstPerson && !lockInput)
            {
                FirstPersonInput(kb, ms, gs);
            }
            else if (!isFirstPerson && !lockInput)
            {
                ThirdPersonInput(kb, ms, gs);
            }
        }

        public void ChangeAnim()
        {
        }

        #region Thrid Person

        private void ThirdPersonInput(KeyboardState kb, MouseState ms, GamePadState gp)
        {
            movement.Input(kb, ms, gp);

            rotatePlayer(kb, ms, gp);
            animatePlayer();
            
            ThirdPersonKeyboardInput(kb);
            ThirdPersonMouseInput(ms);
            ThirdPersonGamepadInput(gp);            
        }

        private void rotatePlayer(KeyboardState kb, MouseState ms, GamePadState gp)
        {
            if (kb.IsKeyDown(Keys.A)) 
                model.WorldMatrix.rY = 270f;
            else if (kb.IsKeyDown(Keys.D)) 
                model.WorldMatrix.rY = 90f;

            if (gp.ThumbSticks.Left.X < -0.15f) 
                model.WorldMatrix.rY = 270f;
            else if (gp.ThumbSticks.Left.X > 0.15f) 
                model.WorldMatrix.rY = 90f;
        }

        private void animatePlayer()
        {
            StillDesign.PhysX.MathPrimitives.Vector3 s = Actor.LinearVelocity;
            if (s.X < 0.3f && s.X > -0.3f)
            {
                if(model.CurrentAnimation != "idle" && model.CurrentAnimation != "attack")
                    model.BeginAnimation("idle");
            }
            else if (s.X > 0.3f || s.X < -0.3f)
            {
                if (model.CurrentAnimation != "walk" && model.CurrentAnimation != "attack")
                    model.BeginAnimation("walk");
            }
        }

        private void ThirdPersonKeyboardInput(KeyboardState kb)
        {
            if (kb.IsKeyDown(Keys.A))
            {
                lookAt.X = -1;
            }
            else if (kb.IsKeyDown(Keys.D))
            {
                lookAt.X = 1;
            }

            if (kb.IsKeyDown(Keys.Q) && buttonTimer.TimesUp())
            {
                index1++;
                if (index1 == spells.Count)
                {
                    index1 = 0;
                }

                if (spells[index1] == thirdPersonSpell2)
                {
                    index1++;

                    if (index1 == spells.Count)
                    {
                        index1 = 0;
                    }
                }

                thirdPersonSpell1 = spells[index1];
                leftSpellIcon = thirdPersonSpell1.Icon;
            }
            else if (kb.IsKeyDown(Keys.E) && buttonTimer.TimesUp())
            {
                index2++;
                if (index2 == spells.Count)
                {
                    index2 = 0;
                }

                if (spells[index2] == thirdPersonSpell1)
                {
                    index2++;

                    if (index2 == spells.Count)
                    {
                        index2 = 0;
                    }
                }

                thirdPersonSpell2 = spells[index2];
                rightSpellIcon = thirdPersonSpell2.Icon;
            }
        }

        private void ThirdPersonMouseInput(MouseState ms)
        {
            if (ms.MiddleButton == ButtonState.Pressed)
            {
                TogglePOV();
            }

            if (ms.LeftButton == ButtonState.Pressed)
            {
                thirdPersonSpell1.Cast();
                beginAttackAnim();
            }

            if (ms.RightButton == ButtonState.Pressed)
            {
                thirdPersonSpell2.Cast();
                beginAttackAnim();
            }
        }

        private void ThirdPersonGamepadInput(GamePadState gs)
        {
            if (gs.IsButtonDown(Buttons.DPadLeft))
            {
                lookAt.X = -1;
            }
            else if (gs.IsButtonDown(Buttons.DPadRight))
            {
                lookAt.X = 1;
            }

            if (gs.ThumbSticks.Left.X < -0.5f)
            {
                lookAt.X = -1;
            }
            else if (gs.ThumbSticks.Left.X > 0.5f)
            {
                lookAt.X = 1;
            }

            if (gs.IsButtonDown(Buttons.LeftShoulder) && buttonTimer.TimesUp())
            {
                index1++;
                if (index1 == spells.Count)
                {
                    index1 = 0;
                }

                if (spells[index1] == thirdPersonSpell2)
                {
                    index1++;

                    if (index1 == spells.Count)
                    {
                        index1 = 0;
                    }
                }

                thirdPersonSpell1 = spells[index1];
                leftSpellIcon = thirdPersonSpell1.Icon;
            }
            else if (gs.IsButtonDown(Buttons.RightShoulder) && buttonTimer.TimesUp())
            {
                index2++;
                if (index2 == spells.Count)
                {
                    index2 = 0;
                }

                if (spells[index2] == thirdPersonSpell1)
                {
                    index2++;

                    if (index2 == spells.Count)
                    {
                        index2 = 0;
                    }
                }

                thirdPersonSpell2 = spells[index2];
                rightSpellIcon = thirdPersonSpell2.Icon;
            }

            if (gs.IsButtonDown(Buttons.B))
            {
                TogglePOV();
            }

            if (gs.IsButtonDown(Buttons.LeftTrigger))
            {
                thirdPersonSpell1.Cast();
                beginAttackAnim();
            }

            if (gs.IsButtonDown(Buttons.RightTrigger))
            {
                thirdPersonSpell2.Cast();
                beginAttackAnim();
            }
        }

        #endregion Third Person

        #region First Person

        private void FirstPersonInput(KeyboardState kb, MouseState ms, GamePadState gs)
        {
            FirstPersonMouseInput(ms);
            FirstPersonGamepadInput(gs);
        }

        private void FirstPersonMouseInput(MouseState ms)
        {
            if (ms.MiddleButton == ButtonState.Pressed)
            {
                TogglePOV();
            }

            if (ms.LeftButton == ButtonState.Pressed)
            {
                lookAt = camera.RotationMatrix.Forward;
                firstPersonSpell1.Cast();
            }

            if (ms.RightButton == ButtonState.Pressed)
            {
                lookAt = camera.RotationMatrix.Forward;
                firstPersonSpell2.Cast();
            }
        }

        private void FirstPersonGamepadInput(GamePadState gs)
        {
            if (gs.IsButtonDown(Buttons.B))
            {
                TogglePOV();
            }

            if (gs.IsButtonDown(Buttons.LeftTrigger))
            {
                lookAt = camera.RotationMatrix.Forward;
                firstPersonSpell1.Cast();
            }

            if (gs.IsButtonDown(Buttons.RightTrigger))
            {
                lookAt = camera.RotationMatrix.Forward;
                firstPersonSpell2.Cast();
            }
        }

        #endregion First Person

        #endregion Input

        #region TogglePOV

        public void TogglePOV()
        {
            if (isFirstPerson)
            {
                camera.SmoothStepTo(
                    new Vector3(WorldMatrix.Position.X, WorldMatrix.Position.Y + 50f, WorldMatrix.Position.Z),
                    lastCameraPos, ToggleThird, 0.30f);


                isFirstPerson = false;
                lockInput = true;
            }
            else
            {
                lastCameraTar = camera.LookAtTarget;
                lastCameraPos = camera.Position;

                camera.SmoothStepTo(
                    new Vector3(WorldMatrix.Position.X, WorldMatrix.Position.Y + 40f, WorldMatrix.Position.Z - 5f),
                    new Vector3(WorldMatrix.Position.X, WorldMatrix.Position.Y + 40f, WorldMatrix.Position.Z),
                    ToggleFirst, 0.30f);


                isFirstPerson = true;
                lockInput = true;
            }
        }

        private void ToggleFirst()
        {
            WorldMatrix w = WorldMatrix;
            camera.Behavior = new FirstPersonCamera(ref camera, ref w);
            lockInput = false;

            leftSpellIcon = firstPersonSpell1.Icon;
            rightSpellIcon = firstPersonSpell2.Icon;
        }

        private void ToggleThird()
        {
            WorldMatrix w = WorldMatrix;
            camera.Behavior = new AxisLockCamera(ref camera, ref w, new Vector3(0, 0, 1), new Vector3(0, 200, 0), 500f);
            lockInput = false;
            lookAt.X = 1;
            lookAt.Y = 0;
            lookAt.Z = 0;

            leftSpellIcon = thirdPersonSpell1.Icon;
            rightSpellIcon = thirdPersonSpell2.Icon;
        }

        #endregion TogglePOV

        private void beginAttackAnim()
        {
            if (model.CurrentAnimation != "attack")
            {
                model.AnimationSpeed = 5f;
                model.BeginAnimation("attack", false, animBackToDefault);
            }
        }

        private void animBackToDefault()
        {
            model.CurrentAnimation = "";
            model.AnimationSpeed = 1f;
        }

        /// <summary>render model</summary>
        public override void Render()
        {
            if(this.isFirstPerson == false)
                model.Render();
        }

        /// <summary>render debugging information</summary>
        public override void RenderDebug()
        {
            model.RenderDebug();
        }

        #endregion API

        #region Mutators

        public Camera PlayerCamera
        {
            get { return camera; }
            set { camera = value; }
        }

        /// <summary>
        /// get the current left spell
        /// </summary>
        public Spell LeftSpell
        {
            get 
            {
                if (isFirstPerson)
                {
                    return firstPersonSpell1;
                }
                else
                {
                    return thirdPersonSpell1;
                }
            }
        }

        /// <summary>
        /// get the current right spell
        /// </summary>
        public Spell RightSpell
        {
            get
            {
                if (isFirstPerson)
                {
                    return firstPersonSpell2;
                }
                else
                {
                    return thirdPersonSpell2;
                }
            }
        }

        /// <summary>
        /// get the left spell icon
        /// </summary>
        public Sprite LeftSpellIcon
        {
            get { return leftSpellIcon; }
        }

        /// <summary>
        /// get the right spell icon
        /// </summary>
        public Sprite RightSpellIcon
        {
            get { return rightSpellIcon; }
        }

        public override Engine.Drawing_Objects.Materials.Material Material
        {
            get { return model.Material; }
            set { model.Material = value; }
        }
        #endregion Mutators

    }
}
