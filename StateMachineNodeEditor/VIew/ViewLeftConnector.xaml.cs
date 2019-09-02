﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace StateMachineNodeEditor
{
    /// <summary>
    /// Interaction logic for ViewLeftConnector.xaml
    /// </summary>
    public partial class ViewLeftConnector : UserControl
    {
        public ViewLeftConnector()
        {
            InitializeComponent();
            this.DataContextChanged += DataContextChange;
        }
        public ViewModelConnector ViewModelConnector { get; set; }
        public void DataContextChange(object sender, DependencyPropertyChangedEventArgs e)
        {
            ViewModelConnector = e.NewValue as ViewModelConnector;
        }
    }
}
