using org.ogre.ai;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.ogre.framework.app.AIStates
{
    public class SinbadAttackAIState : State<SinbadCharacter>
    {
        private static SinbadAttackAIState instance;
        public static SinbadAttackAIState Instance
        {
            get
            {
                if (instance == null)
                { instance = new SinbadAttackAIState(); }
                return instance;
            }
        }

        public override void Enter(SinbadCharacter entityObj)
        {
            base.Enter(entityObj);
        }

        public override void Execute(SinbadCharacter entityObj)
        {
            base.Execute(entityObj);
        }

        public override void Exit(SinbadCharacter entityObj)
        {
            base.Exit(entityObj);
        }
    }
}
