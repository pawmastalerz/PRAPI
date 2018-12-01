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
    [Migration("20181115160207_UserDetailsMigration")]
    partial class UserDetailsMigration
    {
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "2.1.4-rtm-31024")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("PRAPI.Models.Car", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("Brand");

                    b.Property<string>("Class");

                    b.Property<string>("Description");

                    b.Property<int>("Doors");

                    b.Property<float>("LP100Km");

                    b.Property<string>("License");

                    b.Property<string>("Model");

                    b.Property<DateTime>("NextInsurancePayment");

                    b.Property<DateTime>("NextTechReview");

                    b.Property<string>("PhotoUrl");

                    b.Property<int>("Price");

                    b.Property<string>("PublicId");

                    b.Property<DateTime>("ReservedFrom");

                    b.Property<DateTime>("ReservedTo");

                    b.Property<int>("Year");

                    b.HasKey("Id");

                    b.ToTable("Cars");
                });

            modelBuilder.Entity("PRAPI.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd();

                    b.Property<string>("City");

                    b.Property<string>("EMail");

                    b.Property<byte[]>("PasswordHash");

                    b.Property<byte[]>("PasswordSalt");

                    b.Property<string>("PostalCode");

                    b.Property<string>("Street");

                    b.Property<string>("StreetNumber");

                    b.Property<string>("Username");

                    b.HasKey("Id");

                    b.ToTable("Users");
                });
#pragma warning restore 612, 618
        }
    }
}