﻿// <auto-generated />
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.EntityFrameworkCore.Storage.Internal;
using PRIME.Core.Web.Context;
using System;

namespace PRIME.Core.Web.Migrations
{
    [DbContext(typeof(DSSContext))]
    [Migration("20210822085515_PRIME01")]
    partial class PRIME01
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.0.1-rtm-125")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("PRIME.Core.Context.Entities.AggrModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CDSClientId");

                    b.Property<string>("Code")
                        .HasMaxLength(10);

                    b.Property<string>("Config");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(100);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Description")
                        .HasMaxLength(5000);

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(100);

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.Property<string>("Version")
                        .HasMaxLength(10);

                    b.HasKey("Id");

                    b.HasIndex("CDSClientId");

                    b.ToTable("AggrModels");
                });

            modelBuilder.Entity("PRIME.Core.Context.Entities.AlertModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("AggregationPeriodDays");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(100);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Description")
                        .HasMaxLength(500);

                    b.Property<string>("HighPriorityValue")
                        .HasMaxLength(100);

                    b.Property<bool>("IsSystem");

                    b.Property<string>("LowPriorityValue")
                        .HasMaxLength(100);

                    b.Property<string>("MediumPriorityValue")
                        .HasMaxLength(100);

                    b.Property<string>("Message")
                        .HasMaxLength(100);

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(100);

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.Property<string>("TargetValueCode")
                        .HasMaxLength(10);

                    b.Property<bool>("TargetValueNumeric");

                    b.Property<string>("TargetValueSource")
                        .HasMaxLength(50);

                    b.Property<string>("UserId")
                        .HasMaxLength(100);

                    b.HasKey("Id");

                    b.ToTable("AlertModels");
                });

            modelBuilder.Entity("PRIME.Core.Context.Entities.CDSClient", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Code");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(100);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Description");

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(100);

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<string>("Name");

                    b.Property<string>("Url");

                    b.HasKey("Id");

                    b.ToTable("CDSClients");
                });

            modelBuilder.Entity("PRIME.Core.Context.Entities.DSSModel", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<int?>("CDSClientId");

                    b.Property<string>("Code");

                    b.Property<string>("Config");

                    b.Property<string>("CreatedBy")
                        .HasMaxLength(100);

                    b.Property<DateTime>("CreatedDate");

                    b.Property<string>("Description")
                        .HasMaxLength(5000);

                    b.Property<string>("ModifiedBy")
                        .HasMaxLength(100);

                    b.Property<DateTime>("ModifiedDate");

                    b.Property<string>("Name")
                        .HasMaxLength(100);

                    b.Property<string>("Version")
                        .HasMaxLength(10);

                    b.HasKey("Id");

                    b.HasIndex("CDSClientId");

                    b.ToTable("DSSModels");
                });

            modelBuilder.Entity("PRIME.Core.Context.Entities.AggrModel", b =>
                {
                    b.HasOne("PRIME.Core.Context.Entities.CDSClient", "CDSClient")
                        .WithMany("Aggregators")
                        .HasForeignKey("CDSClientId");
                });

            modelBuilder.Entity("PRIME.Core.Context.Entities.DSSModel", b =>
                {
                    b.HasOne("PRIME.Core.Context.Entities.CDSClient", "CDSClient")
                        .WithMany("DSSModels")
                        .HasForeignKey("CDSClientId");
                });
#pragma warning restore 612, 618
        }
    }
}
