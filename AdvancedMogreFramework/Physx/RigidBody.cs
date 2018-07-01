using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using Mogre.PhysX;

namespace AdvancedMogreFramework.Physx
{
    public class RigidBody
    {
        public string Name { get; set; }
        public Actor Actor { get; set; }
        public Scene Scene { get; set; }
        public bool IsDynamic { get; set; }
        public RigidBody()
        {
        }
        
        private void createDynamic(Matrix4 pose, RigidBodyDescription description, Scene scene, ShapeDesc shape)
        {
            BodyDesc bodyDesc = new BodyDesc();
            ActorDesc actorDesc = new ActorDesc();
            actorDesc.GlobalPose = pose;
            actorDesc.Body = bodyDesc;
            description.ToNxActor(ref actorDesc, ref bodyDesc);
            actorDesc.Shapes.Add(shape);
            Actor = Scene.CreateActor(actorDesc);
        }

        private void createStatic(Matrix4 pose, RigidBodyDescription description, Scene scene, ShapeDesc shape)
        {
            ActorDesc actorDesc = new ActorDesc();
            actorDesc.GlobalPose = pose;
            actorDesc.Body = null;
            description.ToNxActor(ref actorDesc);
            actorDesc.Shapes.Add(shape);
            Actor = Scene.CreateActor(actorDesc);
        }

        protected Controller _createCharacterController(Vector3 pose, Scene scene, SimpleShape shape, CharacterControllerDescription description)
        {
            Controller controller = null;
            CapsuleControllerDesc controller_desc = new CapsuleControllerDesc();
            controller_desc.ClimbingMode = description.CapsuleEasyClimbing ? CapsuleClimbingModes.Easy : CapsuleClimbingModes.Constrained;
            Vector3 capsule_desc = shape.to_cc_shape();
            controller_desc.Height = capsule_desc.y;
            controller_desc.Radius = capsule_desc.x;
            //controller_desc.Position = globalPos.as< NxExtendedVec3 > ();
            controller_desc.SkinWidth = description.SkinWidth;
            controller_desc.SlopeLimit = description.SlopeLimit;
            controller_desc.StepOffset = description.StepOffset;
            //controller_desc.s = (NxHeightFieldAxis)(int)description.mUpDirection;
            //controller_desc.Callback = World::getSingleton()->getPhysXCharacterHitReport();
            scene.Physics.ControllerManager.CreateController(scene, controller_desc);
            return controller;
        }
    }
}
