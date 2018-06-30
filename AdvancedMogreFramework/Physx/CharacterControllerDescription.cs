namespace AdvancedMogreFramework.PhysX
{
    public class CharacterControllerDescription
    {
        private bool capsuleEasyClimbing;
        private float skinWidth;
        private float slopeLimit;
        private float stepOffset;

        public bool CapsuleEasyClimbing
        {
            get
            {
                return capsuleEasyClimbing;
            }

            set
            {
                capsuleEasyClimbing = value;
            }
        }

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

        public float SlopeLimit
        {
            get
            {
                return slopeLimit;
            }

            set
            {
                slopeLimit = value;
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
    }
}