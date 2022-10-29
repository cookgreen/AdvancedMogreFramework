using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using Mogre.PhysX;
using System.IO;

namespace org.ogre.framework
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
    }
}
