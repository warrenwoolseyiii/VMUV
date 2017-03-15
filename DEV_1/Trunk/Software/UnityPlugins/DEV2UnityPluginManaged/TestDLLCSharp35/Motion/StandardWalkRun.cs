using VMUVUnityPlugin_NET35_v100.DEV2_Hardware_Specific;

namespace VMUVUnityPlugin_NET35_v100.Motion
{
    static class StandardWalkRun
    {
        private static float translation = 0.0f;
        private static float straffe = 0.0f;
        private static DEV2Platform platform = CurrentValueTable.GetCurrentPlatform();
        private static MotionStates motionSate = MotionStates.no_motion;
        private static int currentNumActivePads = 0;
        private static ushort[] currentActivePadIds = new ushort[8];

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
                    HandleNoMotion();
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
            }
        }

        private static void HandleNoMotion()
        {
            translation = 0;
            straffe = 0;

            if (!ScreenForActivity())
                return;

            ushort[] activePadIds = platform.GetActivePadIds();
            int numHits = GetNumActivePadsUserIsOver(activePadIds);

            if (numHits < 1)
            {
                currentNumActivePads = activePadIds.Length - 1;
                currentActivePadIds = activePadIds;

                if (CheckForStraffe(GetIdOfPadUserIsOver()))
                    return;   
            }
            else
            {
                if ((!DEV2SepcificUtilities.ArePadIdsConsecutive(activePadIds, numHits)) && (numHits > 1))
                    return;

                currentNumActivePads = 3;
                currentActivePadIds = DEV2SepcificUtilities.GetAdjacentPadIds(activePadIds[0]);
                motionSate = MotionStates.forward;
            }
        }

        private static void HandleForward()
        {
            if (!ScreenForActivity())
            {
                EndMotion();
                return;
            }

            if (AreActivePadIdsSameAsCurrent())
            {
                translation = 1.0f;
            }
            else
            {
                translation = 0;
            }
        }

        private static void HandleStraffe(int dir)
        {
            if (!ScreenForActivity())
            {
                EndMotion();
                return;
            }

            if (AreActivePadIdsSameAsCurrent())
            {
                int numHits = GetNumActivePadsUserIsOver(currentActivePadIds);

                if (numHits < 1)
                {
                    straffe = 1.0f * dir;
                }
                else
                {
                    straffe = 0;
                    currentNumActivePads = 3;
                    currentActivePadIds = DEV2SepcificUtilities.GetAdjacentPadIds(currentActivePadIds[0]);
                    motionSate = MotionStates.forward;
                }
            }
            else
            {
                straffe = 0;
            }
        }

        private static void EndMotion()
        {
            motionSate = MotionStates.no_motion;
        }

        private static bool ScreenForActivity()
        {
            if (!platform.IsCenterActive() || IsOnlyCenterActive())
                return false;

            return true;
        }

        private static bool AreActivePadIdsSameAsCurrent()
        {
            ushort[] activePadIds = platform.GetActivePadIds();
            int numActivePads = platform.GetNumActivePads() - 1;

            for (int i = 0; i < currentNumActivePads; i++)
            {
                for (int j = 0; j < numActivePads; j++)
                {
                    if (activePadIds[j] == currentActivePadIds[i])
                        return true;
                }
            }

            return false;
        }

        private static bool IsOnlyCenterActive()
        {
            ushort[] activePadIds = platform.GetActivePadIds();
            return (activePadIds[0] == 8);
        }

        private static int GetNumActivePadsUserIsOver(ushort[] pads)
        {
            int rtn = 0;

            for (int i = 0; i < (pads.Length - 1); i++)
            {
                if (DEV2SepcificUtilities.IsUserOverPad(platform.GetPadCoordinateById(pads[i])))
                    rtn++;
            }

            return rtn;
        }

        private static ushort GetIdOfPadUserIsOver()
        {
            for (ushort i = 0; i < 8; i++)
            {
                if (DEV2SepcificUtilities.IsUserOverPad(platform.GetPadCoordinateById(i)))
                    return i;
            }

            return 8;
        }

        private static bool CheckForStraffe(ushort padUserIsOver)
        {
            ushort deltaCCW, deltaCW;

            deltaCCW = DEV2SepcificUtilities.CalculatePadIdDeltaCCW(padUserIsOver, currentActivePadIds[0]);
            deltaCW = DEV2SepcificUtilities.CalculatePadIdDeltaCW(padUserIsOver, currentActivePadIds[0]);

            if (deltaCCW == 2 || deltaCCW == 1)
            {
                motionSate = MotionStates.straffe_left;
                return true;
            }
            else if (deltaCW == 2 || deltaCW == 1)
            {
                motionSate = MotionStates.straffe_right;
                return true;
            }
            else
            {
                return false;
            }
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
