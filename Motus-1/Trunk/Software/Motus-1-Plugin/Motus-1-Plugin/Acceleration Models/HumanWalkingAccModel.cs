using System;
using UnityEngine;

namespace Motus_1_Plugin.Acceleration_Models
{
    public class HumanWalkingAccModel
    {
        private AccModelStates currentState = AccModelStates.state_constant_vel;
        private Vector3 previousXZVector = new Vector3(0, 0, 0);
        private int currStep = 0;
        private int _accRate = 1;
        private int _decRate = 1;

        /// <summary>
        /// Set the acceleration and deceleration rates via the non default constructor. accRate and decRate can be set to values ranging
        /// from 1 to 90, where 1 being the slowest and 90 being the fastest.
        /// </summary>
        /// <param name="accRate"></param>
        /// <param name="decRate"></param>
        public HumanWalkingAccModel(int accRate, int decRate)
        {
            if (accRate < 1)
                accRate = 1;

            if (decRate < 1)
                decRate = 1;

            _accRate = accRate;
            _decRate = decRate;
        }

        /// <summary>
        /// Runs the acceleration model, simply pass the motus-1 xz vector which has been previously rotated (see ExampleMovementScript.cs) as the argument
        /// into this method and it will return the accelerated or decelerated value.
        /// </summary>
        /// <param name="currentVector"></param>
        /// <returns></returns>
        public Vector3 RunModel(Vector3 currentVector)
        {
            DetermineNextState(currentVector);

            switch (currentState)
            {
                case AccModelStates.state_acc:
                    previousXZVector = RunAccModel(currentVector);
                    return previousXZVector;
                case AccModelStates.state_dec:
                    previousXZVector = RunDecModel(previousXZVector);
                    return previousXZVector;
                default:
                    return currentVector;
            }
        }

        private void DetermineNextState(Vector3 xz)
        {
            float prevMag = previousXZVector.magnitude;
            float currMag = xz.magnitude;

            if (prevMag != 0)
            {
                // launch decelleration if we have gone from none zero to zero
                if (currMag == 0)
                    currentState = AccModelStates.state_dec;
                else if (currentState != AccModelStates.state_constant_vel)
                    currentState = AccModelStates.state_acc;
            }
            else
            {
                // launch accelleration if we have gone from zero to none zero
                if (currMag != 0)
                    currentState = AccModelStates.state_acc;
            }
        }

        private Vector3 RunAccModel(Vector3 xz)
        {
            if (currStep >= 90)
            {
                currStep = 90;
                currentState = AccModelStates.state_constant_vel;
                return xz;
            }
            else
            {
                currStep += _accRate;
                float rad = (float)(currStep * Math.PI / 180.0);
                float multiplyer = (float)Math.Sin(rad);

                xz.x *= multiplyer;
                xz.z *= multiplyer;

                return xz;
            }
        }

        private Vector3 RunDecModel(Vector3 xz)
        {
            if (currStep <= 0)
            {
                currStep = 0;
                currentState = AccModelStates.state_constant_vel;
                return new Vector3(0, 0, 0);
            }
            else
            {
                currStep -= _decRate;
                float rad = (float)(currStep * Math.PI / 180.0);
                float multiplyer = (float)Math.Sin(rad);

                xz.x *= multiplyer;
                xz.z *= multiplyer;

                return xz;
            }
        }

        private enum AccModelStates
        {
            state_acc,
            state_dec,
            state_constant_vel
        };
    }
}
