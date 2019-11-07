﻿using System;
using System.Collections.Generic;
using System.Linq;
using ReactiveUI.Fody.Helpers;
using StateMachineNodeEditor.Helpers;
using ReactiveUI;
using DynamicData;
using DynamicData.Binding;
using System.Reactive.Linq;
using ReactiveUI.Validation.Abstractions;
using ReactiveUI.Validation.Contexts;

namespace StateMachineNodeEditor.ViewModel
{
    public class ViewModelNodesCanvas : ReactiveObject
    {
        public IObservableCollection<ViewModelConnect> Connects = new ObservableCollectionExtended<ViewModelConnect>();
        public IObservableCollection<ViewModelNode> Nodes = new ObservableCollectionExtended<ViewModelNode>();
        [Reactive] public ViewModelSelector Selector { get; set; } = new ViewModelSelector();
        [Reactive] public ViewModelConnect CurrentConnect { get; set; }
        [Reactive] public ViewModelNode CurrentNode { get; set; }

        /// <summary>
        /// Масштаб 
        /// </summary>
        [Reactive] public Scale Scale { get; set; } = new Scale();

        public ViewModelNodesCanvas()
        {
            SetupCommands();
            SetupNodes();
        }
        #region Setup Nodes
        private void SetupNodes()
        {
            ViewModelNode start = new ViewModelNode(this)
            {
                Name = "Start",
                NameEnable = false, 
                CanBeDelete = false
               

            };
            start.Input.Visible = null;

            Nodes.Add(start);

            ViewModelNode end = new ViewModelNode(this)
            {
                Name = "End",
                NameEnable = false,
                CanBeDelete = false,
                Point1 = new MyPoint(100,100)
            };
            end.TransitionsVisible = null;
            end.RollUpVisible = null;
            Nodes.Add(end);
        }

        #endregion Setup Nodes
        #region Setup Commands
        public SimpleCommand CommandRedo { get; set; }
        public SimpleCommand CommandUndo { get; set; }
        public SimpleCommand CommandSelectAll { get; set; }
        public SimpleCommand CommandUnSelectAll { get; set; }
        public SimpleCommand CommandSelectorIntersect { get; set; }
        public SimpleCommand CommandDeleteFreeConnect { get; set; }
        public SimpleCommandWithParameter<object> CommandZoom { get; set; }
        public SimpleCommandWithParameter<MyPoint> CommandSelect { get; set; }
        public SimpleCommandWithParameter<MyPoint> CommandPartMoveAllNode { get; set; }
        public SimpleCommandWithParameter<MyPoint> CommandPartMoveAllSelectedNode { get; set; }

        public SimpleCommandWithParameter<ViewModelConnector> CommandAddFreeConnect { get; set; }
        public Command<ViewModelConnect, ViewModelConnect> CommandAddConnect { get; set; }
        

        //public Command CommandCopy { get; set; }
        //public Command CommandPaste { get; set; }
        //public Command CommandCut { get; set; }

        //public Command CommandMoveDown { get; set; }
        //public Command CommandMoveLeft { get; set; }
        //public Command CommandMoveRight { get; set; }
        //public Command CommandMoveUp { get; set; }



        public Command<MyPoint, List<ViewModelNode>> CommandFullMoveAllNode { get; set; }
        public Command<MyPoint, List<ViewModelNode>> CommandFullMoveAllSelectedNode { get; set; }
        public Command<MyPoint, ViewModelNode> CommandAddNode { get; set; }
        //public Command<MyPoint, ViewModelNode> CommandDeleteNode { get; set; }
        public Command<List<ViewModelNode>, List<ViewModelNode>> CommandDeleteSelectedNode { get; set; }

        public double ScaleMax = 5;
        public double ScaleMin = 0.1;
        public double Scales { get; set; } = 0.05;

        //public Command CommandDropOver { get; set; }

        private void SetupCommands()
        {
            CommandRedo = new SimpleCommand(this, CommandUndoRedo.Redo);
            CommandUndo = new SimpleCommand(this, CommandUndoRedo.Undo);
            CommandSelectAll = new SimpleCommand(this, SelectedAll);
            CommandUnSelectAll = new SimpleCommand(this, UnSelectedAll);
            CommandSelectorIntersect = new SimpleCommand(this, SelectorIntersect);


            CommandPartMoveAllNode = new SimpleCommandWithParameter<MyPoint>(this, PartMoveAllNode);          
            CommandPartMoveAllSelectedNode = new SimpleCommandWithParameter<MyPoint>(this, PartMoveAllSelectedNode);
            CommandZoom = new SimpleCommandWithParameter<object>(this, Zoom);
            
            //CommandAddConnect = new Command<ViewModelConnect, ViewModelConnect>(this, AddConnect, DeleteConnect);
            //CommandDeleteNode = new Command<MyPoint, ViewModelNode>(this, DeleteNode,);
            CommandSelect = new SimpleCommandWithParameter<MyPoint>(this, StartSelect);
            
            CommandAddFreeConnect = new SimpleCommandWithParameter<ViewModelConnector>(this, AddFreeConnect);
            
            CommandDeleteFreeConnect = new SimpleCommand(this, DeleteFreeConnect);
            CommandFullMoveAllNode = new Command<MyPoint, List<ViewModelNode>>(this, FullMoveAllNode, UnFullMoveAllNode);
            CommandFullMoveAllSelectedNode = new Command<MyPoint, List<ViewModelNode>>(this, FullMoveAllSelectedNode, UnFullMoveAllSelectedNode);
            CommandAddNode = new Command<MyPoint, ViewModelNode>(this, AddNode, DeleteNode);
            CommandAddConnect = new Command<ViewModelConnect, ViewModelConnect>(this, AddConnect, DeleteConnect);
            CommandDeleteSelectedNode = new Command<List<ViewModelNode>, List<ViewModelNode>>(this, DeleteSelectedNode, UnDeleteSelectedNode);
        }

        #endregion Setup Commands

        private void StartSelect(MyPoint point)
        {
            Selector.CommandStartSelect.Execute(point);
        }
        private void SelectedAll()
        {
            Nodes.ToList().ForEach(x => x.Selected = true);
        }
        private void UnSelectedAll()
        {
            Nodes.ToList().ForEach(x => x.Selected = false);
        }
        private List<ViewModelNode> FullMoveAllNode(MyPoint delta, List<ViewModelNode> nodes = null)
        {
            MyPoint myPoint = delta.Copy();
            if (nodes == null)
            {
                nodes = Nodes.ToList();
                myPoint.Clear();
            }
            nodes.ForEach(node => node.CommandMove.Execute(myPoint));
            return nodes;
        }
        private List<ViewModelNode> UnFullMoveAllNode(MyPoint delta, List<ViewModelNode> nodes = null)
        {
            MyPoint myPoint = delta.Copy();
            myPoint.Mirror();
            nodes.ForEach(node => node.CommandMove.Execute(myPoint));
            return nodes;
        }
        private List<ViewModelNode> FullMoveAllSelectedNode(MyPoint delta, List<ViewModelNode> nodes = null)
        {
            MyPoint myPoint = delta.Copy();
            if (nodes == null)
            {
                nodes = Nodes.Where(x => x.Selected).ToList();
                myPoint.Clear();
            }
            nodes.ForEach(node => node.CommandMove.Execute(myPoint));
            return nodes;
        }
        private List<ViewModelNode> UnFullMoveAllSelectedNode(MyPoint delta, List<ViewModelNode> nodes = null)
        {
            MyPoint myPoint = delta.Copy();
            myPoint.Mirror();
            nodes.ForEach(node => node.CommandMove.Execute(myPoint));
            return nodes;
        }
        private void PartMoveAllNode(MyPoint delta)
        {
           Nodes.ToList().ForEach(node => node.CommandMove.Execute(delta));
        }
        private void PartMoveAllSelectedNode(MyPoint delta)
        {
            Nodes.Where(x => x.Selected).ToList().ForEach(node => node.CommandMove.Execute(delta));
        }
        private ViewModelNode AddNode(MyPoint parameter, ViewModelNode result)
        {
            ViewModelNode newNode = result;
            if (result == null)
            {
                MyPoint myPoint = parameter.Copy();
                myPoint /= Scale.Value;
                newNode = new ViewModelNode(this)
                {
                    Name = "State " + Nodes.Count.ToString(),
                    Point1 = new MyPoint(myPoint)
                };
            }
            Nodes.Add(newNode);
            return newNode;
        }
        private ViewModelNode DeleteNode(MyPoint parameter, ViewModelNode result)
        {
            Nodes.Remove(result);
            return result;
        }
        private void Zoom(object delta)
        {
            int Delta = (int)delta;
            bool DeltaIsZero = (Delta == 0);
            bool DeltaMax = ((Delta > 0) && (Scale.Value > ScaleMax));
            bool DeltaMin = ((Delta < 0) && (Scale.Value < ScaleMin));
            if (DeltaIsZero || DeltaMax || DeltaMin)
                return;

            Scale.Value += (Delta > 0) ? Scales : -Scales;
        }
        private void SelectorIntersect()
        {
            MyPoint selectorPoint1 = Selector.Point1WithScale / Scale.Value;
            MyPoint selectorPoint2 = Selector.Point2WithScale / Scale.Value;
            foreach (ViewModelNode node in Nodes)
            {
                node.Selected = Utils.Intersect(node.Point1, node.Point2, selectorPoint1, selectorPoint2);
            }
        }
        private void AddFreeConnect(ViewModelConnector fromConnector)
        {
            CurrentConnect = new ViewModelConnect(fromConnector);
            Connects.Add(CurrentConnect);
        }
        private ViewModelConnect AddConnect(ViewModelConnect parameter, ViewModelConnect result)
        {
            if (result == null)
                return parameter;
            result.FromConnector.CommandAdd.Execute();
            Connects.Add(result);
            return result;
        }
        private ViewModelConnect DeleteConnect(ViewModelConnect parameter, ViewModelConnect result)
        {
            Connects.Remove(parameter);
            parameter.FromConnector.CommandDelete.Execute();
            return parameter;
        }
        private void DeleteFreeConnect()
        {
            Connects.Remove(CurrentConnect);
        }
        private List<ViewModelNode> DeleteSelectedNode(List<ViewModelNode> parameter, List<ViewModelNode> result)
        {
            if (result == null)
            {
                result = Nodes.Where(x => x.Selected && x.CanBeDelete).ToList();
            }
            Nodes.RemoveMany(result);
            return result;
        }
        private List<ViewModelNode> UnDeleteSelectedNode(List<ViewModelNode> parameter, List<ViewModelNode> result)
        {
            Nodes.Add(result);
            return result;
        }
        public bool ValidateNodeName(string oldValue, string newValue)
        {
            newValue = oldValue;
            return true;
        }
    }
}
