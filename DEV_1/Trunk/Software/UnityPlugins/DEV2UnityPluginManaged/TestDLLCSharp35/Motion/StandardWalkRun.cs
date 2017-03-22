using VMUVUnityPlugin_NET35_v100.DEV2_Hardware_Specific;

namespace VMUVUnityPlugin_NET35_v100.Motion
{
    static class StandardWalkRun
    {
        private static float translation = 0.0f;
        private static float straffe = 0.0f;
        private static float radius = 0.5f;
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
                return;

            int numHits = GetNumActivePadsUserIsOverExcludingCenter(platform.GetActivePadIds());
            if (numHits >= 1)
            {
                motionSate = MotionStates.forward;
            }
            else
            {
                ushort padId = GetIdOfPadUserIsOver();

                if (CheckForStraffe(padId))
                    return;

                if (CheckForReverse(padId))
                    return;
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
        }

        private static void HandleStraffe(int dir)
        {
            if (!ScreenForActivity())
            {
                EndMotion();
                return;
            }

            straffe = GetLargestActivePadValue() * dir;
        }

        private static void HandleReverse()
        {
            if (!ScreenForActivity())
            {
                EndMotion();
                return;
            }

            translation = GetLargestActivePadValue() * -1;
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

        private static ushort GetIdOfPadUserIsOver()
        {
            for (ushort i = 0; i < 8; i++)
            {
                if (DEV2SepcificUtilities.IsUserOverPad((platform.GetPadById(i)).coordinate, radius))
                    return i;
            }

            return 8;
        }

        private static bool CheckForStraffe(ushort padUserIsOver)
        {
            ushort deltaCCW = 8;
            ushort deltaCW = 8;
            ushort[] activePads = platform.GetActivePadIds();

            // Always parse to the nearest neighbor for straffe, this is done because we could have multiple pads active
            for (ushort i = 0; i < (activePads.Length - 1); i++)
            {
                ushort tmp = DEV2SepcificUtilities.CalculatePadIdDeltaCCW(padUserIsOver, activePads[i]);
      
                if (tmp < deltaCCW)
                    deltaCCW = tmp;

                tmp = DEV2SepcificUtilities.CalculatePadIdDeltaCW(padUserIsOver, activePads[i]);

                if (tmp < deltaCW)
                    deltaCW = tmp;
            }

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

        private static bool CheckForReverse(ushort padUserIsOver)
        {
            ushort deltaCCW = 8;
            ushort deltaCW = 8;
            ushort[] activePads = platform.GetActivePadIds();

            // Always parse to the nearest neighbor for reverse, this is done because we could have multiple pads active
            for (ushort i = 0; i < (activePads.Length - 1); i++)
            {
                ushort tmp = DEV2SepcificUtilities.CalculatePadIdDeltaCCW(padUserIsOver, activePads[i]);

                if (tmp < deltaCCW)
                    deltaCCW = tmp;

                tmp = DEV2SepcificUtilities.CalculatePadIdDeltaCW(padUserIsOver, activePads[i]);

                if (tmp < deltaCW)
                    deltaCW = tmp;
            }

            if (deltaCCW == 3 || deltaCCW == 4 || deltaCCW == 5)
            {
                motionSate = MotionStates.backward;
                return true;
            }
            else if (deltaCW == 5 || deltaCW == 4 || deltaCW == 3)
            {
                motionSate = MotionStates.backward;
                return true;
            }
            else
            {
                return false;
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
