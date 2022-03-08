using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre.PhysX;
using System.IO;

namespace AdvancedMogreFramework.Entities
{
    public class PhysxCommon
    {
        public static void InitCooking()
        {
            CookingInterface.InitCooking();
        }

        public static bool CookClothMesh(ClothMeshDesc meshDesc, Stream stream)
        {
            return CookingInterface.CookClothMesh(meshDesc, stream);
        }

        public static void CloseCooking()
        {
            CookingInterface.CloseCooking();
        }
    }
}
