using System;
using Mogre;

namespace AdvancedMogreFramework.Entities
{
    public class SimpleShape
    {
        private Vector3 vector;
        public SimpleShape()
        {

        }
        public SimpleShape(Vector3 vect)
        {
            vector = vect;
        }
        public Vector3 to_cc_shape()
        {
            return vector;
        }
    }
}