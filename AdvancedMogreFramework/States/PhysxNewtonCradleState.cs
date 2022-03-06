using AdvancedMogreFramework.Helper;
using Mogre;
using Mogre.PhysX;
using Mogre_Procedural.MogreBites;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using MOIS;

namespace AdvancedMogreFramework.States
{
    public class PhysxNewtonCradleState : GameState
    {
        private Scene scene;
        private Physics physx;

        const float centery = 2;
        const float stringlength = 4.08f;
        const float radius = 1.0f;
        const float planesize = 4.5f;

        private List<ActorNode> actorNodes;

        public PhysxNewtonCradleState()
        {
            physx = Physics.Create();
            SceneDesc desc = new SceneDesc();
            scene = physx.CreateScene(desc);

            scene.Gravity = new Mogre.Vector3(0, -9.81f, 0);
            var defm = scene.Materials[0];
            defm.Restitution = 0.5f;
            defm.DynamicFriction = defm.StaticFriction = 0.6f;

            scene.CreateActor(new ActorDesc(new PlaneShapeDesc()));

            actorNodes = new List<ActorNode>();
        }

        public override void Enter()
        {
            //scene.Timing.MaxTimestep = 1.0f / (3 * 24);

            AdvancedMogreFramework.Instance.mTrayMgr.destroyAllWidgets();

            mSceneMgr = AdvancedMogreFramework.Instance.mRoot.CreateSceneManager(SceneType.ST_GENERIC, "PhysxNewtonCradleMgr");
            ColourValue cvAmbineLight = new ColourValue(0.7f, 0.7f, 0.7f);
            mSceneMgr.AmbientLight = cvAmbineLight;

            mCamera = mSceneMgr.CreateCamera("PhysxNewtonCradleCamera");
            mCamera.NearClipDistance = 5;
            mCamera.FarClipDistance = 999;
            mCamera.AspectRatio = AdvancedMogreFramework.Instance.mViewport.ActualWidth / AdvancedMogreFramework.Instance.mViewport.ActualHeight;

            AdvancedMogreFramework.Instance.mViewport.Camera = mCamera;

            var physicsMat = scene.CreateMaterial(new MaterialDesc(0.0f, 0.0f, 1.0f));
            
            var sphereShapeDesc = new SphereShapeDesc(radius);
            sphereShapeDesc.SkinWidth = 0.0f;
            sphereShapeDesc.MaterialIndex = physicsMat.Index;

            var sphereActorDesc = new ActorDesc(new BodyDesc(0.124f, 1.0f), 150.1f, sphereShapeDesc);

            for (int i = -2; i <= 2; i++)
            {
                sphereActorDesc.GlobalPosition = new Mogre.Vector3(2 * radius * i, centery, 0);
                var sphereActor = scene.CreateActor(sphereActorDesc);

                var revoluteJointDesc = new RevoluteJointDesc();
                revoluteJointDesc.JointFlags = JointFlags.CollisionEnabled | JointFlags.Visualization;
                revoluteJointDesc.SetActors(null, sphereActor);
                revoluteJointDesc.GlobalAnchor = sphereActorDesc.GlobalPosition + new Mogre.Vector3(0, stringlength, 0);
                revoluteJointDesc.GlobalAxis = new Mogre.Vector3(0, 0, 1);

                scene.CreateJoint(revoluteJointDesc);
            }

            PhysxExpansion.CreateSphere("SPHERE", radius, mSceneMgr, 32, 32);

            foreach (var actor in scene.Actors)
            {
                if (!actor.IsDynamic)
                    continue;

                var sphereEnt = mSceneMgr.CreateEntity("Sphere_" + Guid.NewGuid(), "SPHERE");
                var sphereSceneNode = mSceneMgr.RootSceneNode.CreateChildSceneNode();
                sphereSceneNode.AttachObject(sphereEnt);
                sphereSceneNode.Position = actor.GlobalPosition;
                actorNodes.Add(new ActorNode(sphereSceneNode, actor));
            }

            for (int i = -2; i <= 2; i++)
            {
                var actor = scene.Actors[i + 3];
                Mogre.Vector3 pos = actor.GlobalPosition;

                ManualObject manualObject = mSceneMgr.CreateManualObject();
                SceneNode manualObjectNode = mSceneMgr.RootSceneNode.CreateChildSceneNode();
                manualObject.Begin("BaseWhiteNoLighting", RenderOperation.OperationTypes.OT_LINE_LIST);
                manualObject.Position(pos.x, pos.y, pos.z);
                manualObject.Position(2 * radius * i, stringlength + centery, -3);
                manualObject.End();
                manualObjectNode.AttachObject(manualObject);

                manualObject = mSceneMgr.CreateManualObject();
                manualObjectNode = mSceneMgr.RootSceneNode.CreateChildSceneNode();
                manualObject.Begin("BaseWhiteNoLighting", RenderOperation.OperationTypes.OT_LINE_LIST);
                manualObject.Position(pos.x, pos.y, pos.z);
                manualObject.Position(2 * radius * i, stringlength + centery, 3);
                manualObject.End();
                manualObjectNode.AttachObject(manualObject);
            }

            //Panel
            ManualObject mo = mSceneMgr.CreateManualObject();
            SceneNode moNode = mSceneMgr.RootSceneNode.CreateChildSceneNode();
            mo.Begin("BaseWhiteNoLighting", RenderOperation.OperationTypes.OT_TRIANGLE_LIST);
            mo.Position(planesize, stringlength + centery, planesize);
            mo.Position(-planesize, stringlength + centery, planesize);
            mo.Position(-planesize, stringlength + centery, -planesize);
            mo.Position(planesize, stringlength + centery, -planesize);
            mo.Quad(0, 1, 2, 3);
            mo.End();
            moNode.AttachObject(mo);
            actorNodes.Add(new ActorNode(moNode, scene.Actors[0]));

            //Look At the table
            mCamera.Position = new Mogre.Vector3(
                moNode.Position.x,
                moNode.Position.y - 2,
                moNode.Position.z + 15
                );
            mCamera.Pitch(new Radian(new Degree(20)));

            scene.Simulate(0);

            AdvancedMogreFramework.Instance.mMouse.MouseMoved += mouseMoved;
            AdvancedMogreFramework.Instance.mMouse.MousePressed += mousePressed;
            AdvancedMogreFramework.Instance.mMouse.MouseReleased += mouseReleased;
            AdvancedMogreFramework.Instance.mKeyboard.KeyPressed += keyPressed;
            AdvancedMogreFramework.Instance.mKeyboard.KeyReleased += keyReleased;
        }

        public override bool keyPressed(KeyEvent keyEventRef)
        {
            if (keyEventRef.key == KeyCode.KC_B)
            {
                scene.Actors[1].AddForce(new Mogre.Vector3(-7, 0, 0), ForceModes.VelocityChange);
            }

            return true;
        }

        private void UpdateActorNodes(float deltaTime)
        {
            foreach(var actorNode in actorNodes)
            {
                actorNode.Update(deltaTime);
            }
        }

        public override void Update(double timeSinceLastFrame)
        {
            getInput();
            moveCamera();

            scene.FlushStream();
            scene.FetchResults(SimulationStatuses.AllFinished, true);
            scene.Simulate(timeSinceLastFrame);

            UpdateActorNodes((float)timeSinceLastFrame);
        }

        public override void Exit()
        {
        }
    }
}
