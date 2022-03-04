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

            trayMgr = AdvancedMogreFramework.instance.m_pTrayMgr;
            cubeActorNodes = new List<ActorNode>();
        }

        public override void enter()
        {
            trayMgr.destroyAllWidgets();

            mSceneMgr = AdvancedMogreFramework.Singleton.m_pRoot.CreateSceneManager(SceneType.ST_GENERIC, "PhysxBasicCubeMgr");
            ColourValue cvAmbineLight = new ColourValue(0.7f, 0.7f, 0.7f);
            mSceneMgr.AmbientLight = cvAmbineLight;//(Ogre::ColourValue(0.7f, 0.7f, 0.7f));

            mCamera = mSceneMgr.CreateCamera("PhysxBasicCubeCamera");
            mCamera.NearClipDistance = 5;
            mCamera.FarClipDistance = 999;
            mCamera.AspectRatio = AdvancedMogreFramework.Singleton.m_pViewport.ActualWidth / AdvancedMogreFramework.Singleton.m_pViewport.ActualHeight;

            AdvancedMogreFramework.Singleton.m_pViewport.Camera = mCamera;

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

            AdvancedMogreFramework.Singleton.m_pMouse.MouseMoved += mouseMoved;
            AdvancedMogreFramework.Singleton.m_pMouse.MousePressed += mousePressed;
            AdvancedMogreFramework.Singleton.m_pMouse.MouseReleased += mouseReleased;
            AdvancedMogreFramework.Singleton.m_pKeyboard.KeyPressed += keyPressed;
            AdvancedMogreFramework.Singleton.m_pKeyboard.KeyReleased += keyReleased;
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

        private void createSphere(string strName, float r, SceneManager sceneMgr, int nRings = 16, int nSegments = 16)
        {
            ManualObject manual = sceneMgr.CreateManualObject(strName);
            manual.Begin("BaseWhiteNoLighting", RenderOperation.OperationTypes.OT_TRIANGLE_LIST);

            float fDeltaRingAngle = (Mogre.Math.PI / nRings);
            float fDeltaSegAngle = (2 * Mogre.Math.PI / nSegments);
            ushort wVerticeIndex = 0;

            // Generate the group of rings for the sphere
            for (int ring = 0; ring <= nRings; ring++) {
                float r0 = r * Mogre.Math.Sin(ring * fDeltaRingAngle);
                float y0 = r * Mogre.Math.Cos(ring * fDeltaRingAngle);

                // Generate the group of segments for the current ring
                for (int seg = 0; seg <= nSegments; seg++) {
                    float x0 = r0 * Mogre.Math.Sin(seg * fDeltaSegAngle);
                    float z0 = r0 * Mogre.Math.Cos(seg * fDeltaSegAngle);

                    // Add one vertex to the strip which makes up the sphere
                    manual.Position(x0, y0, z0);
                    manual.Normal( new Mogre.Vector3(x0, y0, z0).NormalisedCopy);
                    manual.TextureCoord((float)seg / (float)nSegments, (float)ring / (float)nRings);

                    if (ring != nRings) {
                        // each vertex (except the last) has six indicies pointing to it
                        manual.Index((uint)(wVerticeIndex + nSegments + 1));
                        manual.Index(wVerticeIndex);
                        manual.Index((uint)(wVerticeIndex + nSegments));
                        manual.Index((uint)(wVerticeIndex + nSegments + 1));
                        manual.Index((uint)(wVerticeIndex + 1));
                        manual.Index(wVerticeIndex);
                        wVerticeIndex++;
                    }
                }; // end for seg
            } // end for ring
            manual.End();
            MeshPtr mesh = manual.ConvertToMesh(strName);
            mesh._setBounds(new AxisAlignedBox(new Mogre.Vector3(-r, -r, -r),new Mogre.Vector3(r, r, r)), false);

            mesh._setBoundingSphereRadius(r);
            ushort src, dest;
            if (!mesh.SuggestTangentVectorBuildParams(VertexElementSemantic.VES_TANGENT, out src, out dest))
            {
                mesh.BuildTangentVectors(VertexElementSemantic.VES_TANGENT, src, dest);
            }
        }

        private unsafe void createCube(string name, Mogre.Vector3 gpose, double d)
        {
            MeshPtr msh = MeshManager.Singleton.CreateManual(name, "General");
            SubMesh sub1 = msh.CreateSubMesh("1");
            
            const float sqrt13 = 0.577350269f; /* sqrt(1/3) */
            const int nVertices = 8;
            const int vbufCount = 3 * 2 * nVertices;

            float[] vertices = new float[vbufCount]{
                  (float)(gpose.x - d / 2),(float)(gpose.y + d / 2),(float)(gpose.z - d / 2),        //0 position
                  -sqrt13,sqrt13,-sqrt13,     //0 normal A
                  (float)(gpose.x + d / 2),(float)(gpose.y + d / 2),(float)(gpose.z - d / 2),         //1 position
                  sqrt13,sqrt13,-sqrt13,      //1 normal B
                  (float)(gpose.x + d / 2),(float)(gpose.y - d / 2),(float)(gpose.z - d / 2),        //2 position
                  sqrt13,-sqrt13,-sqrt13,     //2 normal F
                  (float)(gpose.x - d / 2),(float)(gpose.y - d / 2),(float)(gpose.z - d / 2),        //3 position
                  -sqrt13,-sqrt13,-sqrt13,    //3 normal H
                  (float)(gpose.x - d / 2),(float)(gpose.y + d / 2),(float)(gpose.z + d / 2),
                  -sqrt13,sqrt13,sqrt13,      //4 normal C
                  (float)(gpose.x + d / 2),(float)(gpose.y + d / 2),(float)(gpose.z + d / 2),
                  sqrt13,sqrt13,sqrt13,       //5 normal D
                  (float)(gpose.x + d / 2),(float)(gpose.y - d / 2),(float)(gpose.z + d / 2),
                  sqrt13,-sqrt13,sqrt13,      //6 normal E
                  (float)(gpose.x - d / 2),(float)(gpose.y - d / 2),(float)(gpose.z + d / 2),
                  -sqrt13,-sqrt13,sqrt13,     //7 normal G
               };

            const int ibufCount = 36;
            ushort[] faces = new ushort[ibufCount]{
                  //back
                  0,2,3,
                  0,1,2,
                  //right
                  1,6,2,
                  1,5,6,
                  //front
                  4,6,5,
                  4,7,6,
                  //left
                  0,7,4,
                  0,3,7,
                  //top
                  0,5,1,
                  0,4,5,
                  //bottom
                  2,7,3,
                  2,6,7
               };

            sub1.vertexData = new VertexData();
            sub1.vertexData.vertexCount = nVertices;

            VertexDeclaration decl = sub1.vertexData.vertexDeclaration;
            uint offset = 0;
            //position
            decl.AddElement(0, offset, VertexElementType.VET_FLOAT3, VertexElementSemantic.VES_POSITION);
            offset += VertexElement.GetTypeSize(VertexElementType.VET_FLOAT3);
            //normal
            decl.AddElement(0, offset, VertexElementType.VET_FLOAT3, VertexElementSemantic.VES_NORMAL);
            offset += VertexElement.GetTypeSize(VertexElementType.VET_FLOAT3);

            HardwareVertexBufferSharedPtr vbuf = HardwareBufferManager.Singleton.CreateVertexBuffer(offset, sub1.vertexData.vertexCount, HardwareBuffer.Usage.HBU_STATIC_WRITE_ONLY);
            VertexBufferBinding bind = sub1.vertexData.vertexBufferBinding;

            void* pVertices;
            fixed(float* pFVertice = vertices)
            {
                pVertices = (void*)pFVertice;
            }

            vbuf.WriteData(0, vbuf.SizeInBytes, pVertices, true);
            bind.SetBinding(0, vbuf);

            void* pFaces;
            fixed(ushort* pUFaces = faces)
            {
                pFaces = (void*)pUFaces;
            }
            

            HardwareIndexBufferSharedPtr ibuf = HardwareBufferManager.Singleton.CreateIndexBuffer(HardwareIndexBuffer.IndexType.IT_16BIT, ibufCount, HardwareBuffer.Usage.HBU_STATIC_WRITE_ONLY);
            ibuf.WriteData(0, ibuf.SizeInBytes, pFaces, true);

            sub1.useSharedVertices = false;
            sub1.indexData.indexBuffer = ibuf;
            sub1.indexData.indexCount = ibufCount;
            sub1.indexData.indexStart = 0;

            sub1.SetMaterialName("Examples/10PointBlock");
            
            msh._setBounds(new AxisAlignedBox(-100, -100, -100, 100, 100, 100));
            msh._setBoundingSphereRadius(Mogre.Math.Sqrt(3 * 100 * 100));
            
            msh.Load();
        }

        public override void update(double timeSinceLastFrame)
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
