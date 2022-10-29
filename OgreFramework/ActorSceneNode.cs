using Mogre;
using Mogre.PhysX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace org.ogre.framework
{
    public class ActorSceneNode
    {
        private Actor actor;
        private SceneNode sceneNode;

        public Actor Actor { get { return actor; } }
        public SceneNode SceneNode { get { return sceneNode; } }

        public ActorSceneNode(Actor actor, SceneNode sceneNode)
        {
            this.actor = actor;
            this.sceneNode = sceneNode;
        }

        public void Update(float deltaTime)
        {
            if (!actor.IsSleeping)
            {
                this.sceneNode.Position = actor.GlobalPosition;
                this.sceneNode.Orientation = actor.GlobalOrientationQuaternion;
            }
        }
    }
}
