using System;
using UnityEngine;

namespace VMUVUnityPlugin_NET35_v100
{
    class DEV2VRMotionFusion
    {
        private float currentTranslation, currentStraffe;
        private Int16 thresholdValueInCnts;
        private int[] currentFwdIndicies, currentRevIndicies;
        private int currentLeftStraffe, currentRightStraffe;
        private bool isMoving;

        public DEV2VRMotionFusion()
        {
            currentTranslation = 0;
            currentStraffe = 0;
            thresholdValueInCnts = 750;
            isMoving = false;
        }

        public float GetTranslation()
        {
            return currentTranslation;
        }

        public float GetStraffe()
        {
            return currentStraffe;
        }

        public void CalculateTranslationAndStaffe(Int16[] pads, Vector3 lH, Vector3 rH)
        {
            PadQuadrants qLH, qRH;

            qLH = DetermineCurrentHandQuadrant(lH);
            qRH = DetermineCurrentHandQuadrant(rH);

            if (!isMoving)
            {
                currentFwdIndicies = AssignPadsToForward(qLH, qRH);
                currentLeftStraffe = AssignPadToLeftStaffe(qLH, qRH);
                currentRightStraffe = AssignPadToRightStaffe(qLH, qRH);
                currentRevIndicies = AssignPadToRevIndicies(currentFwdIndicies);
            }

            CalculateCurrentTranslation(currentFwdIndicies, currentRevIndicies, pads);
            CalculateCurrentStraffe(currentLeftStraffe, currentRightStraffe, pads);

            if (!IsCenterPadActive(pads))
            {
                currentTranslation = currentStraffe = 0.0f;
                isMoving = false;
            }
        }

        private PadQuadrants DetermineCurrentHandQuadrant(Vector3 hC)
        {
            PadQuadrants quad = PadQuadrants.quadPosZAxis;
            float xPos = hC.x;
            float zPos = hC.z;

            if (xPos > 0.0)
            {
                if (zPos > 0.0)
                    quad = PadQuadrants.quad1;
                else if (zPos < 0.0)
                    quad = PadQuadrants.quad2;
                else
                    quad = PadQuadrants.quadPosXAxis;
            }
            else if (xPos < 0.0)
            {
                if (zPos > 0.0)
                    quad = PadQuadrants.quad4;
                else if (zPos < 0.0)
                    quad = PadQuadrants.quad3;
                else
                    quad = PadQuadrants.quadNegXAxis;
            }
            else
            {
                if (zPos >= 0.0)
                    quad = PadQuadrants.quadPosZAxis;
                if (zPos < 0.0)
                    quad = PadQuadrants.quadNegZAxis;
            }

            return quad;
        }

        private bool IsCenterPadActive(Int16[] pads)
        {
            return (pads[8] < 2500);
        }

        private int[] AssignPadsToForward(PadQuadrants qLH, PadQuadrants qRH)
        {
            int[] fwPadIndicies = new int[3];

            if (IsHandInTopLeft(qLH) && IsHandInTopRight(qRH))
            {
                fwPadIndicies[0] = 3;
                fwPadIndicies[1] = 4;
                fwPadIndicies[2] = 5;
            }
            else if (IsHandInSideLeft(qLH) && IsHandInTopLeft(qRH))
            {
                fwPadIndicies[0] = 2;
                fwPadIndicies[1] = 3;
                fwPadIndicies[2] = 4;
            }
            else if (IsHandInSideLeft(qLH) && IsHandInSideLeft(qRH))
            {
                fwPadIndicies[0] = 1;
                fwPadIndicies[1] = 2;
                fwPadIndicies[2] = 3;
            }
            else if (IsHandInBottomLeft(qLH) && IsHandInSideLeft(qRH))
            {
                fwPadIndicies[0] = 0;
                fwPadIndicies[1] = 1;
                fwPadIndicies[2] = 2;
            }
            else if (IsHandInBottomRight(qLH) && IsHandInBottomLeft(qRH))
            {
                fwPadIndicies[0] = 7;
                fwPadIndicies[1] = 0;
                fwPadIndicies[2] = 1;
            }
            else if (IsHandInSideRight(qLH) && IsHandInBottomRight(qRH))
            {
                fwPadIndicies[0] = 6;
                fwPadIndicies[1] = 7;
                fwPadIndicies[2] = 0;
            }
            else if (IsHandInSideRight(qLH) && IsHandInSideRight(qRH))
            {
                fwPadIndicies[0] = 5;
                fwPadIndicies[1] = 6;
                fwPadIndicies[2] = 7;
            }
            else if (IsHandInTopRight(qLH) && IsHandInSideRight(qRH))
            {
                fwPadIndicies[0] = 4;
                fwPadIndicies[1] = 5;
                fwPadIndicies[2] = 6;
            }

            return fwPadIndicies;
        }

        private int AssignPadToLeftStaffe(PadQuadrants qLH, PadQuadrants qRH)
        {
            int leftStraffe = 0;

            if (IsHandInTopLeft(qLH) && IsHandInTopRight(qRH))
            {
                leftStraffe = 2;
            }
            else if (IsHandInSideLeft(qLH) && IsHandInTopLeft(qRH))
            {
                leftStraffe = 1;
            }
            else if (IsHandInSideLeft(qLH) && IsHandInSideLeft(qRH))
            {
                leftStraffe = 0;
            }
            else if (IsHandInBottomLeft(qLH) && IsHandInSideLeft(qRH))
            {
                leftStraffe = 7;
            }
            else if (IsHandInBottomRight(qLH) && IsHandInBottomLeft(qRH))
            {
                leftStraffe = 6;
            }
            else if (IsHandInSideRight(qLH) && IsHandInBottomRight(qRH))
            {
                leftStraffe = 5;
            }
            else if (IsHandInSideRight(qLH) && IsHandInSideRight(qRH))
            {
                leftStraffe = 4;
            }
            else if (IsHandInTopRight(qLH) && IsHandInSideRight(qRH))
            {
                leftStraffe = 3;
            }

            return leftStraffe;
        }

        private int AssignPadToRightStaffe(PadQuadrants qLH, PadQuadrants qRH)
        {
            int leftStraffe = 0;

            if (IsHandInTopLeft(qLH) && IsHandInTopRight(qRH))
            {
                leftStraffe = 6;
            }
            else if (IsHandInSideLeft(qLH) && IsHandInTopLeft(qRH))
            {
                leftStraffe = 5;
            }
            else if (IsHandInSideLeft(qLH) && IsHandInSideLeft(qRH))
            {
                leftStraffe = 4;
            }
            else if (IsHandInBottomLeft(qLH) && IsHandInSideLeft(qRH))
            {
                leftStraffe = 3;
            }
            else if (IsHandInBottomRight(qLH) && IsHandInBottomLeft(qRH))
            {
                leftStraffe = 2;
            }
            else if (IsHandInSideRight(qLH) && IsHandInBottomRight(qRH))
            {
                leftStraffe = 1;
            }
            else if (IsHandInSideRight(qLH) && IsHandInSideRight(qRH))
            {
                leftStraffe = 0;
            }
            else if (IsHandInTopRight(qLH) && IsHandInSideRight(qRH))
            {
                leftStraffe = 7;
            }

            return leftStraffe;
        }

        private int[] AssignPadToRevIndicies(int[] fwdInd)
        {
            int[] revInd = new int[3];

            for (int i = 0; i < fwdInd.Length; i++)
            {
                revInd[i] = fwdInd[i] + 4;
                if (revInd[i] > 7)
                    revInd[i] -= 8;
            }

            return revInd;
        }

        private void CalculateCurrentTranslation(int[] fwdIndicies, int[] revIndicies, Int16[] padValues)
        {
            for (int i = 0; i < fwdIndicies.Length; i++)
            {
                if (padValues[fwdIndicies[i]] < thresholdValueInCnts)
                {
                    currentTranslation = 0.5f;
                    isMoving = true;
                    break;
                }
                else if (padValues[revIndicies[i]] < thresholdValueInCnts)
                {
                    currentTranslation = -0.5f;
                    isMoving = true;
                    break;
                }
                else
                {
                    currentTranslation = 0;
                    isMoving = false;
                }
            }
        }

        private void CalculateCurrentStraffe(int leftStraffeIndex, int rightStraffeIndex, Int16[] padValues)
        {
            if (padValues[leftStraffeIndex] < thresholdValueInCnts)
            {
                currentStraffe = -0.5f;
            }
            else if (padValues[rightStraffeIndex] < thresholdValueInCnts)
            {
                currentStraffe = 0.5f;
            }
            else
            {
                currentStraffe = 0;
            }
        }

        private bool IsHandInTopLeft(PadQuadrants hand)
        {
            return ((hand == PadQuadrants.quadPosZAxis) || (hand == PadQuadrants.quad4));
        }

        private bool IsHandInTopRight(PadQuadrants hand)
        {
            return ((hand == PadQuadrants.quadPosZAxis) || (hand == PadQuadrants.quad1));
        }

        private bool IsHandInSideLeft(PadQuadrants hand)
        {
            return ((hand == PadQuadrants.quadNegXAxis) || (hand == PadQuadrants.quad3) || (hand == PadQuadrants.quad4));
        }

        private bool IsHandInBottomLeft(PadQuadrants hand)
        {
            return ((hand == PadQuadrants.quadNegZAxis) || (hand == PadQuadrants.quad3));
        }

        private bool IsHandInBottomRight(PadQuadrants hand)
        {
            return ((hand == PadQuadrants.quadNegZAxis) || (hand == PadQuadrants.quad2));
        }

        private bool IsHandInSideRight(PadQuadrants hand)
        {
            return ((hand == PadQuadrants.quadPosXAxis) || (hand == PadQuadrants.quad2) || (hand == PadQuadrants.quad1));
        }

        private enum PadQuadrants
        {
            quadPosZAxis = 0,
            quad1 = 1,
            quadPosXAxis = 2,
            quad2 = 3,
            quadNegZAxis = 4,
            quad3 = 5,
            quadNegXAxis = 6,
            quad4 = 7
        }
    }
}
