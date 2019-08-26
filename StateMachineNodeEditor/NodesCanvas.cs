﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Media;
using System.Windows.Media;
using System.Windows.Shapes;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace StateMachineNodeEditor
{
    public class NodesCanvas : Grid
    {
        public ObservableCollection<Node> nodes = new ObservableCollection<Node>();
        public ObservableCollection<Connect> connects = new ObservableCollection<Connect>();
        public List<Node> selectedNodes = new List<Node>();
        TextBox textBox = new TextBox();
        public Selector Selector;
        public MouseEventHandler Moves;
        Point position_click;
        static NodesCanvas()
        {

        }
        public Managers Manager { get; set; }
        protected override void OnDragOver(DragEventArgs e)
        {
            if (e.Data.GetData("Connect") is Connect obj)
            {
                //Point point =;
                //point.X -= 2;
                //point.Y -= 2;
                obj.EndPoint = ForPoint.Subtraction(e.GetPosition(this), 2);
            }
            base.OnDragOver(e);
        }
        public NodesCanvas()
        {
            AllowDrop = true;
            nodes.CollectionChanged += NodesChange;
            connects.CollectionChanged += ConnectsChange;
            ContextMenu contex = new ContextMenu();
            MenuItem add = new MenuItem();
            add.Name = "Add";
            add.Header = "Add";
            add.Click += Add_Click;
            contex.Items.Add(add);
            contex.Margin = new Thickness(10, 0, 0, 0);
            add.Icon = null;
            this.ContextMenu = contex;
            Manager = new Managers(this);
            Selector = new Selector(this);
            this.Children.Add(Selector);
            //this.Children.Add(Selector);
            this.ClipToBounds = true;
            this.MouseMove += mouseMove;
            this.MouseWheel += mouseWheel;
            this.MouseUp += mouseUp;
            this.MouseDown += mouseDown;
            textBox.HorizontalAlignment = HorizontalAlignment.Right;
            textBox.VerticalAlignment = VerticalAlignment.Top;
            this.Children.Add(textBox);
        }
        public NodesCanvas(UIElement _parent) : this()
        {
            parent = _parent;
            this.Background = new SolidColorBrush(Color.FromRgb(20, 20, 20));
            this.AllowDrop = true;
        }
        protected override void OnMouseRightButtonDown(MouseButtonEventArgs e)
        {
            position_click = e.GetPosition(this);
            base.OnMouseRightButtonDown(e);
        }
        public UIElement parent;
        public void NodeMove(object sender, EventArgs e)
        {

        }
        public void NodesChange(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var element in e.NewItems)
                {
                    if (element is Node node)
                        this.Children.Add(node);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var element in e.OldItems)
                {
                    if (element is Node node)
                        this.Children.Remove(node);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                this.Children.Clear();
            }
        }
        public void ConnectsChange(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                foreach (var element in e.NewItems)
                {
                    if (element is Connect node)
                        this.Children.Add(node);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (var element in e.OldItems)
                {
                    if (element is Connect node)
                        this.Children.Remove(node);
                }
            }
            else if (e.Action == NotifyCollectionChangedAction.Reset)
            {
                foreach (var element in this.Children)
                {
                    if (element is Connect connect)
                        this.Children.Remove(connect);
                }
            }
        }
        public void mouseDown(object sender, MouseButtonEventArgs e)
        {
            Point position = e.GetPosition(this);
            textBox.Text = position.ToString();
            if (Keyboard.IsKeyDown(Key.LeftCtrl))
            {
                Selector.Position1 = position;
                Selector.Visibility = Visibility.Visible;
            }
        }
        public void mouseUp(object sender, MouseButtonEventArgs e)
        {
            Selector.Visibility = Visibility.Hidden;
        }
        public int UpdateSeletedNodes()
        {
            Point selectorPoint1 = Selector.Position1;            
            Point selectorPoint2 = Selector.Position2;
            if (nodes.Count > 0)
            {
                selectorPoint1 = ForPoint.Division(selectorPoint1, nodes.First().Manager.zoom);
                selectorPoint2 = ForPoint.Division(selectorPoint2, nodes.First().Manager.zoom);
            }
            foreach (Node node in nodes)
            {
                if (Functions.Intersect(node.Point1, node.Point2, selectorPoint1, selectorPoint2))
                {
                    node.Select();
                }
                else
                    node.UnSelect();
            }
            return 0;
        }
    
    public void mouseMove(object sender, MouseEventArgs e)
        {
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                Point delta = Manager.GetDeltaMove();
                if (Mouse.Captured == this)
                {
                    if (!Keyboard.IsKeyDown(Key.LeftCtrl))
                    {
                        foreach (var node in nodes)
                        {
                            node.Manager.Move(delta);
                        }
                    }
                    else
                    {
                        if(!Selector.IsVisible)
                        {
                            Selector.Visibility = Visibility.Visible;
                        }
                        Selector.Position2 = e.GetPosition(this);
                        UpdateSeletedNodes();
                    }

                }
                else
                {
                    Node node = Mouse.Captured as Node;
                    if (node!=null)
                    {
                        node.Manager.Move(delta);
                    }
                }
            }         
        }
        public void mouseWheel(object sender, MouseWheelEventArgs e)
        {
            if (Mouse.Captured == null) 
            {
                foreach (var node in nodes)
                {
                    node.Manager.Scale(e.Delta);
                }
            }
        }
        public void Add_Click(object sender, RoutedEventArgs e)
        {
            Node node = AddNodes(position_click);
        }
        public Node AddNodes(Point position)
        {
            Node node = new Node("State " + this.nodes.Count.ToString(),this);
            node.Name = "State" + this.nodes.Count.ToString();
            if (nodes.Count > 0)
            {
                Node firstNode = nodes.First();
                node.Manager.scale.ScaleX = firstNode.Manager.scale.ScaleX;
                node.Manager.scale.ScaleY = firstNode.Manager.scale.ScaleY;
                node.Manager.zoom = firstNode.Manager.zoom;
            }
            ForPoint.Equality(node.Manager.translate, ForPoint.Division(position, node.Manager.zoom));
            nodes.Add(node);
            Panel.SetZIndex(node, 1);
            return node;
        }
        public Connect AddConnect(Connect connect)
        {          
            connect.Name = "Connect_" + this.connects.Count.ToString();
            connects.Add(connect);
            return connect;
        }

    }
}
