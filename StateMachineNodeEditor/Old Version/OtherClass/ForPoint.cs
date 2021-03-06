﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media;
namespace StateMachineNodeEditor
{
   static public class ForPoint
    {
        public static Point Addition(Point point1, Point point2)
        {
            return new Point(point1.X + point2.X, point1.Y + point2.Y);
        }
        public static Point Mirror(Point point)
        {
            return new Point(-point.X, -point.Y);
        }
        public static Point Multiplication(Point point1, int number)
        {
            return new Point(point1.X * number, point1.Y * number);
        }
        public static Point Multiplication(Point point1, double number)
        {
            return new Point(point1.X * number, point1.Y * number);
        }
        public static Point Division(Point point1, Point point2)
        {
            return new Point(point1.X / point2.X, point1.Y / point2.Y);
        }
        public static Point Division(Point point1, int number)
        {
            return new Point(point1.X / number, point1.Y / number);
        }
        public static Point Division(Point point1, double number)
        {
            return new Point(point1.X / number, point1.Y / number);
        }
        public static Point Subtraction(Point point1, Point point2)
        {
            return new Point(point1.X - point2.X, point1.Y - point2.Y);
        }
        public static Point Subtraction(Point point1, int number)
        {
            return new Point(point1.X - number, point1.Y - number);
        }
        public static Point Reverse(Point point)
        {
            return new Point(point.Y, point.X);
        }
        public static Point Subtraction(Point point1, double number)
        {
            return new Point(point1.X -number, point1.Y -number);
        }
        public static Point Equality(Point point)
        {
            return new Point(point.X, point.Y);
        }
        public static bool isEmpty(Point point)
        {
            return (point.X == 0) && (point.Y == 0);
         }
        public static Point GetValueAsPoint(System.Windows.Media.TranslateTransform translate)
        {
          return new Point(translate.X, translate.Y);
        }
        public static Point GetEmpty()
        {
            return new Point();
        }
        public static void Addition(System.Windows.Media.TranslateTransform translate, Point point)
        {
            translate.X += point.X;
            translate.Y += point.Y;
        }
        public static void Equality(System.Windows.Media.TranslateTransform translate, Point point)
        {
            translate.X = point.X;
            translate.Y = point.Y;
        }
        public static void EqualityScale(ScaleTransform scale, Point point)
        {
            scale.ScaleX = point.X;
            scale.ScaleY = point.Y;
        }
        public static void EqualityScale(ScaleTransform scale, double number)
        {
            scale.ScaleX = number;
            scale.ScaleY = number;
        }
        public static void AdditionToCenter(RotateTransform rotate, Point point)
        {
            rotate.CenterX += point.X;
            rotate.CenterY += point.Y;
        }
        public static void EqualityCenter(RotateTransform rotate, Point point)
        {
            rotate.CenterX = point.X;
            rotate.CenterY = point.Y;
        }
        public static void EqualityCenter(ScaleTransform scale, Point point)
        {
            scale.CenterX = point.X;
            scale.CenterY = point.Y;
        }
        public static Point DivisionOnScale(Point point1, ScaleTransform scale)
        {
            return new Point(point1.X / scale.ScaleX, point1.Y / scale.ScaleY);
        }

        public static Point GetPoint1(FrameworkElement element, System.Windows.Media.TranslateTransform translate)
        {
            return ForPoint.GetValueAsPoint(translate);
        }
        public static Point GetPoint2(FrameworkElement element, System.Windows.Media.TranslateTransform translate)
        {
            Point point1 = GetPoint1(element, translate);
            return new Point(point1.X + element.ActualWidth, point1.Y + element.ActualHeight);
        }
        public static Point GetPoint1WithScale(FrameworkElement element, Transforms transforms)
        {
            Point point1 = ForPoint.GetPoint1(element, transforms.translate);
            if (transforms.scale.ScaleX < 0)
                point1.X -= element.ActualWidth;
            if (transforms.scale.ScaleY < 0)
                point1.Y -= element.ActualHeight;
            return point1;
        }
        public static Point GetPoint2WithScale(FrameworkElement element, Transforms transforms)
        {
            Point point1 = ForPoint.GetPoint1(element, transforms.translate);
            if (transforms.scale.ScaleX > 0)
                point1.X += element.ActualWidth;
            if (transforms.scale.ScaleY > 0)
                point1.Y += element.ActualHeight;
            return point1;
        }
    }
}
