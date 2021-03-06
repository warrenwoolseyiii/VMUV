﻿using VMUVUnityPlugin_NET35_v100.DEV2_Hardware_Specific;

namespace VMUVUnityPlugin_NET35_v100.Motion
{
    static class StandardWalkRun
    {
        private static float translation = 0.0f;
        private static float straffe = 0.0f;
        private static float radius = 0.40f;
        private static bool strafeEnabled = false;
        private static DEV2Platform platform = CurrentValueTable.GetCurrentPlatform();
        private static MotionStates motionSate = MotionStates.no_motion;

        public static void SetNewDrawRadius(float rad)
        {
            radius = rad;
        }

        public static void EnableStrafe(bool enableStrafe)
        {
            strafeEnabled = enableStrafe;
        }

        public static float GetTranslation()
        {
            return translation;
        }

        public static float GetStraffe()
        {
            return straffe;
        }

        public static void CalculateTranslationAndStraffe()
        {
            if (!DEV2Calibrator.calibrationComplete)
                return;

            WalkRunStateMachine();
        }

        private static void WalkRunStateMachine()
        {
            switch (motionSate)
            {
                case MotionStates.no_motion:
                    LookForMotion();
                    break;
                case MotionStates.forward:
                    HandleForward();
                    break;
                case MotionStates.straffe_left:
                    HandleStraffe(-1);
                    break;
                case MotionStates.straffe_right:
                    HandleStraffe(1);
                    break;
                case MotionStates.backward:
                    HandleReverse();
                    break;
            }
        }

        private static void LookForMotion()
        {
            translation = 0;
            straffe = 0;

            if (!ScreenForActivity())
            {
                return;
            }
            else if (!strafeEnabled)
            {
                motionSate = MotionStates.forward;
                return;
            }

            int numHits = GetNumActivePadsUserIsOverExcludingCenter(platform.GetActivePadIds());
            if (numHits >= 1)
            {
                motionSate = MotionStates.forward;
            }
            else
            {
                ushort[] padId = GetIdOfPadUserIsOver();
                if (padId == null)
                {
                    motionSate = MotionStates.backward;
                    return;
                }

                CheckForStrafeAndReverse(padId);
            }
        }

        private static void HandleForward()
        {
            if (!ScreenForActivity())
            {
                EndMotion();
                return;
            }

            translation = GetLargestActivePadValue();
            //translation = SumActivePads();
        }

        private static void HandleStraffe(int dir)
        {
            if (!ScreenForActivity())
            {
                EndMotion();
                return;
            }

            straffe = GetLargestActivePadValue() * dir;
            //straffe = SumActivePads() * dir * 2/3;
        }

        private static void HandleReverse()
        {
            if (!ScreenForActivity())
            {
                EndMotion();
                return;
            }

            translation = GetLargestActivePadValue() * -1;
            //translation = SumActivePads() * -1;
        }

        private static void EndMotion()
        {
            motionSate = MotionStates.no_motion;
        }

        public static bool ScreenForActivity()
        {
            if (!platform.IsCenterActive() || IsOnlyCenterActive())
                return false;

            return true;
        }

        private static bool IsOnlyCenterActive()
        {
            ushort[] activePadIds = platform.GetActivePadIds();
            return (activePadIds[0] == 8);
        }

        private static int GetNumActivePadsUserIsOverExcludingCenter(ushort[] pads)
        {
            int rtn = 0;

            for (int i = 0; i < (pads.Length - 1); i++)
            {
                if (DEV2SepcificUtilities.IsUserOverPad((platform.GetPadById(pads[i])).coordinate, radius))
                    rtn++;
            }

            return rtn;
        }

        private static ushort[] GetIdOfPadUserIsOver()
        {
            int numPads = 0;

            for (ushort i = 0; i < 8; i++)
            {
                if (DEV2SepcificUtilities.IsUserOverPad((platform.GetPadById(i)).coordinate, radius))
                    numPads++;
            }

            if (numPads > 0)
            {
                ushort[] ids = new ushort[numPads];
                int ndx = 0;

                for (ushort i = 0; i < 8; i++)
                {
                    if (DEV2SepcificUtilities.IsUserOverPad((platform.GetPadById(i)).coordinate, radius))
                        ids[ndx++] = i;
                }

                return ids;
            }

            return null;
        }

        private static void CheckForStrafeAndReverse(ushort[] padUserIsOver)
        {
            ushort deltaCCW, deltaCW;
            ushort[] activePads = platform.GetActivePadIds();
            ushort[] padsOfInterest = new ushort[(activePads.Length - 1) + padUserIsOver.Length];
            bool isLeft = false;

            ushort[] deltas = DEV2SepcificUtilities.FindMinDistanceBetweenPadSets(padUserIsOver, activePads);

            deltaCCW = deltas[0];
            deltaCW = deltas[1];

            if (deltaCCW < deltaCW)
                isLeft = true;

            // Handle edge case
            if (padUserIsOver.Length > 1 && padUserIsOver[0] == 0 && padUserIsOver[1] == 7)
            {
                padUserIsOver[0] = 7;
                padUserIsOver[1] = 0;
            }

            int i,j;

            if (isLeft)
            {
                for (j = 0; j < (activePads.Length - 1); j++)
                    padsOfInterest[j] = activePads[j];
                for (i = 0; i < padUserIsOver.Length; i++)
                    padsOfInterest[i + j] = padUserIsOver[i];
            }
            else
            {
                for (j = 0; j < padUserIsOver.Length; j++)
                    padsOfInterest[j] = padUserIsOver[j];
                for (i = 0; i < (activePads.Length - 1); i++)
                    padsOfInterest[i + j] = activePads[i];
            }

            if (DEV2SepcificUtilities.ArePadIdsConsecutive(padsOfInterest, padsOfInterest.Length))
            {
                if (isLeft)
                    motionSate = MotionStates.straffe_left;
                else
                    motionSate = MotionStates.straffe_right;
            }
            else if (deltaCCW <= 2 || deltaCW <= 2)
            {
                if (isLeft)
                    motionSate = MotionStates.straffe_left;
                else
                    motionSate = MotionStates.straffe_right;
            }
            else
            {
                motionSate = MotionStates.backward;
            }
        }

        private static float GetLargestActivePadValue()
        {
            DEV2Pad[] activePads = platform.GetActivePads();
            float rtn = 0f;

            for (int i = 0; i < (activePads.Length - 1); i++)
            {
                if (activePads[i].GetPctActive() > rtn)
                    rtn = activePads[i].GetPctActive();
            }

            return rtn;
        }

        private static float SumActivePads()
        {
            DEV2Pad[] activePads = platform.GetActivePads();
            float rtn = 0f;

            for (int i = 0; i < (activePads.Length - 1); i++)
            {
                if (activePads[i].GetPctActive() > rtn)
                    rtn += activePads[i].GetPctActive();
            }

            return rtn;
        }

        enum MotionStates
        {
            no_motion,
            forward,
            straffe_left,
            straffe_right,
            backward
        }
    }
}
