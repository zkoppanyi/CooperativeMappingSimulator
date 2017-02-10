using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CooperativeMapping
{
    public static class Utililty
    {
        public static double CalculateAngleDiff(Pose prevPose, Pose currentPose, Pose nextPose)
        {
            double alphap = prevPose.GetHeadingTo(currentPose);
            double alpha = currentPose.GetHeadingTo(nextPose);
            double dalpha = Math.Abs(alpha - alphap) / Math.PI * 180.0;
            return dalpha;
        }

        public static double ConvertAngleTo360(double value)
        {
            double heading = value;

            //heading must be between 0 and 360
            if (value < 0)
            {
                heading = value + 360.0 * ((int)(Math.Abs(value) / 360.0) + 1);
            }
            else if (value >= 360)
            {
                heading = value - 360.0 * (int)(Math.Abs(value) / 360.0);
            }

            return heading;
        }
    }
}
