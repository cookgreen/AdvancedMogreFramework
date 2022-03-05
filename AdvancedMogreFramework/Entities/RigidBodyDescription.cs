using Mogre;
using Mogre.PhysX;

namespace AdvancedMogreFramework.Entities
{
    public class RigidBodyDescription
    {
        private float angularDamping;
        private Vector3 angularVelocity;
        private float ccdMotionThreshold;
        private ContactPairFlags contactReportFlags;
        private float contactReportThreshold;
        private float density;
        private uint dominanceGroup;
        private uint forceFieldMaterial;
        private uint group;
        private float linearDamping;
        private Vector3 linearVelocity;
        private float mass;
        private Matrix4 massLocalPose;
        private Vector3 massSpaceInertia;
        private float maxAngularVelocity;
        private string name;
        private float sleepAngularVelocity;
        private float sleepDamping;
        private float sleepEnergyThreshold;
        private float sleepLinearVelocity;
        private uint solverIterationCount;
        private float wakeUpCounter;

        public ContactPairFlags ContactReportFlags
        {
            get
            {
                return contactReportFlags;
            }

            set
            {
                contactReportFlags = value;
            }
        }
        public float Density
        {
            get
            {
                return density;
            }

            set
            {
                density = value;
            }
        }
        public uint DominanceGroup
        {
            get
            {
                return dominanceGroup;
            }

            set
            {
                dominanceGroup = value;
            }
        }
        public uint ForceFieldMaterial
        {
            get
            {
                return forceFieldMaterial;
            }

            set
            {
                forceFieldMaterial = value;
            }
        }
        public uint Group
        {
            get
            {
                return group;
            }

            set
            {
                group = value;
            }
        }
        public float AngularDamping
        {
            get
            {
                return angularDamping;
            }

            set
            {
                angularDamping = value;
            }
        }
        public Vector3 AngularVelocity
        {
            get
            {
                return angularVelocity;
            }

            set
            {
                angularVelocity = value;
            }
        }
        public float CCDMotionThreshold
        {
            get
            {
                return ccdMotionThreshold;
            }

            set
            {
                ccdMotionThreshold = value;
            }
        }
        public float ContactReportThreshold
        {
            get
            {
                return contactReportThreshold;
            }

            set
            {
                contactReportThreshold = value;
            }
        }
        public float LinearDamping
        {
            get
            {
                return linearDamping;
            }

            set
            {
                linearDamping = value;
            }
        }
        public Vector3 LinearVelocity
        {
            get
            {
                return linearVelocity;
            }

            set
            {
                linearVelocity = value;
            }
        }
        public float Mass
        {
            get
            {
                return mass;
            }

            set
            {
                mass = value;
            }
        }
        public Matrix4 MassLocalPose
        {
            get
            {
                return massLocalPose;
            }

            set
            {
                massLocalPose = value;
            }
        }
        public Vector3 MassSpaceInertia
        {
            get
            {
                return massSpaceInertia;
            }

            set
            {
                massSpaceInertia = value;
            }
        }
        public float MaxAngularVelocity
        {
            get
            {
                return maxAngularVelocity;
            }

            set
            {
                maxAngularVelocity = value;
            }
        }
        public float SleepAngularVelocity
        {
            get
            {
                return sleepAngularVelocity;
            }

            set
            {
                sleepAngularVelocity = value;
            }
        }
        public float SleepDamping
        {
            get
            {
                return sleepDamping;
            }

            set
            {
                sleepDamping = value;
            }
        }
        public float SleepEnergyThreshold
        {
            get
            {
                return sleepEnergyThreshold;
            }

            set
            {
                sleepEnergyThreshold = value;
            }
        }
        public float SleepLinearVelocity
        {
            get
            {
                return sleepLinearVelocity;
            }

            set
            {
                sleepLinearVelocity = value;
            }
        }
        public uint SolverIterationCount
        {
            get
            {
                return solverIterationCount;
            }

            set
            {
                solverIterationCount = value;
            }
        }
        public float WakeUpCounter
        {
            get
            {
                return wakeUpCounter;
            }

            set
            {
                wakeUpCounter = value;
            }
        }
        public string Name
        {
            get
            {
                return name;
            }

            set
            {
                name = value;
            }
        }
        public RigidBodyDescription()
        {
            angularDamping = 0.05f;
            linearDamping = 0;
            mass = 0;
            maxAngularVelocity = -1;
            sleepAngularVelocity = -1;
            sleepDamping = 0;
            ccdMotionThreshold = 0;
            massLocalPose = Matrix4.IDENTITY;
            solverIterationCount = 4;
        }
        public void ToNxActor(ref ActorDesc actorDesc)
        {
            actorDesc.ContactReportFlags = contactReportFlags;
            actorDesc.Density = density;
            actorDesc.DominanceGroup = dominanceGroup;
            actorDesc.ForceFieldMaterial = forceFieldMaterial;
            actorDesc.Group = group;
            actorDesc.Name = name;
        }

        public void ToNxActor(ref ActorDesc actorDesc, ref BodyDesc bodyDesc)
        {
            ToNxActor(ref actorDesc);
            
            bodyDesc.AngularDamping = angularDamping;
            bodyDesc.AngularVelocity = angularVelocity;
            bodyDesc.CCDMotionThreshold = ccdMotionThreshold;
            bodyDesc.LinearDamping = linearDamping;
            bodyDesc.LinearVelocity = linearVelocity;
            bodyDesc.Mass = mass;
            bodyDesc.MassLocalPose = massLocalPose;
            bodyDesc.MassSpaceInertia = massSpaceInertia;
            bodyDesc.MaxAngularVelocity = maxAngularVelocity;
            bodyDesc.SleepAngularVelocity = sleepAngularVelocity;
            bodyDesc.SleepDamping = sleepDamping;
            bodyDesc.SleepEnergyThreshold = sleepEnergyThreshold;
            bodyDesc.SleepLinearVelocity = sleepLinearVelocity;
            bodyDesc.SolverIterationCount = solverIterationCount;
            bodyDesc.WakeUpCounter = wakeUpCounter;
        }
    }
}