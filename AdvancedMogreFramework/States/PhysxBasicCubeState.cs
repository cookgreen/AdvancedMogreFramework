using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using Mogre.PhysX;
using Mogre_Procedural.MogreBites;
using System.Runtime.InteropServices;
using AdvancedMogreFramework.Helper;
using MOIS;

namespace AdvancedMogreFramework.States
{
    public class PhysxBasicCubeState : GameState
    {
        private Scene scene;
        private Physics physx;
        private SdkTrayManager trayMgr;
        private Random rnd;
        private List<ActorNode> cubeActorNodes;

        public PhysxBasicCubeState()
        {
            rnd = new Random();

            physx = Physics.Create();
            SceneDesc desc = new SceneDesc();
            scene = physx.CreateScene(desc);
            scene.Gravity = new Mogre.Vector3(0, -9.81f, 0);

            var defm = scene.Materials[0];
            defm.Restitution = 0.5f;
            defm.DynamicFriction = defm.StaticFriction = 0.6f;

            trayMgr = AdvancedMogreFramework.instance.mTrayMgr;
            cubeActorNodes = new List<ActorNode>();
        }

        public override void Enter()
        {
            trayMgr.destroyAllWidgets();

            mSceneMgr = AdvancedMogreFramework.Instance.mRoot.CreateSceneManager(SceneType.ST_GENERIC, "PhysxBasicCubeMgr");
            ColourValue cvAmbineLight = new ColourValue(0.7f, 0.7f, 0.7f);
            mSceneMgr.AmbientLight = cvAmbineLight;//(Ogre::ColourValue(0.7f, 0.7f, 0.7f));

            mCamera = mSceneMgr.CreateCamera("PhysxBasicCubeCamera");
            mCamera.NearClipDistance = 5;
            mCamera.FarClipDistance = 999;
            mCamera.AspectRatio = AdvancedMogreFramework.Instance.mViewport.ActualWidth / AdvancedMogreFramework.Instance.mViewport.ActualHeight;

            AdvancedMogreFramework.Instance.mViewport.Camera = mCamera;

            Entity tableEnt = mSceneMgr.CreateEntity("PhysxBasicCube_Table_" + Guid.NewGuid(), "PhysxBasicCube_Table.mesh");
            tableEnt.CastShadows = false;
            mSceneMgr.RootSceneNode.AttachObject(tableEnt);
            ActorDesc actorDesc = new ActorDesc();
            actorDesc.Density = 4;
            actorDesc.Body = null;
            actorDesc.Shapes.Add(physx.CreateConvexHull(new StaticMeshData(tableEnt.GetMesh())));
            scene.CreateActor(actorDesc);

            //Look At the table
            mCamera.Position = new Mogre.Vector3(
                mSceneMgr.RootSceneNode.Position.x,
                mSceneMgr.RootSceneNode.Position.y + 5,
                mSceneMgr.RootSceneNode.Position.z + 5
                );
            mCamera.Pitch(new Radian(new Degree(-40)));

            scene.Simulate(0);

            AdvancedMogreFramework.Instance.mMouse.MouseMoved += mouseMoved;
            AdvancedMogreFramework.Instance.mMouse.MousePressed += mousePressed;
            AdvancedMogreFramework.Instance.mMouse.MouseReleased += mouseReleased;
            AdvancedMogreFramework.Instance.mKeyboard.KeyPressed += keyPressed;
            AdvancedMogreFramework.Instance.mKeyboard.KeyReleased += keyReleased;
        }

        private void AddCube()
        {
            double d = 0.1 + 0.2 * rnd.NextDouble();
            Mogre.Vector3 pos = new Mogre.Vector3((float)(-1 * d), (float)20, 0);

            string cubeName = "CUSTOME_CUBE_" + Guid.NewGuid().ToString();
            Entity cubeEnt = mSceneMgr.CreateEntity(cubeName, "Cube.mesh");
            cubeEnt.SetMaterialName("Examples/10PointBlock");
            SceneNode cubeSceneNode = mSceneMgr.RootSceneNode.CreateChildSceneNode();
            cubeSceneNode.AttachObject(cubeEnt);

            var staticMeshData = new StaticMeshData(cubeEnt.GetMesh());
            ActorDesc actorDesc = new ActorDesc();
            actorDesc.Density = 4;
            actorDesc.Body = new BodyDesc();
            actorDesc.Shapes.Add(physx.CreateConvexHull(staticMeshData));
            actorDesc.GlobalPosition = pos;
            var cubeActor = scene.CreateActor(actorDesc);

            cubeActorNodes.Add(new ActorNode(cubeSceneNode, cubeActor));
        }

        private void UpdateCubes(float deltaTime)
        {
            foreach (var actorNode in cubeActorNodes)
            {
                actorNode.Update(deltaTime);
            }
        }

        public override bool keyPressed(KeyEvent keyEventRef)
        {
            base.keyPressed(keyEventRef);

            if (keyEventRef.key == KeyCode.KC_B)
            {
                AddCube();
            }

            return true;
        }

        public override void Update(double timeSinceLastFrame)
        {
            getInput();
            moveCamera();

            UpdateCubes((float)timeSinceLastFrame);

            scene.FlushStream();
            scene.FetchResults(SimulationStatuses.AllFinished, true);
            scene.Simulate(timeSinceLastFrame);
        }
    }
}
