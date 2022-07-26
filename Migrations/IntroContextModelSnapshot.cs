﻿// <auto-generated />
using System;
using Intro.DAL.Context;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Intro.Migrations
{
    [DbContext(typeof(IntroContext))]
    partial class IntroContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("Relational:MaxIdentifierLength", 128)
                .HasAnnotation("ProductVersion", "5.0.17")
                .HasAnnotation("SqlServer:ValueGenerationStrategy", SqlServerValueGenerationStrategy.IdentityColumn);

            modelBuilder.Entity("Intro.DAL.Entities.Article", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedMoment")
                        .HasColumnType("datetime2");

                    b.Property<DateTime?>("DeleteMoment")
                        .HasColumnType("datetime2");

                    b.Property<string>("PictureFile")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid?>("ReplyId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Text")
                        .HasColumnType("nvarchar(max)");

                    b.Property<Guid>("TopicId")
                        .HasColumnType("uniqueidentifier");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.HasIndex("ReplyId");

                    b.HasIndex("TopicId");

                    b.ToTable("Articles");
                });

            modelBuilder.Entity("Intro.DAL.Entities.Topic", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<Guid>("AuthorId")
                        .HasColumnType("uniqueidentifier");

                    b.Property<DateTime>("CreatedTime")
                        .HasColumnType("datetime2");

                    b.Property<string>("Description")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("LastArticleMoment")
                        .HasColumnType("datetime2");

                    b.Property<string>("Title")
                        .HasColumnType("nvarchar(max)");

                    b.HasKey("Id");

                    b.HasIndex("AuthorId");

                    b.ToTable("Topics");
                });

            modelBuilder.Entity("Intro.DAL.Entities.User", b =>
                {
                    b.Property<Guid>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("uniqueidentifier");

                    b.Property<string>("Avatar")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("Email")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime?>("LogMoment")
                        .HasColumnType("datetime2");

                    b.Property<string>("Login")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PassHash")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("PassSalt")
                        .HasColumnType("nvarchar(max)");

                    b.Property<string>("RealName")
                        .HasColumnType("nvarchar(max)");

                    b.Property<DateTime>("RegMoment")
                        .HasColumnType("datetime2");

                    b.HasKey("Id");

                    b.ToTable("Users");

                    b.HasData(
                        new
                        {
                            Id = new Guid("2002755e-4a83-4470-88bd-7102c6300f26"),
                            Avatar = "",
                            Login = "Admin",
                            PassHash = "",
                            PassSalt = "",
                            RealName = "Корневой администратор",
                            RegMoment = new DateTime(2022, 8, 26, 20, 21, 43, 107, DateTimeKind.Local).AddTicks(2015)
                        });
                });

            modelBuilder.Entity("Intro.DAL.Entities.Article", b =>
                {
                    b.HasOne("Intro.DAL.Entities.User", "Author")
                        .WithMany("Articles")
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("Intro.DAL.Entities.Article", "Reply")
                        .WithMany()
                        .HasForeignKey("ReplyId");

                    b.HasOne("Intro.DAL.Entities.Topic", "Topic")
                        .WithMany("Articles")
                        .HasForeignKey("TopicId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");

                    b.Navigation("Reply");

                    b.Navigation("Topic");
                });

            modelBuilder.Entity("Intro.DAL.Entities.Topic", b =>
                {
                    b.HasOne("Intro.DAL.Entities.User", "Author")
                        .WithMany()
                        .HasForeignKey("AuthorId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Author");
                });

            modelBuilder.Entity("Intro.DAL.Entities.Topic", b =>
                {
                    b.Navigation("Articles");
                });

            modelBuilder.Entity("Intro.DAL.Entities.User", b =>
                {
                    b.Navigation("Articles");
                });
#pragma warning restore 612, 618
        }
    }
}
