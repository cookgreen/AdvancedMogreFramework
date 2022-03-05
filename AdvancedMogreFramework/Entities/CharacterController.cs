using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Mogre;
using Mogre.PhysX;


namespace AdvancedMogreFramework.Entities
{
    public class CharacterController : RigidBody
    {
        private Controller controller;
        private float minDistance;
        private float sharpness;
        private uint activeGroups;
        private uint collisionFlags, previousCollisionFlags;
        private float stepOffset;
        private float skinWidth;
        private CharacterControllerInteractionFlag interactionFlag;
        private Vector3 position;
        private float mininalDistance;
        private float movingSharpness;
        private GroupIdentifier shapeGroup;

        public float SkinWidth
        {
            get
            {
                return skinWidth;
            }

            set
            {
                skinWidth = value;
            }
        }
        public float StepOffset
        {
            get
            {
                return stepOffset;
            }

            set
            {
                stepOffset = value;
            }
        }
        public uint ActiveGroups
        {
            get
            {
                return activeGroups;
            }

            set
            {
                activeGroups = value;
            }
        }
        public CharacterControllerInteractionFlag InteractionFlag
        {
            get
            {
                return interactionFlag;
            }

            set
            {
                interactionFlag = value;
            }
        }
        public Vector3 GlobalPosition
        {
            get
            {
                return position;
            }

            set
            {
                position = value;
            }
        }
        public uint CollisionFlags
        {
            get
            {
                return collisionFlags;
            }

            set
            {
                collisionFlags = value;
            }
        }
        public uint PreviousCollisionFlags
        {
            get
            {
                return previousCollisionFlags;
            }

            set
            {
                previousCollisionFlags = value;
            }
        }
        public float MininalDistance
        {
            get
            {
                return mininalDistance;
            }

            set
            {
                mininalDistance = value;
            }
        }
        public float MovingSharpness
        {
            get
            {
                return movingSharpness;
            }

            set
            {
                movingSharpness = value;
            }
        }
        public GroupIdentifier ShapeGroup
        {
            get
            {
                return shapeGroup;
            }

            set
            {
                shapeGroup = value;
            }
        }

        public float MinDistance
        {
            get
            {
                return minDistance;
            }

            set
            {
                minDistance = value;
            }
        }

        public float Sharpness
        {
            get
            {
                return sharpness;
            }

            set
            {
                sharpness = value;
            }
        }
        public CharacterController()
        {

        }
        public CharacterController(Scene scene)
        {
            Scene = scene;
        }
        public CharacterController(SimpleShape shape, Vector3 globalPosition, CharacterControllerDescription description, Scene scene)
        {
            createCharacterController(globalPosition, shape, description, scene);
        }

        public void createCharacterController(Vector3 globalPos, SimpleShape shape, CharacterControllerDescription description, Scene scene)
        {
            stepOffset = description.StepOffset;
            skinWidth = description.SkinWidth;
            controller = _createCharacterController(globalPos, scene, shape, description);
        }

        public virtual void Move(Vector3 displacement)
        {
            ControllerFlags cfs;
            controller.Move(displacement, activeGroups, mininalDistance, out cfs);
        }

        public virtual uint getRigidBodyType() { return 1; }

        public bool isDynamic() { return true; }

        public bool isCharacterControllerBased() { return true; }

        public void setStepOffset(float offset) { }

        public void setCollisionsEnabled(bool isCollisonEnabled) { }

        public void reportSceneChanged() { }

        public bool isBoxShaped() { return true; }

        public bool isCapsuleShaped() { return true; }

        public bool hasCollidedDown()
        {
            return true;
        }

        public bool hasCollidedSides() { return true; }

        public bool hasCollidedUp() { return true; }

        public bool hasPreviouslyCollidedDown() { return true; }

        public bool hasPreviouslyCollidedSides() { return true; }

        public bool hasPreviouslyCollidedUp() { return true; }

    }
}
