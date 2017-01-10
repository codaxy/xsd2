namespace Xsd2 {
    using System;
    using System.Collections.Generic;
    
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://codaxy.com/xsd2/Test.xsd")]
    public partial class UpperCaseType {
        
        private int valueField;
        
        private bool valueFieldSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(AttributeName="Value")]
        public int _Value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool _ValueSpecified {
            get {
                return this.valueFieldSpecified;
            }
            set {
                this.valueFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> Value {
            get {
                if (valueFieldSpecified) {
                    return valueField;
                }
                else {
                    return null;
                }
            }
            set {
                if ((value != null)) {
                    valueFieldSpecified = true;
                    valueField = value.Value;
                }
                else {
                    valueFieldSpecified = false;
                }
            }
        }
    }
    
    /// <remarks/>
    [System.CodeDom.Compiler.GeneratedCodeAttribute("Xsd2", "1.0.0.0")]
    [System.SerializableAttribute()]
    [System.ComponentModel.DesignerCategoryAttribute("code")]
    [System.Xml.Serialization.XmlTypeAttribute(Namespace="http://codaxy.com/xsd2/Test.xsd")]
    public partial class LowerCaseType {
        
        private int valueField;
        
        private bool valueFieldSpecified;
        
        private System.DateTime dateField;
        
        private bool dateFieldSpecified;
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(AttributeName="value")]
        public int _value {
            get {
                return this.valueField;
            }
            set {
                this.valueField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool _valueSpecified {
            get {
                return this.valueFieldSpecified;
            }
            set {
                this.valueFieldSpecified = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlAttributeAttribute(DataType="date", AttributeName="date")]
        public System.DateTime _date {
            get {
                return this.dateField;
            }
            set {
                this.dateField = value;
            }
        }
        
        /// <remarks/>
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public bool _dateSpecified {
            get {
                return this.dateFieldSpecified;
            }
            set {
                this.dateFieldSpecified = value;
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<int> Value {
            get {
                if (valueFieldSpecified) {
                    return valueField;
                }
                else {
                    return null;
                }
            }
            set {
                if ((value != null)) {
                    valueFieldSpecified = true;
                    valueField = value.Value;
                }
                else {
                    valueFieldSpecified = false;
                }
            }
        }
        
        [System.Xml.Serialization.XmlIgnoreAttribute()]
        public System.Nullable<System.DateTime> Date {
            get {
                if (dateFieldSpecified) {
                    return dateField;
                }
                else {
                    return null;
                }
            }
            set {
                if ((value != null)) {
                    dateFieldSpecified = true;
                    dateField = value.Value;
                }
                else {
                    dateFieldSpecified = false;
                }
            }
        }
    }
}
