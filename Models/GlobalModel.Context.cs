﻿//------------------------------------------------------------------------------
// <auto-generated>
//     這個程式碼是由範本產生。
//
//     對這個檔案進行手動變更可能導致您的應用程式產生未預期的行為。
//     如果重新產生程式碼，將會覆寫對這個檔案的手動變更。
// </auto-generated>
//------------------------------------------------------------------------------

namespace LongTermCare_Xml_.Models
{
    using System;
    using System.Data.Entity;
    using System.Data.Entity.Infrastructure;
    
    public partial class PCDEntities : DbContext
    {
        public PCDEntities()
            : base("name=PCDEntities")
        {
        }
    
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            throw new UnintentionalCodeFirstException();
        }
    
        public virtual DbSet<GlobalParam> GlobalParam { get; set; }
        public virtual DbSet<Patient> Patient { get; set; }
        public virtual DbSet<Series> Series { get; set; }
        public virtual DbSet<SOPInstance> SOPInstance { get; set; }
        public virtual DbSet<Study> Study { get; set; }
        public virtual DbSet<sysdiagrams> sysdiagrams { get; set; }
    }
}
