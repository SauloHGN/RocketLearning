﻿// <auto-generated />
using System;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using RocketLearning.Models;

#nullable disable

namespace RocketLearning.Migrations
{
    [DbContext(typeof(DataContext))]
    partial class DataContextModelSnapshot : ModelSnapshot
    {
        protected override void BuildModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder
                .HasAnnotation("ProductVersion", "7.0.5")
                .HasAnnotation("Relational:MaxIdentifierLength", 64);

            modelBuilder.Entity("RocketLearning.Models.Comentario", b =>
                {
                    b.Property<int>("ComentarioID")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("comentarioID");

                    b.Property<int>("AutorID")
                        .HasColumnType("int")
                        .HasColumnName("autorID");

                    b.Property<string>("AutorNome")
                        .HasColumnType("longtext")
                        .HasColumnName("autorNome");

                    b.Property<string>("Data")
                        .HasColumnType("longtext")
                        .HasColumnName("data");

                    b.Property<string>("Text")
                        .HasColumnType("longtext")
                        .HasColumnName("texto");

                    b.Property<string>("VideoID")
                        .HasColumnType("longtext")
                        .HasColumnName("videoID");

                    b.HasKey("ComentarioID");

                    b.ToTable("comentarios");
                });

            modelBuilder.Entity("RocketLearning.Models.Usuarios", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int")
                        .HasColumnName("id");

                    b.Property<string>("Codigo")
                        .HasColumnType("longtext")
                        .HasColumnName("codigo");

                    b.Property<string>("Email")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("email");

                    b.Property<string>("Foto")
                        .HasColumnType("longtext")
                        .HasColumnName("foto");

                    b.Property<string>("Nome")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("nome");

                    b.Property<string>("Senha")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("senha");

                    b.Property<string>("Telefone")
                        .IsRequired()
                        .HasColumnType("longtext")
                        .HasColumnName("telefone");

                    b.HasKey("Id");

                    b.ToTable("usuarios");
                });

            modelBuilder.Entity("RocketLearning.Models.Video", b =>
                {
                    b.Property<int?>("IdVideo")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("int");

                    b.Property<string>("GoogleDriveFileId")
                        .HasColumnType("longtext")
                        .HasColumnName("fileId");

                    b.Property<string>("Thumbnail")
                        .HasColumnType("longtext")
                        .HasColumnName("thumbnail");

                    b.Property<string>("Titulo")
                        .HasColumnType("longtext")
                        .HasColumnName("titulo");

                    b.Property<DateTime?>("UploadDate")
                        .HasColumnType("datetime(6)")
                        .HasColumnName("uploadDate");

                    b.HasKey("IdVideo");

                    b.ToTable("videos");
                });
#pragma warning restore 612, 618
        }
    }
}
