﻿// <auto-generated />
using System;
using AlcoHelper.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

#nullable disable

namespace AlcoHelper.Migrations
{
    [DbContext(typeof(AlcoHelperContext))]
    [Migration("20250531122429_AddRolesTableAndUserRoleId")]
    partial class AddRolesTableAndUserRoleId
    {
        /// <inheritdoc />
        protected override void BuildTargetModel(ModelBuilder modelBuilder)
        {
#pragma warning disable 612, 618
            modelBuilder.HasAnnotation("ProductVersion", "8.0.13");

            modelBuilder.Entity("AlcoHelper.Models.Alcohol", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("AddedDate")
                        .HasColumnType("TEXT");

                    b.Property<double>("AlcoholPercentage")
                        .HasColumnType("REAL");

                    b.Property<string>("Country")
                        .HasColumnType("TEXT");

                    b.Property<string>("Description")
                        .HasColumnType("TEXT");

                    b.Property<string>("ImageUrl")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsApproved")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.Property<string>("Type")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Alcohols");
                });

            modelBuilder.Entity("AlcoHelper.Models.AlcoholStore", b =>
                {
                    b.Property<int>("AlcoholId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("StoreId")
                        .HasColumnType("INTEGER");

                    b.Property<decimal>("Price")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProductUrl")
                        .HasColumnType("TEXT");

                    b.HasKey("AlcoholId", "StoreId");

                    b.HasIndex("StoreId");

                    b.ToTable("AlcoholStores");
                });

            modelBuilder.Entity("AlcoHelper.Models.AlcoholTag", b =>
                {
                    b.Property<int>("AlcoholId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("TagId")
                        .HasColumnType("INTEGER");

                    b.HasKey("AlcoholId", "TagId");

                    b.HasIndex("TagId");

                    b.ToTable("AlcoholTags");
                });

            modelBuilder.Entity("AlcoHelper.Models.Comment", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Content")
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<int>("ReviewId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("ReviewId");

                    b.HasIndex("UserId");

                    b.ToTable("Comments");
                });

            modelBuilder.Entity("AlcoHelper.Models.FavoriteAlco", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AlcoholId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("AlcoholId");

                    b.HasIndex("UserId");

                    b.ToTable("FavoriteAlcos");
                });

            modelBuilder.Entity("AlcoHelper.Models.Report", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AlcoholId")
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Message")
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("AlcoholId");

                    b.HasIndex("UserId");

                    b.ToTable("Reports");
                });

            modelBuilder.Entity("AlcoHelper.Models.Review", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AlcoholId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Comment")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("ImageUrl")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<double>("Rating")
                        .HasColumnType("REAL");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("AlcoholId");

                    b.HasIndex("UserId");

                    b.ToTable("Reviews");
                });

            modelBuilder.Entity("AlcoHelper.Models.Store", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.Property<string>("WebsiteUrl")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Stores");
                });

            modelBuilder.Entity("AlcoHelper.Models.Tag", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Tags");
                });

            modelBuilder.Entity("AlcoHelper.Models.User", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<string>("Email")
                        .HasColumnType("TEXT");

                    b.Property<string>("PasswordHash")
                        .HasColumnType("TEXT");

                    b.Property<string>("ProfilePictureUrl")
                        .HasColumnType("TEXT");

                    b.Property<int>("RoleId")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Username")
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.HasIndex("RoleId");

                    b.ToTable("Users");
                });

            modelBuilder.Entity("AlcoHelper.Models.WishList", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<int>("AlcoholId")
                        .HasColumnType("INTEGER");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("AlcoholId");

                    b.HasIndex("UserId");

                    b.ToTable("Wishlists");
                });

            modelBuilder.Entity("AlcoHelper.ViewModels.EmailNotification", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<DateTime>("CreatedAt")
                        .HasColumnType("TEXT");

                    b.Property<bool>("IsSent")
                        .HasColumnType("INTEGER");

                    b.Property<string>("Type")
                        .HasColumnType("TEXT");

                    b.Property<int>("UserId")
                        .HasColumnType("INTEGER");

                    b.HasKey("Id");

                    b.HasIndex("UserId");

                    b.ToTable("EmailNotifications");
                });

            modelBuilder.Entity("Role", b =>
                {
                    b.Property<int>("Id")
                        .ValueGeneratedOnAdd()
                        .HasColumnType("INTEGER");

                    b.Property<string>("Name")
                        .IsRequired()
                        .HasColumnType("TEXT");

                    b.HasKey("Id");

                    b.ToTable("Roles");
                });

            modelBuilder.Entity("AlcoHelper.Models.AlcoholStore", b =>
                {
                    b.HasOne("AlcoHelper.Models.Alcohol", "Alcohol")
                        .WithMany("AlcoholStores")
                        .HasForeignKey("AlcoholId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AlcoHelper.Models.Store", "Store")
                        .WithMany("AlcoholStores")
                        .HasForeignKey("StoreId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Alcohol");

                    b.Navigation("Store");
                });

            modelBuilder.Entity("AlcoHelper.Models.AlcoholTag", b =>
                {
                    b.HasOne("AlcoHelper.Models.Alcohol", "Alcohol")
                        .WithMany("AlcoholTags")
                        .HasForeignKey("AlcoholId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AlcoHelper.Models.Tag", "Tag")
                        .WithMany("AlcoholTags")
                        .HasForeignKey("TagId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Alcohol");

                    b.Navigation("Tag");
                });

            modelBuilder.Entity("AlcoHelper.Models.Comment", b =>
                {
                    b.HasOne("AlcoHelper.Models.Review", "Review")
                        .WithMany()
                        .HasForeignKey("ReviewId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AlcoHelper.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Review");

                    b.Navigation("User");
                });

            modelBuilder.Entity("AlcoHelper.Models.FavoriteAlco", b =>
                {
                    b.HasOne("AlcoHelper.Models.Alcohol", "Alcohol")
                        .WithMany("Favorites")
                        .HasForeignKey("AlcoholId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AlcoHelper.Models.User", "User")
                        .WithMany("Favorites")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Alcohol");

                    b.Navigation("User");
                });

            modelBuilder.Entity("AlcoHelper.Models.Report", b =>
                {
                    b.HasOne("AlcoHelper.Models.Alcohol", "Alcohol")
                        .WithMany()
                        .HasForeignKey("AlcoholId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AlcoHelper.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Alcohol");

                    b.Navigation("User");
                });

            modelBuilder.Entity("AlcoHelper.Models.Review", b =>
                {
                    b.HasOne("AlcoHelper.Models.Alcohol", "Alcohol")
                        .WithMany("Reviews")
                        .HasForeignKey("AlcoholId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AlcoHelper.Models.User", "User")
                        .WithMany("Reviews")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Alcohol");

                    b.Navigation("User");
                });

            modelBuilder.Entity("AlcoHelper.Models.User", b =>
                {
                    b.HasOne("Role", null)
                        .WithMany()
                        .HasForeignKey("RoleId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();
                });

            modelBuilder.Entity("AlcoHelper.Models.WishList", b =>
                {
                    b.HasOne("AlcoHelper.Models.Alcohol", "Alcohol")
                        .WithMany("Wishlist")
                        .HasForeignKey("AlcoholId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.HasOne("AlcoHelper.Models.User", "User")
                        .WithMany("Wishlist")
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("Alcohol");

                    b.Navigation("User");
                });

            modelBuilder.Entity("AlcoHelper.ViewModels.EmailNotification", b =>
                {
                    b.HasOne("AlcoHelper.Models.User", "User")
                        .WithMany()
                        .HasForeignKey("UserId")
                        .OnDelete(DeleteBehavior.Cascade)
                        .IsRequired();

                    b.Navigation("User");
                });

            modelBuilder.Entity("AlcoHelper.Models.Alcohol", b =>
                {
                    b.Navigation("AlcoholStores");

                    b.Navigation("AlcoholTags");

                    b.Navigation("Favorites");

                    b.Navigation("Reviews");

                    b.Navigation("Wishlist");
                });

            modelBuilder.Entity("AlcoHelper.Models.Store", b =>
                {
                    b.Navigation("AlcoholStores");
                });

            modelBuilder.Entity("AlcoHelper.Models.Tag", b =>
                {
                    b.Navigation("AlcoholTags");
                });

            modelBuilder.Entity("AlcoHelper.Models.User", b =>
                {
                    b.Navigation("Favorites");

                    b.Navigation("Reviews");

                    b.Navigation("Wishlist");
                });
#pragma warning restore 612, 618
        }
    }
}
