using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using Mogre.PhysX;
using AdvancedMogreFramework.Helper;

namespace AdvancedMogreFramework.Entities
{
    /// <summary>
    /// Static Game Object
    /// </summary>
    public class SceneProp
    {
        private SceneManager scm;
        private string name;
        private string meshName;
        private Entity ent;
        private SceneNode node;
        private Scene physicsScene;
        private RigidBody rigidBody;
        public Actor Actor
        {
            get
            {
                return rigidBody.Actor;
            }
        }

        public Entity Entity
        {
            get
            {
                return ent;
            }

            set
            {
                ent = value;
            }
        }

        public SceneProp(
            SceneManager scm, 
            SceneNode node, 
            Scene physicsScene,
            string name, 
            string meshName)
        {
            this.scm = scm;
            this.name = name;
            this.meshName = meshName;
            this.node = node;
            this.physicsScene = physicsScene;
            create();
            createPhysics();
        }

        private void create()
        {
            ent = scm.CreateEntity(name, meshName);
            node.AttachObject(ent);
        }

        private void createPhysics()
        {
            rigidBody = new RigidBody();
            RigidBodyDescription description = new RigidBodyDescription();
            description.Density = 4;
            rigidBody.CreateStatic(
                node.Position,
                description, 
                physicsScene, 
                physicsScene.Physics.CreateTriangleMesh(new
                StaticMeshData(ent.GetMesh())));
        }

        public void SetMaterialName(string materialName)
        {
            ent.SetMaterialName(materialName);
        }
    }
}
