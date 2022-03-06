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
            Framework.Instance.mTrayMgr.destroyAllWidgets();

            mSceneMgr = Framework.Instance.mRoot.CreateSceneManager(SceneType.ST_GENERIC, "PhysxNewtonCradleMgr");
            ColourValue cvAmbineLight = new ColourValue(0.7f, 0.7f, 0.7f);
            mSceneMgr.AmbientLight = cvAmbineLight;

            mCamera = mSceneMgr.CreateCamera("PhysxNewtonCradleCamera");
            mCamera.NearClipDistance = 5;
            mCamera.FarClipDistance = 999;
            mCamera.AspectRatio = Framework.Instance.mViewport.ActualWidth / Framework.Instance.mViewport.ActualHeight;

            Framework.Instance.mViewport.Camera = mCamera;

            string[] mats = new string[]
            {
                "BaseRed",
                "BaseGreen",
                "BaseBlue",
                "BaseBlack",
                "BasePurple"
            };

            var physicsMat = scene.CreateMaterial(new MaterialDesc(0.0f, 0.0f, 1.0f));
            
            var sphereShapeDesc = new SphereShapeDesc(radius);
            sphereShapeDesc.SkinWidth = 0.0f;
            sphereShapeDesc.MaterialIndex = physicsMat.Index;

            var sphereActorDesc = new ActorDesc(new BodyDesc(0.124f, 1.0f), 150.1f, sphereShapeDesc);

            for (int i = -2; i <= 2; i++)
            {
                sphereActorDesc.GlobalPosition = new Mogre.Vector3(2 * radius * i, centery, 0);
                var sphereActor = scene.CreateActor(sphereActorDesc);
            }

            for (int i = -2; i <= 2; i++)
            {
                var actor = scene.Actors[i + 3];
                var pos = actor.GlobalPosition;

                ManualObject linesObject = mSceneMgr.CreateManualObject();
                SceneNode linesObjectNode = mSceneMgr.RootSceneNode.CreateChildSceneNode();
                linesObject.Begin(mats[i + 2], RenderOperation.OperationTypes.OT_LINE_LIST);

                linesObject.Position(pos.x, pos.y, pos.z);
                linesObject.Position(2 * radius * i, stringlength + centery, -3);
                linesObject.Position(pos.x, pos.y, pos.z);
                linesObject.Position(2 * radius * i, stringlength + centery, 3);
                
                linesObject.End();
                linesObjectNode.AttachObject(linesObject);

                var lineActorDesc = new ActorDesc();
                lineActorDesc.Body = new BodyDesc(0.124f, 1.0f);
                lineActorDesc.Density = 80f;
                lineActorDesc.Shapes.Add(new BoxShapeDesc());
                var lineActor = scene.CreateActor(lineActorDesc);

                actorNodes.Add(new ActorNode(linesObjectNode, lineActor));

                var fixedJointDesc = new FixedJointDesc();
                fixedJointDesc.JointFlags = JointFlags.CollisionEnabled | JointFlags.Visualization;
                fixedJointDesc.SetActors(lineActor, actor);
                //fixedJointDesc.GlobalAnchor = new Mogre.Vector3();
                //fixedJointDesc.GlobalAxis = new Mogre.Vector3(0, 0, 1);

                scene.CreateJoint(fixedJointDesc);

                var revoluteJointDesc = new RevoluteJointDesc();
                revoluteJointDesc.JointFlags = JointFlags.CollisionEnabled | JointFlags.Visualization;
                revoluteJointDesc.SetActors(null, lineActor);
                revoluteJointDesc.GlobalAnchor = sphereActorDesc.GlobalPosition + new Mogre.Vector3(0, stringlength, 0);
                revoluteJointDesc.GlobalAxis = new Mogre.Vector3(0, 0, 1);
                
                scene.CreateJoint(revoluteJointDesc);
            }

            PhysxExpansion.CreateSphere("SPHERE", radius, mSceneMgr, 32, 32);

            for (int i = -2; i <= 2; i++)
            {
                var actor = scene.Actors[i + 3];
                if (!actor.IsDynamic)
                {
                    continue;
                }

                Mogre.Vector3 pos = actor.GlobalPosition;

                var sphereEnt = mSceneMgr.CreateEntity("Sphere_" + Guid.NewGuid(), "SPHERE");
                var sphereSceneNode = mSceneMgr.RootSceneNode.CreateChildSceneNode();
                sphereSceneNode.AttachObject(sphereEnt);
                sphereSceneNode.Position = pos;
                actorNodes.Add(new ActorNode(sphereSceneNode, actor));
            }

            //for (int i = -2; i <= 2; i++)
            //{
            //    var actor = scene.Actors[i + 3];
            //    Mogre.Vector3 pos = actor.GlobalPosition;
            //
            //    ManualObject linesObject = mSceneMgr.CreateManualObject();
            //    SceneNode linesObjectNode = mSceneMgr.RootSceneNode.CreateChildSceneNode();
            //    linesObject.Begin(mats[i + 2], RenderOperation.OperationTypes.OT_LINE_LIST);
            //    
            //    linesObject.Position(pos.x, pos.y, pos.z);
            //    linesObject.Position(2 * radius * i, stringlength + centery, -3);
            //    linesObject.Position(pos.x, pos.y, pos.z);
            //    linesObject.Position(2 * radius * i, stringlength + centery, 3);
            //
            //    linesObject.End();
            //    linesObjectNode.AttachObject(linesObject);
            //}

            //Panel
            ManualObject panelObject = mSceneMgr.CreateManualObject();
            SceneNode panelObjectNode = mSceneMgr.RootSceneNode.CreateChildSceneNode();
            panelObject.Begin("BaseWhiteNoLighting", RenderOperation.OperationTypes.OT_TRIANGLE_LIST);
            panelObject.Position(planesize, stringlength + centery, planesize);
            panelObject.Position(-planesize, stringlength + centery, planesize);
            panelObject.Position(-planesize, stringlength + centery, -planesize);
            panelObject.Position(planesize, stringlength + centery, -planesize);
            panelObject.Quad(0, 1, 2, 3);
            panelObject.End();
            panelObjectNode.AttachObject(panelObject);
            actorNodes.Add(new ActorNode(panelObjectNode, scene.Actors[0]));

            var actorPos = scene.Actors[0].GlobalPosition;

            //Look At the table
            mCamera.Position = new Mogre.Vector3(
                actorPos.x,
                actorPos.y - 2,
                actorPos.z + 15
                );
            mCamera.Pitch(new Radian(new Degree(20)));

            Framework.Instance.mMouse.MouseMoved += mouseMoved;
            Framework.Instance.mMouse.MousePressed += mousePressed;
            Framework.Instance.mMouse.MouseReleased += mouseReleased;
            Framework.Instance.mKeyboard.KeyPressed += keyPressed;
            Framework.Instance.mKeyboard.KeyReleased += keyReleased;
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
            scene.FetchResults(SimulationStatuses.AllFinished, false);
            scene.Simulate(timeSinceLastFrame);

            UpdateActorNodes((float)timeSinceLastFrame);
        }

        public override void Exit()
        {
        }
    }
}
