using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using System.IO;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;

namespace AdvancedMogreFramework.Helper
{
    public class ObjToMesh
    {
        /// <summary>
        /// Convert Obj Model File to Ogre Mesh format
        /// </summary>
        /// <param name="fileStream">Obj File Stream</param>
        /// <returns>Ogre Mesh</returns>
        public MeshPtr ConvertObjToMesh(Stream fileStream) 
        {
            List<Vector3> vertexObj = new List<Vector3>();
            List<Vector3> faceObj = new List<Vector3>();
            float[] vertices;
            float[] faces;

            StreamReader reader = new StreamReader(fileStream);
            string line;

            Regex floatNumber = new Regex(@"[\d\-][\d\.]*");
            Regex ushortNumber = new Regex(@"\d+");
            MatchCollection matchList;

            while ((line = reader.ReadLine()) != null)
            {
                //Read vertices
                if (line.Substring(0, 2) == "v ")
                {
                    matchList = floatNumber.Matches(line);
                    float x = Convert.ToSingle(matchList[0].ToString());
                    float y = Convert.ToSingle(matchList[1].ToString());
                    float z = Convert.ToSingle(matchList[2].ToString());
                    vertexObj.Add(new Vector3(x, y, z));
                }

                //Read faces
                else if (line.Substring(0, 2) == "f ")
                {
                    //Error here where invalid indices were given. This is because the OBJ file started indexing the verts from 1 instead of 0.
                    matchList = ushortNumber.Matches(line);
                    int v1 = -1 + Convert.ToUInt16(matchList[0].ToString());
                    int v2 = -1 + Convert.ToUInt16(matchList[1].ToString());
                    int v3 = -1 + Convert.ToUInt16(matchList[2].ToString());
                    faceObj.Add(new Vector3((ushort)v1, (ushort)v2, (ushort)v3));
                }
            }

            int vertexNum = vertexObj.Count;
            vertices = new float[vertexNum * 3];
            for (int i = 0; i < vertexNum; i++)
            {
                vertices[i * 3 + 0] = vertexObj[i].x;
                vertices[i * 3 + 1] = vertexObj[i].y;
                vertices[i * 3 + 2] = vertexObj[i].z;
            }

            int faceNum = faceObj.Count;
            faces = new float[faceNum * 3];
            for (int i = 0; i < faceNum; i++)
            {
                faces[i * 3 + 0] = faceObj[i].x;
                faces[i * 3 + 1] = faceObj[i].y;
                faces[i * 3 + 2] = faceObj[i].z;
            }

            MeshPtr mesh = MeshManager.Singleton.CreateManual("mesh1", ResourceGroupManager.DEFAULT_RESOURCE_GROUP_NAME);
            SubMesh subMesh = mesh.CreateSubMesh();

            mesh.sharedVertexData = new VertexData();
            mesh.sharedVertexData.vertexCount = (uint)vertexObj.Count;

            VertexDeclaration vdecl = mesh.sharedVertexData.vertexDeclaration;
            VertexBufferBinding vbind = mesh.sharedVertexData.vertexBufferBinding;

            vdecl.AddElement(0, 0, VertexElementType.VET_FLOAT3, VertexElementSemantic.VES_POSITION);
            HardwareVertexBufferSharedPtr vertexBuff =
                HardwareBufferManager.Singleton.CreateVertexBuffer((uint)(3 * System.Runtime.InteropServices.Marshal.SizeOf(typeof(float))), (uint)vertexObj.Count, HardwareBuffer.Usage.HBU_DYNAMIC_WRITE_ONLY_DISCARDABLE);
            unsafe
            {
                GCHandle handle=GCHandle.Alloc(vertices, GCHandleType.Pinned);
                void* pVertex = (void*)handle.AddrOfPinnedObject();
                vertexBuff.WriteData(0, vertexBuff.SizeInBytes, pVertex, true);
                handle.Free();
                vbind.SetBinding(0, vertexBuff);
            }
            HardwareIndexBufferSharedPtr indexBuff =
                HardwareBufferManager.Singleton.CreateIndexBuffer(HardwareIndexBuffer.IndexType.IT_32BIT, (uint)(3 * faceObj.Count), HardwareBuffer.Usage.HBU_STATIC, true);
            unsafe{
                GCHandle handle = GCHandle.Alloc(faces, GCHandleType.Pinned);
                void* pFaces = (void*)handle.AddrOfPinnedObject();
                indexBuff.WriteData(0, indexBuff.SizeInBytes, pFaces, true);
                handle.Free();
            }

            subMesh.useSharedVertices = true;
            subMesh.indexData.indexBuffer = indexBuff;
            subMesh.indexData.indexStart = 0;
            subMesh.indexData.indexCount = (uint)(3 * faceObj.Count);

            mesh._setBounds(new AxisAlignedBox(-100, -100, -100, 100, 100, 100));
            mesh.Touch();

            reader.Close();

            return mesh;
        }
    }
}
