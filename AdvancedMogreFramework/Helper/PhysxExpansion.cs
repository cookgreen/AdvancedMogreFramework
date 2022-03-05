using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using Mogre.PhysX;
using System.IO;

namespace AdvancedMogreFramework.Helper
{
    public static class PhysxExpansion
    {
        public static ConvexShapeDesc CreateConvexHull(this Physics physics, StaticMeshData meshData)
        {
            // create descriptor for convex hull
            ConvexShapeDesc convexMeshShapeDesc = null;
            ConvexMeshDesc convexMeshDesc = new ConvexMeshDesc();
            convexMeshDesc.PinPoints<float>(meshData.Points, 0, sizeof(float) * 3);
            convexMeshDesc.PinTriangles<uint>(meshData.Indices, 0, sizeof(uint) * 3);
            convexMeshDesc.VertexCount = (uint)meshData.Vertices.Length;
            convexMeshDesc.TriangleCount = (uint)meshData.TriangleCount;
            convexMeshDesc.Flags = ConvexFlags.ComputeConvex;

            MemoryStream stream = new MemoryStream(1024);
            CookingInterface.InitCooking();

            if (CookingInterface.CookConvexMesh(convexMeshDesc, stream))
            {
                stream.Seek(0, SeekOrigin.Begin);
                ConvexMesh convexMesh = physics.CreateConvexMesh(stream);
                convexMeshShapeDesc = new ConvexShapeDesc(convexMesh);
                CookingInterface.CloseCooking();
            }

            convexMeshDesc.UnpinAll();
            return convexMeshShapeDesc;
        }

        public static TriangleMeshShapeDesc CreateTriangleMesh(this Physics physics, StaticMeshData meshData)
        {
            // create descriptor for triangle mesh
            TriangleMeshShapeDesc triangleMeshShapeDesc = null;
            TriangleMeshDesc triangleMeshDesc = new TriangleMeshDesc();
            triangleMeshDesc.PinPoints<float>(meshData.Points, 0, sizeof(float) * 3);
            triangleMeshDesc.PinTriangles<uint>(meshData.Indices, 0, sizeof(uint) * 3);
            triangleMeshDesc.VertexCount = (uint)meshData.Vertices.Length;
            triangleMeshDesc.TriangleCount = (uint)meshData.TriangleCount;

            MemoryStream stream = new MemoryStream(1024);
            CookingInterface.InitCooking();

            if (CookingInterface.CookTriangleMesh(triangleMeshDesc, stream))
            {
                stream.Seek(0, SeekOrigin.Begin);
                TriangleMesh triangleMesh = physics.CreateTriangleMesh(stream);
                triangleMeshShapeDesc = new TriangleMeshShapeDesc(triangleMesh);
                CookingInterface.CloseCooking();
            }

            triangleMeshDesc.UnpinAll();
            return triangleMeshShapeDesc;
        }

        public static void CreateSphere(string strName, float r, SceneManager sceneMgr, int nRings = 16, int nSegments = 16)
        {
            ManualObject manual = sceneMgr.CreateManualObject(strName);
            manual.Begin("BaseWhiteNoLighting", RenderOperation.OperationTypes.OT_TRIANGLE_LIST);

            float fDeltaRingAngle = (Mogre.Math.PI / nRings);
            float fDeltaSegAngle = (2 * Mogre.Math.PI / nSegments);
            ushort wVerticeIndex = 0;

            // Generate the group of rings for the sphere
            for (int ring = 0; ring <= nRings; ring++)
            {
                float r0 = r * Mogre.Math.Sin(ring * fDeltaRingAngle);
                float y0 = r * Mogre.Math.Cos(ring * fDeltaRingAngle);

                // Generate the group of segments for the current ring
                for (int seg = 0; seg <= nSegments; seg++)
                {
                    float x0 = r0 * Mogre.Math.Sin(seg * fDeltaSegAngle);
                    float z0 = r0 * Mogre.Math.Cos(seg * fDeltaSegAngle);

                    // Add one vertex to the strip which makes up the sphere
                    manual.Position(x0, y0, z0);
                    manual.Normal(new Mogre.Vector3(x0, y0, z0).NormalisedCopy);
                    manual.TextureCoord((float)seg / (float)nSegments, (float)ring / (float)nRings);

                    if (ring != nRings)
                    {
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
            mesh._setBounds(new AxisAlignedBox(new Mogre.Vector3(-r, -r, -r), new Mogre.Vector3(r, r, r)), false);

            mesh._setBoundingSphereRadius(r);
            ushort src, dest;
            if (!mesh.SuggestTangentVectorBuildParams(VertexElementSemantic.VES_TANGENT, out src, out dest))
            {
                mesh.BuildTangentVectors(VertexElementSemantic.VES_TANGENT, src, dest);
            }
        }

        public static unsafe void CreateCube(string name, Mogre.Vector3 gpose, double d)
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
            fixed (float* pFVertice = vertices)
            {
                pVertices = (void*)pFVertice;
            }

            vbuf.WriteData(0, vbuf.SizeInBytes, pVertices, true);
            bind.SetBinding(0, vbuf);

            void* pFaces;
            fixed (ushort* pUFaces = faces)
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
    }
}
