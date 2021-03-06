﻿using UnityEngine;
using UnityEngine.VR;
using System;

namespace VMUVUnityPlugin_NET35_v100
{
    static class DEV2SepcificUtilities
    {
        public static Vector3 AverageVectors(Vector3[] src)
        {
            Vector3 rtn = AccumulateVectors(src);
            int divisor = src.Length;

            rtn.x /= divisor;
            rtn.y /= divisor;
            rtn.z /= divisor;

            return rtn;
        }

        public static Vector3 AccumulateVectors(Vector3[] src)
        {
            Vector3 rtn = new Vector3(0, 0, 0);

            for (int i = 0; i < src.Length; i++)
            {
                rtn.x += src[i].x;
                rtn.y += src[i].y;
                rtn.z += src[i].z;
            }

            return rtn;
        }

        public static Vector3 GetHeadHandsFusion()
        {
            Vector3[] fusion = new Vector3[3];

            try
            {
                fusion[0] = InputTracking.GetLocalPosition(VRNode.Head);
                fusion[1] = InputTracking.GetLocalPosition(VRNode.LeftHand);
                fusion[2] = InputTracking.GetLocalPosition(VRNode.RightHand);

                return AverageVectors(fusion);
            }
            catch (Exception e)
            {
                DEV2ExceptionHandler.TakeActionOnException(e);
                return (new Vector3(0, 0, 0));
            }
        }

        public static Vector3 GetHead()
        {
            Vector3 fusion = new Vector3();

            try
            {
                fusion = InputTracking.GetLocalPosition(VRNode.Head);
                return fusion;
            }
            catch (Exception e)
            {
                DEV2ExceptionHandler.TakeActionOnException(e);
                return (new Vector3(0, 0, 0));
            }
        }

        public static ushort HandlePadIDRollOver(short currId)
        {
            if (currId > 7)
                currId -= 8;
            else if (currId < 0)
                currId += 8;

            return (ushort)currId;
        }

        public static bool IsUserOverPad(Vector3 padCoord, float radius)
        {
            Vector3 user = GetHeadHandsFusion();

            return (DrawDistanceBetweenPoints(padCoord, user) < radius);
        }

        public static float DrawDistanceBetweenPoints(Vector3 p1, Vector3 p2)
        {
            float x = Math.Abs(p2.x - p1.x);
            float z = Math.Abs(p2.z - p1.z);

            x = x * x;
            z = z * z;

            return (float)Math.Sqrt((double)(x + z));
        }

        public static Vector3 GetMidPointBetweenPoints(Vector3 p1, Vector3 p2)
        {
            Vector3 rtn = new Vector3(0, 0, 0);

            rtn.x = (p1.x + p2.x) / 2.0f;
            rtn.y = (p1.y + p2.y) / 2.0f;
            rtn.z = (p1.z + p2.z) / 2.0f;

            return rtn;
        }

        public static bool ArePadIdsConsecutive(ushort[] pads, int numPadsToCheck)
        {
            if ((pads.Length < numPadsToCheck) || (numPadsToCheck < 2))
                return false;

            for (int i = 0; i < (numPadsToCheck - 1); i++)
            {
                if ((HandlePadIDRollOver((short)(pads[i] - 1)) == pads[i + 1]) || (HandlePadIDRollOver((short)(pads[i] + 1)) == pads[i + 1]))
                    continue;
                else
                    return false;
            }

            return true;
        }

        public static ushort[] GetAdjacentPadIds(ushort padId)
        {
            ushort[] padIds = new ushort[3];

            padIds[0] = HandlePadIDRollOver((short)(padId - 1));
            padIds[1] = padId;
            padIds[2] = HandlePadIDRollOver((short)(padId + 1));

            return padIds;
        }

        public static ushort CalculatePadIdDeltaCCW(ushort start, ushort end)
        {
            if (start < end)
                start += 8;
            return (ushort)(start - end);
        }

        public static ushort CalculatePadIdDeltaCW(ushort start, ushort end)
        {
            if (end < start)
                end += 8;
            return (ushort)(end - start);
        }

        public static float CalculatePctChange(float x, float y)
        {
            return (float)((Math.Abs(x - y) / y));
        }

        public static Vector3 CalculateMidPointOnArc(Vector3[] refPts, Vector3 c, float rad)
        {
            Vector3 rtn = new Vector3(0, refPts[0].y, 0);
            var a = refPts[0] - c;
            var b = refPts[1] - c;
            var m = a + b;

            m.Normalize();
            m *= rad;
            rtn = c + m;

            return rtn;
        }

        public static ushort[] FindMinDistanceBetweenPadSets(ushort[] user, ushort[] active)
        {
            ushort[] minDist = new ushort[2];
            int userLen = user.Length;
            int activeLen = active.Length - 1;

            minDist[0] = 9;
            minDist[1] = 9;

            for (int i = 0; i < userLen; i++)
            {
                for (int j = 0; j < activeLen; j++)
                {
                    ushort tmp = CalculatePadIdDeltaCCW(user[i], active[j]);

                    if (tmp < minDist[0])
                        minDist[0] = tmp;

                    tmp = CalculatePadIdDeltaCW(user[i], active[j]);

                    if (tmp < minDist[1])
                        minDist[1] = tmp;
                }   
            }

            return minDist;
        }
    }
}
