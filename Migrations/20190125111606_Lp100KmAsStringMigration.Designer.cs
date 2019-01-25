﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using PRAPI.Data;

namespace PRAPI.Migrations
{
    [DbContext(typeof(DataContext))]
    [Migration("20190125111606_Lp100KmAsStringMigration")]
    partial class Lp100KmAsStringMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("PRAPI.Models.Car", b =>
                {
                    b.Property<int>("CarId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("AirConditioned");

                    b.Property<string>("Body");

                    b.Property<string>("Brand");

                    b.Property<string>("Description");

                    b.Property<int>("Doors");

                    b.Property<string>("Fuel");

                    b.Property<string>("LP100Km");

                    b.Property<string>("Model");

                    b.Property<string>("PhotoUrl");

                    b.Property<int>("Price");

                    b.Property<string>("PublicId");

                    b.Property<string>("Transmission");

                    b.Property<int>("Year");

                    b.HasKey("CarId");

                    b.ToTable("Cars");
                });

            modelBuilder.Entity("PRAPI.Models.Order", b =>
                {
                    b.Property<int>("OrderId")
                        .ValueGeneratedOnAdd();

                    b.Property<int>("CarId");

                    b.Property<string>("IsReturned");

                    b.Property<DateTime>("ReservedFrom");

                    b.Property<DateTime>("ReservedTo");

                    b.Property<decimal>("TotalPrice");

                    b.Property<int>("UserId");

                    b.HasKey("OrderId");

                    b.HasIndex("CarId");

                    b.HasIndex("UserId");

                    b.ToTable("Orders");
                });

            modelBuilder.Entity("PRAPI.Models.User", b =>
                {
                    b.Property<int>("UserId")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("City");

                    b.Property<string>("EMail");

                    b.Property<byte[]>("PasswordHash");

                    b.Property<byte[]>("PasswordSalt");

                    b.Property<string>("PostalCode");

                    b.Property<string>("Street");

                    b.Property<string>("StreetNumber");

                    b.Property<string>("Username");

                    b.HasKey("UserId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("PRAPI.Models.Order", b =>
                {
                    b.HasOne("PRAPI.Models.Car", "CarOrdered")
                        .WithMany()
                        .HasForeignKey("CarId")
                        .OnDelete(DeleteBehavior.Cascade);

                    b.HasOne("PRAPI.Models.User", "UserOrdering")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade);
                });
#pragma warning restore 612, 618
        }
    }
}
