using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;

namespace Web4976_Assignment01.Data.Migrations
{
    [DbContext(typeof(ApplicationDbContext))]
    [Migration("20261021000100_InitialCreate")]
    partial class InitialCreate
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder) => InitialSchema.Build(modelBuilder);
    }
}

