﻿using System;
using System.Collections.Generic;
using RTSafe.DxfCore.DxfCore.Blocks;
using RTSafe.DxfCore.DxfCore;
using RTSafe.DxfCore.DxfCore.Entities;
using RTSafe.DxfCore.DxfCore.Tables;
using RTSafe.DxfCore.Entities;
using RTSafe.DxfCore.Tables;



namespace RTSafe.DxfCore.DxfCore.Entities
{

    /// <summary>
    /// Represents a block insertion <see cref="RTSafe.DxfCore.Entities.IEntityObject">entity</see>.
    /// </summary>
    public class Insert :
        DxfObject,
        IEntityObject
    {
        #region private fields

        private readonly EndSequence endSequence;
        private const EntityType TYPE = EntityType.Insert;
        private AciColor color;
        private Layer layer;
        private LineType lineType;
        private readonly Block block;
        private Vector3f insertionPoint;
        private Vector3f scale;
        private double rotation;
        private Vector3f normal;
        private readonly List<RTSafe.DxfCore.Entities.Attribute> attributes;
        private Dictionary<ApplicationRegistry, XData> xData;

        #endregion

        #region constructors

        /// <summary>
        /// Initializes a new instance of the <c>Insert</c> class.
        /// </summary>
        /// <param name="block">Insert block definition.</param>
        /// <param name="insertionPoint">Insert <see cref="Vector3f">point</see>.</param>
        public Insert(Block block, Vector3f insertionPoint)
            : base (DxfObjectCode.Insert)
        {
            if (block == null)
                throw new ArgumentNullException("block");

            this.block = block;
            this.insertionPoint = insertionPoint;
            this.scale = new Vector3f(1.0f, 1.0f, 1.0f);
            this.rotation = 0.0f;
            this.normal = Vector3f.UnitZ;
            this.layer = Layer.Default;
            this.color = AciColor.ByLayer;
            this.lineType = LineType.ByLayer;
            this.attributes = new List<RTSafe.DxfCore.Entities.Attribute>();
            foreach (AttributeDefinition attdef in block.Attributes.Values)
            {
                //this.attributes.Add(new Attribute(attdef));
                //this.attributes.Add(()attdef);
            }
            this.endSequence = new EndSequence();
        }

        /// <summary>
        /// Initializes a new instance of the <c>Insert</c> class.
        /// </summary>
        /// <param name="block">Insert <see cref="DxfCore.Blocks.Block">block definition</see>.</param>
        public Insert(Block block)
            : base(DxfObjectCode.Insert)
        {
            if (block == null)
                throw new ArgumentNullException("block");

            this.block = block;
            this.insertionPoint = Vector3f.Zero;
            this.scale = new Vector3f(1.0f, 1.0f, 1.0f);
            this.rotation = 0.0f;
            this.normal = Vector3f.UnitZ;
            this.layer = Layer.Default;
            this.color = AciColor.ByLayer;
            this.lineType = LineType.ByLayer;
            this.attributes = new List<RTSafe.DxfCore.Entities.Attribute>();
            foreach (AttributeDefinition attdef in block.Attributes.Values)
            {
                //this.attributes.Add(new Attribute(attdef));
            }
            this.endSequence = new EndSequence();
        }

        #endregion

        #region public properties

        /// <summary>
        /// Gets the insert list of <see cref="Attribute">attributes</see>.
        /// </summary>
        public List<RTSafe.DxfCore.Entities.Attribute> Attributes
        {
            get { return this.attributes; }
        }

        /// <summary>
        /// Gets the insert <see cref="DxfCore.Blocks.Block">block definition</see>.
        /// </summary>
        public Block Block
        {
            get { return this.block; }
        }

        /// <summary>
        /// Gets or sets the insert <see cref="Vector3f">point</see>.
        /// </summary>
        public Vector3f InsertionPoint
        {
            get { return this.insertionPoint; }
            set { this.insertionPoint = value; }
        }

        /// <summary>
        /// Gets or sets the insert <see cref="Vector3f">scale</see>.
        /// </summary>
        public Vector3f Scale
        {
            get { return this.scale; }
            set { this.scale = value; }
        }

        /// <summary>
        /// Gets or sets the insert rotation along the normal vector in degrees.
        /// </summary>
        public double Rotation
        {
            get { return this.rotation; }
            set { this.rotation = value; }
        }

        /// <summary>
        /// Gets or sets the insert <see cref="Vector3f">normal</see>.
        /// </summary>
        public Vector3f Normal
        {
            get { return this.normal; }
            set
            {
                if (Vector3f.Zero == value)
                    throw new ArgumentNullException("value", "The normal can not be the zero vector");
                value.Normalize();
                this.normal = value;
            }
        }

        public EndSequence EndSequence
        {
            get { return this.endSequence; }
        }

        #endregion

        #region IEntityObject Members

        /// <summary>
        /// Gets the entity <see cref="EntityType">type</see>.
        /// </summary>
        public EntityType Type
        {
            get { return TYPE; }
        }

        /// <summary>
        /// Gets or sets the entity <see cref="AciColor">color</see>.
        /// </summary>
        public AciColor Color
        {
            get { return this.color; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                this.color = value;
            }
        }

        /// <summary>
        /// Gets or sets the entity <see cref="DxfCore.Tables.Layer">layer</see>.
        /// </summary>
        public Layer Layer
        {
            get { return this.layer; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                this.layer = value;
            }
        }

        /// <summary>
        /// Gets or sets the entity <see cref="RTSafe.DxfCore.Tables.LineType">line type</see>.
        /// </summary>
        public LineType LineType
        {
            get { return this.lineType; }
            set
            {
                if (value == null)
                    throw new ArgumentNullException("value");
                this.lineType = value;
            }
        }

        /// <summary>
        /// Gets or sets the entity <see cref="RTSafe.DxfCore.XData">extende data</see>.
        /// </summary>
        public Dictionary<ApplicationRegistry, XData> XData
        {
            get { return this.xData; }
            set { this.xData = value; }
        }

        #endregion

        #region overrides

        /// <summary>
        /// Asigns a handle to the object based in a integer counter.
        /// </summary>
        /// <param name="entityNumber">Number to asign.</param>
        /// <returns>Next avaliable entity number.</returns>
        /// <remarks>
        /// Some objects might consume more than one, is, for example, the case of polylines that will asign
        /// automatically a handle to its vertexes. The entity number will be converted to an hexadecimal number.
        /// </remarks>
        public override int AsignHandle(int entityNumber)
        {
            entityNumber = this.endSequence.AsignHandle(entityNumber);
            foreach(RTSafe.DxfCore.Entities.Attribute attrib in this.attributes )
            {
                entityNumber = attrib.AsignHandle(entityNumber);
            }
            
            return base.AsignHandle(entityNumber);
        }
        /// <summary>
        /// Converts the value of this instance to its equivalent string representation.
        /// </summary>
        /// <returns>The string representation.</returns>
        public override string ToString()
        {
            return TYPE.ToString();
        }

        #endregion




        Layer IEntityObject.Layer
        {
            get
            {
                throw new NotImplementedException();
            }
            set
            {
                throw new NotImplementedException();
            }
        }

       
    }
}