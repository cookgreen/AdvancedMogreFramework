using org.ogre.ai;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.ogre.framework.app.AIStates
{
    public class SinbadRunwayAIState : State<SinbadCharacter>
    {
        private static SinbadRunwayAIState instance;
        public static SinbadRunwayAIState Instance
        {
            get
            {
                if (instance == null)
                { instance = new SinbadRunwayAIState(); }
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
