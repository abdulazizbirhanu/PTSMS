﻿using System.Data.Entity;
using PTSMSDAL.APIModels.Models;

namespace PTSMSDAL.Context
{
    public class EAA_API_Context : DbContext
    {
        public EAA_API_Context()
          : base("name=EAA_API_Context")//GetConnectionString()
        {
        }
        public static EAA_API_Context Create()
        {
            return new EAA_API_Context();
        }
        public DbSet<TraineeInfoBO> TraineeInfobo { get; set; }
    }
}