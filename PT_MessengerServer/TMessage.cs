//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace PT_MessengerServer
{
    using System;
    using System.Collections.Generic;
    
    public partial class TMessage
    {
        public int TMessage_id { get; set; }
        public string TMessage_text { get; set; }
        public System.DateTime TMessage_ts { get; set; }
        public int TMessage_src { get; set; }
        public int TMessage_dst { get; set; }
        public bool TMessage_deliver { get; set; }
    
        public virtual TUsers User_src { get; set; }
        public virtual TUsers User_dst { get; set; }
    }
}
