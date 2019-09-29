﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using ReactiveUI.Fody.Helpers;
using StateMachineNodeEditor.Helpers;
using ReactiveUI;
using ReactiveUI.Wpf;
using DynamicData;
using DynamicData.Binding;
using System.Collections.ObjectModel;
using System.Reactive.Linq;
using System.Windows;

namespace StateMachineNodeEditor.ViewModel
{
    public class ViewModelNodesCanvas : ReactiveObject
    {
        //public IObservableCollection<ViewModelNode> Nodes { get; set; } = new ObservableCollectionExtended<ViewModelNode>();
        //public IObservableCollection<ViewModelConnect> Connects { get; set; } = new ObservableCollectionExtended<ViewModelConnect>();
        private SourceList<ViewModelConnect> ListConnects { get; set; } = new SourceList<ViewModelConnect>();
        public IObservableCollection<ViewModelConnect> Connects = new ObservableCollectionExtended<ViewModelConnect>();

        private readonly SourceList<ViewModelNode> ListNodes = new SourceList<ViewModelNode>();
        public IObservableCollection<ViewModelNode> Nodes = new ObservableCollectionExtended<ViewModelNode>();
        
        public IObservableList<ViewModelNode> NodesSelected { get; }
        public ViewModelSelector Selector { get; set; } = new ViewModelSelector();
        public ViewModelConnect CurrentConnect { get; set; }
        public ViewModelNode CurrentNode { get; set; }

        /// <summary>
        /// Масштаб (Общий для всех узлов)
        /// </summary>
        [Reactive] public Scale Scale { get; set; } = new Scale();

        private ViewModelConnect AddEmptyConnect()
        {
            CurrentConnect = new ViewModelConnect(CurrentNode.CurrentConnector);
            Connects.Add(CurrentConnect);
            return CurrentConnect;
        }
        public ViewModelNodesCanvas()
        {
            ListNodes.Connect().ObserveOnDispatcher().Bind(Nodes).Subscribe();
            NodesSelected = ListNodes.Connect().AutoRefresh(node => node.Selected).Filter(node => node.Selected).AsObservableList();
            ListConnects.Connect().ObserveOnDispatcher().Bind(Connects).Subscribe();

            ListNodes.Add(new ViewModelNode(this)
            {
                Name= "State 1"
            });

            //AddEmptyConnect();
            SetupCommands();
         
        }      
        #region Commands
        public SimpleCommand CommandRedo { get; set; }
        public SimpleCommand CommandUndo { get; set; }
        public SimpleCommand CommandSelectAll { get; set; }
        public SimpleCommand CommandUnSelectAll { get; set; }
        public SimpleCommandWithParameter<MyPoint> CommandSelect { get; set; }
        //public Command CommandNew { get; set; }
        //public Command CommandDelete { get; set; }
        //public Command CommandCopy { get; set; }
        //public Command CommandPaste { get; set; }
        //public Command CommandCut { get; set; }
        //public Command CommandMoveDown { get; set; }
        //public Command CommandMoveLeft { get; set; }
        //public Command CommandMoveRight { get; set; }
        //public Command CommandMoveUp { get; set; }
        //public Command CommandSimpleMoveAllNode { get; set; }
        //public Command CommandSimpleMoveAllSelectedNode { get; set; }

        public SimpleCommand CommandSelectorIntersect { get; set; }
        public Command<MyPoint, List<ViewModelNode>> CommandMoveAllNode { get; set; }
        public Command<MyPoint, List<ViewModelNode>> CommandMoveAllSelectedNode { get; set; }
        //public Command CommandDropOver { get; set; }

        public void SetupCommands()
        {
            CommandRedo = new SimpleCommand(this, CommandUndoRedo.Redo);
            CommandUndo = new SimpleCommand(this, CommandUndoRedo.Undo);
            CommandMoveAllNode = new Command<MyPoint, List<ViewModelNode>>(this, MoveAllNode);
            CommandMoveAllNode = new Command<MyPoint, List<ViewModelNode>>(this, MoveAllSelectedNode);
            CommandSelect = new SimpleCommandWithParameter<MyPoint>(this, StartSelect);
            CommandSelectorIntersect = new SimpleCommand(this, SelectorIntersect);
        }

        #endregion Commands
        public void StartSelect(MyPoint position)
        {
            Selector.CommandStartSelect.Execute(position);
        }
        public List<ViewModelNode> MoveAllNode(MyPoint delta, List<ViewModelNode> nodes = null)
        {
            if (nodes == null)
                nodes = Nodes.ToList();
            nodes.ForEach(node => node.Move(delta));
            return nodes;
        }
        public List<ViewModelNode> MoveAllSelectedNode(MyPoint delta, List<ViewModelNode> nodes = null)
        {
            if (nodes == null)
                nodes = NodesSelected.Items.ToList();
            nodes.ForEach(node => node.Move(delta));
            return nodes;
        }

        private void SelectorIntersect()
        {
            MyPoint selectorPoint1 = Selector.Point1 / Scale.Value;
            MyPoint selectorPoint2 = Selector.Point2 / Scale.Value;

            foreach (ViewModelNode node in Nodes)
            {
                node.Selected = Utils.Intersect(node.Point1, node.Point2, selectorPoint1, selectorPoint2);
            }
        }
    }
}
