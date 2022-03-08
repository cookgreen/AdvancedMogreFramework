using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using MOIS;
using Mogre.PhysX;
using AdvancedMogreFramework.Helper;
using System.IO;
using AdvancedMogreFramework.Entities;
using System.Runtime.InteropServices;

namespace AdvancedMogreFramework.States
{
    public class PhysxClothState : GameState
    {
        private Physics physics;
        private Scene scene;
        private List<ActorNode> actorNodes;

        public PhysxClothState()
        {
            actorNodes = new List<ActorNode>();

            physics = Physics.Create();
            scene = physics.CreateScene(new SceneDesc());
        }

        public override void Enter()
        {
        }

        public override void Exit()
        {
        }

        public override void Update(double timeSinceLastFrame)
        {
            getInput();
            moveCamera();

            scene.FlushStream();
            scene.FetchResults(SimulationStatuses.AllFinished, true);
            scene.Simulate(timeSinceLastFrame);
        }
    }

    public class MyCloth : IDisposable
    {
        private bool mInitDone;
        private Scene mScene;
        private MeshData mReceiveBuffers;
        private Cloth mCloth;
        private ClothMesh mClothMesh;
        private uint mIndexRenderBuffer;

        private uint mMaxVertices;
        private uint mMaxIndices;
        private uint mNumIndices;
        private uint mNumParentIndices;
        private uint mNumVertices;
        private uint mLastNumVertices;

        private uint mMeshDirtyFlags;
        private bool mTeared;

        public MyCloth(Scene scene, ClothDesc clothDesc, string objFileName, 
            float scale, Mogre.Vector3 offset, string textureFileName)
        {
            mInitDone = false;
            ClothMeshDesc clothMeshDesc = new ClothMeshDesc();
            generateObjMeshDesc(ref clothMeshDesc, objFileName, scale, offset, !string.IsNullOrEmpty(textureFileName));
            init(scene, clothDesc, clothMeshDesc);
            if(!string.IsNullOrEmpty(textureFileName))
            {
                createTexture(textureFileName);
            }
        }

        private void init(Scene scene, ClothDesc desc, ClothMeshDesc meshDesc)
        {
            mScene = scene;

            if (((int)desc.Flags & (int)ClothFlags.Tearable) == 1)
            {
            }

            desc.Flags |= ClothFlags.Visualization;

            cookMesh(meshDesc);

            desc.ClothMesh = mClothMesh;
            mCloth = mScene.CreateCloth(desc);

            mInitDone = true;
        }

        private bool cookMesh(ClothMeshDesc meshDesc)
        {
            mLastNumVertices = meshDesc.VertexCount;

            if(meshDesc.IsValid)
            {
                MemoryStream memoryStream = new MemoryStream();
                bool success = PhysxCommon.CookClothMesh(meshDesc, memoryStream);
                if(!success)
                {
                    return false;
                }
                else
                {
                    mClothMesh = mScene.Physics.CreateClothMesh(memoryStream);
                    return true;
                }
            }
            else
            {
                return false;
            }
        }

        private unsafe void generateObjMeshDesc(ref ClothMeshDesc desc, string fileName, float scale, Mogre.Vector3 offset, bool textured)
        {
            MeshPtr meshPtr = MeshManager.Singleton.Load(fileName, ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME);
            StaticMeshData staticMeshData = new StaticMeshData(meshPtr);

            desc.VertexCount = (uint)staticMeshData.Vertices.Length;
            desc.TriangleCount = (uint)staticMeshData.TriangleCount;
            desc.PointsByteStride = (uint)sizeof(Mogre.Vector3);
            desc.TrianglesByteStride = sizeof(uint);
            desc.VertexMassesByteStride = sizeof(float);

            IntPtr ptr = new IntPtr();
            Marshal.Copy(staticMeshData.Points, 0, ptr, staticMeshData.Points.Length);
            desc.PointsPtr = ptr;

            int[] arr = new int[staticMeshData.Indices.Length];
            for (int i = 0; i < arr.Length; i++)
            {
                arr[i] = (int)staticMeshData.Indices[i];
            }
            Marshal.Copy(arr, 0, ptr, staticMeshData.Indices.Length);
            desc.TrianglesPtr = ptr;
            desc.VertexMassesPtr = new IntPtr();
            desc.VertexFlagsPtr = new IntPtr();
            desc.Flags = MeshFlags.HardwareMesh;
            desc.WeldingDistance = 0.0001f;
        }

        private void createTexture(string textureFileName)
        {

        }

        public void Dispose()
        {
            if(mInitDone)
            {
                if (mCloth != null) { mCloth.Dispose(); }
                if (mClothMesh != null) { mClothMesh.Dispose(); }
            }
        }
    }
}
