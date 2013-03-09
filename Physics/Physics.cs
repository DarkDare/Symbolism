﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Symbolism;

namespace Physics
{
    public static class Misc
    {
        public static MathObject QuadraticEquation(MathObject a, MathObject b, MathObject c, int solution=0)
        {
            if (a == new Integer(0) || a == new DoubleFloat(0.0))
                throw new Exception("a is zero. Equation is not quadratic.");

            var discriminant = b * b - 4 * a * c;

            var half = new Integer(1) / 2;

            if (solution == 0)
                return (-b + (discriminant ^ half)) / (2 * a);

            if (solution == 1)
                return (-b - (discriminant ^ half)) / (2 * a);

            throw new Exception();
        }
    }

    public static class Trig
    {
        public static Symbol Pi = new Symbol("Pi");

        public static MathObject ToRadians(this MathObject n) { return n * Pi / 180; }

        public static MathObject ToDegrees(this MathObject n) { return 180 * n / Pi; }

        public static MathObject ToRadians(this int n) { return new Integer(n) * Pi / 180; }

        public static MathObject ToDegrees(this int n) { return 180 * new Integer(n) / Pi; }

        public static MathObject Sin(MathObject arg)
        { return new Symbolism.Sin(arg).Simplify(); }

        public static MathObject Cos(MathObject arg)
        { return new Symbolism.Cos(arg).Simplify(); }        
    }

    public class Point
    {
        public MathObject x;
        public MathObject y;

        public Point() { }

        public Point(MathObject x_val, MathObject y_val)
        { x = x_val; y = y_val; }

        //////////////////////////////////////////////////////////////////////
        // overloads for 'int'
        public Point(int x_val, int y_val)
        { x = new Integer(x_val); y = new Integer(y_val); }

        public Point(int x_val, MathObject y_val)
        { x = new Integer(x_val); y = y_val; }

        public Point(MathObject x_val, int y_val)
        { x = x_val; y = new Integer(y_val); }
        //////////////////////////////////////////////////////////////////////

        public static Point FromAngle(MathObject angle, MathObject mag)
        { return new Point(Trig.Cos(angle) * mag, Trig.Sin(angle) * mag); }

        public override string ToString()
        { return "Point(" + x + ", " + y + ")"; }

        public static Point operator +(Point a, Point b)
        { return new Point(a.x + b.x, a.y + b.y); }

        public static Point operator *(Point a, MathObject b)
        { return new Point(a.x * b, a.y * b); }

        public static Point operator /(Point a, MathObject b)
        { return new Point(a.x / b, a.y / b); }

        public MathObject Norm()
        { return (x * x + y * y) ^ (new Integer(1) / 2); }
    }

    public class Obj
    {
        public Point position = new Point();
        public Point velocity = new Point();
        public Point acceleration = new Point();

        public MathObject time;

        public void Print()
        {
            Console.WriteLine(
                "time: " + time + "\n" +
                "position.x: " + position.x + "\n" +
                "position.y: " + position.y + "\n" +
                "velocity.x: " + velocity.x + "\n" +
                "velocity.y: " + velocity.y + "\n" +
                "acceleration.x: " + acceleration.x + "\n" +
                "acceleration.y: " + acceleration.y + "\n");
        }

        public Obj AtTime(MathObject t)
        {
            var dt = t - time;

            return
                new Obj()
                {
                    time = t,
                    acceleration = acceleration,
                    velocity = velocity + acceleration * dt,
                    position = position + velocity * dt + acceleration * dt * dt / new Integer(2)
                };
        }


        #region ProjectileInclineIntersection derivation

        // xB = xA + vAx t + 1/2 ax t^2 					(9)

        // yB = yA + vAy t + 1/2 ay t^2 					(10)

        // xB - xA = d cos th								(13)

        // yB - yA = d sin th								(14)

        // ax = 0											(11)

        // vAx = vA cos(thA)								(6)

        // vAy = vA sin(thA)								(7)


        // (9):	xB = xA + vAx t + 1/2 ax t^2

        //         xB - xA = vAx t + 1/2 ax t^2 			(9.1)

        // (10):	yB = yA + vAy t + 1/2 ay t^2

        //         yB - yA = vAy t + 1/2 ay t^2 			(10.1)


        // (13):		xB - xA = d cos th

        // /. (9.1)	vAx t + 1/2 ax t^2 = d cos th

        // /. (11)		vAx t = d cos th

        // t 			t = d cos(th) / vAx 				(13.1)


        // (14):		yB - yA = d sin th

        // /. (10.1)	vAy t + 1/2 ay t^2 = d sin th

        // /. (13.1)	vAy [d cos(th) / vAx] + 1/2 ay [d cos(th) / vAx]^2 = d sin th

        //             vAy / vAx d cos(th) + 1/2 ay [d cos(th) / vAx]^2 = d sin th

        //             1/2 ay [d cos(th) / vAx]^2 = d sin th - vAy / vAx d cos(th)

        //             1/2 ay [d cos(th) / vAx]^2 = d [sin(th) - vAy / vAx cos(th)]

        //             1/2 ay d^2 [cos(th) / vAx]^2 = d [sin(th) - vAy / vAx cos(th)]

        //             1/2 ay d [cos(th) / vAx]^2 = [sin(th) - vAy / vAx cos(th)]

        //             d = 2 [sin(th) - vAy / vAx cos(th)] [vAx / cos(th)]^2 / ay 

        // if vAy = 0 then it simplifies to:

        //             d = 2 sin(th) [vAx / cos(th)]^2 / ay 

        #endregion

        public Point ProjectileInclineIntersection(MathObject theta)
        {
            if (theta != null &&
                velocity.x != null &&
                velocity.y != null &&
                acceleration.y != null &&
                acceleration.y != 0 &&
                acceleration.y != 0.0)
            {
                var d =
                    2 * (Trig.Sin(theta) - velocity.y / velocity.x * Trig.Cos(theta))
                    * ((velocity.x / Trig.Cos(theta)) ^ 2)
                    / acceleration.y;

                return
                    new Point(
                        position.x + d * Trig.Cos(theta),
                        position.y + d * Trig.Sin(theta));
            }

            throw new Exception();
        }
    }
    
    public static class Calc
    {
        public static MathObject Time(Obj a, Obj b, int solution = 0)
        {
            if (a.velocity.x != null &&
                b.velocity.x != null &&
                a.acceleration.x != null &&
                a.acceleration.x != new DoubleFloat(0.0) &&
                a.acceleration.x != new Integer(0))
                return (b.velocity.x - a.velocity.x) / a.acceleration.x;

            if (a.velocity.y != null &&
                b.velocity.y != null &&
                a.acceleration.y != null &&
                a.acceleration.y != new DoubleFloat(0.0) &&
                a.acceleration.y != new Integer(0))
                return (b.velocity.y - a.velocity.y) / a.acceleration.y;

            // yf = yi + vyi * t + 1/2 * ay * t^2
            // 0 = 1/2 * ay * t^2 + vyi * t + yi - yf
            // apply quadratic equation to find t
            
            if (a.position.y != null &&
                b.position.y != null &&
                a.velocity.y != null &&
                a.acceleration.y != null &&
                a.acceleration.y != new Integer(0) &&
                a.acceleration.y != new DoubleFloat(0.0))
            {
                var half = new Integer(1) / 2;

                return
                    Misc.QuadraticEquation(
                        half * a.acceleration.y,
                        a.velocity.y,
                        a.position.y - b.position.y,
                        solution);
            }

            throw new Exception();
        }
    }
}
