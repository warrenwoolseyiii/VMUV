﻿using UnityEngine;
using UnityEngine.VR;

namespace VMUVUnityPlugin_NET35_v100.DEV2_Hardware_Specific
{
    static class DEV2Calibrator
    {
        private static CalibrationStates calState = CalibrationStates.force_ranges;
        private static CoordinateAcquisitionStates coordState = CoordinateAcquisitionStates.complete;
        private const int positionAccumLimit = 100;
        private static Vector3[] positionAccumVector = new Vector3[positionAccumLimit];
        private static int positionAccumIndex = 0;
        private static ushort northId, eastId, southId, westId;
        private static bool idIsSet = false;
        public static bool initialized = false;
        public static bool calibrationComplete = false;
        private static DEV2Platform plat;

        public static void Init()
        {
            plat = CurrentValueTable.GetCurrentPlatform();
            calState = CalibrationStates.force_ranges;
            coordState = CoordinateAcquisitionStates.get_north;
            idIsSet = false;
            initialized = true;
            calibrationComplete = false;
        }

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
                        idIsSet = false;
                        calState = CalibrationStates.acquire_coordinates;
                    }
                    break;
                case CalibrationStates.acquire_coordinates:
                    if (ForceCoordinatePositions())
                    {
                        Logger.LogMessage("Coordinal Positions Set!");
                        calState = CalibrationStates.interpolate_coordinates;
                    }
                    break;
                case CalibrationStates.interpolate_coordinates:
                    InterpolateCoordinates();
                    calState = CalibrationStates.export_calibration_file;
                    break;
                case CalibrationStates.export_calibration_file:
                    calState = CalibrationStates.complete;
                    break;
                case CalibrationStates.complete:
                    plat.SetCalibrationComplete();
                    calibrationComplete = true;
                    break;
            }
        }

        private static bool ForceAllRangesToBeSet()
        {
            return plat.IsPlatformInitialized();
        }

        private static bool WaitForCenter()
        {
            if (plat.IsCenterActive())
            {
                ushort[] activePads = plat.GetActivePadIds();
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
                    if (GetOrdinalCoordinate())
                    {
                        idIsSet = false;
                        coordState = CoordinateAcquisitionStates.get_east;
                    }
                    break;
                case CoordinateAcquisitionStates.get_east:
                    if (GetOrdinalCoordinate())
                    {
                        idIsSet = false;
                        coordState = CoordinateAcquisitionStates.get_south;
                    }
                    break;
                case CoordinateAcquisitionStates.get_south:
                    if (GetOrdinalCoordinate())
                    {
                        idIsSet = false;
                        coordState = CoordinateAcquisitionStates.get_west;
                    }
                    break;
                case CoordinateAcquisitionStates.get_west:
                    if (GetOrdinalCoordinate())
                    {
                        idIsSet = false;
                        coordState = CoordinateAcquisitionStates.complete;
                    }
                    break;
                case CoordinateAcquisitionStates.complete:
                    return true;
            }

            return false;
        }

        private static bool GetOrdinalCoordinate()
        {
            if (plat.IsCenterActive())
            {
                SetId(plat.GetActivePadIds());
                
                if (CheckId(plat.GetActivePadIds()))
                {
                    return SetCoordinate();
                }
            }

            return false;
        }

        private static void InterpolateCoordinates()
        {
            ushort northEastId, southEastId, southWestId, northWestId;
            Vector3 pt;

            northEastId = DEV2SepcificUtilities.HandlePadIDRollOver((short)(northId - 1));
            southEastId = DEV2SepcificUtilities.HandlePadIDRollOver((short)(eastId - 1));
            southWestId = DEV2SepcificUtilities.HandlePadIDRollOver((short)(southId - 1));
            northWestId = DEV2SepcificUtilities.HandlePadIDRollOver((short)(westId - 1));

            pt = DEV2SepcificUtilities.GetMidPointBetweenPoints(plat.GetPadCoordinateById(northId), plat.GetPadCoordinateById(eastId));
            plat.SetPlatformCoordinate(pt, northEastId);
            pt = DEV2SepcificUtilities.GetMidPointBetweenPoints(plat.GetPadCoordinateById(eastId), plat.GetPadCoordinateById(southId));
            plat.SetPlatformCoordinate(pt, southEastId);
            pt = DEV2SepcificUtilities.GetMidPointBetweenPoints(plat.GetPadCoordinateById(southId), plat.GetPadCoordinateById(westId));
            plat.SetPlatformCoordinate(pt, southWestId);
            pt = DEV2SepcificUtilities.GetMidPointBetweenPoints(plat.GetPadCoordinateById(westId), plat.GetPadCoordinateById(northId));
            plat.SetPlatformCoordinate(pt, northWestId);
        }

        private static void SetId(ushort[] activePads)
        {
            if (idIsSet)
                return;

            if (activePads.Length != 2)
                return;

            switch (coordState)
            {
                case CoordinateAcquisitionStates.get_north:
                    northId = activePads[0];
                    break;
                case CoordinateAcquisitionStates.get_east:
                    eastId = DEV2SepcificUtilities.HandlePadIDRollOver((short)(northId - 2));
                    break;
                case CoordinateAcquisitionStates.get_west:
                    westId = DEV2SepcificUtilities.HandlePadIDRollOver((short)(southId - 2));
                    break;
                case CoordinateAcquisitionStates.get_south:
                    southId = DEV2SepcificUtilities.HandlePadIDRollOver((short)(eastId - 2));
                    break;
            }

            idIsSet = true;
        }

        private static bool CheckId(ushort[] activePads)
        {
            if (activePads.Length != 2)
                return false;

            switch (coordState)
            {
                case CoordinateAcquisitionStates.get_north:
                    return (northId == activePads[0]);
                case CoordinateAcquisitionStates.get_east:
                    return (eastId == activePads[0]);
                case CoordinateAcquisitionStates.get_west:
                    return (westId == activePads[0]);
                case CoordinateAcquisitionStates.get_south:
                    return (southId == activePads[0]);
                default:
                    return false;
            }
        }

        private static bool SetCoordinate()
        {
            if (AccumulateCoordinate(DEV2SepcificUtilities.GetHeadHandsFusion()))
            {
                switch (coordState)
                {
                    case CoordinateAcquisitionStates.get_north:
                        plat.SetPlatformCoordinate(DEV2SepcificUtilities.AverageVectors(positionAccumVector), northId);
                        return true;
                    case CoordinateAcquisitionStates.get_east:
                        plat.SetPlatformCoordinate(DEV2SepcificUtilities.AverageVectors(positionAccumVector), eastId);
                        return true;
                    case CoordinateAcquisitionStates.get_west:
                        plat.SetPlatformCoordinate(DEV2SepcificUtilities.AverageVectors(positionAccumVector), westId);
                        return true;
                    case CoordinateAcquisitionStates.get_south:
                        plat.SetPlatformCoordinate(DEV2SepcificUtilities.AverageVectors(positionAccumVector), southId);
                        return true;
                    default:
                        return false;
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
        acquire_coordinates,
        interpolate_coordinates,
        export_calibration_file,
        complete
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
