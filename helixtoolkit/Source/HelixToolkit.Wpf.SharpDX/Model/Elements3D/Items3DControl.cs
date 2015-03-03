﻿// --------------------------------------------------------------------------------------------------------------------
// <copyright file="CompositeModel3D.cs" company="Helix 3D Toolkit">
//   http://helixtoolkit.codeplex.com, license: MIT
// </copyright>
// --------------------------------------------------------------------------------------------------------------------

namespace HelixToolkit.Wpf.SharpDX
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Windows.Controls.Primitives;

    using global::SharpDX;

    /// <summary>
    /// Represents a items control for Elements3D
    /// </summary>
    public class Items3DControl : Selector, IRenderable, IHitable, IThrowingShadow
    {  
  
        /// <summary>
        /// Initializes a new instance of the <see cref="CompositeModel3D" /> class.
        /// </summary>
        public Items3DControl()
        {                                   
        }

        /// <summary>
        /// Handles changes in the Children collection.
        /// </summary>
        /// <param name="sender">
        /// The sender.
        /// </param>
        /// <param name="e">
        /// The <see cref="NotifyCollectionChangedEventArgs"/> instance containing the event data.
        /// </param>
        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Replace:
                    {
                        if (this.ItemTemplate != null)
                        {
                            foreach (var item in e.NewItems)
                            {
                                var element = this.ItemTemplate.LoadContent() as Element3D;
                                if (element != null)
                                {
                                    element.DataContext = item;
                                    this.children.Add(element);

                                    var meshModel = element as IMouse3D;
                                    if (meshModel != null)
                                    {
                                        var selectable = meshModel as ISelectable;
                                        meshModel.MouseDown3D += (o, e1) =>
                                        {
                                            this.SelectedItem = item;

                                            if (selectable != null)
                                                selectable.IsSelected = true;
                                        };
                                    }
                                }
                                else
                                {
                                    throw new InvalidOperationException("Cannot create a Element3D from ItemTemplate.");
                                }
                            }
                        }
                        else
                        {
                            foreach (var item in e.NewItems)
                            {
                                var element = item as IRenderable;
                                if (element != null)
                                {
                                    this.children.Add(item);

                                    var meshModel = element as IMouse3D;
                                    if (meshModel != null)
                                    {
                                        var selectable = meshModel as ISelectable;
                                        meshModel.MouseDown3D += (o, e1) =>
                                        {
                                            this.SelectedItem = item;

                                            if (selectable != null)
                                                selectable.IsSelected = true;
                                        };
                                    }
                                }
                            }
                        }
                        break;
                    }
                case NotifyCollectionChangedAction.Remove:
                    {
                        if (this.ItemTemplate != null)
                        {
                            int ii = 0;
                            foreach (var item in e.OldItems)
                            {
                                var element = this.children[e.OldStartingIndex + ii++] as IRenderable;
                                if (element != null)
                                {
                                    element.Detach();
                                    this.children.Remove(element);
                                }
                                else
                                {
                                    throw new InvalidOperationException("Cannot remove a Element3D from Items.");
                                }
                            }
                        }
                        else
                        {
                            foreach (var item in e.OldItems)
                            {
                                var element = item as IRenderable;
                                if (element != null)
                                {
                                    element.Detach();
                                    this.children.Remove(item);
                                }
                            }
                        }

                        break;
                    }
                case NotifyCollectionChangedAction.Reset:
                    {
                        foreach (var item in this.children)
                        {
                            var element = item as IRenderable;
                            if (element != null)
                            {
                                element.Detach();
                            }
                            else
                            {
                                throw new InvalidOperationException("Cannot remove a Element3D from Items.");
                            }
                        }
                        this.children.Clear();
                        break;
                    }
            }
            // UpdateBounds();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="oldValue"></param>
        /// <param name="newValue"></param>
        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            this.children.Clear();

            if (this.ItemsSource != null)
            {
                if (this.ItemTemplate != null)
                {
                    foreach (var item in this.ItemsSource)
                    {
                        var model = this.ItemTemplate.LoadContent() as Element3D;
                        if (model != null)
                        {
                            model.DataContext = item;                            
                            this.children.Add(model);

                            var meshModel = model as IMouse3D;
                            if (meshModel != null)
                            {
                                var selectable = meshModel as ISelectable;
                                meshModel.MouseDown3D += (o, e1) =>
                                {
                                    this.SelectedItem = item;
                                    if (selectable != null)
                                        selectable.IsSelected = true;
                                };
                            }
                        }
                        else
                        {
                            throw new InvalidOperationException("Cannot create a Element3D from ItemTemplate.");
                        }
                    }
                }
                else
                {
                    foreach (var item in newValue)
                    {
                        this.children.Add(item);

                        var meshModel = item as IMouse3D;
                        if (meshModel != null)
                        {
                            var selectable = meshModel as ISelectable;
                            meshModel.MouseDown3D += (o, e1) =>
                            {
                                this.SelectedItem = item;
                                
                                if (selectable != null)
                                    selectable.IsSelected = true;
                            };
                        }
                    }
                }

                /// attach new elements if they are renderable
                if (this.IsAttached)
                {
                    foreach (var item in newValue)
                    {
                        var element = item as IRenderable;
                        element.Attach(this.renderHost);
                    }
                }
            }
            else
            {
                foreach (var item in oldValue)
                {
                    var element = item as Element3D;
                    if (element != null)
                    {
                        element.Detach();
                    }
                }
            }
        }        

        /// <summary>
        /// Attaches the specified host.
        /// </summary>
        /// <param name="host">
        /// The host.
        /// </param>
        public virtual void Attach(IRenderHost host)
        {
            this.renderHost = host;
            this.IsAttached = true;

            foreach (var item in this.children)
            {
                var model = item as Element3D;
                if (model != null)
                {
                    model.Attach(host);
                }
            }            
        }

        /// <summary>
        ///     Detaches this instance.
        /// </summary>
        public virtual void Detach()
        {
            foreach (var item in this.children)
            {
                var model = item as Element3D;
                if (model != null)
                {
                    model.Detach();
                }
            }
            this.IsAttached = false;
        }

        /// <summary>
        /// 
        /// </summary>        
        public virtual void Update(TimeSpan timeSpan)
        {
            foreach (var item in this.children)
            {
                var element = item as IRenderable;
                if (element != null)
                {
                    element.Update(timeSpan);
                }
            }
        }

        /// <summary>
        /// Compute hit-testing for all children
        /// </summary>
        public virtual bool HitTest(Ray ray, ref List<HitTestResult> hits)
        {
            bool hit = false;

            foreach (var item in this.children)
            {
                var hc = item as IHitable;
                if (hc != null)
                {
                    var tc = item as ITransformable;
                    if (tc != null)
                    {
                        //tc.PushMatrix(this.modelMatrix);
                        if (hc.HitTest(ray, ref hits))
                        {
                            hit = true;
                        }
                        //tc.PopMatrix();
                    }
                    else
                    {
                        if (hc.HitTest(ray, ref hits))
                        {
                            hit = true;
                        }
                    }
                }
            }
            return hit;
        }        

        /// <summary>
        /// 
        /// </summary>
        public virtual void Dispose()
        {
            this.Detach();
        }

        /// <summary>
        /// Renders the specified context.
        /// </summary>
        /// <param name="context">
        /// The context.
        /// </param>
        public virtual void Render(RenderContext context)
        {
            foreach (var item in this.children)
            {
                var element = item as Element3D;
                if (element != null)
                {
                    //var model = item as ITransformable;
                    //if (model != null)
                    //{
                    //    // push matrix                    
                    //    model.PushMatrix(this.modelMatrix);
                    //    // render model
                    //    element.Render(context);
                    //    // pop matrix                   
                    //    model.PopMatrix();
                    //}
                    //else
                    if (!element.IsAttached)
                    {
                        element.Attach(this.renderHost);
                    }

                    {
                        element.Render(context);
                    }
                }
            }
        }

        /// <summary>
        /// a Model3D does not have bounds, 
        /// if you want to have a model with bounds, use GeometryModel3D instead:
        /// but this prevents the CompositeModel3D containg lights, etc. (Lights3D are Models3D, which do not have bounds)
        /// </summary>
        //private void UpdateBounds()
        //{
        //    var bb = this.Bounds;
        //    foreach (var item in this.Items)
        //    {
        //        var model = item as IBoundable;
        //        if (model != null)
        //        {
        //            bb = BoundingBox.Merge(bb, model.Bounds);
        //        }
        //    }
        //    this.Bounds = bb;            
        //}

        /// <summary>
        /// 
        /// </summary>
        public bool IsAttached { get; private set; }

        /// <summary>
        /// 
        /// </summary>
        public bool IsThrowingShadow
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        protected IRenderHost renderHost;

        /// <summary>
        /// 
        /// </summary>
        protected IList children = new ArrayList();
    }
}