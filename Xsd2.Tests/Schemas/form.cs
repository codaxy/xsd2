namespace Xsd2 {
    using System;
    using System.Collections.Generic;
    
    
    /// <remarks/>
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Container))]
    [System.Xml.Serialization.XmlIncludeAttribute(typeof(Field))]
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://example.org/Form.xsd")]
    public partial class Element {
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://example.org/Form.xsd")]
    public partial class Container : Element {
        
        private List<Field> itemsField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("field")]
        public List<Field> Items {
            get {
                return this.itemsField;
            }
            set {
                this.itemsField = value;
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://example.org/Form.xsd")]
    [System.Xml.Serialization.XmlRootAttribute("field", Namespace="http://example.org/Form.xsd", IsNullable=false)]
    public partial class Field : Element {
        
        private string nameField;
        
        private string labelField;
        
        private int minLengthField;
        
        private bool minLengthFieldSpecified;
        
        private bool writeField;
        
        private Align alignField;
        
        private bool alignFieldSpecified;
        
        public Field() {
            this.writeField = false;
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("name")]
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("label")]
        public string Label {
            get {
                return this.labelField;
            }
            set {
                this.labelField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("minLength")]
        public int _minLength {
            get {
                return this.minLengthField;
            }
            set {
                this.minLengthField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool _minLengthSpecified {
            get {
                return this.minLengthFieldSpecified;
            }
            set {
                this.minLengthFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("write")]
        [System.ComponentModel.DefaultValueAttribute(false)]
        public bool Write {
            get {
                return this.writeField;
            }
            set {
                this.writeField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("align")]
        public Align _align {
            get {
                return this.alignField;
            }
            set {
                this.alignField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool _alignSpecified {
            get {
                return this.alignFieldSpecified;
            }
            set {
                this.alignFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public Nullable<System.Int32> MinLength {
            get {
                if (minLengthFieldSpecified) {
                    return minLengthField;
                }
                else {
                    return null;
                }
            }
            set {
                if (value != null) {
                    minLengthFieldSpecified = true;
                    minLengthField = value.Value;
                }
                else {
                    minLengthFieldSpecified = false;
                }
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public Nullable<Align> Align {
            get {
                if (alignFieldSpecified) {
                    return alignField;
                }
                else {
                    return null;
                }
            }
            set {
                if (value != null) {
                    alignFieldSpecified = true;
                    alignField = value.Value;
                }
                else {
                    alignFieldSpecified = false;
                }
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://example.org/Form.xsd")]
    public enum Align {
        
        /// <remarks/>
        auto,
        
        /// <remarks/>
        left,
        
        /// <remarks/>
        center,
        
        /// <remarks/>
        right,
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://example.org/Form.xsd")]
    [System.Xml.Serialization.XmlRootAttribute("form", Namespace="http://example.org/Form.xsd", IsNullable=false)]
    public partial class Form {
        
        private Container itemsField;
        
        private string nameField;
        
        private string descriptionField;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlElementAttribute("items")]
        public Container Items {
            get {
                return this.itemsField;
            }
            set {
                this.itemsField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("name")]
        public string Name {
            get {
                return this.nameField;
            }
            set {
                this.nameField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute("description")]
        public string Description {
            get {
                return this.descriptionField;
            }
            set {
                this.descriptionField = value;
            }
        }
    }
}
