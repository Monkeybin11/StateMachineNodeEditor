﻿using System.Windows;
using System.Windows.Controls;
using ReactiveUI;
using StateMachineNodeEditor.ViewModel;

namespace StateMachineNodeEditor.View
{
    /// <summary>
    /// Interaction logic for ViewConnect.xaml
    /// </summary>
    public partial class ViewConnect : UserControl, IViewFor<ViewModelConnect>
    {
        #region ViewModel
        public static readonly DependencyProperty ViewModelProperty = DependencyProperty.Register(nameof(ViewModel), typeof(ViewModelConnect), typeof(ViewConnect), new PropertyMetadata(null));

        public ViewModelConnect ViewModel
        {
            get { return (ViewModelConnect)GetValue(ViewModelProperty); }
            set { SetValue(ViewModelProperty, value); }
        }

        object IViewFor.ViewModel
        {
            get { return ViewModel; }
            set { ViewModel = (ViewModelConnect)value; }
        }
        #endregion ViewModel
        public ViewConnect()
        {
            InitializeComponent();
            this.WhenActivated(disposable =>
            {
                // Цвет линии
                this.OneWayBind(this.ViewModel, x => x.Stroke, x => x.Path.Stroke);

                // Точка, из которой выходит линия
                this.OneWayBind(this.ViewModel, x => x.StartPoint.Value, x => x.PathFigure.StartPoint);

                // Первая промежуточная точка линии 
                this.OneWayBind(this.ViewModel, x => x.Point1.Value, x => x.BezierSegment.Point1);

                // Вторая промежуточная точка линии
                this.OneWayBind(this.ViewModel, x => x.Point2.Value, x => x.BezierSegment.Point2);

                // Точка, в которую приходит линия
                this.OneWayBind(this.ViewModel, x => x.EndPoint.Value, x => x.BezierSegment.Point3);

                this.OneWayBind(this.ViewModel, x => x.StrokeDashArray, x => x.Path.StrokeDashArray);

                this.OneWayBind(this.ViewModel, x => x.StrokeThickness, x => x.Path.StrokeThickness);
            });
        }
    }
}
