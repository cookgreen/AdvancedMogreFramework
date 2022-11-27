using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using org.ogre.ai;

namespace org.ogre.framework.app.AIStates
{
    public class SinbadPatrolAIState : State<SinbadCharacter>
    {
        private static SinbadPatrolAIState instance;
        public static SinbadPatrolAIState Instance
        {
            get { 
                if (instance == null) 
                { instance = new SinbadPatrolAIState(); } 
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
