﻿// <auto-generated />
using System;
using DAL.DataContext;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace DAL.Migrations
{
    [DbContext(typeof(DatabaseContext))]
    partial class DatabaseContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "3.1.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("DAL.Entities.Account", b =>
                {
                    b.Property<int>("AccountId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<double>("Balance")
                        .HasColumnType("float");

                    b.Property<string>("UserName")
                        .IsRequired()
                        .HasColumnType("nvarchar(450)");

                    b.HasKey("AccountId");

                    b.HasIndex("UserName")
                        .IsUnique();

                    b.ToTable("Accounts");
                });

            modelBuilder.Entity("DAL.Entities.MTTransaction", b =>
                {
                    b.Property<int>("TransactionId")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

                    b.Property<int>("DestinationAccountIdAccountId")
                        .HasColumnType("int");

                    b.Property<int?>("SourceAccountIdAccountId")
                        .HasColumnType("int");

                    b.Property<double>("TransferAmount")
                        .HasColumnType("float");

                    b.HasKey("TransactionId");

                    b.HasIndex("DestinationAccountIdAccountId");

                    b.HasIndex("SourceAccountIdAccountId");

                    b.ToTable("MTTransactions");
                });

            modelBuilder.Entity("DAL.Entities.MTTransaction", b =>
                {
                    b.HasOne("DAL.Entities.Account", "DestinationAccountId")
                        .WithMany()
                        .HasForeignKey("DestinationAccountIdAccountId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("DAL.Entities.Account", "SourceAccountId")
                        .WithMany()
                        .HasForeignKey("SourceAccountIdAccountId");
                });
#pragma warning restore 612, 618
        }
    }
}
