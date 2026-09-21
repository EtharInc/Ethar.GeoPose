// <copyright file="OrientationConversions.cs" company="Ethar">
// Copyright (c) Ethar. All rights reserved.
// </copyright>

namespace Ethar.GeoPose.Conventions
{
    using System;
    using Ethar.GeoPose.DataTypes;
    using Ethar.GeoPose.Geodesy;

    /// <summary>
    /// Quaternion arithmetic and conversions between yaw, pitch, roll angles and unit quaternions.
    /// </summary>
    /// <remarks>
    /// <para>
    /// Quaternions follow the Hamilton convention. <c>a.Multiply(b)</c> is the rotation that applies <c>b</c> first and then <c>a</c>.
    /// A <see cref="UnitQuaternion"/> produced by these helpers rotates a vector expressed in the inner (posed) frame into the outer
    /// frame: <c>v_outer = q · v_inner · q*</c>, which is what <see cref="Rotate(UnitQuaternion, UnitVector3)"/> computes.
    /// </para>
    /// <para>
    /// The parameterless <see cref="ToQuaternion(YawPitchRollAngles)"/> and <see cref="ToYawPitchRoll(UnitQuaternion)"/> use
    /// <see cref="EulerConvention.GeoPoseStandard"/>: OGC GeoPose 1.0 /req/basic-ypr/angles, intrinsic rotations about local z (yaw),
    /// then y (pitch), then x (roll), in degrees, <c>q = qz(yaw) · qy(pitch) · qx(roll)</c>. The overloads that take an
    /// <see cref="EulerConvention"/> support any Tait-Bryan sequence, intrinsic or extrinsic, in degrees or radians, so data produced under
    /// another convention can be converted to the standard one with <see cref="ConvertTo"/>.
    /// </para>
    /// </remarks>
    public static class OrientationConversions
    {
        private const double GimbalLockThreshold = 1.0 - 1e-12;

        /// <summary>
        /// Builds the unit quaternion for a rotation of <paramref name="degrees"/> about an arbitrary axis.
        /// </summary>
        /// <param name="axis">The axis of rotation; it is normalised before use.</param>
        /// <param name="degrees">The angle in degrees, positive in the right-handed sense about the axis.</param>
        /// <returns>The unit quaternion.</returns>
        public static UnitQuaternion FromAxisAngle(UnitVector3 axis, double degrees)
        {
            var length = Math.Sqrt((axis.X * axis.X) + (axis.Y * axis.Y) + (axis.Z * axis.Z));
            if (length < double.Epsilon)
            {
                throw new ArgumentException("The rotation axis must not be zero.", nameof(axis));
            }

            var half = Angles.DegreesToRadians(degrees) / 2.0;
            var s = Math.Sin(half) / length;
            return new UnitQuaternion(axis.X * s, axis.Y * s, axis.Z * s, Math.Cos(half));
        }

        /// <summary>
        /// Builds the unit quaternion for a rotation of <paramref name="degrees"/> about a frame axis.
        /// </summary>
        /// <param name="axis">The frame axis.</param>
        /// <param name="degrees">The angle in degrees, positive in the right-handed sense about the axis.</param>
        /// <returns>The unit quaternion.</returns>
        public static UnitQuaternion FromAxisAngle(RotationAxis axis, double degrees) => FromAxisAngle(AxisVector(axis), degrees);

        /// <summary>
        /// Converts GeoPose yaw, pitch and roll angles to the equivalent unit quaternion using <see cref="EulerConvention.GeoPoseStandard"/>.
        /// </summary>
        /// <param name="angles">The angles in degrees.</param>
        /// <returns>The unit quaternion <c>qz(yaw) · qy(pitch) · qx(roll)</c>.</returns>
        public static UnitQuaternion ToQuaternion(this YawPitchRollAngles angles) => angles.ToQuaternion(EulerConvention.GeoPoseStandard);

        /// <summary>
        /// Converts yaw, pitch and roll angles expressed under a given convention to the equivalent unit quaternion.
        /// </summary>
        /// <param name="angles">The angles, in the convention's unit.</param>
        /// <param name="convention">The axis sequence, intrinsic or extrinsic application, and unit.</param>
        /// <returns>The unit quaternion.</returns>
        public static UnitQuaternion ToQuaternion(this YawPitchRollAngles angles, EulerConvention convention)
        {
            var first = FromAxisAngle(convention.YawAxis, convention.ToDegrees(angles.Yaw));
            var second = FromAxisAngle(convention.PitchAxis, convention.ToDegrees(angles.Pitch));
            var third = FromAxisAngle(convention.RollAxis, convention.ToDegrees(angles.Roll));

            // Intrinsic rotations compose by right multiplication; extrinsic rotations by left multiplication.
            return convention.IsIntrinsic ? first.Multiply(second).Multiply(third) : third.Multiply(second).Multiply(first);
        }

        /// <summary>
        /// Converts a unit quaternion to GeoPose yaw, pitch and roll angles using <see cref="EulerConvention.GeoPoseStandard"/>.
        /// </summary>
        /// <param name="quaternion">The unit quaternion; it is normalised before use.</param>
        /// <returns>The angles in degrees. Yaw and roll are in [-180, 180); pitch is in [-90, 90].</returns>
        public static YawPitchRollAngles ToYawPitchRoll(this UnitQuaternion quaternion) => quaternion.ToYawPitchRoll(EulerConvention.GeoPoseStandard);

        /// <summary>
        /// Converts a unit quaternion to yaw, pitch and roll angles under a given convention.
        /// </summary>
        /// <param name="quaternion">The unit quaternion; it is normalised before use.</param>
        /// <param name="convention">The axis sequence, intrinsic or extrinsic application, and unit.</param>
        /// <returns>The angles in the convention's unit. Yaw and roll cover a half turn either way; pitch covers a quarter turn either way.</returns>
        public static YawPitchRollAngles ToYawPitchRoll(this UnitQuaternion quaternion, EulerConvention convention)
        {
            if (!convention.IsIntrinsic)
            {
                // Extrinsic a-b-c with angles (α, β, γ) is intrinsic c-b-a with angles (γ, β, α).
                var reversed = new EulerConvention(convention.RollAxis, convention.PitchAxis, convention.YawAxis, AngleUnit.Degrees, true);
                var intrinsic = quaternion.ToYawPitchRoll(reversed);
                return new YawPitchRollAngles(convention.FromDegrees(intrinsic.Roll), convention.FromDegrees(intrinsic.Pitch), convention.FromDegrees(intrinsic.Yaw));
            }

            var q = quaternion.Normalize();
            var i = (int)convention.YawAxis;
            var j = (int)convention.PitchAxis;
            var k = (int)convention.RollAxis;
            var epsilon = j == ((i + 1) % 3) ? 1.0 : -1.0;
            var m = ToMatrix(q);

            var sinPitch = Math.Max(-1.0, Math.Min(1.0, epsilon * m[i, k]));
            double yaw;
            double pitch;
            double roll;

            if (Math.Abs(sinPitch) >= GimbalLockThreshold)
            {
                // Gimbal lock: only (yaw ± roll) is determined. Fix roll at zero and recover yaw from q = q_yaw · q_pitch.
                pitch = sinPitch > 0 ? 90.0 : -90.0;
                roll = 0.0;
                var yawOnly = q.Multiply(FromAxisAngle(convention.PitchAxis, pitch).Conjugate());
                yaw = Angles.RadiansToDegrees(2.0 * Math.Atan2(Component(yawOnly, i), yawOnly.W));
            }
            else
            {
                pitch = Angles.RadiansToDegrees(Math.Asin(sinPitch));
                yaw = Angles.RadiansToDegrees(Math.Atan2(-epsilon * m[j, k], m[k, k]));
                roll = Angles.RadiansToDegrees(Math.Atan2(-epsilon * m[i, j], m[i, i]));
            }

            return new YawPitchRollAngles(
                convention.FromDegrees(Angles.Normalize180(yaw)),
                convention.FromDegrees(pitch),
                convention.FromDegrees(Angles.Normalize180(roll)));
        }

        /// <summary>
        /// Re-expresses yaw, pitch and roll angles from one convention in another.
        /// </summary>
        /// <param name="angles">The angles in the source convention.</param>
        /// <param name="from">The source convention.</param>
        /// <param name="to">The target convention.</param>
        /// <returns>The same rotation as angles in the target convention.</returns>
        public static YawPitchRollAngles ConvertTo(this YawPitchRollAngles angles, EulerConvention from, EulerConvention to)
        {
            return from == to ? angles : angles.ToQuaternion(from).ToYawPitchRoll(to);
        }

        /// <summary>
        /// Gets the Euclidean norm of the quaternion. A valid GeoPose quaternion has a norm of one.
        /// </summary>
        /// <param name="quaternion">The quaternion.</param>
        /// <returns>The norm.</returns>
        public static double Norm(this UnitQuaternion quaternion)
        {
            return Math.Sqrt((quaternion.X * quaternion.X) + (quaternion.Y * quaternion.Y) + (quaternion.Z * quaternion.Z) + (quaternion.W * quaternion.W));
        }

        /// <summary>
        /// Checks that the quaternion is a unit quaternion within a tolerance.
        /// </summary>
        /// <param name="quaternion">The quaternion.</param>
        /// <param name="tolerance">The allowed deviation of the norm from one.</param>
        /// <returns>True if the norm is within the tolerance of one.</returns>
        public static bool IsUnit(this UnitQuaternion quaternion, double tolerance = 1e-6)
        {
            return Math.Abs(quaternion.Norm() - 1.0) <= tolerance;
        }

        /// <summary>
        /// Returns the quaternion scaled to unit length.
        /// </summary>
        /// <param name="quaternion">The quaternion.</param>
        /// <returns>The normalised quaternion.</returns>
        /// <exception cref="InvalidOperationException">Thrown if the quaternion has zero length.</exception>
        public static UnitQuaternion Normalize(this UnitQuaternion quaternion)
        {
            var norm = quaternion.Norm();
            if (norm < double.Epsilon)
            {
                throw new InvalidOperationException("A zero quaternion cannot be normalised.");
            }

            return new UnitQuaternion(quaternion.X / norm, quaternion.Y / norm, quaternion.Z / norm, quaternion.W / norm);
        }

        /// <summary>
        /// Returns the conjugate, which for a unit quaternion is the inverse rotation.
        /// </summary>
        /// <param name="quaternion">The quaternion.</param>
        /// <returns>The conjugate.</returns>
        public static UnitQuaternion Conjugate(this UnitQuaternion quaternion)
        {
            return new UnitQuaternion(-quaternion.X, -quaternion.Y, -quaternion.Z, quaternion.W);
        }

        /// <summary>
        /// Hamilton product. The result applies <paramref name="second"/> first and then <paramref name="first"/>.
        /// </summary>
        /// <param name="first">The rotation applied last.</param>
        /// <param name="second">The rotation applied first.</param>
        /// <returns>The combined rotation.</returns>
        public static UnitQuaternion Multiply(this UnitQuaternion first, UnitQuaternion second)
        {
            var a = first;
            var b = second;
            return new UnitQuaternion(
                (a.W * b.X) + (a.X * b.W) + (a.Y * b.Z) - (a.Z * b.Y),
                (a.W * b.Y) - (a.X * b.Z) + (a.Y * b.W) + (a.Z * b.X),
                (a.W * b.Z) + (a.X * b.Y) - (a.Y * b.X) + (a.Z * b.W),
                (a.W * b.W) - (a.X * b.X) - (a.Y * b.Y) - (a.Z * b.Z));
        }

        /// <summary>
        /// Rotates a vector by the quaternion: <c>v' = q · v · q*</c>.
        /// </summary>
        /// <param name="quaternion">The unit quaternion.</param>
        /// <param name="vector">The vector to rotate.</param>
        /// <returns>The rotated vector.</returns>
        public static UnitVector3 Rotate(this UnitQuaternion quaternion, UnitVector3 vector)
        {
            var qx = quaternion.X;
            var qy = quaternion.Y;
            var qz = quaternion.Z;
            var qw = quaternion.W;

            // t = 2 (q_v × v); v' = v + w t + (q_v × t)
            var tx = 2.0 * ((qy * vector.Z) - (qz * vector.Y));
            var ty = 2.0 * ((qz * vector.X) - (qx * vector.Z));
            var tz = 2.0 * ((qx * vector.Y) - (qy * vector.X));

            return new UnitVector3(
                vector.X + (qw * tx) + ((qy * tz) - (qz * ty)),
                vector.Y + (qw * ty) + ((qz * tx) - (qx * tz)),
                vector.Z + (qw * tz) + ((qx * ty) - (qy * tx)));
        }

        /// <summary>
        /// Rotates an ENU displacement by a quaternion expressed in the ENU frame.
        /// </summary>
        /// <param name="quaternion">The unit quaternion in the ENU frame.</param>
        /// <param name="displacement">The displacement to rotate.</param>
        /// <returns>The rotated displacement.</returns>
        public static EnuVector Rotate(this UnitQuaternion quaternion, EnuVector displacement)
        {
            var rotated = quaternion.Rotate(new UnitVector3(displacement.East, displacement.North, displacement.Up));
            return new EnuVector(rotated.X, rotated.Y, rotated.Z);
        }

        /// <summary>
        /// Returns the smallest angle between two rotations, in degrees. Zero means the same rotation (q and -q count as equal).
        /// </summary>
        /// <param name="a">The first rotation.</param>
        /// <param name="b">The second rotation.</param>
        /// <returns>The angle in degrees, in [0, 180].</returns>
        public static double AngleTo(this UnitQuaternion a, UnitQuaternion b)
        {
            var na = a.Normalize();
            var nb = b.Normalize();
            var dot = Math.Abs((na.X * nb.X) + (na.Y * nb.Y) + (na.Z * nb.Z) + (na.W * nb.W));
            return Angles.RadiansToDegrees(2.0 * Math.Acos(Math.Min(1.0, dot)));
        }

        private static UnitVector3 AxisVector(RotationAxis axis)
        {
            switch (axis)
            {
                case RotationAxis.X:
                    return UnitVector3.UnitX;
                case RotationAxis.Y:
                    return UnitVector3.UnitY;
                case RotationAxis.Z:
                    return UnitVector3.UnitZ;
                default:
                    throw new ArgumentOutOfRangeException(nameof(axis));
            }
        }

        private static double Component(UnitQuaternion q, int axis)
        {
            switch (axis)
            {
                case 0:
                    return q.X;
                case 1:
                    return q.Y;
                default:
                    return q.Z;
            }
        }

        private static double[,] ToMatrix(UnitQuaternion q)
        {
            var x = q.X;
            var y = q.Y;
            var z = q.Z;
            var w = q.W;
            return new[,]
            {
                { 1.0 - (2.0 * ((y * y) + (z * z))), 2.0 * ((x * y) - (w * z)), 2.0 * ((x * z) + (w * y)) },
                { 2.0 * ((x * y) + (w * z)), 1.0 - (2.0 * ((x * x) + (z * z))), 2.0 * ((y * z) - (w * x)) },
                { 2.0 * ((x * z) - (w * y)), 2.0 * ((y * z) + (w * x)), 1.0 - (2.0 * ((x * x) + (y * y))) },
            };
        }
    }
}
