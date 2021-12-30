using Engine;
using Microsoft.Xna.Framework.Content;
using Requiem.Entities.Enemy.AI;
using Requiem.Scenes;
using Engine.Math_Physics;
using Engine.Drawing_Objects.Materials;
using Engine.Managers;

namespace Requiem.Entities.Enemy
{
    /// <summary>Static API to create different enemies</summary>
    /// <author>Daniel Cuccia</author>
    public abstract class EnemyFactory
    {
        /// <summary>Un-Creatable</summary>
        private EnemyFactory() { }

        /// <summary>this will eventually wrap all of the enemyFactory logic</summary>
        /// <param name="content">xna's content manger</param>
        /// <param name="level">pointer to the active level</param>
        /// <param name="modelFilepath">filepath of the requested model</param>
        /// <param name="aiType">type of AI this enemy is getting</param>
        /// <returns>fully ready to go enemy</returns>
        public static Enemy GetEnemy(ContentManager content, LevelScene level, string modelFilepath, EnemyEnums.EnemySpawnLocationType aiType)
        {
            return null;
        }

        public static Enemy GetDistanceShooter(LevelScene level, ContentManager content, string modelFilepath)
        {
            Enemy output = new StandardEnemy(modelFilepath);
            output.Initialize(content);

            Spells.FireballSpell s = new Spells.FireballSpell(output);
            s.Initialize();
            output.SpellList.Add(s);

            FiniteStateMachine ai = new FiniteStateMachine(output, level);
            ai.AddState("wait", new WaitBehavior(ai, MyMath.GetRandomFloat(3500f, 7500f)), true);
            ai.AddState("shoot", new ShootBehavior(ai));
            output.Ai = ai;
            
            return output;
        }

        public static Enemy GetLocalStandardEnemy(LevelScene level, ContentManager content, string modelFilepath)
        {
            Enemy output = new StandardEnemy(modelFilepath);
            output.Initialize(content);
            
            Spells.EnergyBoltSpell s = new Spells.EnergyBoltSpell(output);
            s.Initialize();
            output.SpellList.Add(s);

            FiniteStateMachine ai = new FiniteStateMachine(output, level);
            ai.AddState("wait", new WaitBehavior(ai, 3000), true);
            ai.AddState("patrol", new PatrolBehavior(ai));
            ai.AddState("pursue", new PursueBehavior(ai));
            ai.AddState("flee", new FleeBehavior(ai));
            ai.AddState("melee", new MeleeBehavior(ai));
            output.Ai = ai;

            return output;
        }

        public static Enemy GetBossEnemy(LevelScene level, ContentManager content, string modelFilepath)
        {
            Enemy output = new BossEnemy(modelFilepath);
            output.WorldMatrix.UniformScale = 200f;

            Spells.WebTrapSpell s = new Spells.WebTrapSpell(output);
            s.Initialize();
            output.SpellList.Add(s);

            output.Material = new NormalMappedAnimatedMaterial(output,
                TextureManager.getInstance().GetTexture("models//bossRoom//texture//brood_diffuse"),
                TextureManager.getInstance().GetTexture("models//bossRoom//texture//brood_normal"));

            FiniteStateMachine ai = new FiniteStateMachine(output, level);
            ai.AddState("wait", new BossWait(ai, 2000f), true);
            ai.AddState("attack", new BossAttack(ai));
            output.Ai = ai;

            return output;
        }
    }
}
