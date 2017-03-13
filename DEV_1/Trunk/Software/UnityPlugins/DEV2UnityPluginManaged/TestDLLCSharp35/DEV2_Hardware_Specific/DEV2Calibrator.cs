using UnityEngine;
using UnityEngine.VR;

namespace VMUVUnityPlugin_NET35_v100.DEV2_Hardware_Specific
{
    static class DEV2Calibrator
    {
        private static CalibrationStates calState = CalibrationStates.force_ranges;
        private static CoordinateAcquisitionStates coordState = CoordinateAcquisitionStates.get_north;
        private const int positionAccumLimit = 100;
        private static Vector3[] positionAccumVector = new Vector3[positionAccumLimit];
        private static int positionAccumIndex = 0;

        public static void RunCalibration()
        {
            switch (calState)
            {
                case CalibrationStates.force_ranges:
                    if (ForceAllRangesToBeSet())
                    {
                        Logger.LogMessage("Ranges Set!");
                        calState = CalibrationStates.wait_for_center;
                    }
                    break;
                case CalibrationStates.wait_for_center:
                    if (WaitForCenter())
                    {
                        Logger.LogMessage("Center Set!");
                        calState = CalibrationStates.acquire_coordinates;
                    }
                    break;
                case CalibrationStates.acquire_coordinates:
                    ForceCoordinatePositions();
                    break;
            }
        }

        private static bool ForceAllRangesToBeSet()
        {
            DEV2Platform plat = CurrentValueTable.GetCurrentPlatform();
            return plat.IsPlatformInitialized();
        }

        private static bool WaitForCenter()
        {
            DEV2Platform plat = CurrentValueTable.GetCurrentPlatform();

            if (plat.IsCenterActive())
            {
                ushort[] activePads = plat.GetCurrentActivePads();
                if (activePads.Length == 1)
                {
                    Vector3 test = new Vector3(0, 0, 0);
                    return AccumulateCoordinate(test);
                }
            }

            return false;
        }

        private static bool ForceCoordinatePositions()
        {
            switch (coordState)
            {
                case CoordinateAcquisitionStates.get_north:
                    if (GetNorth())
                        coordState = CoordinateAcquisitionStates.get_east;
                    break;
            }

            return false;
        }

        private static bool GetNorth()
        {
            DEV2Platform plat = CurrentValueTable.GetCurrentPlatform();
            ushort[] activePads = plat.GetCurrentActivePads();

            if (plat.IsCenterActive())
            {
                if (activePads.Length == 2)
                {
                    ushort northId = activePads[0];
                    Vector3 test = new Vector3(0.5f, 0.5f, 0.5f);

                    if (AccumulateCoordinate(test))
                    {
                        Logger.LogMessage("North set " + test.ToString() + "\n\rId: " + northId.ToString());
                        plat.SetPlatformCoordinate(DEV2SepcificUtilities.AverageVectors(positionAccumVector), northId);
                        return true;
                    }
                }
            }

            return false;
        }

        private static bool AccumulateCoordinate(Vector3 coord)
        {
            positionAccumVector[positionAccumIndex++] = coord;

            if (positionAccumIndex >= positionAccumLimit)
            {
                positionAccumIndex = 0;
                return true;
            }
            else
            {
                return false;
            }
        }
    }

    enum CalibrationStates
    {
        force_ranges,
        wait_for_center,
        acquire_coordinates
    }

    enum CoordinateAcquisitionStates
    {
        get_north,
        get_east,
        get_south,
        get_west,
        complete
    }
}
