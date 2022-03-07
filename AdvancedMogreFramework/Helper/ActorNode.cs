﻿using Mogre;
using Mogre.PhysX;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace AdvancedMogreFramework.Helper
{
    public class ActorNode
    {
        private SceneNode sceneNode;
        private Actor actor;

        public SceneNode SceneNode
        {
            get { return sceneNode; }
        }
        public Actor Actor
        {
            get { return actor; }
        }

        public ActorNode(SceneNode sceneNode, Actor actor)
        {
            this.sceneNode = sceneNode;
            this.actor = actor;
        }

        internal void Update(float deltaTime)
        {
            if (actor != null)
            {
                if (!actor.IsSleeping)
                {
                    this.sceneNode.Position = actor.GlobalPosition;
                    this.sceneNode.Orientation = actor.GlobalOrientationQuaternion;
                }
            }
        }
    }
}
