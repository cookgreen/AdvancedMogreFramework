#region License
/*
This is a conversion of the
MOC - Minimal Ogre Collision v 1.0 beta
Copyright (c) 2008 MouseVolcano (Thomas Gradl, Esa Kylli, Erik Biermann, Karolina Sefyrin)
into C#

MMOC - Minimal Mogre Collision v 1.0 beta
Copyright (c) 2008 Tobias Bohnen

Permission is hereby granted, free of charge, to any person
obtaining a copy of this software and associated documentation
files (the "Software"), to deal in the Software without
restriction, including without limitation the rights to use,
copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the
Software is furnished to do so, subject to the following
conditions:

The above copyright notice and this permission notice shall be
included in all copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
OTHER DEALINGS IN THE SOFTWARE.
 */
#endregion

namespace MMOC
{
   using System;
   using System.Collections.Generic;
   //using MET;
   using Mogre;
   
   public class CollisionTools
   {
      private SceneManager sceneMgr;
      //private TerrainInfo terrainInfo;
      
      public CollisionTools(SceneManager sceneMgr)
      {
         this.sceneMgr = sceneMgr;
         this.HeightAdjust = 0.0f;
      }
      
      public float HeightAdjust { get; set; }
      
      public void CalculateY(SceneNode n, bool doTerrainCheck, bool doGridCheck, float gridWidth, uint queryMask)
      {
         Vector3 pos = n.Position;
         
         float x = pos.x;
         float z = pos.z;
         float y = pos.y;
         
         float terrY = 0, colY = 0, colY2 = 0;

         RaycastResult rr = this.RaycastFromPoint(new Vector3(x, y, z), Vector3.NEGATIVE_UNIT_Y, queryMask);
         if (rr != null)
         {
            if (rr.Target != null)
            {
               colY = rr.Position.y;
            }
            else
            {
               colY = -99999;
            }
         }
         
         // if doGridCheck is on, repeat not to fall through small holes for example when crossing a hangbridge
         if (doGridCheck)
         {
            RaycastResult rr2 = this.RaycastFromPoint(new Vector3(x, y, z) + (n.Orientation * new Vector3(0, 0, gridWidth)), Vector3.NEGATIVE_UNIT_Y, queryMask);
            if (rr2 != null)
            {
               if (rr2.Target != null)
               {
                  colY2 = rr2.Position.y;
               }
               else
               {
                  colY2 = -99999;
               }
            }
            
            if (colY < colY2)
            {
               colY = colY2;
            }
         }
         
         // set the parameter to false if you are not using ETM or TSM
         if (doTerrainCheck)
         {
            //if (this.terrainInfo != null)
            //{
            //   terrY = this.terrainInfo.GetHeightAt(x,z);
            //}
            //else
            //{
            //   terrY = this.GetTSMHeightAt(x, z);
            //}
            //
            //if (terrY < colY)
            //{
            //   n.Position = new Vector3(x, colY + this.HeightAdjust, z);
            //}
            //else
            //{
            //   n.Position = new Vector3(x, terrY + this.HeightAdjust, z);
            //}
         }
         else
         {
            if (!doTerrainCheck && colY == -99999)
            {
               colY = y;
            }
            
            n.Position = new Vector3(x, colY + this.HeightAdjust, z);
         }
      }
      
      public bool CollidesWithEntity(Vector3 fromPoint, Vector3 toPoint, float collisionRadius, float rayHeightLevel, uint queryMask)
      {
         Vector3 fromPointAdj = new Vector3(fromPoint.x, fromPoint.y + rayHeightLevel, fromPoint.z);
         Vector3 toPointAdj = new Vector3(toPoint.x, toPoint.y + rayHeightLevel, toPoint.z);
         Vector3 normal = toPointAdj - fromPointAdj;
         float distToDest = normal.Normalise();

         RaycastResult rr = this.RaycastFromPoint(fromPointAdj, normal, queryMask);
         if (rr != null)
         {
            rr.Distance -= collisionRadius;
            return rr.Distance <= distToDest;
         }
         else
         {
            return false;
         }
      }

      public float GetTSMHeightAt(float x, float z)
      {
         float y = 0.0f;
         
         Ray updateRay = new Ray();
         
         updateRay.Origin = new Vector3(x, 9999, z);
         updateRay.Direction = Vector3.NEGATIVE_UNIT_Y;
         
         RaySceneQuery tsmRaySceneQuery = this.sceneMgr.CreateRayQuery(updateRay);

         RaySceneQueryResult qryResult = tsmRaySceneQuery.Execute();
         
         RaySceneQueryResult.Iterator i = qryResult.Begin();
         if (i != qryResult.End() && i.Value.worldFragment != null)
         {
            y = i.Value.worldFragment.singleIntersection.y;
         }
         
         this.sceneMgr.DestroyQuery(tsmRaySceneQuery);
         tsmRaySceneQuery.Dispose();
         
         return y;
      }

      public RaycastResult Raycast(Ray ray, uint queryMask)
      {
         RaycastResult rr = new RaycastResult();
         
         RaySceneQuery raySceneQuery = this.sceneMgr.CreateRayQuery(new Ray());
         raySceneQuery.SetSortByDistance(true);
         
         // check we are initialised
         if (raySceneQuery != null)
         {
            // create a query object
            raySceneQuery.Ray = ray;
            raySceneQuery.SetSortByDistance(true);
            raySceneQuery.QueryMask = queryMask;
            
            // execute the query, returns a vector of hits
            if (raySceneQuery.Execute().Count <= 0)
            {
               // raycast did not hit an objects bounding box
               return null;
            }
         }
         else
         {
            // LogManager.Singleton.LogMessage("Cannot raycast without RaySceneQuery instance");
            return null;
         }

         // at this point we have raycast to a series of different objects bounding boxes.
         // we need to test these different objects to see which is the first polygon hit.
         // there are some minor optimizations (distance based) that mean we wont have to
         // check all of the objects most of the time, but the worst case scenario is that
         // we need to test every triangle of every object.
         // Ogre::Real closest_distance = -1.0f;
         rr.Distance = -1.0f;
         Vector3 closest_result = Vector3.ZERO;
         RaySceneQueryResult query_result = raySceneQuery.GetLastResults();
         for (int qridx = 0; qridx < query_result.Count; qridx++)
         {
            // stop checking if we have found a raycast hit that is closer
            // than all remaining entities
            if ((rr.Distance >= 0.0f) && (rr.Distance < query_result[qridx].distance))
            {
               break;
            }

            // only check this result if its a hit against an entity
            if ((query_result[qridx].movable != null)  &&
                (query_result[qridx].movable.MovableType.CompareTo("Entity") == 0))
            {
               // get the entity to check
               Entity pentity = (Entity)query_result[qridx].movable;
               
               // mesh data to retrieve
               uint vertex_count;
               uint index_count;
               Vector3[] vertices;
               ulong[] indices;

               // get the mesh information
               GetMeshInformation(
                  pentity.GetMesh(),
                  out vertex_count,
                  out vertices,
                  out index_count,
                  out indices,
                  pentity.ParentNode._getDerivedPosition(),
                  pentity.ParentNode._getDerivedOrientation(),
                  pentity.ParentNode.GetScale());

               // test for hitting individual triangles on the mesh
               bool new_closest_found = false;
               for (int i = 0; i < (int)index_count; i += 3)
               {
                  // check for a hit against this triangle
                  Pair<bool, float> hit = Mogre.Math.Intersects(ray, vertices[indices[i]], vertices[indices[i + 1]], vertices[indices[i + 2]], true, false);

                  // if it was a hit check if its the closest
                  if (hit.first)
                  {
                     if ((rr.Distance < 0.0f) ||
                         (hit.second < rr.Distance))
                     {
                        // this is the closest so far, save it off
                        rr.Distance = hit.second;
                        new_closest_found = true;
                     }
                  }
               }
               
               // if we found a new closest raycast for this object, update the
               // closest_result before moving on to the next object.
               if (new_closest_found)
               {
                  rr.Target = pentity;
                  closest_result = ray.GetPoint(rr.Distance);
               }
            }
         }
         
         this.sceneMgr.DestroyQuery(raySceneQuery);
         raySceneQuery.Dispose();
         
         // return the result
         if (rr.Distance >= 0.0f)
         {
            // raycast success
            rr.Position = closest_result;
            return rr;
         }
         else
         {
            return null;
         }
      }

      public RaycastResult RaycastFromCamera(RenderWindow window, Camera camera, Vector2 point, uint queryMask)
      {
         // Create the ray to test
         float tx = (float)point.x / (float)window.Width;
         float ty = (float)point.y / (float)window.Height;
         Ray ray = camera.GetCameraToViewportRay(tx, ty);

         return this.Raycast(ray, queryMask);
      }

      public RaycastResult RaycastFromPoint(Vector3 origin, Vector3 direction, uint queryMask)
      {
         // create the ray to test
         Ray ray = new Ray();
         ray.Origin = origin;
         ray.Direction = direction;

         return this.Raycast(ray, queryMask);
      }

      // Get the mesh information for the given mesh.
      // Code found on this forum link: http://www.ogre3d.org/wiki/index.php/RetrieveVertexData
      private static unsafe void GetMeshInformation(MeshPtr mesh, out uint vertex_count, out Vector3[] vertices, out uint index_count, out ulong[] indices, Vector3 position, Quaternion orient, Vector3 scale)
      {
         bool added_shared = false;
         uint current_offset = 0;
         uint shared_offset = 0;
         uint next_offset = 0;
         uint index_offset = 0;

         vertex_count = index_count = 0;

         // Calculate how many vertices and indices we're going to need
         for (ushort i = 0; i < mesh.NumSubMeshes; ++i)
         {
            SubMesh submesh = mesh.GetSubMesh(i);

            // We only need to add the shared vertices once
            if (submesh.useSharedVertices)
            {
               if (!added_shared)
               {
                  vertex_count += mesh.sharedVertexData.vertexCount;
                  added_shared = true;
               }
            }
            else
            {
               vertex_count += submesh.vertexData.vertexCount;
            }

            // Add the indices
            index_count += submesh.indexData.indexCount;
         }

         // Allocate space for the vertices and indices
         vertices = new Vector3[vertex_count];
         indices = new ulong[index_count];

         added_shared = false;

         // Run through the submeshes again, adding the data into the arrays
         for (ushort i = 0; i < mesh.NumSubMeshes; ++i)
         {
            SubMesh submesh = mesh.GetSubMesh(i);

            VertexData vertex_data = submesh.useSharedVertices ? mesh.sharedVertexData : submesh.vertexData;

            if ((!submesh.useSharedVertices) || (submesh.useSharedVertices && !added_shared))
            {
               if (submesh.useSharedVertices)
               {
                  added_shared = true;
                  shared_offset = current_offset;
               }

               VertexElement posElem = vertex_data.vertexDeclaration.FindElementBySemantic(VertexElementSemantic.VES_POSITION);

               HardwareVertexBufferSharedPtr vbuf = vertex_data.vertexBufferBinding.GetBuffer(posElem.Source);
               
               byte* vertex = (byte*)vbuf.Lock(HardwareBuffer.LockOptions.HBL_READ_ONLY);
               
               // There is _no_ baseVertexPointerToElement() which takes an Ogre::Real or a double
               //  as second argument. So make it float, to avoid trouble when Ogre::Real will
               //  be comiled/typedefed as double:
               //      Ogre::Real* pReal;
               float* preal;

               for (uint j = 0; j < vertex_data.vertexCount; ++j, vertex += vbuf.VertexSize)
               {
                  posElem.BaseVertexPointerToElement(vertex, &preal);
                  Vector3 pt = new Vector3(preal[0], preal[1], preal[2]);

                  vertices[current_offset + j] = (orient * (pt * scale)) + position;
               }

               vbuf.Unlock();
               next_offset += vertex_data.vertexCount;
            }

            IndexData index_data = submesh.indexData;
            uint numTris = index_data.indexCount / 3;
            HardwareIndexBufferSharedPtr ibuf = index_data.indexBuffer;

            bool use32bitindexes = (ibuf.Type == HardwareIndexBuffer.IndexType.IT_32BIT);

            ulong* plong = (ulong*)ibuf.Lock(HardwareBuffer.LockOptions.HBL_READ_ONLY);
            ushort* pshort = (ushort*)plong;
            uint offset = submesh.useSharedVertices ? shared_offset : current_offset;

            if (use32bitindexes)
            {
               for (uint k = 0; k < numTris * 3; ++k)
               {
                  indices[index_offset++] = (ulong)plong[k] + (ulong)offset;
               }
            }
            else
            {
               for (uint k = 0; k < numTris * 3; ++k)
               {
                  indices[index_offset++] = pshort[k] + (ulong)offset;
               }
            }

            ibuf.Unlock();
            current_offset = next_offset;
         }
      }
      
      public class RaycastResult
      {
         public Vector3 Position { get; set; }
         
         public Entity Target { get; set; }
         
         public float Distance { get; set; }
      }
   }
}
