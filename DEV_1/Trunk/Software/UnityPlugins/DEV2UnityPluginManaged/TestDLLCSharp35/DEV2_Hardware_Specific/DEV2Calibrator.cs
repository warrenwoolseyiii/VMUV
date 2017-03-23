using UnityEngine;
using System.IO;
using System;

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
        private static string calFilePath = Path.Combine(Environment.CurrentDirectory, "calFile.txt");
            //"C:\\Users\\Warren Woolsey\\Desktop\\calFile.txt";

        public static void Init()
        {
            plat = CurrentValueTable.GetCurrentPlatform();
            calState = CalibrationStates.init;
            coordState = CoordinateAcquisitionStates.get_north;
            idIsSet = false;
            initialized = true;
            calibrationComplete = false;
            plat.WipeCalibrationFile();
        }

        public static void RunCalibration()
        {
            switch (calState)
            {
                case CalibrationStates.init:
                    calState = CalibrationStates.init;
                    coordState = CoordinateAcquisitionStates.get_north;
                    idIsSet = false;
                    initialized = true;
                    calibrationComplete = false;
                    plat.WipeCalibrationFile();
                    calState = CalibrationStates.force_ranges;
                    break;
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
                    CreateCalibrationFile();
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
                        Logger.LogMessage("North is set!");
                        idIsSet = false;
                        coordState = CoordinateAcquisitionStates.get_east;
                    }
                    break;
                case CoordinateAcquisitionStates.get_east:
                    if (GetOrdinalCoordinate())
                    {
                        Logger.LogMessage("East is set!");
                        idIsSet = false;
                        coordState = CoordinateAcquisitionStates.get_south;
                    }
                    break;
                case CoordinateAcquisitionStates.get_south:
                    if (GetOrdinalCoordinate())
                    {
                        Logger.LogMessage("South is set!");
                        idIsSet = false;
                        coordState = CoordinateAcquisitionStates.get_west;
                    }
                    break;
                case CoordinateAcquisitionStates.get_west:
                    if (GetOrdinalCoordinate())
                    {
                        Logger.LogMessage("West is set!");
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
            Vector3 pt, center;
            Vector3[] pts = new Vector3[2];
            float offset;

            center = CalculateCenterCoord();
            (plat.GetPadById(8)).coordinate = center;

            offset = CalculatePlatformRadius() * 0.707107f;

            northEastId = DEV2SepcificUtilities.HandlePadIDRollOver((short)(northId - 1));
            southEastId = DEV2SepcificUtilities.HandlePadIDRollOver((short)(eastId - 1));
            southWestId = DEV2SepcificUtilities.HandlePadIDRollOver((short)(southId - 1));
            northWestId = DEV2SepcificUtilities.HandlePadIDRollOver((short)(westId - 1));

            pts[0] = (plat.GetPadById(northId)).coordinate;
            pts[1] = (plat.GetPadById(eastId)).coordinate;
            pt = DEV2SepcificUtilities.CalculateMidPointOnArc(pts, center, offset);
            (plat.GetPadById(northEastId)).coordinate = pt;

            pts[0] = (plat.GetPadById(eastId)).coordinate;
            pts[1] = (plat.GetPadById(southId)).coordinate;
            pt = DEV2SepcificUtilities.CalculateMidPointOnArc(pts, center, offset);
            (plat.GetPadById(southEastId)).coordinate = pt;

            pts[0] = (plat.GetPadById(southId)).coordinate;
            pts[1] = (plat.GetPadById(westId)).coordinate;
            pt = DEV2SepcificUtilities.CalculateMidPointOnArc(pts, center, offset);
            (plat.GetPadById(southWestId)).coordinate = pt;

            pts[0] = (plat.GetPadById(westId)).coordinate;
            pts[1] = (plat.GetPadById(northId)).coordinate;
            pt = DEV2SepcificUtilities.CalculateMidPointOnArc(pts, center, offset);
            (plat.GetPadById(northWestId)).coordinate = pt;
        }

        private static Vector3 CalculateCenterCoord()
        {
            Vector3[] midPoint = new Vector3[2];

            midPoint[0] = DEV2SepcificUtilities.GetMidPointBetweenPoints((plat.GetPadById(northId)).coordinate, (plat.GetPadById(southId)).coordinate);
            midPoint[1] = DEV2SepcificUtilities.GetMidPointBetweenPoints((plat.GetPadById(westId)).coordinate, (plat.GetPadById(eastId)).coordinate);

            return DEV2SepcificUtilities.AverageVectors(midPoint);
        }

        private static float CalculatePlatformRadius()
        {
            float[] diameter = new float[2];

            diameter[0] = DEV2SepcificUtilities.DrawDistanceBetweenPoints((plat.GetPadById(northId)).coordinate, (plat.GetPadById(southId)).coordinate);
            diameter[1] = DEV2SepcificUtilities.DrawDistanceBetweenPoints((plat.GetPadById(westId)).coordinate, (plat.GetPadById(eastId)).coordinate);

            return ((diameter[0] + diameter[1]) / 4.0f);
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
                        (plat.GetPadById(northId)).coordinate = DEV2SepcificUtilities.AverageVectors(positionAccumVector);
                        return true;
                    case CoordinateAcquisitionStates.get_east:
                        (plat.GetPadById(eastId)).coordinate = DEV2SepcificUtilities.AverageVectors(positionAccumVector);
                        return true;
                    case CoordinateAcquisitionStates.get_west:
                        (plat.GetPadById(westId)).coordinate = DEV2SepcificUtilities.AverageVectors(positionAccumVector);
                        return true;
                    case CoordinateAcquisitionStates.get_south:
                        (plat.GetPadById(southId)).coordinate = DEV2SepcificUtilities.AverageVectors(positionAccumVector);
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

        private static void CreateCalibrationFile()
        {
            string jsonCalFile = "";
            string path = calFilePath;

            jsonCalFile = JSONUtilities.CreatePadCalTerms(plat.GetAllPads());
            JSONUtilities.WriteJsonFile(path, jsonCalFile);
            Logger.LogMessage("Calibration file written to " + path);
        }

        public static CalTerms[] ReadCalibrationFile()
        {
            string jsonCalFile = "";
            string path = calFilePath;

            jsonCalFile = JSONUtilities.ReadJsonFile(path);

            if (jsonCalFile != null)
            {
                CalTerms[] terms = JSONUtilities.ReadPadCalTermsFromJson(jsonCalFile);
                calibrationComplete = true;
                return terms;
            }

            return null;
        }
    }

    enum CalibrationStates
    {
        init,
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
